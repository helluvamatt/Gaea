using GDIPlusX.GDIPlus11.Effects;
using System.Drawing;

namespace Gaea.Services.Impl
{
	internal class GdiPlusImageProcessor : IImageProcessor
	{
		public Bitmap PostProcess(Bitmap original, int blur, int darken, int desaturate, bool optimizeLayout, int height, int width)
		{
			Size targetSize = new Size { Width = original.Width, Height = original.Height };
			RectangleF destRect = new RectangleF { X = 0, Y = 0, Width = original.Width, Height = original.Height };
			RectangleF srcRect = destRect;
			if (optimizeLayout)
			{
				targetSize.Width = width;
				targetSize.Height = height;
				destRect.Width = width;
				destRect.Height = height;
				srcRect = Utils.GetCenteredCropRectangle(new SizeF { Width = original.Width, Height = original.Height }, width, height);
			}
			var tempBitmap = new Bitmap(targetSize.Width, targetSize.Height);
			using (var g = Graphics.FromImage(tempBitmap))
			{
				g.DrawImage(original, destRect, srcRect, GraphicsUnit.Pixel);
			}

			if (blur > 0)
			{
				BlurEffect blurEffect = new BlurEffect(blur, false);
				tempBitmap.ApplyEffect(blurEffect, Rectangle.Empty);
			}

			if (darken > 0 || desaturate > 0)
			{
				HueSaturationLightnessEffect hslEffect = new HueSaturationLightnessEffect(0, -desaturate, -darken);
				tempBitmap.ApplyEffect(hslEffect, Rectangle.Empty);
			}

			return tempBitmap;
		}
	}
}
