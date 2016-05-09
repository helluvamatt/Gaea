using Gaea.UI.Domain;
using Prism.Commands;
using System;
using System.Windows;
using System.Windows.Input;

namespace Gaea
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		#region Private members

		private Bootstrapper bootstrapper;

		private readonly ICommand shutdownCommand;

		#endregion

		public App()
		{
			shutdownCommand = new DelegateCommand(GracefulShutdown);
		}

		internal Bootstrapper Bootstrapper
		{
			get
			{
				return bootstrapper;
			}
		}

		public static new App Current
		{
			get
			{
				return (App)Application.Current;
			}
		}

		private void GracefulShutdown()
		{
			if (bootstrapper != null)
			{
				bootstrapper.Dispose();
			}
			Dispatcher.InvokeAsync(() => Shutdown(0));
		}

		#region Event handlers

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Wire shutdown command
			GlobalCommands.ShutdownCommand.RegisterCommand(shutdownCommand);

			// Create and run bootstrapper
			bootstrapper = new Bootstrapper(e.Args);
			bootstrapper.Run();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			Environment.Exit(0);
		}

		#endregion
	}
}
