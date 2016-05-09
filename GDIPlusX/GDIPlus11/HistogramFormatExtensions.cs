//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using GDIPlusX.GDIPlus11.Internal;

namespace GDIPlusX.GDIPlus11
{
    /// <summary>
    /// Provides extension methods for the HistogramFormat enumeration.
    /// </summary>
    public static class HistogramFormatExtensions
    {
        /// <summary>
        /// Gets the number of channels for a HistogramFormat enumeration value.
        /// </summary>
        /// <param name="format">The format to get the number of channels for or 0 if not defined.</param>
        /// <returns>The number of channels</returns>
        public static int ChannelCount(this HistogramFormat format)
        {
            // Get the member information
            Type ltType = typeof(HistogramFormat);
            MemberInfo[] lmiInfos = ltType.GetMember(format.ToString());

            if (lmiInfos.Length > 0)
            {
                // Get the attribute
                object[] loAtts =
                    lmiInfos[0].GetCustomAttributes(typeof(HistogramFormatChannelCountAttribute), false);

                // Return the count
                if (loAtts.Length > 0)
                    return ((HistogramFormatChannelCountAttribute)loAtts[0]).ChannelCount;
            }

            return 0;
        }
    }
}
