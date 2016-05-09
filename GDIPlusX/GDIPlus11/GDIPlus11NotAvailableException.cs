//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using GDIPlusX.GDIPlus10;

namespace GDIPlusX.GDIPlus11
{
    /// <summary>
    /// The exception that is thrown when GDIPlus Version 1.1 is not available.
    /// </summary>
    public class GDIPlus11NotAvailableException : GDIPlusVersionNotAvailableException
    {
        /// <summary>
        /// Initializes a new instance of the GDIPlusX.GDIPlus10.GDIPlus11NotAvailableException class.
        /// </summary>
        public GDIPlus11NotAvailableException()
            : base(1, 1)
        {
        }
    }
}
