using Gaea.Api;
using Gaea.Api.Configuration;
using Gaea.Api.Data;
using Gaea.Services.Data;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Input;

namespace Gaea.Services.Impl
{
	public class WallpaperService : BindableBase, IWallpaperService
	{
		#region Private members

		private readonly ICommand _NextWallpaperCommand;
		private readonly ICommand _OpenWallpaperLinkCommand;

		private ILoggingService _Logger;
		private IUnityContainer _Container;
		private IConfiguration _Configuration;
		private IEventAggregator _EventAggregator;
		private IImageManager _ImageManager;

		private Thread wallpaperServiceThread;
		private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
		private CancellationTokenSource fetchCancelTokenSource = new CancellationTokenSource();

		private BlockingCollection<ServiceMessage> messageQueue;
		// TODO Allow automatic advancing of the wallpaper
		//private Timer nextWallpaperTimeout;

		#endregion

		public WallpaperService(ILoggingService logger, IUnityContainer container, IEventAggregator eventAggregator, IModuleManager moduleManager, IConfiguration configuration, IImageManager imageManager)
		{
			_Logger = logger;
			_Container = container;
			_EventAggregator = eventAggregator;
			_Configuration = configuration;
			_ImageManager = imageManager;
			messageQueue = new BlockingCollection<ServiceMessage>();

			_NextWallpaperCommand = new DelegateCommand(NextWallpaper);
			_OpenWallpaperLinkCommand = new DelegateCommand(OpenWallpaperLink);

			// Bind events
			_Configuration.ConfigurationChanged += _Configuration_ConfigurationChanged;

			moduleManager.LoadModuleCompleted += ModuleManager_LoadModuleCompleted;
			_ImageManager.Complete += _ImageManager_Complete;
			_ImageManager.Error += _ImageManager_Error;

			// Start service thread
			wallpaperServiceThread = new Thread(WallpaperServiceThreadStart);
			wallpaperServiceThread.Name = "WallpaperServiceThread";
			wallpaperServiceThread.Start();
		}

		#region Properties

		public ICommand NextWallpaperCommand
		{
			get
			{
				return _NextWallpaperCommand;
			}
		}

		public ICommand OpenWallpaperLinkCommand
		{
			get
			{
				return _OpenWallpaperLinkCommand;
			}
		}

		private IEnumerable<ISource> _Sources;
		public IEnumerable<ISource> Sources
		{
			get
			{
				return _Sources;
			}
			private set
			{
				SetProperty(ref _Sources, value);
			}
		}

		private ISource _CurrentSource;
		public ISource SelectedSource
		{
			get
			{
				return _CurrentSource;
			}
			set
			{
				InitializeCurrentSource(value);
			}
		}

		private bool _CanConfigureSource;
		public bool CanConfigureSource
		{
			get
			{
				return _CanConfigureSource;
			}
			private set
			{
				SetProperty(ref _CanConfigureSource, value);
			}
		}

		#endregion

		#region Events

		public event EventHandler<PostProcessCompletedEventArgs> PostProcessComplete;

		private void RaisePostProcessComplete(GaeaImage image)
		{
			if (PostProcessComplete != null)
			{
				PostProcessComplete(this, new PostProcessCompletedEventArgs { Image = image });
			}
		}

		#endregion

		#region Event handlers

		private void _Configuration_ConfigurationChanged(object sender, EventArgs e)
		{
			EnqueueMessage(new ServiceMessage { Type = MessageType.ConfigChange });
		}

		private void CurrentSource_FetchError(string errorTitle, string errorMessage)
		{
			Error(errorTitle, errorMessage);
			// TODO Set timeout for next attempt
		}

		private void CurrentSource_FetchNextComplete(GaeaImage next)
		{
			EnqueueMessage(new ServiceMessage { Type = MessageType.EndNextWallpaper, Data1 = next });
		}

		private void ModuleManager_LoadModuleCompleted(object sender, LoadModuleCompletedEventArgs e)
		{
			if (e.Error == null)
			{
				Sources = _Container.ResolveAll<ISource>();
			}
		}

		private void _ImageManager_Error(object sender, WallpaperServiceError e)
		{
			// Call common error handling code
			CurrentSource_FetchError(e.Subject, e.Message);
		}

		private void _ImageManager_Complete(object sender, PostProcessCompletedEventArgs e)
		{
			EnqueueMessage(new ServiceMessage { Type = MessageType.EndPostProcess, Data1 = e.Image });
		}

		#endregion

		#region Utility methods

		private void InitializeCurrentSource(string name)
		{
			try
			{
				ISource newSource = _Container.Resolve<ISource>(name);
				InitializeCurrentSource(newSource);
			}
			catch (Exception ex)
			{
				_Logger.Exception(ex, "Failed to resolve source with name \"{0}\": {1}", name, ex.Message);
				Error("Failed to load source.", "Failed to load source with name \"" + name + "\". Did you remove the source's plugin?");
			}
		}

