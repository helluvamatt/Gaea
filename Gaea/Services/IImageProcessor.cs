using System;
using System.Drawing;

namespace Gaea.Services
{
	public interface IImageProcessor
	{
		Bitmap PostProcess(Bitmap original, int blur, int darken, int desaturate, bool optimizeLayout, int height, int width);
	}
}
