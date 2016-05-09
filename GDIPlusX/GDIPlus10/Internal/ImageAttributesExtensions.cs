//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing.Imaging;

namespace GDIPlusX.GDIPlus10.Internal
{
    /// <summary>
    /// Provides internal extensions for the ImageAttributes class.
    /// </summary>
    internal static class ImageAttributesExtensions
    {
        /// <summary>
        /// Gets the native handle for an ImageAttributes object.
        /// </summary>
        /// <param name="imageAttributes">The ImageAttributes to get the native handle for.</param>
        /// <returns>An IntPtr.</returns>
        public static IntPtr NativeHandle(this ImageAttributes imageAttributes)
        {
            return imageAttributes.GetPrivateField<IntPtr>("nativeImageAttributes");
        }
    }
}
