//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates the ability for an effect which has BGRA lookup tables after its applied.
    /// </summary>
    public interface IAuxDataEffectBGRA
    {
        /// <summary>
        /// Gets the Red channel lookup table after the effect has been applied.
        /// </summary>
        byte[] LUTInfoR { get; }

        /// <summary>
        /// Gets the Green channel lookup table after the effect has been applied.
        /// </summary>
        byte[] LUTInfoG { get; }

        /// <summary>
        /// Gets the Blue channel lookup table after the effect has been applied.
        /// </summary>
        byte[] LUTInfoB { get; }

        /// <summary>
        /// Gets the Alpha channel lookup table after the effect has been applied.
        /// </summary>
        byte[] LUTInfoA { get; }
    }
}
