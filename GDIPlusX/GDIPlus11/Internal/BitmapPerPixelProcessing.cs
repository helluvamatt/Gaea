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
using System.Threading;

namespace GDIPlusX.GDIPlus11.Internal
{
    /// <summary>
    /// Encapsulates static functions for per pixel image processing.
    /// </summary>
    /// <remarks>
    /// This class supports Unsafe and Safe mode, define Unsafe to compile in unsafe mode.
    /// Unsafe mode is faster.
    /// </remarks>
    internal static class BitmapPerPixelProcessing
    {
        #region Structures

#if Unsafe

        /// <summary>
        /// Structure for casting from UInt32 to access color channels.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct C32
        {
            /// <summary>
            /// The blue color channel.
            /// </summary>
            [FieldOffset(0)]
            public byte B;

            /// <summary>
            /// The green color channel.
            /// </summary>
            [FieldOffset(1)]
            public byte G;

            /// <summary>
            /// The red color channel.
            /// </summary>
            [FieldOffset(2)]
            public byte R;

            /// <summary>
            /// The alpha color channel.
            /// </summary>
            [FieldOffset(3)]
            public byte A;
        }

        /// <summary>
        /// Structure for casting from UInt64 to access dual pixel color channels.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct C64
        {
            /// <summary>
            /// The first pixels blue color channel.
            /// </summary>
            [FieldOffset(0)]
            public byte B0;

            /// <summary>
            /// The first pixels green color channel.
            /// </summary>
            [FieldOffset(1)]
            public byte G0;

            /// <summary>
            /// The first pixels red color channel.
            /// </summary>
            [FieldOffset(2)]
            public byte R0;

            /// <summary>
            /// The first pixels alpha color channel.
            /// </summary>
            [FieldOffset(3)]
            public byte A0;

            /// <summary>
            /// The second pixels blue color channel.
            /// </summary>
            [FieldOffset(4)]
            public byte B1;

            /// <summary>
            /// The second pixels green color channel.
            /// </summary>
            [FieldOffset(5)]
            public byte G1;

            /// <summary>
            /// The second pixels red color channel.
            /// </summary>
            [FieldOffset(6)]
            public byte R1;

            /// <summary>
            /// The second pixels alpha color channel.
            /// </summary>
            [FieldOffset(7)]
            public byte A1;
        }

#endif

        #endregion

        #region Delegates

#if Unsafe

        /// <summary>
        /// Callback for 64-bit dual pixel processing.
        /// </summary>
        /// <param name="pixelData">A pointer to a list of UInt64 values, each value represents two pixels from a bitmap passed in.</param>
        /// <param name="thread">The thread index of the thread, 0 is the current thread, range is 0 to threads.</param>
        public unsafe delegate void ProcessPixel64(UInt64* pixelData, int thread);

        /// <summary>
        /// Callback for 32-bit per pixel processing.
        /// </summary>
        /// <param name="pixelData">A pointer to a list of UInt32 values, each value represents a pixel from a bitmap passed in.</param>
        /// <param name="thread">The thread index of the thread, 0 is the current thread, range is 0 to threads.</param>
        public unsafe delegate void ProcessPixel32(UInt32* pixelData, int thread);

#else

        /// <summary>
        /// Callback for 64-bit dual pixel processing.
        /// </summary>
        /// <param name="pixelData">An array to a list of UInt64 values, each value represents two pixels from a bitmap passed in.</param>
        /// <param name="thread">The thread index of the thread, 0 is the current thread, range is 0 to threads.</param>
        public delegate void ProcessPixel64(UInt64[] pixelData, int thread);

