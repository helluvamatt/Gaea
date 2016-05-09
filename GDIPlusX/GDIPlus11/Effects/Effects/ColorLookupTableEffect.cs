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
    /// Encapsulates an effect which allows for a lookup table to be applied per channel to an area.
    /// </summary>
    /// <see>http://msdn.microsoft.com/en-us/library/ms534430(v=VS.85).aspx</see>
    public class ColorLookupTableEffect : LegacyEffect
    {
        #region Protected Static Locals

        /// <summary>
        /// GUID for the GDI+ lookup table effect.
        /// </summary>
        protected static Guid mgEffectGuid = new Guid("{A7CE72A9-0F7F-40D7-B3CC-D0C02D5C3212}");

        #endregion

        #region Private Locals

        /// <summary>
        /// Holds the parameters for the color lookup table effect.
        /// </summary>
        private Internal.Interop11.ColorLUTParams mclParams;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new color lookup table effect.
        /// </summary>
        public ColorLookupTableEffect()
            : this(null, null, null, null)
        {
        }

        /// <summary>
        /// Creates a new color lookup table effect.
        /// </summary>
        /// <param name="alphaLUT">The alpha lookup table or null for default (null or 256 elements).</param>
        /// <param name="redLUT">The red lookup table or null for default (null or 256 elements).</param>
        /// <param name="greenLUT">The green lookup table or null for default (null or 256 elements).</param>
        /// <param name="blueLUT">The blue lookup table or null for default (null or 256 elements).</param>
        public ColorLookupTableEffect(byte[] alphaLUT, byte[] redLUT, byte[] greenLUT, byte[] blueLUT)
            : base(mgEffectGuid)
        {
            RedLUT = redLUT;
            GreenLUT = greenLUT;
            BlueLUT = blueLUT;
            AlphaLUT = alphaLUT;
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
            return mclParams;
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
            LegacyBitmapPerPixelEffect.ApplyLookupTables(
                bitmap, mclParams.A, mclParams.R, mclParams.G, mclParams.B,
                rectOfInterest, Effect.LegacyThreads, LegacyCloneApplyPixelFormat());
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Defaults a lookup table.
        /// </summary>
        /// <param name="table">A reference to the lookup table to default.</param>
        protected void DefaultLUTTable(ref byte[] table)
        {
            // If the table is null or does not contain the required
            // number of elements, then recreate it.
            if(table == null || table.Length != 256)
                table = new byte[256];

            // Set the lookup table to perform no changes
            for (int liCounter = 0; liCounter < table.Length; liCounter++)
                table[liCounter] = (byte)liCounter;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets an array reference to the red channel lookup table. (null or 256 elements).
        /// </summary>
        /// <exception cref="System.ArgumentException">RedLUT must be null or contain 256 elements.</exception>
        public byte[] RedLUT
        {
            get
            {
                return mclParams.R;
            }
            set
            {
                if (value == null)
                    DefaultLUTTable(ref mclParams.R);
                else
                {
                    if (value.Length != 256)
                        throw new ArgumentException("Must be null or contain 256 elements", "RedLUT");

                    mclParams.R = value;
                }

                InvalidateParameters();
            }
        }

        /// <summary>
        /// Gets or sets an array reference to the blue channel lookup table. (null or 256 elements).
        /// </summary>
        /// <exception cref="System.ArgumentException">BlueLUT must be null or contain 256 elements.</exception>
        public byte[] BlueLUT
        {
            get
            {
                return mclParams.B;
            }
            set
            {
                if (value == null)
                    DefaultLUTTable(ref mclParams.B);
                else
                {
                    if (value.Length != 256)
                        throw new ArgumentException("Must be null or contain 256 elements", "BlueLUT");

                    mclParams.B = value;
                }

                InvalidateParameters();
            }
        }

        /// <summary>
        /// Gets or sets an array reference to the green channel lookup table. (null or 256 elements).
        /// </summary>
        /// <exception cref="System.ArgumentException">GreenLUT must be null or contain 256 elements.</exception>
        public byte[] GreenLUT
        {
            get
            {
                return mclParams.G;
            }
            set
            {
                if (value == null)
                    DefaultLUTTable(ref mclParams.G);
                else
                {
                    if (value.Length != 256)
                        throw new ArgumentException("Must be null or contain 256 elements", "GreenLUT");

                    mclParams.G = value;
                }

                InvalidateParameters();
            }
        }

        /// <summary>
        /// Gets or sets an array reference to the alpha channel lookup table. (null or 256 elements).
        /// </summary>
        /// <exception cref="System.ArgumentException">AlphaLUT must be null or contain 256 elements.</exception>
        public byte[] AlphaLUT
        {
            get
            {
                return mclParams.A;
            }
            set
            {
                if (value == null)
                    DefaultLUTTable(ref mclParams.A);
                else
                {
                    if (value.Length != 256)
                        throw new ArgumentException("Must be null or contain 256 elements", "AlphaLUT");

                    mclParams.A = value;
                }

                InvalidateParameters();
            }
        }

        #endregion
    }
}
