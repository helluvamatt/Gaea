//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Specifies the channel(s) to apply the color curve effect.
    /// </summary>
    public enum CurveChannel
    {
        /// <summary>
        /// Apply to all three red, green and blue channels.
        /// </summary>
        All = 0,

        /// <summary>
        /// Apply to red channel only.
        /// </summary>
        Red = 1,

        /// <summary>
        /// Apply to green channel only.
        /// </summary>
        Green = 2,

        /// <summary>
        /// Apply to blue channel only.
        /// </summary>
        Blue = 3
    }
}
