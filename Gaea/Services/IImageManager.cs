using Gaea.Api.Data;
using Gaea.Services.Data;
using System;

namespace Gaea.Services
{
	public interface IImageManager : IDisposable
	{
		void EnqueueUpdate(PostProcessMessage message);

		event EventHandler<PostProcessCompletedEventArgs> Complete;
		event EventHandler<WallpaperServiceError> Error;
	}
}
