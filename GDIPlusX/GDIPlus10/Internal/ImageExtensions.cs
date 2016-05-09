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
    /// Provides internal extensions for the Image class.
    /// </summary>
    internal static class ImageExtensions
    {
        /// <summary>
        /// Gets the native handle for an Image objct.
        /// </summary>
        /// <param name="image">The Image to get the native handle for.</param>
        /// <returns>An IntPtr.</returns>
        public static IntPtr NativeHandle(this Image image)
        {
            return image.GetPrivateField<IntPtr>("nativeImage");
        }
    }
}
