using Gaea.Api.Data;

namespace Gaea.Services.Data
{
	public class PostProcessMessage
	{
		public string CacheDir { get; set; }
		public string SourceName { get; set; }

		public int Blur { get; set; }
		public int Darken { get; set; }
		public int Desaturate { get; set; }
		public bool OptimizeLayout { get; set; }
		public int ScreenWidth { get; set; }
		public int ScreenHeight { get; set; }

		public int PreviewWidth { get; set; }
		public int PreviewHeight { get; set; }

		public GaeaImage Image { get; set; }
	}
}
