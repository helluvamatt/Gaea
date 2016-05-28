using Microsoft.Win32;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Gaea.Api.Data;
using Newtonsoft.Json;
using System.Drawing;
using Gaea.Api.Configuration;
using System.Collections.Generic;
using Gaea.Services.Data;

namespace Gaea.Services.Impl
{
	internal class ConfigurationService : BindableBase, IConfigurationService
	{
		#region Constants

		public const string NAME_CURRENT_SOURCE = "CurrentSource"; // SZ
		public const string NAME_CURRENT_IMAGE = "CurrentImage"; // SZ
		public const string NAME_LAST_UPDATED = "LastUpdated"; // DWORD
		public const string NAME_BLUR = "Blur"; // DWORD
		public const string NAME_DARKEN = "Darken"; // DWORD
		public const string NAME_DESATURATE = "Desaturate"; // DWORD
		public const string NAME_OPTIMIZE_LAYOUT = "OptimizeLayout"; // DWORD 0x0 or 0x1

		private const string NAME_SOURCE_CONFIG = "Config"; // SZ

		public const int DEFAULT_BLUR = 128;
		public const int DEFAULT_DARKEN = 0;
		public const int DEFAULT_DESATURATE = 0;
		public const int DEFAULT_CACHE_COUNT = 1;

		private const string REGKEY_APPLICATION = "Software";

		private const string REGKEY_CURRENT_USER_RUN = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

		private const string REGKEY_SOURCE_CONFIGS = "SourceConfigs";

		#endregion

		#region Private members

		private ILoggingService _Logger;

		#endregion

		public ConfigurationService(ILoggingService logger)
		{
			_Logger = logger;
			_Logger.Info("Initializing ConfigurationService...");

			using (RegistryKey appKey = GetApplicationBaseKey())
			{
				_CurrentSource = (string)appKey.GetValue(NAME_CURRENT_SOURCE);
				var currentImageJson = (string)appKey.GetValue(NAME_CURRENT_IMAGE);
				if (currentImageJson != null)
				{
					_CurrentImage = JsonConvert.DeserializeObject<GaeaImage>(currentImageJson);
				}
				string lastUpdateStr = (string)appKey.GetValue(NAME_LAST_UPDATED);
				if (lastUpdateStr != null)
				{
					_LastUpdate = DateTime.Parse(lastUpdateStr);
				}
				else
				{
					_LastUpdate = DateTime.MinValue;
				}
				_Blur = (int)appKey.GetValue(NAME_BLUR, DEFAULT_BLUR);
				_Darken = (int)appKey.GetValue(NAME_DARKEN, DEFAULT_DARKEN);
				_Desaturate = (int)appKey.GetValue(NAME_DESATURATE, DEFAULT_DESATURATE);
				_OptimizeLayout = (int)appKey.GetValue(NAME_OPTIMIZE_LAYOUT, 1) == 1;
			}

			PropertyChanged += RegistryConfiguration_PropertyChanged;
		}

		#region Event handlers

		private void RegistryConfiguration_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			using (RegistryKey appKey = GetApplicationBaseKey())
			{
				switch (e.PropertyName)
				{
					case NAME_BLUR:
						appKey.SetValue(NAME_BLUR, _Blur, RegistryValueKind.DWord);
						RaiseConfigurationChanged();
						break;
					case NAME_DARKEN:
						appKey.SetValue(NAME_DARKEN, _Darken, RegistryValueKind.DWord);
						RaiseConfigurationChanged();
						break;
					case NAME_DESATURATE:
						appKey.SetValue(NAME_DESATURATE, _Desaturate, RegistryValueKind.DWord);
						RaiseConfigurationChanged();
						break;
					case NAME_OPTIMIZE_LAYOUT:
						appKey.SetValue(NAME_OPTIMIZE_LAYOUT, _OptimizeLayout ? 1 : 0, RegistryValueKind.DWord);
						RaiseConfigurationChanged();
						break;
					case NAME_CURRENT_SOURCE:
						appKey.SetValue(NAME_CURRENT_SOURCE, _CurrentSource, RegistryValueKind.String);
						break;
					case NAME_CURRENT_IMAGE:
						appKey.SetValue(NAME_CURRENT_IMAGE, JsonConvert.SerializeObject(_CurrentImage), RegistryValueKind.String);
						break;
					case NAME_LAST_UPDATED:
						appKey.SetValue(NAME_LAST_UPDATED, string.Format("{0:O}", _LastUpdate), RegistryValueKind.String);
						break;
				}
			}
		}