        /// <summary>
        /// Callback for 32-bit per pixel processing.
        /// </summary>
        /// <param name="pixelData">An array to a list of UInt32 values, each value represents a pixel from a bitmap passed in.</param>
        /// <param name="thread">The thread index of the thread, 0 is the current thread, range is 0 to threads.</param>
        public delegate void ProcessPixel32(UInt32[] pixelData, int thread);
#endif
        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the size in bits of the pixels for a PixelFormat.
        /// </summary>
        /// <param name="pixelFormat">The pixel format to get the bit size of.</param>
        /// <returns>The size in vits of the pixel format.</returns>
        public static int BitSize(this PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                default:
                    return 0;

                case PixelFormat.Format1bppIndexed:
                    return 1;

                case PixelFormat.Format4bppIndexed:
                    return 4;

                case PixelFormat.Format8bppIndexed:
                    return 8;

                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                    return 16;

                case PixelFormat.Format24bppRgb:
                    return 24;

                case PixelFormat.Gdi:
                case PixelFormat.Canonical:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppArgb:
                    return 32;

                case PixelFormat.Format48bppRgb:
                    return 48;

                case PixelFormat.Format64bppPArgb:
                case PixelFormat.Format64bppArgb:
                    return 64;
            }
        }

        /// <summary>
        /// Performs a fast integer divide by 255 for an int value in the range -65535 - 65536.
        /// </summary>
        /// <param name="value">The value to divide. Range is -65535 to 65536.</param>
        /// <returns>Returns the value divided by 255.</returns>
        /// <remarks>Only about 6% faster. This method should be inlined by the C# compiler when optimizations are on.</remarks>
        public static int FastDivBy255(int value)
        {
            return (value * 4112) >> 20;
        }

        /// <summary>
        /// Gets a standard lookup table where all values are equal to their original.
        /// </summary>
        /// <param name="lut">The lookup array to use.</param>
        public static void StandardLUT(ref byte[] lut)
        {
            if (lut == null || lut.Length != 256) lut = new byte[256];
            for (int liCounter = 0; liCounter < 256; liCounter++)
                lut[liCounter] = (byte)liCounter;

        }

        /// <summary>
        /// Allocates an array for a lookup table if it is null or not the correct size.
        /// </summary>
        /// <param name="lut">The lookup table to allocate.</param>
        public static void AllocLUT(ref byte[] lut)
        {
            if (lut == null || lut.Length != 256)
                lut = new byte[256];
        }

        /// <summary>
        /// Truncates a channel value from a float.
        /// </summary>
        /// <param name="channel">The floating point channel value.</param>
        /// <returns>The channel value truncated to range 0 to 255.</returns>
        /// <remarks>This method should be inlined by the C# compiler when optimizations are on.</remarks>
        public static byte TruncateChannel(float channel)
        {
            if (channel > 255.0f) return 255;
            if (channel < 0f) return 0;
            return (byte)channel;
        }

        /// <summary>
        /// Truncates a channel value from an int.
        /// </summary>
        /// <param name="channel">The int channel value.</param>
        /// <returns>The channel value truncated to range 0 to 255.</returns>
        /// <remarks>This method should be inlined by the C# compiler when optimizations are on.</remarks>
        public static byte TruncateChannel(int channel)
        {
            if (channel > 255) return 255;
            if (channel < 0) return 0;
            return (byte)channel;
        }

        #endregion

        #region Kernel Processing Methods

