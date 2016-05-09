//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using GDIPlusX.GDIPlus11.Internal;

namespace GDIPlusX.GDIPlus11
{
    /// <summary>
    /// Specifies the format for a histogram
    /// </summary>
    public enum HistogramFormat
    {
        /// <summary>
        /// Alpha, Red, Green, Blue channels.
        /// </summary>
        [HistogramFormatChannelCount(4)]
        ARGB,

        /// <summary>
        /// Premultiplied Alpha, Red, Green, Blue channels.
        /// </summary>
        [HistogramFormatChannelCount(4)]
        PremultipliedARGB,

        /// <summary>
        /// Red, Green, Blue channels.
        /// </summary>
        [HistogramFormatChannelCount(3)]
        RGB,

        /// <summary>
        /// Gray channel only.
        /// </summary>
        [HistogramFormatChannelCount(1)]
        Gray,

        /// <summary>
        /// Blue channel only.
        /// </summary>
        [HistogramFormatChannelCount(1)]
        B,

        /// <summary>
        /// Green channel only.
        /// </summary>
        [HistogramFormatChannelCount(1)]
        G,

        /// <summary>
        /// Red channel only.
        /// </summary>
        [HistogramFormatChannelCount(1)]
        R,


        /// <summary>
        /// Alpha channel only.
        /// </summary>
        [HistogramFormatChannelCount(1)]
        A
    }
}
