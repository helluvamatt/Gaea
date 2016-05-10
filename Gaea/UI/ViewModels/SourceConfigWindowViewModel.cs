using Gaea.Services;
using Gaea.UI.Domain;
using System;
using System.Collections.Generic;

namespace Gaea.UI.ViewModels
{
	internal class SourceConfigWindowViewModel : LocalizableViewModel
	{
		public SourceConfigWindowViewModel(IWallpaperService wallpaperService, IConfiguration configService)
		{
			ConfigService = configService;
			WallpaperService = wallpaperService;

			object configObj = WallpaperService.CurrentSource.Configuration;
			List<SourceConfigItem> itemsModel = new List<SourceConfigItem>();
			foreach (var kvp in ConfigService.CurrentSourceConfigurationMetaModel.Data)
			{
				string name = kvp.Key.Name;
				Type objType = configObj.GetType();
				SourceConfigItem item = new SourceConfigItem();
				item.Attribute = kvp.Value;
				item.Name = name;
				item.Value = objType.GetProperty(name).GetValue(configObj);
				itemsModel.Add(item);
			}
			itemsModel.Sort();
			ItemsModel = itemsModel;
		}

		public IConfiguration ConfigService { get; private set; }
		public IWallpaperService WallpaperService { get; private set; }

		public IEnumerable<SourceConfigItem> ItemsModel { get; private set; }

		// TODO Need a way to save configs and pass them back to the source and the ConfigService to be persisted
	}
}
