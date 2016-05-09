//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing.Drawing2D;

namespace GDIPlusX.GDIPlus10.Internal
{
    /// <summary>
    /// Provides internal extensions for the Matrix class.
    /// </summary>
    internal static class MatrixExtensions
    {
        /// <summary>
        /// Gets the native handle for a Matrix object.
        /// </summary>
        /// <param name="matrix">The matrix to get the native handle for.</param>
        /// <returns>An IntPtr.</returns>
        public static IntPtr NativeHandle(this Matrix matrix)
        {
            return matrix.GetPrivateField<IntPtr>("nativeMatrix");
        }
    }
}
