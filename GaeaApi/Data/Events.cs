using Prism.Events;

namespace Gaea.Api.Data
{
	/// <summary>
	/// Event indicating the wallpaper image is changing.
	/// 
	/// Payload: Boolean indicating if the change was triggered manually (by the user) or not
	/// </summary>
	public class WallpaperChangingEvent : PubSubEvent<bool> { }

	/// <summary>
	/// Event indicating the wallpaper image has changed.
	/// 
	/// Payload: The new GaeaImage object
	/// </summary>
	public class WallpaperChangedEvent : PubSubEvent<GaeaImage> { }

	/// <summary>
	/// Event indicating there was an error
	/// 
	/// Payload: Object representing the error
	/// </summary>
	public class WallpaperServiceErrorEvent : PubSubEvent<WallpaperServiceError> { }

	/// <summary>
	/// Wallpaper service error descriptor object
	/// </summary>
	public class WallpaperServiceError
	{
		/// <summary>
		/// Subject of the error, used as a title for a dialog
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Message of the error, used as the body for a dialog
		/// </summary>
		public string Message { get; set; }
	}
}
