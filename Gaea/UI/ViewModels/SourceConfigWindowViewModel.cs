﻿using Gaea.Services;
using Gaea.Services.Data;
using Gaea.UI.Domain;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Gaea.UI.ViewModels
{
	internal class SourceConfigWindowViewModel : LocalizableViewModel
	{
		public SourceConfigWindowViewModel(ILoggingService loggingService, IWallpaperService wallpaperService, IConfigurationService configService)
		{
			LoggingService = loggingService;
			ConfigService = configService;
			WallpaperService = wallpaperService;

			AcceptCommand = new DelegateCommand(Accept);
		}

		public ILoggingService LoggingService { get; private set; }
		public IConfigurationService ConfigService { get; private set; }
		public IWallpaperService WallpaperService { get; private set; }

		private IEnumerable<SourceConfigItem> _ItemsModel;
		public IEnumerable<SourceConfigItem> ItemsModel
		{
			get
			{
				if (_ItemsModel == null)
				{
					object configObj = WallpaperService.CurrentSource.Configuration;
					Type objType = configObj.GetType();
					List<SourceConfigItem> itemsModel = new List<SourceConfigItem>();
					foreach (var kvp in WallpaperService.CurrentSourceConfigurationMetaModel.Data)
					{
						string name = kvp.Key.Name;
						SourceConfigItem item = new SourceConfigItem(name, kvp.Value);
						item.Value = objType.GetProperty(name).GetValue(configObj);
						itemsModel.Add(item);
					}
					itemsModel.Sort();
					_ItemsModel = itemsModel;
				}
				return _ItemsModel;
			}
		}

		#region Events

		public event EventHandler<DismissDialogEventArgs> DismissDialog;
		private void RaiseDismissDialog(bool result)
		{
			if (DismissDialog != null)
			{
				DismissDialog(this, new DismissDialogEventArgs { Result = result });
			}
		}

		public event EventHandler<ErrorEventArgs> Error;
		private void RaiseError(string subject, string message)
		{
			if (Error != null)
			{
				Error(this, new ErrorEventArgs { Subject = subject, Message = message });
			}
		}

		#endregion

		#region Commands

		public ICommand AcceptCommand { get; private set; }

		private void Accept()
		{
			if (WallpaperService == null || WallpaperService.CurrentSource == null) throw new ArgumentNullException("WallpaperService.CurrentSource");

			var configObj = WallpaperService.CurrentSource.Configuration;

			try
			{
				// Validate the model and copy properties back to the configObj
				ConfigService.PersistModel(ItemsModel, configObj);
			}
			catch (Exception ex)
			{
				LoggingService.Exception(ex, "Exception caught from PersistModel: {0}", ex.Message);
				RaiseError("Error", "There was a problem configuring the source: " + ex.Message);
				return;
			}

			try
			{
				// Inform the WallpaperService that we need to configure the source
				WallpaperService.ConfigureCurrentSource(configObj);
			}
			catch (Exception ex)
			{
				LoggingService.Exception(ex, "Exception caught from ConfigureCurrentSource: {0}", ex.Message);
				RaiseError("Error", "There was a problem configuring the source: " + ex.Message);
				return;
			}

			RaiseDismissDialog(true);
		}

		#endregion
	}
}
