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
    /// Provides internal extensions for the Graphics class.
    /// </summary>
    internal static class GraphicsExtensions
    {
        /// <summary>
        /// Retrieves the native handle for a Graphics object.
        /// </summary>
        /// <param name="g">The Graphics object to get the native handle for.</param>
        /// <returns>An IntPtr.</returns>
        /// <exception cref="System.InvalidOperationException">Property could not be located.</exception>
        public static IntPtr NativeHandle(this Graphics g)
        {
            return g.GetPrivateProperty<IntPtr>("NativeGraphics");
        }
    }
}
