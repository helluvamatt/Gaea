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
using System.Threading;
using System.Windows.Input;

namespace Gaea.Services.Impl
{
	internal class WallpaperService : BindableBase, IWallpaperService
	{
		#region Private members

		private readonly ICommand _NextWallpaperCommand;
		private readonly ICommand _OpenWallpaperLinkCommand;

		private ILoggingService _Logger;
		private IUnityContainer _Container;
		private IConfigurationService _Configuration;
		private IEventAggregator _EventAggregator;
		private IImageManager _ImageManager;

		private ISource _CurrentSource;
		private ConfigurationMetaModel _CurrentSourceConfigurationMetaModel;

		private Thread _WallpaperServiceThread;
		private CancellationTokenSource _CancelTokenSource = new CancellationTokenSource();
		private CancellationTokenSource _FetchCancelTokenSource = new CancellationTokenSource();

		private BlockingCollection<ServiceMessage> _MessageQueue;
		private Timer _NextWallpaperTimeout;

		#endregion

		public WallpaperService(ILoggingService logger, IUnityContainer container, IEventAggregator eventAggregator, IModuleManager moduleManager, IConfigurationService configuration, IImageManager imageManager)
		{
			_Logger = logger;
			_Container = container;
			_EventAggregator = eventAggregator;
			_Configuration = configuration;
			_ImageManager = imageManager;
			_MessageQueue = new BlockingCollection<ServiceMessage>();
			_NextWallpaperTimeout = new Timer(_NextWallpaperTimeout_TimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

			_NextWallpaperCommand = new DelegateCommand(NextWallpaper);
			_OpenWallpaperLinkCommand = new DelegateCommand(OpenWallpaperLink);

			// Bind events
			_Configuration.ConfigurationChanged += _Configuration_ConfigurationChanged;
			_Configuration.AutoSettingsChanged += _Configuration_AutoSettingsChanged;

			moduleManager.LoadModuleCompleted += ModuleManager_LoadModuleCompleted;
			_ImageManager.Complete += _ImageManager_Complete;
			_ImageManager.Error += _ImageManager_Error;

			// Start service thread
			_WallpaperServiceThread = new Thread(WallpaperServiceThreadStart);
			_WallpaperServiceThread.Name = "WallpaperServiceThread";
			_WallpaperServiceThread.Start();


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

		private IEnumerable<SourceItem> _Sources;
		public IEnumerable<SourceItem> Sources
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

		public string SelectedSource
		{
			get
			{
				return _CurrentSource.GetName();
			}
			set
			{
				InitializeCurrentSource(value);
			}
		}

		public ISource CurrentSource
		{
			get
			{
				return _CurrentSource;
			}
		}

		public ConfigurationMetaModel CurrentSourceConfigurationMetaModel
		{
			get
			{
				return _CurrentSourceConfigurationMetaModel;
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

		private void RaisePostProcessComplete(GaeaImage image)
		{
			if (PostProcessComplete != null)
			{
				PostProcessComplete(this, new PostProcessCompletedEventArgs { Image = image });
			}
		}

		public event EventHandler<PostProcessCompletedEventArgs> PostProcessComplete;

		#endregion

		#region Event handlers

		private void _Configuration_ConfigurationChanged(object sender, EventArgs e)
		{
			EnqueueMessage(new ServiceMessage { Type = MessageType.ConfigChange });
		}

		private void _Configuration_AutoSettingsChanged(object sender, EventArgs e)
		{
			if (_Configuration.AutomaticMode == AutomaticMode.TimeInterval)
			{
				// Time delayed, base the initial delay since the last update
				TimeSpan initialDelay = _Configuration.AutomaticDelay;
				if (_Configuration.LastUpdate.HasValue)
				{
					DateTime nextUpdate = _Configuration.LastUpdate.Value + _Configuration.AutomaticDelay;
					initialDelay = nextUpdate - DateTime.Now;
					if (initialDelay < TimeSpan.Zero)
					{
						initialDelay = TimeSpan.Zero;
					}
				}
				_NextWallpaperTimeout.Change(initialDelay, _Configuration.AutomaticDelay);
			}
			else
			{
				// Disable time delay
				_NextWallpaperTimeout.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			}
		}

		private void CurrentSource_FetchError(string errorTitle, string errorMessage)
		{
			Error(errorTitle, errorMessage);
			if (_Configuration.CurrentImage == null)
			{
				NextWallpaper();
			}
		}

		private void CurrentSource_FetchNextComplete(GaeaImage next)
		{
			EnqueueMessage(new ServiceMessage { Type = MessageType.EndNextWallpaper, Data1 = next });
		}

		private void ModuleManager_LoadModuleCompleted(object sender, LoadModuleCompletedEventArgs e)
		{
			if (e.Error == null)
			{
				var sources = _Container.ResolveAll<ISource>();
				var items = new List<SourceItem>();
				foreach (var source in sources)
				{
					items.Add(new SourceItem { Name = source.GetName(), DisplayName = source.DisplayName, Description = source.Description, Icon = source.Icon });
				}
				Sources = items;
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

		private void _NextWallpaperTimeout_TimerCallback(object state)
		{
			NextWallpaper();
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
			if (newSource == null) throw new ArgumentNullException("newSource");

			// Clean up old source
			if (_CurrentSource != null)
			{
				CancelPendingFetch();

				_CurrentSource.Dispose();
				_CurrentSource.FetchError -= CurrentSource_FetchError;
				_CurrentSource.FetchNextComplete -= CurrentSource_FetchNextComplete;
			}

			ConfigurationMetaModel configModel = null;
			try
			{
				// Load source configuration and pass it to the new source
				object configObject = _Configuration.GetSourceConfiguration(newSource.GetName());
				if (configObject != null)
				{
					newSource.Configure(configObject);
				}

				// Build config object model
				configModel = _Configuration.BuildModelFromAttributes(newSource.Configuration);
			}
			catch (Exception ex)
			{
				_Logger.Exception(ex, "Caught exception while configuring source: {0}", ex.Message);
				Error("Failed to Configure Source", "There was a problem configuring the wallpaper source. Check the logs for more details.");
				return;
			}

			try
			{
				newSource.Initialize();
			}
			catch (Exception ex)
			{
				_Logger.Exception(ex, "Caught exception in Initialize: {0}", ex.Message);
				Error("Failed to Initialize Source", "There was a problem initializing the wallpaper source. Check the logs for more details.");
				return;
			}

			// Bind events to new source
			newSource.FetchNextComplete += CurrentSource_FetchNextComplete;
			newSource.FetchError += CurrentSource_FetchError;

			// Source configuration
			_CurrentSourceConfigurationMetaModel = configModel;
			CanConfigureSource =
				_CurrentSourceConfigurationMetaModel != null ?
				_CurrentSourceConfigurationMetaModel.CanConfigureSource :
				false;

			// Set the new source
			SetProperty(ref _CurrentSource, newSource);
			
			// Save source selection to configuration service
			_Configuration.CurrentSource = _CurrentSource.GetName();
			
			// Immediately trigger a fetch next event
			NextWallpaper(false);
		}

		private void CancelPendingFetch()
		{
			// Cancel pending fetch and reset the fetchCancelTokenSource
			_FetchCancelTokenSource.Cancel();
			_FetchCancelTokenSource.Dispose();
			_FetchCancelTokenSource = new CancellationTokenSource();
		}

		private void Error(string subject, string message)
		{
			_EventAggregator.GetEvent<WallpaperServiceErrorEvent>().Publish(new WallpaperServiceError { Subject = subject, Message = message });
		}

		private void NextWallpaper(bool manual)
		{
			EnqueueMessage(new ServiceMessage { Type = MessageType.BeginNextWallpaper, Data1 = manual });
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
							if (initialImage.RawCacheUrl != null)
							{
								initialImage.Image = new Bitmap(Image.FromFile(initialImage.RawCacheUrl));
							}
							_Configuration.CurrentImage = initialImage;
						}
						catch (Exception ex)
						{
							_Logger.Exception(ex, "Failed to load current image: {0}", ex.Message);
						}

						if (_Configuration.AutomaticMode == AutomaticMode.OnStartup)
						{
							NextWallpaper(true);
						}
						else if (_Configuration.AutomaticMode == AutomaticMode.TimeInterval)
						{
							_NextWallpaperTimeout.Change(_Configuration.AutomaticDelay, _Configuration.AutomaticDelay);
						}
					}
					else
					{
						NextWallpaper(false);
					}
				}
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
			NextWallpaper(true);
		}

		public void OpenWallpaperLink()
		{
			if (_Configuration.CurrentImage != null)
			{
				Process.Start(new ProcessStartInfo(_Configuration.CurrentImage.URL));
			}
			
		}

		public void ConfigureCurrentSource(object configuration)
		{
			if (CurrentSource != null)
			{
				// Possibly cancel pending fetch
				CancelPendingFetch();

				// Reconfigure the source
				CurrentSource.Configure(configuration);

				// Write configuration
				_Configuration.WriteSourceConfiguration(CurrentSource.GetName(), configuration);

				// Immediately schedule a new fetch
				NextWallpaper(false);
			}
		}

		#endregion

		#region IDisposable implementation

		public void Dispose()
		{
			// Prepare the message processor to shut down
			_CancelTokenSource.Cancel();

			// Cancel pending requests
			_FetchCancelTokenSource.Cancel();

			// Dispose the current source
			if (_CurrentSource != null)
			{
				_CurrentSource.Dispose();
			}

			// Wait for the thread to terminate
			_WallpaperServiceThread.Join();
		}

		#endregion

		#region Service thread

		private void EnqueueMessage(ServiceMessage msg)
		{
			try
			{
				_MessageQueue.Add(msg, _CancelTokenSource.Token);
			}
			catch (OperationCanceledException) { } // Ignore
		}

		private void WallpaperServiceThreadStart()
		{
			try
			{
				foreach (ServiceMessage msg in _MessageQueue.GetConsumingEnumerable(_CancelTokenSource.Token))
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
								_EventAggregator.GetEvent<WallpaperChangingEvent>().Publish((bool)msg.Data1);
								_CurrentSource.BeginFetchNext(_FetchCancelTokenSource.Token);
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
				_MessageQueue.CompleteAdding();
			}
			catch (OperationCanceledException) { } // Ignore
		}

		#endregion
	}
}
