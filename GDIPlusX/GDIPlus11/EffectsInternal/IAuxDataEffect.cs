//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Provides functionality for an effect which supports auxillary data.
    /// </summary>
    internal interface IAuxDataEffect
    {
        /// <summary>
        /// Gets whether the effect should process and return auxillary data.
        /// </summary>
        bool UseAuxData { get; }

        /// <summary>
        /// Sets the data to the effect after an ApplyEffect or DrawImageFX call.
        /// </summary>
        /// <param name="data">A pointer to the data.</param>
        /// <param name="size">The size of the data.</param>
        /// <remarks>The data should NOT be freed, this is automatically done.</remarks>
        void SetAuxData(IntPtr data, int size);
    }
}
