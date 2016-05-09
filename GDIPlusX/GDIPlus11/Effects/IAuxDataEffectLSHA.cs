//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates the ability for an effect which has LSHA lookup tables after its applied.
    /// </summary>
    public interface IAuxDataEffectLSHA
    {
        /// <summary>
        /// Gets the Lightness lookup table after the effect has been applied.
        /// </summary>
        byte[] LUTInfoL { get; }

        /// <summary>
        /// Gets the Saturation lookup table after the effect has been applied.
        /// </summary>
        byte[] LUTInfoS { get; }

        /// <summary>
        /// Gets the Hue lookup table after the effect has been applied.
        /// </summary>
        byte[] LUTInfoH { get; }

        /// <summary>
        /// Gets the Alpha lookup table after the effect has been applied.
        /// </summary>
        byte[] LUTInfoA { get; }
    }
}
