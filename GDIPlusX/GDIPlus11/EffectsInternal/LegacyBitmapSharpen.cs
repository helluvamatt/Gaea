//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

// Define AccurateSharpen to use Singles instead of Int32s for
// processing. This does make accuracy higher at the expense
// of performance.
//#define AccurateSharpen

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

#if !Unsafe
using System.Runtime.InteropServices;
#endif

namespace GDIPlusX.GDIPlus11.EffectsInternal
{
    #region Aliases

    using Proc = GDIPlusX.GDIPlus11.Internal.BitmapPerPixelProcessing;

    // Scalar value used for summing and averaging
#if AccurateSharpen
    using Scalar = System.Single;
#else
    using Scalar = System.Int32;
#endif

    #endregion

    /// <summary>
    /// Encapsulates a class which can apply a sharpening to an image.
    /// </summary>
    /// <remarks>
    /// This class supports Unsafe and Safe mode, define Unsafe to compile in unsafe mode.
    /// Unsafe mode is faster.
    /// </remarks>
    internal class LegacyBitmapSharpen
    {
        #region Private Constants

        /// <summary>
        /// Defines the number of channels to process
        /// </summary>
        private const int ciChannels = 3;

        #endregion

        #region Protected Locals

        /// <summary>
        /// True if the tables are invalid and need repopulating.
        /// </summary>
        protected bool mbTablesInvalid = true;

        /// <summary>
        /// The radius of the blur convolution, size = 1 + (radius * 2).
        /// </summary>
        protected int miRadius;

        /// <summary>
        /// The radius of the blur operation.
        /// </summary>
        protected float mfRadius;

        /// <summary>
        /// The amount of sharpening to apply (0 - 100).
        /// </summary>
        protected float mfAmount;

        /// <summary>
        /// The length of the kernel.
        /// </summary>
        protected int miKernelLen;

        /// <summary>
        /// The last calculated kernel multiply table.
        /// </summary>
        protected Scalar[,] msMulTable;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new sharpen class with a certain radius and amount.
        /// </summary>
        /// <param name="radius">The radius for the sharpen.</param>
        /// <param name="amount">The amount of sharpening (0 - 100).</param>
        public LegacyBitmapSharpen(float radius, float amount)
        {
            Amount = amount;
            Radius = radius;
        }

        #endregion

        #region Protected Virtual Methods 

        /// <summary>
        /// Converts from a float value to a scalar value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A scalar value.</returns>
        protected virtual Scalar FloatToScalar(float value)
        {
#if AccurateSharpen
            return (Scalar)value;
#else
            // When scalar is set to use Int32s, all values
            // are multiplied by 128. This is to increase
            // sharpen accuracy to 1 / 128 radius.
            return (Scalar)Math.Floor(value * 128f);
#endif
        }

