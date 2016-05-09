//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;

namespace GDIPlusX.GDIPlus11.EffectsInternal
{
    #region Aliases

    using Scalar = System.Double;

    #endregion

    /// <summary>
    /// Encapsulates a class to convert to and from HSL and Color objects.
    /// </summary>
    internal class HSLColor
    {
        #region Public Constants

        /// <summary>
        /// The scale value used for input hue values.
        /// </summary>
        public const Scalar HueScale = (Scalar)360.0;

        /// <summary>
        /// The scale value used for input saturation values.
        /// </summary>
        public const Scalar SatScale = (Scalar)100.0;

        /// <summary>
        /// The scale value used for input luminosity values.
        /// </summary>
        public const Scalar LumScale = (Scalar)100.0;

        #endregion

        #region Protected Locals

        /// <summary>
        /// The current Hue value (0.0 - 1.0).
        /// </summary>
        protected Scalar msHue = (Scalar)1.0;

        /// <summary>
        /// The current saturation value (0.0 - 1.0).
        /// </summary>
        protected Scalar msSaturation = (Scalar)1.0;

        /// <summary>
        /// The current luminosity value (0.0 - 1.0).
        /// </summary>
        protected Scalar msLuminosity = (Scalar)1.0;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Implicitly converts a HSLColor object to a Color struct.
        /// </summary>
        /// <param name="hslColor">The HSL Color to convert.</param>
        /// <returns>A new Color struct.</returns>
        public static implicit operator Color(HSLColor hslColor)
        {
            byte lbR, lbG, lbB;
            hslColor.ToRGB(out lbR, out lbG, out lbB);
            return Color.FromArgb(lbR, lbG, lbB);
        }

        /// <summary>
        /// Implicitly converts a Color struct to a HSLColor object.
        /// </summary>
        /// <param name="color">The Color struct to convert.</param>
        /// <returns>A new HSLColor class.</returns>
        public static implicit operator HSLColor(Color color)
        {
            HSLColor hslColor = new HSLColor();
            hslColor.SetRGB(color.R, color.G, color.B);
            return hslColor;
        }

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new HSLColor with HSL of 0, 0, 0.
        /// </summary>
        public HSLColor()
        {
        }

        /// <summary>
        /// Creates a new HSLColor based on a Color struct.
        /// </summary>
        /// <param name="color">The structure to base the HSLColor on.</param>
        public HSLColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }

        /// <summary>
        /// Creates a new HSLColor based on seperate RGB values.
        /// </summary>
        /// <param name="red">The red channel value.</param>
        /// <param name="green">The green channel value.</param>
        /// <param name="blue">The blue channel value.</param>
        public HSLColor(byte red, byte green, byte blue)
        {
            SetRGB(red, green, blue);
        }

        /// <summary>
        /// Creates a new HSLCOlor based on scales HSL values.
        /// </summary>
        /// <param name="hue">The hue value (0 - HueScale).</param>
        /// <param name="saturation">The saturation value (0 - SatScale).</param>
        /// <param name="luminosity">The luminosity value (0 - LumScale).</param>
        public HSLColor(Scalar hue, Scalar saturation, Scalar luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Formats the HSLColor as a string.
        /// </summary>
        /// <returns>The HSLColor as a string.</returns>
        public override string ToString()
        {
            return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
        }

        /// <summary>
        /// Formats the HSLColor as an RGB string.
        /// </summary>
        /// <returns>The HSLColor as an RGB string.</returns>
        public string ToRGBString()
        {
            byte lbR, lbG, lbB;
            ToRGB(out lbR, out lbG, out lbB);
            return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", lbR, lbB, lbG);
        }

        /// <summary>
        /// Converts the HSLColor to seperate RGB values.
        /// </summary>
        /// <param name="red">The value to contain the Red channel (0 - 255).</param>
        /// <param name="green">The value to contain the Green channel (0 - 255).</param>
        /// <param name="blue">The value to contain the Blue channel (0 - 255).</param>
        public void ToRGB(out byte red, out byte green, out byte blue)
        {
            HSLColorConverter.HSL2RGB(
                out red, out green, out blue,
                msHue, msSaturation, msLuminosity);
        }

        /// <summary>
        /// Sets the HSLColor based on seperate RGB values.
        /// </summary>
        /// <param name="red">The value for the Red channel (0 - 255).</param>
        /// <param name="green">The value for the Green channel (0 - 255).</param>
        /// <param name="blue">The value for the Blue channel (0 - 255).</param>
        public void SetRGB(byte red, byte green, byte blue)
        {
            HSLColorConverter.RGB2HSL(
                red, green, blue,
                out msHue, out msSaturation, out msLuminosity);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the scaled Hue value.
        /// </summary>
        /// <remarks>The Hue valued is rolled into range.</remarks>
        public Scalar Hue
        {
            get
            {
                return msHue * HueScale;
            }
            set
            {
                msHue = HSLColorConverter.Roll(value / HueScale);
            }
        }

        /// <summary>
        /// Gets or sets the scaled Saturation value.
        /// </summary>
        /// <remarks>The Saturation value is clamped into range.</remarks>
        public Scalar Saturation
        {
            get
            {
                return msSaturation * SatScale;
            }
            set
            {
                msSaturation = HSLColorConverter.Clamp(value / SatScale);
            }
        }

        /// <summary>
        /// Gets or sets the scaled Luminosity value.
        /// </summary>
        /// <remarks>The Luminosity is clamped into range.</remarks>
        public Scalar Luminosity
        {
            get
            {
                return msLuminosity * LumScale;
            }
            set
            {
                msLuminosity = HSLColorConverter.Clamp(value / LumScale);
            }
        }

        /// <summary>
        /// Gets or sets the raw Hue value (0 - 1).
        /// </summary>
        /// <remarks>The Hue valued is rolled into range.</remarks>
        public Scalar RawHue
        {
            get
            {
                return msHue;
            }
            set
            {
                msHue = HSLColorConverter.Roll(value);
            }
        }

        /// <summary>
        /// Gets or sets the raw Saturation value (0 - 1).
        /// </summary>
        /// <remarks>The Saturation value is clamped into range.</remarks>
        public Scalar RawSaturation
        {
            get
            {
                return msSaturation;
            }
            set
            {
                msSaturation = HSLColorConverter.Clamp(value);
            }
        }

        /// <summary>
        /// Gets or sets the raw Luminosity value (0 - 1).
        /// </summary>
        /// <remarks>The Luminosity is clamped into range.</remarks>
        public Scalar RawLuminosity
        {
            get
            {
                return msLuminosity;
            }
            set
            {
                msLuminosity = HSLColorConverter.Clamp(value);
            }
        }

        #endregion
    }
}