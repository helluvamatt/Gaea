//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;

namespace GDIPlusX.GDIPlus10.Internal
{
    /// <summary>
    /// Provides internal extensions for the Brush class.
    /// </summary>
    internal static class BrushExtensions
    {
        /// <summary>
        /// Retrieves the native handle for a Brush object.
        /// </summary>
        /// <param name="brush">The Brush object to get the native handle for.</param>
        /// <returns>An IntPtr.</returns>
        /// <exception cref="System.InvalidOperationException">Property could not be located.</exception>
        public static IntPtr NativeHandle(this Brush brush)
        {
            return brush.GetPrivateProperty<IntPtr>("NativeBrush");
        }
    }
}
