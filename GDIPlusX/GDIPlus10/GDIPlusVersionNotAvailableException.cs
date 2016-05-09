//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;

namespace GDIPlusX.GDIPlus10
{
    /// <summary>
    /// The exception that is thrown when a version GDIPlus is not available.
    /// </summary>
    public class GDIPlusVersionNotAvailableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the GDIPlusX.GDIPlus10.GDIPlusVersionNotAvailableException class.
        /// </summary>
        /// <param name="majorVersion">The major version number of GDI plus that was not available</param>
        /// <param name="minorVersion">The minor version number of GDI plus that was not available</param>
        public GDIPlusVersionNotAvailableException(int majorVersion, int minorVersion)
            : base(String.Format("GDI Plus Version {0}.{1} is not available", majorVersion, minorVersion))
        {
        }
    }
}