		private void InitializeCurrentSource(ISource newSource)
		{
			// Clean up old source
			if (_CurrentSource != null)
			{
				_CurrentSource.Dispose();
				_CurrentSource.FetchError -= CurrentSource_FetchError;
				_CurrentSource.FetchNextComplete -= CurrentSource_FetchNextComplete;
			}

			// Bind events to new source
			newSource.FetchNextComplete += CurrentSource_FetchNextComplete;
			newSource.FetchError += CurrentSource_FetchError;

			// Set the new source
			SetProperty(ref _CurrentSource, newSource);
			if (_CurrentSource != null)
			{
				// Set configuration and initialize
				_Configuration.CurrentSource = _CurrentSource.Name;
				_Configuration.BuildModelFromAttributes(_CurrentSource.Configuration);
				CanConfigureSource =
					_Configuration.CurrentSourceConfigurationMetaModel != null ?
					_Configuration.CurrentSourceConfigurationMetaModel.CanConfigureSource :
					false;
				_CurrentSource.Initialize();
			
				// Immediately trigger a fetch next event
				NextWallpaper();
			}
		}

		private void Error(string subject, string message)
		{
			_EventAggregator.GetEvent<WallpaperServiceErrorEvent>().Publish(new WallpaperServiceError { Subject = subject, Message = message });
		}

		#endregion

		#region Public methods

		public void Initialize()
		{
			// Populate initial state from config
			if (_Configuration.CurrentSource != null)
			{
				// Set currently selected source from configuration
				InitializeCurrentSource(_Configuration.CurrentSource);
				if (_CurrentSource != null)
				{
					if (_Configuration.CurrentImage != null)
					{
						try
						{
							var initialImage = _Configuration.CurrentImage;
							initialImage.Source = _CurrentSource;
							initialImage.Image = new Bitmap(Image.FromFile(initialImage.RawCacheUrl));
							_Configuration.CurrentImage = initialImage;
						}
						catch (Exception ex)
						{
							_Logger.Exception(ex, "Failed to load current image: {0}", ex.Message);
						}
					}
					else
					{
						NextWallpaper();
					}
				}

				// TODO Schedule auto updates
			}
			else
			{
				// No source selected, throw a friendly error message
				_Logger.Warning("No source selected at startup.");
				Error("No source selected!", "Please select a source in the configuration window.");
			}
		}

		public void NextWallpaper()
		{
			EnqueueMessage(new ServiceMessage { Type = MessageType.BeginNextWallpaper });
		}

		public void OpenWallpaperLink()
		{
			if (_Configuration.CurrentImage != null)
			{
				Process.Start(new ProcessStartInfo(_Configuration.CurrentImage.URL));
			}
			
		}

		#endregion

		#region IDisposable implementation

		public void Dispose()
		{
			// Prepare the message processor to shut down
			cancelTokenSource.Cancel();

			// Cancel pending requests
			fetchCancelTokenSource.Cancel();

			// Dispose the current source
			if (_CurrentSource != null)
			{
				_CurrentSource.Dispose();
			}

			// Wait for the thread to terminate
			wallpaperServiceThread.Join();
		}

		#endregion

		#region Service thread

		private void EnqueueMessage(ServiceMessage msg)
		{
			try
			{
				messageQueue.Add(msg, cancelTokenSource.Token);
			}
			catch (OperationCanceledException) { } // Ignore
		}

		private void WallpaperServiceThreadStart()
		{
			try
			{
				foreach (ServiceMessage msg in messageQueue.GetConsumingEnumerable(cancelTokenSource.Token))
				{
					_Logger.Debug("[WallpaperServiceThread] Processing message: [Type = {0}  Data1 = {1}  Data2 = {2}]", msg.Type, msg.Data1, msg.Data2);
					switch (msg.Type)
					{
						case MessageType.ConfigChange:
							Rectangle screenBounds = _Configuration.ScreenBounds;
							var ppMsg = new PostProcessMessage
							{
								Image = _Configuration.CurrentImage,
								CacheDir = Path.Combine(_Configuration.AppDirectory, "Cache"),
								SourceName = _CurrentSource.GetType().FullName,
								OptimizeLayout = _Configuration.OptimizeLayout,
								Blur = _Configuration.Blur,
								Darken = _Configuration.Darken,
								Desaturate = _Configuration.Desaturate,
								ScreenWidth = screenBounds.Width,
								ScreenHeight = screenBounds.Height
							};
							_ImageManager.EnqueueUpdate(ppMsg);
							break;
						case MessageType.BeginNextWallpaper:
							// Call into the current source
							if (_CurrentSource != null)
							{
								_CurrentSource.BeginFetchNext(fetchCancelTokenSource.Token);
							}
							break;
						case MessageType.EndNextWallpaper:
							System.Windows.Application.Current.Dispatcher.Invoke(() => {
								_Configuration.CurrentImage = (GaeaImage)msg.Data1;
							});
							_EventAggregator.GetEvent<WallpaperChangedEvent>().Publish(_Configuration.CurrentImage);
							EnqueueMessage(new ServiceMessage { Type = MessageType.ConfigChange });
							break;
						case MessageType.EndPostProcess:
							_Logger.Info("Setting wallpaper...");
							RaisePostProcessComplete(_Configuration.CurrentImage);
							// TODO Set the wallpaper in the system registry
							// uses: CurrentImage.ProcessedRawCacheUrl
							break;
					}
				}
				messageQueue.CompleteAdding();
			}
			catch (OperationCanceledException) { } // Ignore
		}

		#endregion
	}
}
