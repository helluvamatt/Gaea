using Gaea.Services.Data;
using System;
using System.Threading;
using Gaea.Api.Data;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Diagnostics;

namespace Gaea.Services.Impl
{
	public class FileCacheImageManager : IImageManager
	{

		#region Private members

		private const int CONFIG_UPDATE_DELAY_MS = 1000;
		private static readonly TimeSpan CONFIG_UPDATE_DELAY = TimeSpan.FromMilliseconds(CONFIG_UPDATE_DELAY_MS);

		private ILoggingService _Logger;
		private IImageProcessor _ImageProcessor;
		private CancellationTokenSource _CancelTokenSource;

		private object _LockObject;
		private PostProcessMessage _QueuedJob;
		private Timer _EnqueueTimer;

		#endregion

		public FileCacheImageManager(ILoggingService logger, IImageProcessor imageProcessor)
		{
			_Logger = logger;
			_ImageProcessor = imageProcessor;
			_LockObject = new { };
			_CancelTokenSource = new CancellationTokenSource();
			_EnqueueTimer = new Timer(PostProcessImage);
		}

		#region Public methods

		public void EnqueueUpdate(PostProcessMessage message)
		{
			lock (_LockObject)
			{
				_QueuedJob = message;
				_EnqueueTimer.Change(CONFIG_UPDATE_DELAY_MS, Timeout.Infinite);
			}
		}

		#endregion

		#region Post-processing threads

		private void PostProcessImage(object state)
		{
			PostProcessMessage message;
			lock (_LockObject)
			{
				if (_QueuedJob == null) return;
				message = _QueuedJob;
				_QueuedJob = null;
			}

			try
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();

				_CancelTokenSource.Token.ThrowIfCancellationRequested();
				PostProcessCompletedEventArgs completed = new PostProcessCompletedEventArgs();

				_Logger.Info("Post processing image: {0}", message.Image.Title);
				_Logger.Debug("Blur = {0}  Darken = {1}  Desat = {2}  OptimizeLayout = {3}  ScreenWidth = {4}  ScreenHeight = {5}", message.Blur, message.Darken, message.Desaturate, message.OptimizeLayout, message.ScreenWidth, message.ScreenHeight);
				_Logger.Debug("SourceName = {0}  CacheDir = {1}", message.SourceName, message.CacheDir);

				// Update timestamp
				var ts = DateTime.Now;

				// Prepare cache if it isn't already available
				if (!Directory.Exists(message.CacheDir))
				{
					Directory.CreateDirectory(message.CacheDir);
				}

				// Cache images
				string cacheFileBaseName = string.Format("{0}_{1}_{2}_{3}_{4}", message.SourceName, ts.ToFileTime(), message.Blur, message.Darken, message.Desaturate);
				string cacheFileName = cacheFileBaseName + ".png";
				string cacheProcessedFileName = cacheFileBaseName + "_processed.png";

				// Save unprocessed image to cache
				message.Image.RawCacheUrl = Path.Combine(message.CacheDir, cacheFileName);
				using (FileStream fileStream = new FileStream(message.Image.RawCacheUrl, FileMode.Create, FileAccess.ReadWrite))
				{
					using (Bitmap rawCopy = new Bitmap(message.Image.Image))
					{
						rawCopy.Save(fileStream, ImageFormat.Png);
					}
				}

				// Determine processed image location
				message.Image.ProcessedRawCacheUrl = Path.Combine(message.CacheDir, cacheProcessedFileName);

				// Post-process and save image
				message.Image.ProcessedImage = _ImageProcessor.PostProcess(message.Image.Image, message.Blur, message.Darken, message.Desaturate, message.OptimizeLayout, message.ScreenHeight, message.ScreenWidth);
				using (FileStream processedImageFileStream = new FileStream(message.Image.ProcessedRawCacheUrl, FileMode.Create, FileAccess.ReadWrite))
				{
					using (Bitmap processedRawCopy = new Bitmap(message.Image.ProcessedImage))
					{
						processedRawCopy.Save(processedImageFileStream, ImageFormat.Png);
					}
				}

				sw.Stop();
				_Logger.Info("Finished post-processing in {0} ms.", sw.ElapsedMilliseconds);

				completed.Image = message.Image;
				RaiseCompleteEvent(completed);
			}
			catch (OperationCanceledException) { return; } // Ignore and bail out
			catch (Exception ex)
			{
				_Logger.Exception(ex, "Error processing image: \"{0}\": {1}", message.Image.Title, ex.Message);
				RaiseErrorEvent("There was an error processing the image: " + ex.Message);
			}
		}

		#endregion

		#region IDisposable impl

		public void Dispose()
		{
			_CancelTokenSource.Cancel();
			_CancelTokenSource.Dispose();
			_EnqueueTimer.Dispose();
		}

		#endregion

		#region Events

		private void RaiseCompleteEvent(PostProcessCompletedEventArgs args)
		{
			if (Complete != null)
			{
				Complete(this, args);
			}
		}
		public event EventHandler<PostProcessCompletedEventArgs> Complete;

		private void RaiseErrorEvent(string message)
		{
			if (Error != null)
			{
				Error(this, new WallpaperServiceError { Message = message, Subject = "Image Processing Error" });
			}
		}
		public event EventHandler<WallpaperServiceError> Error;
		
		#endregion

	}
}
