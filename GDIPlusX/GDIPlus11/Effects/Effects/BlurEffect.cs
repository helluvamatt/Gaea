//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Imaging;
using GDIPlusX.GDIPlus11.EffectsInternal;
using GDIPlusX.GDIPlus11.Internal;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates an effect which guassian blurs an area.
    /// </summary>
    /// <see>http://msdn.microsoft.com/en-us/library/ms534422(v=VS.85).aspx</see>
    public class BlurEffect : LegacyEffect
    {
        #region Protected Static Locals

        /// <summary>
        /// GUID for the GDI+ Blur effect.
        /// </summary>
        protected static Guid mgEffectGuid = new Guid("{633C80A4-1843-482B-9EF2-BE2834C5FDD4}");

        #endregion

        #region Private Locals

        /// <summary>
        /// Holds the parameters for the blur effect.
        /// </summary>
        private Interop11.BlurParams mbpParams;

        /// <summary>
        /// Holds the last bitmap blur legacy object.
        /// </summary>
        LegacyBitmapBlur mlbbBlur = null;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new blur effect.
        /// </summary>
        public BlurEffect()
            : this(0, false)
        {
        }

        /// <summary>
        /// Creates a new blur effect.
        /// </summary>
        /// <param name="radius">The radius of the blur calculation. 0f to 255f.</param>
        /// <param name="expandEdges">True to expand the edges of the output area for the blur effect.</param>
        public BlurEffect(float radius, bool expandEdges)
            : base(mgEffectGuid)
        {
            Radius = radius;
            ExpandEdges = expandEdges;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Gets whether the parameters object is pinnable for this effect
        /// </summary>
        /// <returns>True if its pinnable, false to copy to other memory first</returns>
        protected override bool ParametersPinnable()
        {
            return false;
        }

        /// <summary>
        /// Returns the parameter data for this effect.
        /// </summary>
        /// <returns>An object containing the parameter data.</returns>
        protected override object InitialiseParameterData()
        {
            mlbbBlur = null;
            return mbpParams;
        }

        /// <summary>
        /// Gets the pixel format for the legacy clone apply function.
        /// </summary>
        /// <returns>The pixel format.</returns>
        protected override PixelFormat LegacyCloneApplyPixelFormat()
        {
            return PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Copies an area bitmap and applys an effect using legacy code.
        /// </summary>
        /// <param name="bitmap">The Bitmap to apply the effect to.</param>
        /// <param name="rect">
        /// The rectangle to apply the Effect or Rectangle.Empty for 
        /// entire bitmap, on out the area actually applied.
        /// </param>
        /// <returns>A new bitmap object with the effect applied.</returns>
        /// <exception cref="GDIPlusX.GDIPlus11.GDIPlus11NotAvailableException">GDI Plus 1.1 functions not available.</exception>
        /// <exception cref="System.ArgumentNullException">bitmap is null or effect is null.</exception>
        /// <exception cref="GDIPlusX.GDIPlus11.Effects.EffectValidateException">Effect validation with bitmap failed.</exception>
        /// <remarks>Auxillary data is calculated if the effect supports it.</remarks>
        protected override Bitmap LegacyCloneApply(Bitmap bitmap, ref Rectangle rect)
        {
            // Intersect the rectangle
            Rectangle lrEntire = new Rectangle(new Point(), bitmap.Size);
            rect.Intersect(lrEntire);

            // Clone the bitmap
            Bitmap lbmpBitmap;

            if (mbpParams.ExpandEdges)
            {
                int liRadius = (int)Math.Ceiling(Radius);

                lbmpBitmap = new Bitmap(
                    lrEntire.Width + liRadius * 2,
                    lrEntire.Height + liRadius * 2,
                    LegacyCloneApplyPixelFormat());

                using (Graphics lgGraphics = Graphics.FromImage(lbmpBitmap))
                {
                    lgGraphics.DrawImage(
                        bitmap,
                        new Rectangle(liRadius, liRadius, bitmap.Width, bitmap.Height),
                        new Rectangle(new Point(), bitmap.Size),
                        GraphicsUnit.Pixel);
                }
            }
            else
            {
                lbmpBitmap = bitmap.Clone(lrEntire, LegacyCloneApplyPixelFormat());

                using (Graphics lgGraphics = Graphics.FromImage(lbmpBitmap))
                {
                    lgGraphics.DrawImage(
                        bitmap,
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        new Rectangle(new Point(), bitmap.Size),
                        GraphicsUnit.Pixel);
                }
            }

            // Apply to bitmap
            lbmpBitmap.ApplyEffect(this, rect);

            if (mbpParams.ExpandEdges)
                rect.Inflate((int)Math.Ceiling(Radius), (int)Math.Ceiling(Radius));

            // Return the value
            return lbmpBitmap;
        }

        /// <summary>
        /// Applys an effect to a Bitmap using legacy code.
        /// </summary>
        /// <param name="bitmap">The Bitmap to apply the effect to.</param>
        /// <param name="rectOfInterest">
        /// The rectangle to apply the Effect or Rectangle.Empty 
        /// for entire bitmap.
        /// </param>
        /// <exception cref="GDIPlusX.GDIPlus11.GDIPlus11NotAvailableException">GDI Plus 1.1 functions not available.</exception>
        /// <exception cref="System.ArgumentNullException">bitmap is null or effect is null.</exception>
        /// <exception cref="GDIPlusX.GDIPlus11.Effects.EffectValidateException">Effect validation with bitmap failed.</exception>
        /// <remarks>Auxillary data is calculated if the effect supports it.</remarks>
        protected override void LegacyApplyToBitmap(Bitmap bitmap, Rectangle rectOfInterest)
        {
            if (mlbbBlur == null)
                mlbbBlur = new LegacyBitmapBlur(mbpParams.Radius);
            else
                mlbbBlur.Radius = mbpParams.Radius;

            mlbbBlur.ApplyToBitmap(bitmap, rectOfInterest, LegacyCloneApplyPixelFormat(), Effect.LegacyThreads);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the radius of blur calculation. 0f to 255f.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Radius must be from 0f to 255f.</exception>
        public float Radius
        {
            get
            {
                return mbpParams.Radius;
            }
            set
            {
                if (Radius != value)
                {
                    if (value < 0f || value > 255.0f) 
                        throw new ArgumentOutOfRangeException("Radius", value, "Must be from 0f to 255f");

                    mbpParams.Radius = value;

                    // Invalidate the effect parameters
                    InvalidateParameters();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to expand the edges of the output area for the blur effect.
        /// </summary>
        public bool ExpandEdges
        {
            get
            {
                return mbpParams.ExpandEdges;
            }
            set
            {
                if (ExpandEdges != value)
                {
                    mbpParams.ExpandEdges = value;

                    // Invalidate the effect parameters
                    InvalidateParameters();
                }
            }
        }

        #endregion
    }
}
