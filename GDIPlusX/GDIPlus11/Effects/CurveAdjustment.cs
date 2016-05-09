//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using GDIPlusX.GDIPlus11.EffectsInternal;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Specifys what type of adjustment to perform for the ColorCurveEffect.
    /// </summary>
    public enum CurveAdjustment
    {
        /// <summary>
        /// Adjusts exposure (AdjustValue range is -255 to 255).
        /// </summary>
        [CurveAdjustmentValueRange(-255, 255)]
        Exposure = 0,

        /// <summary>
        /// Adjusts density (AdjustValue range is -255 to 255).
        /// </summary>
        [CurveAdjustmentValueRange(-255, 255)]
        Density = 1,

        /// <summary>
        /// Adjusts contrast (AdjustValue range is -100 to 100).
        /// </summary>
        [CurveAdjustmentValueRange(-100, 100)]
        Contrast = 2,

        /// <summary>
        /// Adjusts highlights (AdjustValue range is -100 to 100).
        /// </summary>
        [CurveAdjustmentValueRange(-100, 100)]
        Highlights = 3,

        /// <summary>
        /// Adjusts shadows (AdjustValue range is -100 to 100).
        /// </summary>
        [CurveAdjustmentValueRange(-100, 100)]
        Shadows = 4,

        /// <summary>
        /// Adjusts midtones (AdjustValue range is -100 to 100).
        /// </summary>
        [CurveAdjustmentValueRange(-100, 100)]
        Midtones = 5,

        /// <summary>
        /// Adjusts white saturation (AdjustValue range is 1 to 255).
        /// </summary>
        [CurveAdjustmentValueRange(1, 255)]
        WhiteSaturation = 6,

        /// <summary>
        /// Adjusts black saturation (AdjustValue range is 0 to 254).
        /// </summary>
        [CurveAdjustmentValueRange(0, 254)]
        BlackSaturation = 7
    }
}
