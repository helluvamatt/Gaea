//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

namespace GDIPlusX.GDIPlus10
{
    /// <summary>
    /// The exception that is thrown when GDIPlus Version 1.0 is not available.
    /// </summary>
    public class GDIPlus10NotAvailableException : GDIPlusVersionNotAvailableException
    {
        /// <summary>
        /// Initializes a new instance of the GDIPlusX.GDIPlus10.GDIPlus10NotAvailableException class.
        /// </summary>
        public GDIPlus10NotAvailableException()
            : base(1, 0)
        {
        }
    }
}
