using Prism.Unity;
using System;
using Microsoft.Practices.Unity;
using Gaea.UI;
using Gaea.UI.Domain;
using NLog;
using Gaea.Services;
using Gaea.Services.Impl;
using Mono.Options;
using System.Windows;
using Prism.Modularity;
using Prism.Commands;
using Prism.Logging;
using Gaea.UI.ViewModels;
using Gaea.Api;

namespace Gaea
{
	internal class Bootstrapper : UnityBootstrapper, IDisposable
	{
		#region Private members

		private bool showWindow;
		private ILoggingService _LoggingService;
		private TrayIcon trayIcon;

		private LogWindow _LogWindow;

		#endregion

		public Bootstrapper(string[] args)
		{
			showWindow = true;
			var options = new OptionSet()
			{
				{ "s|startup", v => showWindow = false }
			};
			var extra = options.Parse(args);
			GlobalCommands.ShowConfigCommand.RegisterCommand(new DelegateCommand(ShowConfig));
			GlobalCommands.ShowLogWindowCommand.RegisterCommand(new DelegateCommand(ShowLogWindow));
		}

		#region Prism application lifecycle

		public override void Run(bool runWithDefaultConfiguration)
		{
			base.Run(runWithDefaultConfiguration);
			InitializeUI();
		}

		protected override ILoggerFacade CreateLogger()
		{
			_LoggingService = new NLogLoggingService(LogManager.GetCurrentClassLogger());
			return _LoggingService;
		}

		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();

			Container.RegisterInstance(_LoggingService, new ContainerControlledLifetimeManager());
			Container.RegisterType<NLogEventEmitter>();

			Container.RegisterType<ILocalizationService, DummyLocalizationService>(new ContainerControlledLifetimeManager());
			Container.RegisterType<IWallpaperService, WallpaperService>(new ContainerControlledLifetimeManager());
			Container.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());
			Container.RegisterType<IImageManager, FileCacheImageManager>(new ContainerControlledLifetimeManager());
			Container.RegisterType<IImageProcessor, GdiPlusImageProcessor>(new ContainerControlledLifetimeManager());
			Container.RegisterType<ISourceRegistry, SourceRegistry>(new ContainerControlledLifetimeManager());

			Container.RegisterType<LogWindowViewModel>();
			Container.RegisterType<LogWindow>();
			Container.RegisterType<ConfigWindowViewModel>();
			Container.RegisterType<ConfigWindow>();
			Container.RegisterType<TrayIconViewModel>();
			Container.RegisterType<TrayIcon>();
			Container.RegisterType<SourceConfigWindowViewModel>();
			Container.RegisterType<SourceConfigWindow>();
		}

		protected override IModuleCatalog CreateModuleCatalog()
		{
			return new DirectoryModuleCatalog() { ModulePath = @".\Plugins" };
		}

		protected override DependencyObject CreateShell()
		{
			return Container.Resolve<ConfigWindow>();
		}

		#endregion

		#region Utility methods

		private void InitializeUI()
		{
			// Initialize tray icon
			trayIcon = Container.Resolve<TrayIcon>();

			if (showWindow)
			{
				ShowConfig();
			}

#if DEBUG
			ShowLogWindow();
#endif

			// Start emitting LogEvents
			Container.Resolve<NLogEventEmitter>();
		}

		private void ShowConfig()
		{
			var configWindow = ((ConfigWindow)Shell);
			ActivateWindow(configWindow);
		}

		private void ShowLogWindow()
		{
			if (_LogWindow == null)
			{
				_LogWindow = Container.Resolve<LogWindow>();
				_LogWindow.Closed += _LogWindow_Closed;
				var configWindow = ((ConfigWindow)Shell);
				if (configWindow != null && configWindow.IsVisible && configWindow.WindowState == WindowState.Normal)
				{
					_LogWindow.WindowStartupLocation = WindowStartupLocation.Manual;
					_LogWindow.Left = configWindow.Left + configWindow.Width;
					_LogWindow.Top = configWindow.Top;
				}
			}
			ActivateWindow(_LogWindow);
		}

		private void _LogWindow_Closed(object sender, EventArgs e)
		{
			_LogWindow = null;
		}

		private void ActivateWindow(Window window)
		{
			// From: http://stackoverflow.com/a/4831839/407804
			if (!window.IsVisible)
			{
				window.Show();
			}

			if (window.WindowState == WindowState.Minimized)
			{
				window.WindowState = WindowState.Normal;
			}

			window.Activate();
			window.Topmost = true;  // important
			window.Topmost = false; // important
			window.Focus();         // important
		}

		#endregion

		#region IDisposable impl

		public void Dispose()
		{
			trayIcon.Dispose();
			Container.Dispose();
		}

		#endregion
	}
}