		#endregion

		#region Public methods

		public ConfigurationMetaModel BuildModelFromAttributes(object configObject)
		{
			ConfigurationMetaModel model = new ConfigurationMetaModel();
			if (configObject == null) return model;
			Type configType = configObject.GetType();
			PropertyInfo[] props = configType.GetProperties();
			foreach (PropertyInfo prop in props)
			{
				var attr = prop.GetCustomAttribute<ConfigurationItemAttribute>();
				if (attr != null)
				{
					if (attr.AllowedTypes.Contains(prop.PropertyType))
					{
						model.Data.Add(prop, attr);
					}
					else
					{
						throw new TypeNotAllowedException();
					}
				}
			}
			return model;
		}

		public void PersistModel(IEnumerable<SourceConfigItem> model, object configObject)
		{
			// Use reflection to copy values back to the config object
			Type objType = configObject.GetType();
			foreach (var item in model)
			{
				PropertyInfo prop = objType.GetProperty(item.Name);
				if (item.Value != null)
				{
					object value = item.Value;
					if (item.Value.GetType() != prop.PropertyType)
					{
						value = TypeDescriptor.GetProperties(configObject)[item.Name].Converter.ConvertTo(value, prop.PropertyType);
					}
					prop.SetValue(configObject, value);
				}
				else
				{
					prop.SetValue(configObject, null);
				}
			}
		}

