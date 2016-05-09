//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using GDIPlusX.GDIPlus11.EffectsInternal;
using GDIPlusX.GDIPlus11.Internal;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates an effect which allows for hue, saturation, and lightness to be adjusted for an area.
    /// </summary>
    /// <see>http://msdn.microsoft.com/en-us/library/ms534461(v=VS.85).aspx</see>
    public class HueSaturationLightnessEffect : LUTTablesLegacyAuxDataEffectLSHA
    {
        #region Protected Static Locals

        /// <summary>
        /// GUID for the GDI+ hue saturation lightness effect.
        /// </summary>
        protected static Guid mgEffectGuid = new Guid("{8B2DD6C3-EB07-4D87-A5F0-7108E26A9C5F}");

        #endregion

        #region Private Locals

        /// <summary>
        /// Holds the parameters for the hue, saturation lightness effect.
        /// </summary>
        private Internal.Interop11.HueSaturationLightnessParams mhslParams;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new hue, saturation, lightness effect.
        /// </summary>
        public HueSaturationLightnessEffect()
            : this(0, 0, 0)
        {
        }
        /// <summary>
        /// Creates a new hue, saturation, lightness effect.
        /// </summary>
        /// <param name="hueLevel">The amount of change in hue in degrees. -180 to 180. 0 is no change.</param>
        /// <param name="saturationLevel">The amount of change in saturation. -100 to 100. 0 is no change.</param>
        /// <param name="lightnessLevel">The amount of change in lightness. -100 to 100. 0 is no change.</param>
        public HueSaturationLightnessEffect(int hueLevel, int saturationLevel, int lightnessLevel)
            : base(mgEffectGuid)
        {
            HueLevel = hueLevel;
            SaturationLevel = saturationLevel;
            LightnessLevel = lightnessLevel;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Returns the parameter data for this effect.
        /// </summary>
        /// <returns>An object containing the parameter data.</returns>
        protected override object InitialiseParameterData()
        {
            return mhslParams;
        }

        /// <summary>
        /// Gets the lookup tables for this effect when in legacy mode.
        /// </summary>
        /// <param name="l">The luminosity lookup table.</param>
        /// <param name="s">The saturation lookup table.</param>
        /// <param name="h">The hue lookup table.</param>
        /// <param name="a">The alpha lookup table.</param>
        protected override void GetLegacyLookupTables(out byte[] l, out byte[] s, out byte[] h, out byte[] a)
        {
            h = s = l = a = null;

            LegacyBitmapPerPixelEffect.GetHSLLookupTables(
                mhslParams.HueLevel, mhslParams.SaturationLevel, mhslParams.LightnessLevel,
                ref h, ref s, ref l);

            BitmapPerPixelProcessing.StandardLUT(ref a);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the amount of change in hue in degrees. -180 to 180. 0 is no change.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">HueLevel out of range.</exception>
        public int HueLevel
        {
            get
            {
                return mhslParams.HueLevel;
            }
            set
            {
                if (HueLevel != value)
                {
                    if (value < -180 || value > 180)
                        throw new ArgumentOutOfRangeException("HueLevel", value, "Must be from -180 to 180");

                    mhslParams.HueLevel = value;
                    InvalidateParameters();
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of change in saturation. -100 to 100. 0 is no change.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">SaturationLevel out of range.</exception>
        public int SaturationLevel
        {
            get
            {
                return mhslParams.SaturationLevel;
            }
            set
            {
                if (SaturationLevel != value)
                {
                    if (value < -100 || value > 100)
                        throw new ArgumentOutOfRangeException("SaturationLevel", value, "Must be from -100 to 100");

                    mhslParams.SaturationLevel = value;
                    InvalidateParameters();
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of change in lightness. -100 to 100. 0 is no change.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">LightnessLevel out of range.</exception>
        public int LightnessLevel
        {
            get
            {
                return mhslParams.LightnessLevel;
            }
            set
            {
                if (SaturationLevel != value)
                {
                    if (value < -100 || value > 100)
                        throw new ArgumentOutOfRangeException("LightnessLevel", value, "Must be from -100 to 100");

                    mhslParams.LightnessLevel = value;
                    InvalidateParameters();
                }
            }
        }

        #endregion
    }
}
