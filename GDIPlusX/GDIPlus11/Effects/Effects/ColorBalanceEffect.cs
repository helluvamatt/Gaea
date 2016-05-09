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
    /// Encapsulates an effect which alters the color balance of an area.
    /// </summary>
    /// <see>http://msdn.microsoft.com/en-us/library/ms534428(v=vs.85).aspx</see>
    public class ColorBalanceEffect : LUTTablesLegacyAuxDataEffectBGRA
    {
        #region Protected Static Locals

        /// <summary>
        /// GUID for the GDI+ color balance effect.
        /// </summary>
        protected static Guid mgEffectGuid = new Guid("{537E597D-251E-48DA-9664-29CA496B70F8}");

        #endregion

        #region Private Locals

        /// <summary>
        /// Holds the parameters for the color balance effect.
        /// </summary>
        private Internal.Interop11.ColorBalanceParams mcbParams;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creatse a new brightness contrast effect.
        /// </summary>
        public ColorBalanceEffect()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// Creatse a new brightness contrast effect.
        /// </summary>
        /// <param name="cyanRed">The cyan to red level for the effect. -100 to 100. 0 is no change.</param>
        /// <param name="magentaGreen">The magenta to green level for the effect. -100 to 100. 0 is no change.</param>
        /// <param name="yellowBlue">The yellow to blue level for the effect. -100 to 100. 0 is no change.</param>
        public ColorBalanceEffect(int cyanRed, int magentaGreen, int yellowBlue)
            : base(mgEffectGuid)
        {
            CyanRed = cyanRed;
            MagentaGreen = magentaGreen;
            YellowBlue = yellowBlue;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Gets the lookup tables for this effect when in legacy mode.
        /// </summary>
        /// <param name="b">The blue lookup table.</param>
        /// <param name="g">The red lookup table.</param>
        /// <param name="r">The green lookup table.</param>
        /// <param name="a">The alpha lookup table.</param>
        protected override void GetLegacyLookupTables(out byte[] b, out byte[] g, out byte[] r, out byte[] a)
        {
            a = r = g = b = null;

            BitmapPerPixelProcessing.StandardLUT(ref a);
            LegacyBitmapPerPixelEffect.GetColorBalanceLUTs(
                mcbParams.CyanRed, mcbParams.MagentaGreen, mcbParams.YellowBlue,
                ref b, ref g, ref r);
        }

        /// <summary>
        /// Returns the parameter data for this effect.
        /// </summary>
        /// <returns>An object containing the parameter data.</returns>
        protected override object InitialiseParameterData()
        {
            return mcbParams;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the cyan to red level for the effect -100 to 100. 0 is no change.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">CyanRed must be from -100 to 100.</exception>
        public int CyanRed
        {
            get
            {
                return mcbParams.CyanRed;
            }
            set
            {
                if (CyanRed != value)
                {
                    if (value < -100 || value > 100) 
                        throw new ArgumentOutOfRangeException("CyanRed", value, "Must be from -100 to 100");

                    mcbParams.CyanRed = value;
                    InvalidateParameters();
                }
            }
        }

        /// <summary>
        /// Gets or sets the magenta to green level for the effect -100 to 100. 0 is no change.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">MagentaGreen must be from -100 to 100.</exception>
        public int MagentaGreen
        {
            get
            {
                return mcbParams.MagentaGreen;
            }
            set
            {
                if (MagentaGreen != value)
                {
                    if (value < -100 || value > 100)
                        throw new ArgumentOutOfRangeException("MagentaGreen", value, "Must be from -100 to 100");

                    mcbParams.MagentaGreen = value;
                    InvalidateParameters();
                }
            }
        }

        /// <summary>
        /// Gets or sets the yellow to blue level for the effect -100 to 100. 0 is no change.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">YellowBlue must be from -100 to 100.</exception>
        public int YellowBlue
        {
            get
            {
                return mcbParams.YellowBlue;
            }
            set
            {
                if (YellowBlue != value)
                {
                    if (value < -100 || value > 100)
                        throw new ArgumentOutOfRangeException("YellowBlue", value, "Must be from -100 to 100");

                    mcbParams.YellowBlue = value;
                    InvalidateParameters();
                }
            }
        }

        #endregion
    }
}