        /// <summary>
        /// Calls a delegate per pixel using multi-threading.
        /// </summary>
        /// <param name="bitmaps">The bitmaps to pass into the delegate.</param>
        /// <param name="lockModes">The lock modes for each bitmap.</param>
        /// <param name="pixelFormats">The pixel formats for each bitmap (must be 32-bits or 64-bits in size).</param>
        /// <param name="startPoints">The top left points of the locking rectangle for each bitmap.</param>
        /// <param name="size">The size of the locking rectangle for all bitmaps.</param>
        /// <param name="threads">The number of threads to use (in addition to the current thread).</param>
        /// <param name="processPixel64">The delegate to call for dual pixel processing. Or null for 32-bit only.</param>
        /// <param name="processPixel32">The delegate to call for dual single pixel processing.</param>
        public static void ApplyPerPixel(
            Bitmap[] bitmaps, ImageLockMode[] lockModes, PixelFormat[] pixelFormats, Point[] startPoints,
            Size size, int threads,
            ProcessPixel64 processPixel64, ProcessPixel32 processPixel32)
        {
            if (bitmaps.Length != lockModes.Length || bitmaps.Length != startPoints.Length)
                throw new ArgumentException("bitmaps, lockModes, and startPoints must be the same length.");

            Point lptSize = new Point(size);
            lptSize.Offset(-1, -1);

            // Check for errors
            for (int liCounter = 0; liCounter < bitmaps.Length; liCounter++)
            {
                Bitmap lbmpBitmap = bitmaps[liCounter];
                Rectangle lrBitmap = new Rectangle(new Point(), lbmpBitmap.Size);
                Point lptBottomRight = startPoints[liCounter];
                lptBottomRight.Offset(lptSize);

                if (lbmpBitmap == null)
                    throw new ArgumentException("Cannot contain null values.", "bitmaps");

                if (!lrBitmap.Contains(startPoints[liCounter]))
                    throw new ArgumentException("Must be inside all bitmaps.", "startPoints");

                if (!lrBitmap.Contains(lptBottomRight))
                    throw new ArgumentException("Size must fit in all bitmaps.");
            }

            BitmapData[] lbdD = new BitmapData[bitmaps.Length];

            try
            {
                for (int liCounter = 0; liCounter < bitmaps.Length; liCounter++)
                    lbdD[liCounter] = bitmaps[liCounter].LockBits(
                    new Rectangle(startPoints[liCounter], size),
                    lockModes[liCounter], pixelFormats[liCounter]);

                ApplyPerPixel(lbdD, threads, processPixel64, processPixel32);
            }
            finally
            {
                for (int liCounter = 0; liCounter < bitmaps.Length; liCounter++)
                    if(lbdD[liCounter].Scan0 != IntPtr.Zero) bitmaps[liCounter].UnlockBits(lbdD[liCounter]);
            }
        }

        /// <summary>
        /// Calls a delegate per pixel using multi-threading.
        /// </summary>
        /// <param name="bitmapDatas">The datas for the bitmaps, these must all be the same size and bit size (32-bit or 64-bit only).</param>
        /// <param name="threads">The number of threads to use (in addition to the current thread).</param>
        /// <param name="processPixel64">The delegate to call for dual pixel processing. Or null for 32-bit only.</param>
        /// <param name="processPixel32">The delegate to call for dual single pixel processing.</param>
        public static void ApplyPerPixel(
            BitmapData[] bitmapDatas, int threads,
            ProcessPixel64 processPixel64, ProcessPixel32 processPixel32)
        {
            for (int liCounter = 0; liCounter < bitmapDatas.Length; liCounter++)
            {
                if (bitmapDatas[0].Width != bitmapDatas[liCounter].Width)
                    throw new ArgumentException("Must be the same width.", "bitmapDatas");

                if (bitmapDatas[0].Height != bitmapDatas[liCounter].Height)
                    throw new ArgumentException("Must be the same height.", "bitmapDatas");

                if (bitmapDatas[0].PixelFormat.BitSize() != 32 && bitmapDatas[0].PixelFormat.BitSize() != 64)
                    throw new ArgumentException("All PixelFormats must be 32-bit or 64-bit.", "bitmapDatas");

                if (bitmapDatas[0].PixelFormat.BitSize() != bitmapDatas[liCounter].PixelFormat.BitSize())
                    throw new ArgumentException("All PixelFormats sizes must be the same.", "bitmapDatas");
            }

            ThreadedApplyPerPixel(bitmapDatas, processPixel64, processPixel32, threads);
        }

        /// <summary>
        /// Calls a delegate per pixel using multi-threading.
        /// </summary>
        /// <param name="bitmapDatas">The datas for the bitmaps, these must all be the same size and bit size (32-bit or 64-bit only).</param>
        /// <param name="threads">The number of threads to use (in addition to the current thread).</param>
        /// <param name="processPixel32">The delegate to call for dual single pixel processing.</param>
        public static void ApplyPerPixel(BitmapData[] bitmapDatas, int threads, ProcessPixel32 processPixel32)
        {
            ThreadedApplyPerPixel(bitmapDatas, null, processPixel32, threads);
        }

