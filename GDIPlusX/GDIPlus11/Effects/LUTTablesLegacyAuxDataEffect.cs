//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using GDIPlusX.GDIPlus11.Internal;
using System.Drawing.Imaging;
using GDIPlusX.GDIPlus11.EffectsInternal;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates an effect which uses lookup tables and has auxillary data for them.
    /// </summary>
    public abstract class LUTTablesLegacyAuxDataEffect : LUTTablesAuxDataEffect, ILegacyEffectApply
    {
        #region Protected Locals

        /// <summary>
        /// True to force legacy mode.
        /// </summary>
        protected bool mbForceLegacy = false;

        #endregion
        
        #region Initialisation

        /// <summary>
        /// Creatse a new brightness contrast effect.
        /// </summary>
        /// <param name="effectGuid">The Guid for this effect.</param>
        public LUTTablesLegacyAuxDataEffect(Guid effectGuid)
            : base(effectGuid)
        {
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Gets the lookup tables for this effect when in legacy mode.
        /// </summary>
        /// <param name="table0">The blue or lightness lookup table.</param>
        /// <param name="table1">The green or saturation lookup table.</param>
        /// <param name="table2">The red or hue lookup table.</param>
        /// <param name="table3">The alpha lookup table.</param>
        protected abstract void GetLegacyLookupTables(out byte[] table0, out byte[] table1, out byte[] table2, out byte[] table3);

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
        protected abstract void LegacyApplyToBitmap(Bitmap bitmap, Rectangle rectOfInterest);

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the pixel format for the legacy clone apply function.
        /// </summary>
        /// <returns>The pixel format.</returns>
        protected virtual PixelFormat LegacyCloneApplyPixelFormat()
        {
            return LegacyEffect.DefaultLegacyCloneApplyPixelFormat();
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
        protected virtual Bitmap LegacyCloneApply(Bitmap bitmap, ref Rectangle rect)
        {
            return LegacyEffect.DefaultLegacyCloneApply(this, bitmap, ref rect, LegacyCloneApplyPixelFormat());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets whether to force native mode.
        /// </summary>
        public bool ForceLegacy
        {
            get
            {
                return mbForceLegacy;
            }
            set
            {
                if (value != mbForceLegacy)
                {
                    if (value) DisposeEffect();
                    InvalidateParameters();
                    mbForceLegacy = value;
                }
            }
        }

        /// <summary>
        /// Gets whether the effect can be applied in the current environment.
        /// </summary>
        public override bool CanApply
        {
            get
            {
                return RunningLegacy || Interop11.Ver11Available;
            }
        }

        /// <summary>
        /// Gets whether the effect is running in legacy mode.
        /// </summary>
        public bool RunningLegacy
        {
            get
            {
                return (!Interop11.Ver11Available || mbForceLegacy);
            }
        }

        #endregion

        #region ILegacyEffect Members

        void ILegacyEffectApply.LegacyApplyToBitmap(Bitmap bitmap, Rectangle rectOfInterest)
        {
            LegacyApplyToBitmap(bitmap, rectOfInterest);
        }

        Bitmap ILegacyEffectApply.LegacyCloneApply(Bitmap bitmap, ref Rectangle rect)
        {
            return LegacyCloneApply(bitmap, ref rect);
        }

        #endregion
    }
}