		public void WriteSourceConfiguration(string sourceName, object configObject)
		{
			if (!string.IsNullOrEmpty(sourceName))
			{
				using (var appKey = GetApplicationBaseKey())
				{
					using (var sourcesKey = appKey.CreateSubKey(REGKEY_SOURCE_CONFIGS))
					{
						using (var sourceKey = sourcesKey.CreateSubKey(sourceName))
						{
							string serialized = JsonConvert.SerializeObject(configObject, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
							sourceKey.SetValue(NAME_SOURCE_CONFIG, serialized, RegistryValueKind.String);
						}
					}
				}
			}
		}

		public object GetSourceConfiguration(string sourceName)
		{
			if (!string.IsNullOrEmpty(sourceName))
			{
				using (var appKey = GetApplicationBaseKey())
				{
					using (var sourcesKey = appKey.CreateSubKey(REGKEY_SOURCE_CONFIGS))
					{
						using (var sourceKey = sourcesKey.CreateSubKey(sourceName))
						{
							string serialized = (string)sourceKey.GetValue(NAME_SOURCE_CONFIG);
							if (serialized == null) return null;
							return JsonConvert.DeserializeObject(serialized, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
						}
					}
				}
			}
			return null;
		}

		#endregion

		#region Assembly properties

		private string _ProductName;
		public string ProductName
		{
			get
			{
				if (_ProductName == null)
				{
					object[] attributes = GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
					AssemblyProductAttribute attribute = null;
					if (attributes.Length > 0)
					{
						attribute = attributes[0] as AssemblyProductAttribute;
					}
					_ProductName = attribute.Product;
				}
				return _ProductName;
			}
		}

		private string _CompanyName;
		public string CompanyName
		{
			get
			{
				if (_CompanyName == null)
				{
					object[] attributes = GetType().Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
					AssemblyCompanyAttribute attribute = null;
					if (attributes.Length > 0)
					{
						attribute = attributes[0] as AssemblyCompanyAttribute;
					}
					_CompanyName = attribute.Company;
				}
				return _CompanyName;
			}
		}

		private string _ExecutablePath;
		public string ExecutablePath
		{
			get
			{
				if (_ExecutablePath == null)
				{
					// Based on http://stackoverflow.com/a/28319367/407804 
					_ExecutablePath = Path.GetFullPath(Assembly.GetExecutingAssembly().CodeBase.Substring(@"file:///".Length).Replace('/', '\\'));
				}
				return _ExecutablePath;
			}
		}

		private string _AppDirectory;
		public string AppDirectory
		{
			get
			{
				if (_AppDirectory == null)
				{
					_AppDirectory = Path.GetDirectoryName(ExecutablePath);
				}
				return _AppDirectory;
			}
		}

		#endregion

		#region Utility methods

		private RegistryKey GetApplicationBaseKey()
		{
			string key = REGKEY_APPLICATION + "\\" + CompanyName + "\\" + ProductName;
			return RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).CreateSubKey(key);
		}

		#endregion

		#region Configuration properties

		private int _Blur;
		public int Blur
		{
			get
			{
				return _Blur;
			}
			set
			{
				SetProperty(ref _Blur, value);
			}
		}

		private int _Darken;
		public int Darken
		{
			get
			{
				return _Darken;
			}
			set
			{
				SetProperty(ref _Darken, value);
			}
		}

		private int _Desaturate;
		public int Desaturate
		{
			get
			{
				return _Desaturate;
			}
			set
			{
				SetProperty(ref _Desaturate, value);
			}
		}

		private bool _OptimizeLayout;
		public bool OptimizeLayout
		{
			get
			{
				return _OptimizeLayout;
			}
			set
			{
				SetProperty(ref _OptimizeLayout, value);
			}
		}

		private string _CurrentSource;
		public string CurrentSource
		{
			get
			{
				return _CurrentSource;
			}
			set
			{
				SetProperty(ref _CurrentSource, value);
			}
		}

		private GaeaImage _CurrentImage;
		public GaeaImage CurrentImage
		{
			get
			{
				return _CurrentImage;
			}

			set
			{
				SetProperty(ref _CurrentImage, value);
				RaiseCurrentImageChanged();
			}
		}

		private DateTime _LastUpdate;
		public DateTime LastUpdate
		{
			get
			{
				return _LastUpdate;
			}
			set
			{
				SetProperty(ref _LastUpdate, value);
			}
		}

		/// <summary>
		/// Should the application be started at login? This writes directly to the Windows registry
		/// </summary>
		public bool AutoStart
		{
			get
			{
				using (RegistryKey runKey = Registry.CurrentUser.OpenSubKey(REGKEY_CURRENT_USER_RUN))
				{
					return runKey.GetValue(ProductName) != null;
				}
			}
			set
			{
				using (RegistryKey runKey = Registry.CurrentUser.OpenSubKey(REGKEY_CURRENT_USER_RUN))
				{
					if (value)
					{
						var command = ExecutablePath + " --startup";
						runKey.SetValue(ProductName, command, RegistryValueKind.String);
					}
					else
					{
						runKey.DeleteValue(ProductName);
					}
				}
			}
		}

		public Rectangle ScreenBounds
		{
			get
			{
				Rectangle screenBounds = new Rectangle();
				foreach (var screen in System.Windows.Forms.Screen.AllScreens)
				{
					screenBounds = Rectangle.Union(screenBounds, screen.Bounds);
				}
				return screenBounds;
			}
		}

		#endregion

		#region Events

		protected void RaiseConfigurationChanged()
		{
			if (ConfigurationChanged != null)
			{
				ConfigurationChanged(this, EventArgs.Empty);
			}
		}
		public event EventHandler ConfigurationChanged;

		private void RaiseCurrentImageChanged()
		{
			if (CurrentImageChanged != null)
			{
				CurrentImageChanged(this, EventArgs.Empty);
			}
		}

		public event EventHandler CurrentImageChanged;

		#endregion
	}
}
