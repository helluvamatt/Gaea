using Gaea.Api;
using Gaea.Api.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleSource
{
	public class ExampleGaeaSource : ISource
	{
		// Keep state here
		private int currentIndex;

		// This example is just a static list of embedded images, but the asynchrounous 
		private List<GaeaImage> images;

		private ExampleSourceConfig _Configuration = new ExampleSourceConfig() { SkipThirdImage = false, FetchDelay = new TimeSpan(0) };

		#region ISource implementation

		public string DisplayName
		{
			get
			{
				return "Example Source";
			}
		}

		public string Description
		{
			get
			{
				return "Example source with two included images.";
			}
		}

		public Icon Icon
		{
			get
			{
				return new Icon(typeof(ExampleGaeaSource), "ExampleSource.ico");
			}
		}

		public object Configuration
		{
			get
			{
				return _Configuration;
			}
		}

		public void Initialize()
		{
			// Here you would initialize the source given it's config loaded from the registry
			images = new List<GaeaImage>();

			using (Stream s1 = GetType().Assembly.GetManifestResourceStream("ExampleSource.Assets.example_flowers.jpg"))
			{
				Bitmap b1 = new Bitmap(s1);
				GaeaImage g1 = new GaeaImage();
				g1.Title = "Crocus tommasinianus, flowers";
				g1.Description = "Flowering Woodland crocus in the garden reserve Jonkervallei, Joure, Netherlands.";
				g1.URL = "https://commons.wikimedia.org/wiki/File:Krokussen_(Crocus),_Locatie,_Tuinreservaat_Jonkervallei.jpg";
				g1.ImageURL = "https://upload.wikimedia.org/wikipedia/commons/0/0a/Krokussen_%28Crocus%29%2C_Locatie%2C_Tuinreservaat_Jonkervallei.jpg";
				g1.License = new License {
					Name = "Creative Commons Attribution-Share Alike 4.0 International",
					URL = "https://creativecommons.org/licenses/by-sa/4.0/deed.en"
				};
				g1.Image = b1;
				images.Add(g1);
			}

			using (Stream s2 = GetType().Assembly.GetManifestResourceStream("ExampleSource.Assets.example_yosemite.jpg"))
			{
				Bitmap b2 = new Bitmap(s2);
				GaeaImage g2 = new GaeaImage();
				g2.Title = "Yosemite meadows 2004-09-04";
				g2.Description = "Yosemite Valley meadows – Half Dome — in Yosemite National Park, California, USA.";
				g2.URL = "https://commons.wikimedia.org/wiki/File:Yosemite_meadows_2004-09-04.jpg";
				g2.ImageURL = "https://upload.wikimedia.org/wikipedia/commons/9/91/Yosemite_meadows_2004-09-04.jpg";
				g2.License = new License
				{
					Name = "CC0 - Public Domain",
					URL = "https://creativecommons.org/publicdomain/zero/1.0/"
				};
				g2.Image = b2;
				images.Add(g2);
			}
		}

		public void Configure(object configuration)
		{
			// Configure the source given the configuration object
			_Configuration = (ExampleSourceConfig)configuration;

			// Gaea will generally call BeginFetchNext(...) immediately after calling this

		}

		public void Dispose()
		{
			// Perform clean up, including canceling any pending fetch operations. At this point, this source object is no longer needed and will be recreated if needed.
		}

		// Call this event handler when the fetch is complete
		public event FetchNextCompleteHandler FetchNextComplete;

		// Call this event handler if there was a problem with the fetch
		public event FetchErrorHandler FetchError;

		public void BeginFetchNext(CancellationToken cancelToken)
		{
			// Here, you would fire an asynchronous job to fetch the next image.
			// You might download a feed based on state, then download the image associated with the next feed item.

			// For this example, we just advance the list index, then call the completed handler with the image from that index
			// Additionally, if the index is 2 (the same as the size of the array, we return an error to show the error handling
			currentIndex++;
			if (_Configuration.SkipThirdImage && currentIndex > (images.Count - 1)) currentIndex = 0;
			if (currentIndex > images.Count) currentIndex = 0;
			if (currentIndex < images.Count)
			{
				Task.Factory.StartNew(() => {
					Thread.Sleep(_Configuration.FetchDelay);
					if (FetchNextComplete != null)
					{
						FetchNextComplete(images[currentIndex]);
					}
				});
			}
			else
			{
				if (FetchError != null)
				{
					// Eventually gets passed to a two line bubble tooltip
					FetchError("Third Example Image", "Failed to load the third example image. This is an example error popup.");
				}
			}
		}

		public void CancelFetch()
		{
			// Do nothing
			// Here, you can cancel the asynchrounous fetch operation
		}

		#endregion
	}
}
