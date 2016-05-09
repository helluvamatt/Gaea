//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using GDIPlusX.GDIPlus10.Internal;
using GDIPlusX.GDIPlus11.Internal;

namespace GDIPlusX.GDIPlus11
{
    /// <summary>
    /// Provides GDI Plus 1.1 related extensions for the Bitmap class.
    /// </summary>
    public static class BitmapExtensions
    {
        #region Public Extension Methods

        /// <summary>
        /// Gets a single channel histogram for a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to get the histogram for.</param>
        /// <param name="format">The format to use for the histogram. One of Gray, B, G, R, or A.</param>
        /// <param name="channel">The array to contain the channel histogram data.</param>
        /// <returns>The number of channels of data returned (1).</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">format is not a supported single channel format.</exception>
        /// <exception cref="System.ArgumentNullException">bitmap is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">format enumeration is out of range.</exception>
        /// <exception cref="GDIPlusX.GDIPlus10.GDIPlus10NotAvailableException">GDI Plus 1.1 not available.</exception>
        public static int GetHistogram(
            this Bitmap bitmap, HistogramFormat format,
            ref uint[] channel)
        {
            if(format.ChannelCount() == 1)
            {
                uint[] dummy = null;
                return bitmap.GetHistogram(format, ref channel, ref dummy, ref dummy, ref dummy);
            }
            else
                throw new ArgumentOutOfRangeException(
                    "format",
                    String.Format(
                    "Histogram format '{0}{1}' returns '{2}' channels not 1.",
                    typeof(HistogramFormat).FullName, format.ToString(),
                    format.ChannelCount()));
        }

        /// <summary>
        /// Gets a three channel RGB histogram for a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to get the histogram for.</param>
        /// <param name="rChannel">The array to contain the red channel histogram data.</param>
        /// <param name="gChannel">The array to contain the green channel histogram data.</param>
        /// <param name="bChannel">The array to contain the blue channel histogram data.</param>
        /// <returns>The number of channels of data returned (3).</returns>
        /// <exception cref="System.ArgumentNullException">bitmap is null.</exception>
        /// <exception cref="GDIPlusX.GDIPlus10.GDIPlus10NotAvailableException">GDI Plus 1.1 not available.</exception>
        public static int GetHistogram(
            this Bitmap bitmap, ref uint[] rChannel, ref uint[] gChannel, ref uint[] bChannel)
        {
            uint[] dummy = null;
            return bitmap.GetHistogram(HistogramFormat.RGB, ref rChannel, ref gChannel, ref bChannel, ref dummy);
        }

        /// <summary>
        /// Gets a histograms for a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to get the histograms for.</param>
        /// <param name="format">The format to use for the histogram. One of Gray, B, G, R, or A.</param>
        /// <param name="channel0">The array to contain the first channel histogram data. Null if not used.</param>
        /// <param name="channel1">The array to contain the second channel histogram data. Null if not used.</param>
        /// <param name="channel2">The array to contain the third channel histogram data. Null if not used.</param>
        /// <param name="channel3">The array to contain the forth channel histogram data. Null if not used.</param>
        /// <returns>The number of channels of data returned.</returns>
        /// <exception cref="System.ArgumentNullException">bitmap is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">format enumeration is out of range.</exception>
        /// <exception cref="GDIPlusX.GDIPlus10.GDIPlus10NotAvailableException">GDI Plus 1.1 not available.</exception>
        public static int GetHistogram(
            this Bitmap bitmap, HistogramFormat format,
            ref uint[] channel0, ref uint[] channel1, ref uint[] channel2, ref uint[] channel3)
        {
            return GetHistogram(bitmap, format, ref channel0, ref channel1, ref channel2, ref channel3, false);
        }

