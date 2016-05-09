using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;

namespace Gaea.Api.Data
{
	/// <summary>
	/// Represents an image that is to be displayed
	/// </summary>
	public class GaeaImage
	{
		/// <summary>
		/// Title of image
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// URL to web page for the image (eg. author webpage URL, Flickr page, Reddit post, etc.)
		/// </summary>
		public string URL { get; set; }

		/// <summary>
		/// URL to download the full-resolution image
		/// </summary>
		public string ImageURL { get; set; }

		/// <summary>
		/// Image description, if applicable
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Image's license
		/// </summary>
		public License License { get; set; }

		/// <summary>
		/// Cached location of unprocessed image (if it exists)
		/// </summary>
		public string RawCacheUrl { get; set; }

		/// <summary>
		/// Cached location of processed image (if it exists)
		/// </summary>
		public string ProcessedRawCacheUrl { get; set; }

		/// <summary>
		/// Original image bitmap
		/// </summary>
		[JsonIgnore]
		public Bitmap Image { get; set; }

		/// <summary>
		/// Processed image bitmap
		/// </summary>
		[JsonIgnore]
		public Bitmap ProcessedImage { get; set; }

		/// <summary>
		/// Source (plugin) class
		/// </summary>
		[JsonIgnore]
		public ISource Source { get; set; }
	}
}
