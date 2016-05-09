using Prism.Events;

namespace Gaea.Api.Data
{
	public class WallpaperChangedEvent : PubSubEvent<GaeaImage> { }

	public class WallpaperServiceErrorEvent : PubSubEvent<WallpaperServiceError> { }

	public class WallpaperServiceError
	{
		public string Subject { get; set; }
		public string Message { get; set; }
	}
}