        /// <summary>
        /// Gets a histograms for a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to get the histograms for.</param>
        /// <param name="format">The format to use for the histogram. One of Gray, B, G, R, or A.</param>
        /// <param name="channel0">The array to contain the first channel histogram data. Null if not used.</param>
        /// <param name="channel1">The array to contain the second channel histogram data. Null if not used.</param>
        /// <param name="channel2">The array to contain the third channel histogram data. Null if not used.</param>
        /// <param name="channel3">The array to contain the forth channel histogram data. Null if not used.</param>
        /// <param name="forceLegacy">True to force legacy mode. (not required if GDI Plus 1.1 is not available).</param>
        /// <returns>The number of channels of data returned.</returns>
        /// <exception cref="System.ArgumentNullException">bitmap is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">format enumeration is out of range.</exception>
        /// <exception cref="GDIPlusX.GDIPlus10.GDIPlus10NotAvailableException">GDI Plus 1.1 not available.</exception>
        public static int GetHistogram(
            this Bitmap bitmap, HistogramFormat format, 
            ref uint[] channel0, ref uint[] channel1, ref uint[] channel2, ref uint[] channel3,
            bool forceLegacy)
        {
            if(bitmap == null) throw new ArgumentNullException("bitmap");
            Utils10.CheckEnumRange<HistogramFormat>(format, HistogramFormat.ARGB, HistogramFormat.A, "format");

            int liStatus;

            uint luiNumberOfEntries;

            if (!forceLegacy && Info.Ver11Available)
            {
                liStatus = Interop11.GdipBitmapGetHistogramSize((Interop11.GpHistogramFormat)format, out luiNumberOfEntries);
                Utils10.CheckErrorStatus(liStatus);
            }
            else
                luiNumberOfEntries = 256;

            int liChannels = format.ChannelCount();

            if(liChannels > 0 && (channel0 == null || channel0.Length != luiNumberOfEntries))
                channel0 = new uint[luiNumberOfEntries];

            if (liChannels > 1 && (channel1 == null || channel1.Length != luiNumberOfEntries))
                channel1 = new uint[luiNumberOfEntries];

            if (liChannels > 2 && (channel2 == null || channel2.Length != luiNumberOfEntries))
                channel2 = new uint[luiNumberOfEntries];

            if (liChannels > 3 && (channel3 == null || channel3.Length != luiNumberOfEntries))
                channel3 = new uint[luiNumberOfEntries];

            if (!forceLegacy && Info.Ver11Available)
                GetHistogramGDIPlus(bitmap, format, channel0, channel1, channel2, channel3, luiNumberOfEntries);
            else
                GetHistogramLegacy(bitmap, format, channel0, channel1, channel2, channel3);

            return liChannels;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Gets a histograms for a bitmap using legacy code.
        /// </summary>
        /// <param name="bitmap">The bitmap to get the histograms for.</param>
        /// <param name="format">The format to use for the histogram. One of Gray, B, G, R, or A.</param>
        /// <param name="channel0">The array to contain the first channel histogram data. Null if not used.</param>
        /// <param name="channel1">The array to contain the second channel histogram data. Null if not used.</param>
        /// <param name="channel2">The array to contain the third channel histogram data. Null if not used.</param>
        /// <param name="channel3">The array to contain the forth channel histogram data. Null if not used.</param>
        private static void GetHistogramLegacy(Bitmap bitmap, HistogramFormat format, uint[] channel0, uint[] channel1, uint[] channel2, uint[] channel3)
        {
            Bitmap[] lbmpBitmaps = new Bitmap[] { bitmap };
            ImageLockMode[] lilmModes = new ImageLockMode[] { ImageLockMode.ReadOnly };
            Point[] lptStartPoints = new Point[] { new Point() };

#if Unsafe
            unsafe
            {
#else
            {
#endif
                switch (format)
                {
                    case HistogramFormat.A:
                    case HistogramFormat.R:
                    case HistogramFormat.G:
                    case HistogramFormat.B:
                        int liIndex;

                        if (format == HistogramFormat.A) liIndex = 3;
                        else
                            if (format == HistogramFormat.R) liIndex = 2;
                            else
                                if (format == HistogramFormat.G) liIndex = 1;
                                else
                                    liIndex = 0;

#if !Unsafe
                        int liShift = liIndex * 8;
#endif

                        BitmapPerPixelProcessing.ApplyPerPixel(
                            lbmpBitmaps, lilmModes,
                            new PixelFormat[] { PixelFormat.Format32bppArgb },
                            lptStartPoints, bitmap.Size, 0,
#if Unsafe
                            (d, t) => { channel0[((byte*)d)[liIndex]]++; }
#else
                             (d, t) => { channel0[(d[0] >> liShift) & 0xFF]++; }
#endif
                        );
                        break;

                    case HistogramFormat.Gray:
                        BitmapPerPixelProcessing.ApplyPerPixel(
                            lbmpBitmaps, lilmModes,
                            new PixelFormat[] { PixelFormat.Format32bppArgb },
                            lptStartPoints, bitmap.Size, 0,
                            (d, t) => 
                            { 
#if Unsafe
                                channel0[Interop11.LuminosityF(
                                    ((byte*)d)[2], 
                                    ((byte*)d)[1], 
                                    ((byte*)d)[0])
                                ]++;
#else
                                channel0[Interop11.LuminosityF(
                                    (byte)((d[0] >> 16) & 0xFF), 
                                    (byte)((d[0] >> 8) & 0xFF), 
                                    (byte)(d[0] & 0xFF))]++;
#endif
                            }
                        );
                        break;

                    case HistogramFormat.RGB:
                        BitmapPerPixelProcessing.ApplyPerPixel(
                            lbmpBitmaps, lilmModes,
                            new PixelFormat[] { PixelFormat.Format32bppArgb },
                            lptStartPoints, bitmap.Size, 0,
                            (d, t) =>
                            {
#if Unsafe
                                channel2[((byte*)d)[0]]++;
                                channel1[((byte*)d)[1]]++;
                                channel0[((byte*)d)[2]]++;
#else
                                channel2[(d[0] & 0xFF)]++;
                                channel1[((d[0] >> 8) & 0xFF)]++;
                                channel0[((d[0] >> 16) & 0xFF)]++;
#endif
                            }
                        );
                        break;

                    case HistogramFormat.PremultipliedARGB:
                    case HistogramFormat.ARGB:
                        BitmapPerPixelProcessing.ApplyPerPixel(
                            lbmpBitmaps, lilmModes,
                            new PixelFormat[] 
                                { 
                                    (format == HistogramFormat.ARGB ? PixelFormat.Format32bppArgb : PixelFormat.Format32bppPArgb) 
                                },
                            lptStartPoints, bitmap.Size, 0,
                            (d, t) =>
                            {
#if Unsafe
                                channel3[((byte*)d)[0]]++;
                                channel2[((byte*)d)[1]]++;
                                channel1[((byte*)d)[2]]++;
                                channel0[((byte*)d)[3]]++;
#else
                                channel3[(d[0] & 0xFF)]++;
                                channel2[((d[0] >> 8) & 0xFF)]++;
                                channel1[((d[0] >> 16) & 0xFF)]++;
                                channel0[((d[0] >> 24) & 0xFF)]++;
#endif

                            }
                        );
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a histograms for a bitmap using GDI Plus 1.1.
        /// </summary>
        /// <param name="bitmap">The bitmap to get the histograms for.</param>
        /// <param name="format">The format to use for the histogram. One of Gray, B, G, R, or A.</param>
        /// <param name="channel0">The array to contain the first channel histogram data. Null if not used.</param>
        /// <param name="channel1">The array to contain the second channel histogram data. Null if not used.</param>
        /// <param name="channel2">The array to contain the third channel histogram data. Null if not used.</param>
        /// <param name="channel3">The array to contain the forth channel histogram data. Null if not used.</param>
        /// <param name="numberOfEntries">The number of entries per channel</param>
        private static void GetHistogramGDIPlus(Bitmap bitmap, HistogramFormat format, uint[] channel0, uint[] channel1, uint[] channel2, uint[] channel3, uint numberOfEntries)
        {
            int liStatus;

#if Unsafe
            unsafe
            {
                fixed (uint*
                    luiChannel0 = channel0,
                    luiChannel1 = channel1,
                    luiChannel2 = channel2,
                    luiChannel3 = channel3)
                {
                    liStatus = Interop11.GdipBitmapGetHistogram(
                        new HandleRef(bitmap, bitmap.NativeHandle()),
                        (Interop11.GpHistogramFormat)format, numberOfEntries,
                        (IntPtr)luiChannel0, (IntPtr)luiChannel1, (IntPtr)luiChannel2, (IntPtr)luiChannel3);
                }
            }

#else
                GCHandle[] lhHandles = Utils10.PinObjects(channel0, channel1, channel2, channel3);

                try
                {
                    liStatus = Interop11.GdipBitmapGetHistogram(
                        new HandleRef(bitmap, bitmap.NativeHandle()),
                        (Interop11.GpHistogramFormat)format, numberOfEntries,
                        lhHandles[0].AddrOfPinnedObject(),
                        lhHandles[1].AddrOfPinnedObject(),
                        lhHandles[2].AddrOfPinnedObject(),
                        lhHandles[3].AddrOfPinnedObject());
                }
                finally
                {
                    Utils10.UnpinObjects(lhHandles);
                }
#endif

            Utils10.CheckErrorStatus(liStatus);
        }

        #endregion
    }
}
