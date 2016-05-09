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
    /// Encapsulates an effect which allows for color tint to be applied to an area.
    /// </summary>
    /// <see>http://msdn.microsoft.com/en-us/library/ms534513(v=VS.85).aspx</see>
    public class TintEffect : LegacyEffect
    {
        #region Protected Static Locals

        /// <summary>
        /// GUID for the GDI+ tint effect.
        /// </summary>
        protected static Guid mgEffectGuid = new Guid("{1077AF00-2848-4441-9489-44AD4C2D7A2C}");

        #endregion

        #region Private Locals

        /// <summary>
        /// Holds the parameters for the tint effect.
        /// </summary>
        private Internal.Interop11.TintParams mtParams;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new tint effect.
        /// </summary>
        public TintEffect()
            : this(0, 0)
        {
        }

        /// <summary>
        /// Creates a new tint effect.
        /// </summary>
        /// <param name="hue">The hue for the tint. -180 to 180. -120 is blue. -60 is magenta. 0 is red. 60 is yellow. 120 is green. 180 is cyan.</param>
        /// <param name="amount">The amount of tint to apply. -100 to 100. 0 is no change.</param>
        public TintEffect(int hue, int amount)
            : base(mgEffectGuid)
        {
            Hue = hue;
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
            return mtParams;
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
            LegacyBitmapPerPixelEffect.ApplyTint(
                bitmap, mtParams.Hue, mtParams.Amount, rectOfInterest, 
                Effect.LegacyThreads, LegacyCloneApplyPixelFormat()); 
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the hue for the tint. -120 is blue. -60 is magenta. 
        /// 0 is red. 60 is yellow. 120 is green. 180 is cyan.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Hue is out of range.</exception>
        public int Hue
        {
            get
            {
                return mtParams.Hue;
            }
            set
            {
                if (Hue != value)
                {
                    if (value < -180 || value > 180) 
                        throw new ArgumentOutOfRangeException("Hue", value, "Must be from -180 to 180");

                    mtParams.Hue = value;
                    InvalidateParameters();
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of tint to apply. -100 to 100. 0 is no change.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Amount is out of range.</exception>
        public int Amount
        {
            get
            {
                return mtParams.Amount;
            }
            set
            {
                if (Amount != value)
                {
                    if (value < -100 || value > 100)
                        throw new ArgumentOutOfRangeException("Amount", value, "Must be from -100 to 100");

                    mtParams.Amount = value;
                    InvalidateParameters();
                }
            }
        }

        #endregion
    }
}
