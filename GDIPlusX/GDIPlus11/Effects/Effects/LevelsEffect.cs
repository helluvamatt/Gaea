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
    /// Encapsulates an effect which allows for curve level to be adjusted for an area.
    /// </summary>
    /// <see>http://msdn.microsoft.com/en-us/library/ms534471(v=VS.85).aspx</see>
    public class LevelsEffect : LUTTablesLegacyAuxDataEffectBGRA
    {
        #region Protected Static Locals

        /// <summary>
        /// GUID for the GDI+ levels effect.
        /// </summary>
        protected static Guid mgEffectGuid = new Guid("{99C354EC-2A31-4F3A-8C34-17A803B33A25}");

        #endregion

        #region Private Locals

        /// <summary>
        /// Holds the parameters for the levels effect.
        /// </summary>
        private Internal.Interop11.LevelsParams mlParams;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new level effect.
        /// </summary>
        public LevelsEffect()
            : this(100, 0, 0)
        {
        }

        /// <summary>
        /// Creates a new level effect.
        /// </summary>
        /// <param name="highlights">The lighten level for highlights. 0 to 100. 100 is no change.</param>
        /// <param name="midtones">The adjustment level for midtones. -100 to 100. 0 is no change.</param>
        /// <param name="shadows">The darken level for shadows. 0 to 100. 0 is no change.</param>
        public LevelsEffect(int highlights, int midtones, int shadows)
            : base(mgEffectGuid)
        {
            Highlights = highlights;
            Midtones = midtones;
            Shadows = shadows;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Returns the parameter data for this effect.
        /// </summary>
        /// <returns>An object containing the parameter data.</returns>
        protected override object InitialiseParameterData()
        {
            return mlParams;
        }

        /// <summary>
        /// Gets the lookup tables for this effect when in legacy mode.
        /// </summary>
        /// <param name="b">The alpha lookup table.</param>
        /// <param name="g">The red lookup table.</param>
        /// <param name="r">The green lookup table.</param>
        /// <param name="a">The blue lookup table.</param>
        protected override void GetLegacyLookupTables(out byte[] b, out byte[] g, out byte[] r, out byte[] a)
        {
            r = null;
            LegacyBitmapPerPixelEffect.GetLevelsLUT(mlParams.Highlight, mlParams.Midtone, mlParams.Shadow, ref r);
            
            b = (byte[])r.Clone();
            g = (byte[])r.Clone();

            a = null;
            BitmapPerPixelProcessing.StandardLUT(ref a);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the lighten level for highlights. 0 to 100. 100 is no change.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Highlights out of range.</exception>
        public int Highlights
        {
            get
            {
                return mlParams.Highlight;
            }
            set
            {
                if (Highlights != value)
                {
                    if (value < 0 || value > 100)
                        throw new ArgumentOutOfRangeException("Highlights", value, "Must be from 0 to 100");

                    mlParams.Highlight = value;
                    InvalidateParameters();
                }
            }
        }

        /// <summary>
        /// Gets or sets the adjustment level for midtones. -100 to 100. 0 is no change.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Midtones out of range.</exception>
        public int Midtones
        {
            get
            {
                return mlParams.Midtone;
            }
            set
            {
                if (Midtones != value)
                {
                    if (value < -100 || value > 100)
                        throw new ArgumentOutOfRangeException("Midtones", value, "Must be from -100 to 100");

                    mlParams.Midtone = value;
                    InvalidateParameters();
                }
            }
        }

        /// <summary>
        /// Gets or sets the darken level for shadows. 0 to 100. 0 is no change.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Shadows out of range.</exception>
        public int Shadows
        {
            get
            {
                return mlParams.Shadow;
            }
            set
            {
                if (Midtones != value)
                {
                    if (value < 0 || value > 100)
                        throw new ArgumentOutOfRangeException("Shadows", value, "Must be from 0 to 100");

                    mlParams.Shadow = value;
                    InvalidateParameters();
                }
            }
        }

        #endregion
    }
}
