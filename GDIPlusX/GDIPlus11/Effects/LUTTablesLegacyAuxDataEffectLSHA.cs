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

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates an effect which uses BGRA lookup tables and has auxillary data for them.
    /// </summary>
    public abstract class LUTTablesLegacyAuxDataEffectLSHA : LUTTablesLegacyAuxDataEffect, IAuxDataEffectLSHA 
    {
        #region Initialisation

        /// <summary>
        /// Creatse a new legacy BGRA aux data effect.
        /// </summary>
        /// <param name="effectGuid">The Guid for this effect.</param>
        public LUTTablesLegacyAuxDataEffectLSHA(Guid effectGuid)
            : base(effectGuid)
        {
        }

        #endregion

        #region Protected Overrides

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
            byte[] lb3, lb2, lb1, lb0;
            GetLegacyLookupTables(out lb0, out lb1, out lb2, out lb3);

            LegacyBitmapPerPixelEffect.ApplyHSLLookupTables(
                bitmap, lb3, lb2, lb1, lb0,
                rectOfInterest, Effect.LegacyThreads, PixelFormat.Format32bppPArgb);

            if (ProcessLUTInfo)
            {
                mbLUTInfo3 = lb3;
                mbLUTInfo2 = lb2;
                mbLUTInfo1 = lb1;
                mbLUTInfo0 = lb0;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The last red channel lookup table data.
        /// </summary>
        public byte[] LUTInfoH
        {
            get
            {
                return mbLUTInfo2;
            }
        }

        /// <summary>
        /// The last green channel lookup table data.
        /// </summary>
        public byte[] LUTInfoS
        {
            get
            {
                return mbLUTInfo1;
            }
        }

        /// <summary>
        /// The last blue channel lookup table data.
        /// </summary>
        public byte[] LUTInfoL
        {
            get
            {
                return mbLUTInfo0;
            }
        }

        /// <summary>
        /// The last alpha channel lookup table data.
        /// </summary>
        public byte[] LUTInfoA
        {
            get
            {
                return mbLUTInfo3;
            }
        }

        #endregion
    }
}
