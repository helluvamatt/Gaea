using MaterialDesignThemes.Wpf;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gaea.UI
{
	/// <summary>
	/// Interaction logic for Balloon.xaml
	/// </summary>
	public partial class Balloon : UserControl
	{
		public Balloon(string title, string caption, PackIconKind icon)
		{
			InitializeComponent();
			textBlockTitle.Text = title;
			textBlockCaption.Text = caption;
			mdIcon.Kind = icon;
			_Timer = new Timer(_Timer_Elapsed);
		}

		public event EventHandler BalloonClicked;
		public event CancelEventHandler BalloonClosing;
		public event EventHandler BalloonClosed;

		private Timer _Timer;

		#region Event handlers

		private void Balloon_Loaded(object sender, RoutedEventArgs e)
		{
			StartClosingTimeout();
		}

		private void Balloon_MouseEnter(object sender, MouseEventArgs e)
		{
			if (_Timer != null)
			{
				_Timer.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}

		private void Balloon_MouseLeave(object sender, MouseEventArgs e)
		{
			StartClosingTimeout();
		}

		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			HandleClosing();	
		}

		private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (BalloonClicked != null)
			{
				BalloonClicked(this, EventArgs.Empty);
			}
		}

		private void _Timer_Elapsed(object state)
		{
			HandleClosing();
		}

		#endregion

		#region Utility methods

		private void StartClosingTimeout()
		{
			if (_Timer != null)
			{
				_Timer.Change(10000, Timeout.Infinite);
			}
		}

		private void HandleClosing()
		{
			if (BalloonClosing != null)
			{
				CancelEventArgs args = new CancelEventArgs();
				BalloonClosing(this, args);
				if (args.Cancel)
				{
					return;
				}
			}

			if (BalloonClosed != null)
			{
				BalloonClosed(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