        /// <summary>
        /// Calls a delegate per pixel using multi-threading.
        /// </summary>
        /// <param name="bitmaps">The bitmaps to pass into the delegate.</param>
        /// <param name="lockModes">The lock modes for each bitmap.</param>
        /// <param name="pixelFormats">The pixel formats for each bitmap (must be 32-bits or 64-bits in size).</param>
        /// <param name="startPoints">The top left points of the locking rectangle for each bitmap.</param>
        /// <param name="size">The size of the locking rectangle for all bitmaps.</param>
        /// <param name="threads">The number of threads to use (in addition to the current thread).</param>
        /// <param name="processPixel32">The delegate to call for dual single pixel processing.</param>
        public static void ApplyPerPixel(
            Bitmap[] bitmaps, ImageLockMode[] lockModes, PixelFormat[] pixelFormats, Point[] startPoints,
            Size size, int threads,
            ProcessPixel32 processPixel32)
        {
            ApplyPerPixel(bitmaps, lockModes, pixelFormats, startPoints, size, threads, null, processPixel32);
        }

        /// <summary>
        /// Calls a delegate per pixel for specified scan lines.
        /// </summary>
        /// <param name="processPixel64">The 64-bit dual pixel delegate to call. Or null for none.</param>
        /// <param name="processPixel32">The 32-bit dual pixel delegate to call.</param>
        /// <param name="d">The bitmap datas to call for.</param>
        /// <param name="lines">The number of scan lines to process.</param>
        /// <param name="pixelsWidth">The size in bytes of the one scan line of all the bitmaps actual pixels.</param>
        /// <param name="offset">The starting offset of the first scan line for each bitmap data.</param>
        /// <param name="stride">The entire stride size in bytes of one scan line of each of the bitmap datas.</param>
        /// <param name="thread">The thread number for this call (passed to delegate).</param>
        private static void Process(
            ProcessPixel64 processPixel64, ProcessPixel32 processPixel32,
            int lines, int pixelsWidth, BitmapData[] d, int[] offset, int[] stride,
            int thread)
        {
#if Unsafe
            unsafe
            {
                // Create the pointer array for each bitmap
                byte*[] lbpX = new byte*[d.Length];

                // Initialise each one
                for (int liC = 0; liC < d.Length; liC++)
                    lbpX[liC] = (((byte*)d[liC].Scan0) + offset[liC]);

                // If 64-bit processing is required
                if (processPixel64 != null)
                {
                    // Calculate line end offset
                    int li64BitBytes = pixelsWidth - (pixelsWidth % 8);

                    // Determine if 32-bit processing for last pixel is required
                    bool lb32Bit = (pixelsWidth % 8) != 0;

                    // Calculate line end to line start offset
                    int[] liStride = new int[d.Length];
                    for (int liC = 0; liC < d.Length; liC++)
                        liStride[liC] = (stride[liC] - pixelsWidth) + (lb32Bit ? 4 : 0);

                    // Fix the pointer
                    fixed (byte** lbpbpX = lbpX)
                    {
                        // For each line
                        while (lines > 0)
                        {
                            // Get the line end
                            byte* lbpLineEnd = lbpX[0] + li64BitBytes;

                            // For each 2-pixels
                            while (lbpX[0] < lbpLineEnd)
                            {
                                // Process the 64-bit dual pixel delegate
                                processPixel64((UInt64*)*lbpbpX, thread);

                                // Update the pointers to move to the next 2 pixels
                                for (int liC = 0; liC < lbpX.Length; liC++) lbpX[liC] += 8;
                            }

                            // If there is an odd number of pixels then process the last pixel with the 32-bit function
                            if (lb32Bit) processPixel32((UInt32*)*lbpbpX, thread);

                            // Increment the pointers to the start of the next line
                            for (int liC = 0; liC < lbpX.Length; liC++) lbpX[liC] += liStride[liC];

                            // Decrement the lines proecessed
                            lines--;
                        }
                    }
                }
                else
                {
                    // Calculate offset for line start to line end
                    int li32BitBytes = pixelsWidth;

                    // Calculate line end to line start offset
                    int[] liStride = new int[d.Length];
                    for (int liC = 0; liC < d.Length; liC++)
                        liStride[liC] = (stride[liC] - pixelsWidth);

                    // Fix the pointer
                    fixed (byte** lbpbpX = lbpX)
                    {
                        // For each line
                        while (lines > 0)
                        {
                            // Get the line end
                            byte* lbpLineEnd = lbpX[0] + li32BitBytes;

                            // For each pixel
                            while (lbpX[0] < lbpLineEnd)
                            {
                                // Process the 32-bit pixel delegate
                                processPixel32((UInt32*)*lbpbpX, thread);

                                // Update the pointers to move to the next pixel
                                for (int liC = 0; liC < lbpX.Length; liC++) lbpX[liC] += 4;
                            }

                            // Increment the pointers to the start of the next line
                            for (int liC = 0; liC < lbpX.Length; liC++) lbpX[liC] += liStride[liC];

                            // Decrement the lines proecessed
                            lines--;
                        }
                    }
                }
            }
#else
            // Create the index array for each bitmap data
            Int64[] lbpX = new Int64[d.Length];

            // Initialise it to the starting point of each one
            for (int liC = 0; liC < d.Length; liC++)
                lbpX[liC] = (((Int64)d[liC].Scan0) + offset[liC]);

            // If 64-bit processing is needed
            if (processPixel64 != null)
            {
                // Calculate line end offset
                int li64BitBytes = pixelsWidth - (pixelsWidth % 8);

                // Determine if 32-bit processing for last pixel is required
                bool lb32Bit = (pixelsWidth % 8) != 0;

                // Create stride array
                int[] liStride = new int[d.Length];
                for (int liC = 0; liC < d.Length; liC++)
                    liStride[liC] = stride[liC] - pixelsWidth + (lb32Bit ? 4 : 0);

                // Array passed to 64-bit delegate
                UInt64[] luiValues = new UInt64[d.Length];

                // Array passed to 32-bit delegate
                UInt32[] luiValues32 = new UInt32[d.Length];

                // For each line
                while (lines > 0)
                {
                    // Get the line end
                    Int64 lbpLineEnd = (Int64)((int)lbpX[0] + li64BitBytes);

                    // For each 2-pixels
                    while (lbpX[0] < lbpLineEnd)
                    {
                        // Load the values into the array
                        for (int liC = 0; liC < lbpX.Length; liC++)
                            luiValues[liC] = (UInt64)Marshal.ReadInt64((IntPtr)lbpX[liC]);

                        // Call the 64-bit delegate
                        processPixel64(luiValues, thread);

                        // Load the values back into the image data
                        for (int liC = 0; liC < lbpX.Length; liC++)
                            Marshal.WriteInt64((IntPtr)lbpX[liC], (Int64)luiValues[liC]);

                        // Update the pointer indexes to move to the next pixel
                        for (int liC = 0; liC < lbpX.Length; liC++) lbpX[liC] += 8;
                    }

                    // If there is an odd number of pixels then process the last pixel with the 32-bit function
                    if (lb32Bit)
                    {
                        // Load the values into the array
                        for (int liC = 0; liC < lbpX.Length; liC++)
                            luiValues32[liC] = (UInt32)Marshal.ReadInt32((IntPtr)lbpX[liC]);
                        
                        // Call the 32-bit delegate
                        processPixel32(luiValues32, thread);

                        // Load the values back into the image data
                        for (int liC = 0; liC < lbpX.Length; liC++)
                            Marshal.WriteInt32((IntPtr)lbpX[liC], (Int32)luiValues32[liC]);
                    }

                    // Increment the pointers to the start of the next line
                    for (int liC = 0; liC < lbpX.Length; liC++) lbpX[liC] += liStride[liC];

                    // Decrement the lines proecessed
                    lines--;
                }
            }
            else
            {
                // Calculate offset for line start to line end
                int li32BitBytes = pixelsWidth;

                // Create stride array
                int[] liStride = new int[d.Length];
                for (int liC = 0; liC < d.Length; liC++)
                    liStride[liC] = stride[liC] - pixelsWidth;

                // Array passed to 32-bit delegate
                UInt32[] luiValues32 = new UInt32[d.Length];

                // For each line
                while (lines > 0)
                {
                    // Get the line end
                    Int64 lbpLineEnd = lbpX[0] + li32BitBytes;

                    // For each pixel
                    while (lbpX[0] < lbpLineEnd)
                    {
                        // Load the values into the array
                        for (int liC = 0; liC < lbpX.Length; liC++)
                            luiValues32[liC] = (UInt32)Marshal.ReadInt32((IntPtr)lbpX[liC]);

                        // Call the 32-bit delegate
                        processPixel32(luiValues32, thread);

                        // Load the values back into the image data
                        for (int liC = 0; liC < lbpX.Length; liC++)
                            Marshal.WriteInt32((IntPtr)lbpX[liC], (Int32)luiValues32[liC]);

                        // Update the pointer indexes to move to the next pixel
                        for (int liC = 0; liC < lbpX.Length; liC++) lbpX[liC] += 4;
                    }

                    // Increment the pointers to the start of the next line
                    for (int liC = 0; liC < lbpX.Length; liC++) lbpX[liC] += liStride[liC];

                    // Decrement the lines proecessed
                    lines--;
                }
            }
#endif
        }

