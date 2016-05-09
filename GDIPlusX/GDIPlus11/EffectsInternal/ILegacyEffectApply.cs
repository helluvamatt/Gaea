//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System.Drawing;
using GDIPlusX.GDIPlus11.Effects;

namespace GDIPlusX.GDIPlus11.EffectsInternal
{
    /// <summary>
    /// Provides functionality for an effect which which supports legacy applying to bitmap.
    /// </summary>
    internal interface ILegacyEffectApply : ILegacyEffect 
    {
        /// <summary>
        /// Applys an effect to a Bitmap using legacy code.
        /// </summary>
        /// <param name="bitmap">The Bitmap to apply the effect to.</param>
        /// <param name="rectOfInterest">
        /// The rectangle to apply the Effect or Rectangle.Empty 
        /// for entire bitmap.
        /// </param>
        /// <remarks>Auxillary data is calculated if the effect supports it.</remarks>
        void LegacyApplyToBitmap(Bitmap bitmap, Rectangle rectOfInterest);

        /// <summary>
        /// Copies an area bitmap and applys an effect using legacy code.
        /// </summary>
        /// <param name="bitmap">The Bitmap to apply the effect to.</param>
        /// <param name="rect">
        /// The rectangle to apply the Effect or Rectangle.Empty for 
        /// entire bitmap, on out the area actually applied.
        /// </param>
        /// <returns>A new bitmap object with the effect applied.</returns>
        /// <remarks>Auxillary data is calculated if the effect supports it.</remarks>
        Bitmap LegacyCloneApply(Bitmap bitmap, ref Rectangle rect);
    }
}
