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
    /// Provides internal extensions for the IntPtr struct.
    /// </summary>
    internal static class IntPtrExtensions
    {
        /// <summary>
        /// Gets a Bitmap object for a native GDI+ bitmap handle.
        /// </summary>
        /// <param name="nativeBitmap">The native handle to get the bitmap for.</param>
        /// <returns>A Bitmap.</returns>
        public static Bitmap NativeBitmapPtrToBitmap(this IntPtr nativeBitmap)
        {
            return typeof(Bitmap).InvokeStaticPrivateMethod<Bitmap>("FromGDIplus", nativeBitmap);
        }
    }
}
