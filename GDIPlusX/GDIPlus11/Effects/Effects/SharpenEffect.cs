//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using GDIPlusX.GDIPlus11.EffectsInternal;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates an effect which allows for sharpening to be applied to an area.
    /// </summary>
    /// <see>http://msdn.microsoft.com/en-us/library/ms534503(v=VS.85).aspx</see>
    public class SharpenEffect : LegacyEffect
    {
        #region Protected Static Locals

        /// <summary>
        /// GUID for the GDI+ sharpen effect.
        /// </summary>
        protected static Guid mgEffectGuid = new Guid("{63CBF3EE-C526-402C-8F71-62C540BF5142}");

        #endregion

        #region Private Locals

        /// <summary>
        /// Holds the parameters for the sharpen effect.
        /// </summary>
        private Internal.Interop11.SharpenParams msParams;

        /// <summary>
        /// Holds the last bitmap sharpen legacy object.
        /// </summary>
        LegacyBitmapSharpen mlbsSharpen = null;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new sharpen effect.
        /// </summary>
        public SharpenEffect()
            : this(0f, 0f)
        {
        }

        /// <summary>
        /// Creates a new sharpen effect.
        /// </summary>
        /// <param name="radius">The radius for calculation of the effect. 0f to 255f.</param>
        /// <param name="amount">The amount of sharpening to apply. 0f to 100f.</param>
        public SharpenEffect(float radius, float amount)
            : base(mgEffectGuid)
        {
            Radius = radius;
            Amount = amount;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Returns the parameter data for this effect.
        /// </summary>
        /// <returns>An object containing the parameter data.</returns>
        protected override object InitialiseParameterData()
        {
            mlbsSharpen = null; 
            return msParams;
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
            if (mlbsSharpen == null)
                mlbsSharpen = new LegacyBitmapSharpen(msParams.Radius, msParams.Amount);
            else
            {
                mlbsSharpen.Amount = msParams.Amount;
                mlbsSharpen.Radius = mlbsSharpen.Radius;
            }

            mlbsSharpen.ApplyToBitmap(bitmap, rectOfInterest, LegacyCloneApplyPixelFormat(), Effect.LegacyThreads);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the radius for calculation of the effect. 0f to 255f.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Radius is out of range.</exception>
        public float Radius
        {
            get
            {
                return msParams.Radius;
            }
            set
            {
                if (Radius != value)
                {
                    if (value < 0.0f || value > 255.0f) 
                        throw new ArgumentOutOfRangeException("Radius", value, "Must be from 0f to 255f");

                    msParams.Radius = value;
                    InvalidateParameters();
                }
            }
        }

        /// <summary>
        /// Gets ot sets the amount of sharpening to apply. 0f to 100f.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Amount is out of range.</exception>
        public float Amount
        {
            get
            {
                return msParams.Amount;
            }
            set
            {
                if (Amount != value)
                {
                    if (value < 0.0f || value > 100.0f)
                        throw new ArgumentOutOfRangeException("Radius", value, "Must be from 0f to 100f");

                    msParams.Amount = value;
                    InvalidateParameters();
                }
            }
        }

        #endregion
    }
}