        /// <summary>
        /// Uses multi-threading to process each pixel using a delegate.
        /// </summary>
        /// <param name="data">The array of datas to process (all datas must be the same size and bit size).</param>
        /// <param name="processPixel64">The 64-bit dual pixel delegate.</param>
        /// <param name="processPixel32">The 32-bit pixel delegate.</param>
        /// <param name="threads">The number of threads to use in addition to the current thread.</param>
        private static void ThreadedApplyPerPixel(
                   BitmapData[] data,
                   ProcessPixel64 processPixel64,
                   ProcessPixel32 processPixel32,
                   int threads)
        {
            // All bitmap datas must be the same size
            int lines = data[0].Height;
            int liPixelsWidth = data[0].Width * 4;

            // If there isnt enough scanlines then minimise the threads used
            if (lines < threads + 1) threads = lines - 1;

            // Calculates scanlines per thread
            int liLinesPerThread = lines / (threads + 1);

            // Calculate per bitmap data values
            int[] liBytesPerThread = new int[data.Length];
            int[] liStrides = new int[data.Length];
            int[] liOffsets = new int[data.Length];
            for (int liC = 0; liC < data.Length; liC++)
            {
                liStrides[liC] = data[liC].Stride;
                liBytesPerThread[liC] = liLinesPerThread * liStrides[liC];
            }

            // Array for threads
            Thread[] ltThreads = new Thread[threads];

            // Started auto reset events
            AutoResetEvent[] lareStartedEvents = new AutoResetEvent[threads];

            // For each thread
            for (int liCounter = 0; liCounter < threads; liCounter++)
            {
                // Cache the offsets for the thread
                int[] liCurrOffset = (int[])liOffsets.Clone();

                // Get the thread number
                int liThread = liCounter + 1;

                // Create the ARE
                lareStartedEvents[liCounter] = new AutoResetEvent(false);

                // Create the thread
                ltThreads[liCounter] = new Thread(
                    (object args) =>
                    {
                        ((AutoResetEvent)(((Object[])args)[0])).Set();
                        Process(
                            processPixel64, processPixel32, liLinesPerThread, liPixelsWidth, 
                            data, liCurrOffset, liStrides, liThread);
                    }
                );

                // Start the thread
                ltThreads[liCounter].Start(new object[] { lareStartedEvents[liCounter] });

                // Increment the offsets for the next thread
                for(int liC = 0; liC < data.Length; liC++)
                    liOffsets[liC] += liBytesPerThread[liC];
            }

            // Call the current thread process
            Process(
                processPixel64, processPixel32, lines - (liLinesPerThread * threads), liPixelsWidth, 
                data, liOffsets, liStrides, 0);

            // Ensure each thread has started
            foreach (AutoResetEvent lareEvent in lareStartedEvents)
                lareEvent.WaitOne();

            // Ensure each thread has finished
            foreach (Thread ltThread in ltThreads)
                ltThread.Join();
        }

        #endregion
    }
}
