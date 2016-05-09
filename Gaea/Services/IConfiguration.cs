using Gaea.Api;
using Gaea.Api.Configuration;
using Gaea.Api.Data;
using System;
using System.Drawing;

namespace Gaea.Services
{
	public interface IConfiguration
	{
		#region Methods

		void BuildModelFromAttributes(object configObject);

		#endregion

		#region Assembly attributes

		/// <summary>
		/// Name of the application
		/// </summary>
		string ProductName { get; }

		/// <summary>
		/// Publisher name of the application
		/// </summary>
		string CompanyName { get; }

		/// <summary>
		/// Absolute path to the executable
		/// </summary>
		string ExecutablePath { get; }

		/// <summary>
		/// App base directory
		/// </summary>
		string AppDirectory { get; }

		#endregion

		#region Configuration properties

		/// <summary>
		/// Blur the image
		/// </summary>
		int Blur { get; set; }

		/// <summary>
		/// Darken the image
		/// </summary>
		int Darken { get; set; }

		/// <summary>
		/// Desaturate the image
		/// </summary>
		int Desaturate { get; set; }

		/// <summary>
		/// Optimize the processed image for the user's desktop layout
		/// </summary>
		bool OptimizeLayout { get; set; }

		/// <summary>
		/// Currently selected source
		/// </summary>
		string CurrentSource { get; set; }

		/// <summary>
		/// Currently display image
		/// </summary>
		GaeaImage CurrentImage { get; set; }

		/// <summary>
		/// Model representing the configuration of the current source
		/// </summary>
		ConfigurationMetaModel CurrentSourceConfigurationMetaModel { get; }

		/// <summary>
		/// Timestamp for the last wallpaper update
		/// </summary>
		DateTime LastUpdate { get; set; }

		/// <summary>
		/// Should the application be started at login? Implementation should handle configuring the system.
		/// </summary>
		bool AutoStart { get; set; }

		/// <summary>
		/// Get a Rectangle representing the size of the entire desktop (including all active monitors)
		/// </summary>
		Rectangle ScreenBounds { get; }

		#endregion

		#region Events

		event EventHandler ConfigurationChanged;
		event EventHandler CurrentImageChanged;

		#endregion
	}
}