        /// <summary>
        /// Recalculates the kernel multiply table if needed.
        /// </summary>
        protected virtual void ReCalculateTables()
        {
            // If the tables are valid
            if (mbTablesInvalid)
            {
                // Cache values
                miKernelLen = miRadius * 2 + 1;
                msMulTable = new Scalar[miKernelLen, 256];
                float lfKernelSum = 0.0f;
                float lfRadius;

                for (int liKernelIndex = 1; liKernelIndex <= miRadius; liKernelIndex++)
                {
                    // If its the outside radial value then use the floating
                    // point part if radius isnt a whole value
                    if (liKernelIndex == miRadius && miRadius != mfRadius)
                        lfRadius = mfRadius - (float)Math.Truncate(mfRadius);
                    else
                        // Otherwise just use the whole part
                        lfRadius = 1.0f + (miRadius - liKernelIndex);

                    // Calculate left and right values for kernel
                    int liLeft = miRadius - liKernelIndex;
                    int liRight = miRadius + liKernelIndex;

                    // Add to the kernel sum
                    lfKernelSum += lfRadius * 2.0f;

                    // Set the multiply table values
                    for (int liValue = 0; liValue < 256; liValue++)
                        msMulTable[liLeft, liValue] = msMulTable[liRight, liValue] = -(FloatToScalar(lfRadius) * liValue);
                }

                // Set the center kernel value to the kernel sum
                lfRadius = lfKernelSum;

                // Set the multiply table values for the center kernel
                for (int j = 0; j < 256; j++)
                    msMulTable[miRadius, j] = FloatToScalar(lfRadius) * j;

                // Validate the tables
                mbTablesInvalid = false;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Processes the horizontal scanlines for sharpening.
        /// </summary>
        /// <param name="width">The width of the source and dest parameters.</param>
        /// <param name="height">The height of the source and dest parameters.</param>
        /// <param name="source">The source data, one element per channel, width then height.</param>
        /// <param name="dest">The destination data, one element per channel, width then height.</param>
        /// <param name="startLine">The starting line to process.</param>
        /// <param name="countLines">The number of lines to process.</param>
        protected void ProcessHorizontal(
            int width, int height, 
            Scalar[] source, Scalar[] dest, 
            int startLine, int countLines)
        {
            // Sums for channels
            Scalar lsBSum, lsGSum, lsRSum;

            // Calculate channel width in elements
            int liChannelWidth = width * ciChannels;

            // Claculate offset for next kernel point
            int liKernelChannelsMinus1 = (miKernelLen - 1) * ciChannels;

#if Unsafe
            unsafe
            {
                // Fix the source, destination and multiply table
                fixed (Scalar* lpsSource = source, lpsDest = dest, lpsMultable = msMulTable)
                {
                    // Pointer to multiply table
                    Scalar* lpsMulTableBase;

                    // Source kernel point pointer
                    Scalar* lpsKernelPoint = &lpsSource[-(miRadius * ciChannels)];

                    // Destination pointer
                    Scalar* lpsDestPoint = &lpsDest[0];

                    // Kernel point start pointer
                    Scalar* lpsKernelPointStart = lpsSource;

                    // Kernel point end pointer
                    Scalar* lpsKernelPointEnd = lpsSource + liChannelWidth;

                    // Offset pointers for starting line
                    int liStartOffset = liChannelWidth * startLine;
                    lpsKernelPointStart += liStartOffset;
                    lpsKernelPointEnd += liStartOffset;
                    lpsKernelPoint += liStartOffset;
                    lpsDestPoint += liStartOffset;

                    // Process number of lines requested
                    for (int liY = 0; liY < countLines; liY++)
                    {
                        // Process each pixel in scan line
                        for (int liX = 0; liX < width; liX++)
                        {
                            // Default sums
                            lsBSum = lsGSum = lsRSum = 0;

                            // Set the multiply table base
                            lpsMulTableBase = lpsMultable;

                            // For each kernel value
                            for (int liKernelIndex = 0; liKernelIndex < miKernelLen; liKernelIndex++)
                            {
                                // If in range then add to the kernel sums
                                if (lpsKernelPoint >= lpsKernelPointStart && lpsKernelPoint < lpsKernelPointEnd)
                                {
                                    lsBSum += lpsMulTableBase[(int)lpsKernelPoint[0]];
                                    lsGSum += lpsMulTableBase[(int)lpsKernelPoint[1]];
                                    lsRSum += lpsMulTableBase[(int)lpsKernelPoint[2]];
                                }
                                else
                                {
                                    // Otherwise assume gray (this is to put it inline with GDI+ 1.1 functionality)
                                    lsBSum += lpsMulTableBase[127];
                                    lsGSum += lpsMulTableBase[127];
                                    lsRSum += lpsMulTableBase[127];
                                }

                                // Increment to kernel line
                                lpsMulTableBase += 256;

                                // Incremenent kernel point
                                lpsKernelPoint += ciChannels;
                            }

                            // Save sums to destination
                            lpsDestPoint[0] = (Scalar)lsBSum;
                            lpsDestPoint[1] = (Scalar)lsGSum;
                            lpsDestPoint[2] = (Scalar)lsRSum;

                            // Next destination point
                            lpsDestPoint += ciChannels;

                            // Reset kernel point
                            lpsKernelPoint -= liKernelChannelsMinus1;
                        }

                        // Increment start and end kernel points
                        lpsKernelPointStart += liChannelWidth;
                        lpsKernelPointEnd += liChannelWidth;
                    }
                }
            }
#else
            // Kernel point index
            int liKernelPoint = -(miRadius * ciChannels);

            // Destination point index
            int liDestPoint = 0;

            // Kernel point start
            int liKernelPointStart = 0;

            // Kernel point end
            int liKernelPointEnd = liChannelWidth;

            // Offset indexes for starting line
            int liStartOffset = liChannelWidth * startLine;
            liKernelPointStart += liStartOffset;
            liKernelPointEnd += liStartOffset;
            liKernelPoint += liStartOffset;
            liDestPoint += liStartOffset;

            // Process number of lines requested
            for (int liY = 0; liY < countLines; liY++)
            {
                // Process each pixel in scan line
                for (int liX = 0; liX < width; liX++)
                {
                    // Reset sums
                    lsBSum = lsGSum = lsRSum = 0;

                    // For each kernel value
                    for (int liKernelIndex = 0; liKernelIndex < miKernelLen; liKernelIndex++)
                    {
                        // If in range then add to the kernel sums
                        if (liKernelPoint >= liKernelPointStart && liKernelPoint < liKernelPointEnd)
                        {
                            lsBSum += msMulTable[liKernelIndex, (int)source[liKernelPoint]];
                            lsGSum += msMulTable[liKernelIndex, (int)source[liKernelPoint + 1]];
                            lsRSum += msMulTable[liKernelIndex, (int)source[liKernelPoint + 2]];
                        }
                        else
                        {
                            // Otherwise assume gray (this is to put it inline with GDI+ 1.1 functionality)
                            lsBSum += msMulTable[liKernelIndex, 127];
                            lsGSum += msMulTable[liKernelIndex, 127];
                            lsRSum += msMulTable[liKernelIndex, 127];
                        }

                        // Incremenent kernel point
                        liKernelPoint += ciChannels;
                    }

                    // Save sums to destination
                    dest[liDestPoint] = (Scalar)lsBSum;
                    dest[liDestPoint + 1] = (Scalar)lsGSum;
                    dest[liDestPoint + 2] = (Scalar)lsRSum;

                    // Next destination point
                    liDestPoint += ciChannels;

                    // Reset kernel point
                    liKernelPoint -= liKernelChannelsMinus1;
                }

                // Increment start and end kernel points
                liKernelPointStart += liChannelWidth;
                liKernelPointEnd += liChannelWidth;
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width">The width of the source and dest parameters.</param>
        /// <param name="height">The height of the source and dest parameters.</param>
        /// <param name="destData">The destination image data to write to.</param>
        /// <param name="destDataStride">The scan line stride size in bytes for the dest data parameter.</param>
        /// <param name="dataLineOffset">The number of bytes from the end of a line to the start of the next for the dest data parameter.</param>
        /// <param name="source">The source data, one element per channel, width then height.</param>
        /// <param name="horSource">The horizontal scan source data, one element per channel, width then height.</param>
        /// <param name="startLine">The starting horizontal scan line to process.</param>
        /// <param name="countLines">The number of horizontal scan lines to process.</param>
        protected void ProcessVertical(
            int width, int height, 
            IntPtr destData, int destDataStride, int dataLineOffset, 
            Scalar[] source, Scalar[] horSource, 
            int startLine, int countLines)
        {
            // Calculate divider for sharpening amount (radius * amount) ^ 2
            float lfDiv = ((mfRadius + 1.0f) * (100.0f / mfAmount));
            Scalar lsDiv = FloatToScalar(lfDiv * lfDiv);

            // Sums for channels
            Scalar lsBSum, lsGSum, lsRSum;
            int liChannelWidth = width * ciChannels;
            int liKernelNextCol = ciChannels - (miKernelLen * liChannelWidth);

#if Unsafe
            unsafe
            {
                // Fix the source, destination and multiply table
                fixed (Scalar* lpsSource = source, lpsHorSource = horSource, lpsMultable = msMulTable)
                {
                    // Pointer to multiply table
                    Scalar* lpsMulTableBase;

                    byte* lpbDest = (byte*)destData;

                    // Kernel point start pointer
                    Scalar* lpsKernelPointStart = lpsSource;

                    // Kernel point end pointer
                    Scalar* lpsKernelPointEnd = lpsSource + (height * liChannelWidth);

                    // Source kernel point pointer
                    Scalar* lpsKernelPoint = lpsSource - (miRadius * liChannelWidth);
                    
                    // Horizontal processing pointer
                    Scalar* lpsHorResult = lpsHorSource;

                    // Offset pointers for starting line
                    int liStartOffset = liChannelWidth * startLine;
                    lpsKernelPoint += liStartOffset;
                    lpsHorResult += liStartOffset;
                    lpbDest += destDataStride * startLine;

                    // Process number of lines requested
                    for (int liY = 0; liY < countLines; liY++)
                    {
                        // Process each pixel in scan line
                        for (int liX = 0; liX < width; liX++)
                        {
                            // Default sums
                            lsBSum = lsGSum = lsRSum = 0;

                            // Set the multiply table base
                            lpsMulTableBase = lpsMultable;

                            // For each kernel value
                            for (int liKernelIndex = 0; liKernelIndex < miKernelLen; liKernelIndex++)
                            {
                                // If in range then add to the kernel sums
                                if (lpsKernelPoint >= lpsKernelPointStart && lpsKernelPoint < lpsKernelPointEnd)
                                {
                                    lsBSum += lpsMulTableBase[(int)lpsKernelPoint[0]];
                                    lsGSum += lpsMulTableBase[(int)lpsKernelPoint[1]];
                                    lsRSum += lpsMulTableBase[(int)lpsKernelPoint[2]];
                                }
                                else
                                {
                                    // Otherwise assume gray (this is to put it inline with GDI+ 1.1 functionality)
                                    lsBSum += lpsMulTableBase[127];
                                    lsGSum += lpsMulTableBase[127];
                                    lsRSum += lpsMulTableBase[127];
                                }

                                // Increment to kernel line
                                lpsMulTableBase += 256;

                                // Incremenent kernel point
                                lpsKernelPoint += liChannelWidth;
                            }

                            // Write average of horizontal and vertical sharpening plus the current pixel to the destination
                            lpbDest[0] = Proc.TruncateChannel(lpbDest[0] + (((lsBSum + lpsHorResult[0]) / lsDiv)));
                            lpbDest[1] = Proc.TruncateChannel(lpbDest[1] + (((lsGSum + lpsHorResult[1]) / lsDiv)));
                            lpbDest[2] = Proc.TruncateChannel(lpbDest[2] + (((lsRSum + lpsHorResult[2]) / lsDiv)));

                            // Incremement the destination
                            lpbDest += 4;

                            // Increment the kernel point to the start of the next column
                            lpsKernelPoint += liKernelNextCol;

                            // Increment the horizontal result pointer
                            lpsHorResult += ciChannels;
                        }

                        // Incremenent the destination pointer
                        lpbDest += dataLineOffset;
                    }
                }
            }
#else
            // Index to destination
            int liDest = 0;

            // Kernel point start index
            int liKernelPointStart = 0;

            // Kernel point end index
            int liKernelPointEnd = (height * liChannelWidth);

            // Kernel point index
            int liKernelPoint = -(miRadius * liChannelWidth);

            // Hor result index
            int liHorResult = 0;

            // Offset indexes for starting line
            int liStartOffset = liChannelWidth * startLine;
            liKernelPoint += liStartOffset;
            liHorResult += liStartOffset;
            liDest += destDataStride * startLine;

            // Process number of lines requested
            for (int liY = 0; liY < countLines; liY++)
            {
                // For each pixel
                for (int liX = 0; liX < width; liX++)
                {
                    // Reset sums
                    lsBSum = lsGSum = lsRSum = 0;

                    // For each kernel value
                    for (int liKernelIndex = 0; liKernelIndex < miKernelLen; liKernelIndex++)
                    {
                        // If in range then add to the kernel sums
                        if (liKernelPoint >= liKernelPointStart && liKernelPoint < liKernelPointEnd)
                        {
                            lsBSum += msMulTable[liKernelIndex, (int)source[liKernelPoint]];
                            lsGSum += msMulTable[liKernelIndex, (int)source[liKernelPoint+1]];
                            lsRSum += msMulTable[liKernelIndex, (int)source[liKernelPoint+2]];
                        }
                        else
                        {
                            // Otherwise assume gray (this is to put it inline with GDI+ 1.1 functionality)
                            lsBSum += msMulTable[liKernelIndex, 127];
                            lsGSum += msMulTable[liKernelIndex, 127];
                            lsRSum += msMulTable[liKernelIndex, 127];
                        }

                        // Increment the kernel point
                        liKernelPoint += liChannelWidth;
                    }

                    // Write average of horizontal and vertical sharpening plus the current pixel to the destination
                    Marshal.WriteByte(
                        destData, liDest, 
                        Proc.TruncateChannel(Marshal.ReadByte(destData, liDest) + ((lsBSum + horSource[liHorResult]) / lsDiv)));

                    // Write average of horizontal and vertical sharpening plus the current pixel to the destination
                    Marshal.WriteByte(
                        destData, liDest + 1, 
                        Proc.TruncateChannel(Marshal.ReadByte(destData, liDest+1) + ((lsGSum + horSource[liHorResult + 1]) / lsDiv)));

                    // Write average of horizontal and vertical sharpening plus the current pixel to the destination
                    Marshal.WriteByte(
                        destData, liDest + 2, 
                        Proc.TruncateChannel(Marshal.ReadByte(destData, liDest+2) + ((lsRSum + horSource[liHorResult + 2]) / lsDiv)));

                    // Next destination pixel
                    liDest += 4;

                    // Next kernel point column
                    liKernelPoint += liKernelNextCol;

                    // Next horizontal result pixel
                    liHorResult += ciChannels;
                }

                // Next destination scan line
                liDest += dataLineOffset;
            }
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Applys sharpening to a Bitmap.
        /// </summary>
        /// <param name="bitmap">The Bitmap to apply the sharpening to.</param>
        /// <param name="rectOfInterest">The rectangle to apply the Effect or Rectangle.Empty for entire bitmap.</param>
        /// <param name="pixelFormat">The pixel format to use (must be a 32-bit pixel format).</param>
        /// <param name="threads">The maximum number of threads in addition to the current thread to use.</param>
        public void ApplyToBitmap(Bitmap bitmap, Rectangle rectOfInterest, PixelFormat pixelFormat, int threads)
        {
            if (rectOfInterest.IsEmpty) rectOfInterest = new Rectangle(new Point(), bitmap.Size);

            if (Amount > 0 && Radius > 0)
            {
                BitmapData lbdData = bitmap.LockBits(rectOfInterest, ImageLockMode.ReadWrite, pixelFormat);

                try
                {
                    Process(lbdData, lbdData, threads);
                }
                finally
                {
                    bitmap.UnlockBits(lbdData);
                }
            }
        }

        /// <summary>
        /// Processes the blur effect.
        /// </summary>
        /// <param name="src">The source bitmap data.</param>
        /// <param name="dest">The destination bitmap data.</param>
        /// <param name="threads">The maximum number of threads in addition to the current thread to use.</param>
        public void Process(BitmapData src, BitmapData dest, int threads)
        {
            // Check the tables to ensure they are updated
            ReCalculateTables();

            // Create the temporary data holders
            int liPixelCount = src.Width * src.Height;
            Scalar[] lsSource = new Scalar[liPixelCount * ciChannels];
            Scalar[] lsHorSource = new Scalar[liPixelCount * ciChannels];

            // Calculate the number of bytes from the end of one line to the start of the next for src
            int liSrcDataLineOffset = src.Stride - (src.Width * 4);

            // Calculate the number of bytes from the end of one line to the start of the next for src
            int liDestDataLineOffset = dest.Stride - (dest.Width * 4);

#if Unsafe
            // Copy the data from the source to the array
            unsafe
            {
                // Fix the array
                fixed (Scalar* lpsSource = lsSource)
                {
                    // Get the start src pixel
                    byte* lpbPixel = (byte*)src.Scan0;

                    // Get the start dest pixel
                    Scalar* lpsDest = lpsSource;

                    // For each line
                    for (int liY = 0; liY < src.Height; liY++)
                    {
                        // For each pixel
                        for (int liX = 0; liX < src.Width; liX++)
                        {
                            // Copy the data from bytes to image scalars
                            lpsDest[0] = lpbPixel[0];
                            lpsDest[1] = lpbPixel[1];
                            lpsDest[2] = lpbPixel[2];

                            // Incrmement the dest pixel
                            lpsDest += ciChannels;

                            // Increment the source pixel
                            lpbPixel += 4;
                        }

                        // Increment the src line
                        lpbPixel += liSrcDataLineOffset;
                    }
                }
            }
#else
            // Dest index
            int liIndex = 0;

            // Source index
            int liPixelIndex = 0;

            // For each line
            for (int liY = 0; liY < src.Height; liY++)
            {
                // For each pixel
                for (int liX = 0; liX < src.Width; liX++)
                {
                    // Copy the data from bytes to image scalars
                    lsSource[liIndex] = Marshal.ReadByte(src.Scan0, liPixelIndex);
                    lsSource[liIndex + 1] = Marshal.ReadByte(src.Scan0, liPixelIndex + 1);
                    lsSource[liIndex + 2] = Marshal.ReadByte(src.Scan0, liPixelIndex + 2);

                    // Next destination pixel
                    liIndex += ciChannels;

                    // Next source pixel
                    liPixelIndex += 4;
                }

                // Next source line
                liPixelIndex += liSrcDataLineOffset;
            }
#endif

            // Calculate number of liens
            int liTotalLines = src.Height;

            // Reduce threads to maximum of number of lines - 1
            if (liTotalLines < threads + 1) threads = liTotalLines - 1;

            // Calculate lines per thread
            int liLinesPerThread = liTotalLines / (threads + 1);

            // Thread arrays
            Thread[] ltThreads = new Thread[threads];
            AutoResetEvent[] lareStartedEvents = new AutoResetEvent[threads];

            // Two modes, Horizontal and Vertical, must be processed seperately for blur effect
            for (int liMode = 0; liMode <= 1; liMode++)
            {
                // Line offset
                int liLineOffset = 0;

                // For each thread
                for (int liCounter = 0; liCounter < threads; liCounter++)
                {
                    // Cache line offset for thread
                    int liCurrLineOffset = liLineOffset;

                    // Create the started event
                    lareStartedEvents[liCounter] = new AutoResetEvent(false);

                    // If mode 0 then 
                    if (liMode == 0)
                        // Execute horizontal processing
                        ltThreads[liCounter] = new Thread(
                            (object args) =>
                            {
                                ((AutoResetEvent)(((Object[])args)[0])).Set();
                                ProcessHorizontal(
                                    src.Width, src.Height, 
                                    lsSource, lsHorSource, 
                                    liCurrLineOffset, liLinesPerThread);
                            }
                        );
                    else
                        // Otherwise execute vertical processing
                        ltThreads[liCounter] = new Thread(
                            (object args) =>
                            {
                                ((AutoResetEvent)(((Object[])args)[0])).Set();
                                ProcessVertical(
                                    src.Width, src.Height, 
                                    dest.Scan0, dest.Stride, liDestDataLineOffset, 
                                    lsSource, lsHorSource, 
                                    liCurrLineOffset, liLinesPerThread);
                            }
                        );

                    // Start the thread
                    ltThreads[liCounter].Start(new object[] { lareStartedEvents[liCounter] });

                    // Incremenent the line offset
                    liLineOffset += liLinesPerThread;
                }

                int liLines = liTotalLines - (liLinesPerThread * threads);

                // If mode 0 then 
                if (liMode == 0)
                    // Execute horizontal processing
                    ProcessHorizontal(
                        src.Width, src.Height, 
                        lsSource, lsHorSource, 
                        liLineOffset, liLines);
                else
                    // Otherwise execute vertical processing
                    ProcessVertical(
                        src.Width, src.Height, 
                        dest.Scan0, dest.Stride, liDestDataLineOffset, 
                        lsSource, lsHorSource, 
                        liLineOffset, liLines);

                // Ensure each thread has started
                foreach (AutoResetEvent lareEvent in lareStartedEvents)
                    lareEvent.WaitOne();

                // Ensure each thread has finished
                foreach (Thread ltThread in ltThreads)
                    ltThread.Join();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the amount to sharpen (0 - 100).
        /// </summary>
        public float Amount
        {
            get
            {
                return mfAmount;
            }
            set
            {
                mfAmount = value;
            }
        }

        /// <summary>
        /// Gets or sets the readius of the sharpen.
        /// </summary>
        public float Radius
        {
            get
            {
                return mfRadius;
            }
            set
            {
                if (mfRadius != value)
                {
                    miRadius = (int)Math.Ceiling(value);
                    mfRadius = value;
                    mbTablesInvalid = true;
                }
            }
        }

        #endregion
    }
}