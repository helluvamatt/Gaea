//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

namespace GDIPlusX
{
    /// <summary>
    /// Provides general information for the GDIPlusX namespace.
    /// </summary>
    public static class Info
    {
        #region Public Static Properties

        /// <summary>
        /// Gets whether GDI version 1.0 functions are available. Should always be true.
        /// </summary>
        public static bool Ver10Available
        {
            get
            {
                return GDIPlusX.GDIPlus10.Internal.Utils10.Ver10Available;
            }
        }

        /// <summary>
        /// Gets whether GDI Version 1.1 functions are available. (Should be true for Vista and above).
        /// </summary>
        public static bool Ver11Available
        {
            get
            {
                return GDIPlusX.GDIPlus11.Internal.Interop11.Ver11Available;
            }
        }

        /// <summary>
        /// Gets whether this GDIPlusX library was compiled using faster unsafe code, or slower safe code.
        /// </summary>
        public static bool CompiledWithUnsafe
        {
            get
            {
#if Unsafe
                return true;
#else
                return false;
#endif
            }
        }

        #endregion
    }
}
