//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Imaging;
using GDIPlusX.GDIPlus11.Internal;

namespace GDIPlusX.GDIPlus11.EffectsInternal
{
    #region Aliases

    using Proc = BitmapPerPixelProcessing;

#if Unsafe
    using C32 = BitmapPerPixelProcessing.C32;
    using C64 = BitmapPerPixelProcessing.C64;
#endif

    #endregion

    internal static class LegacyBitmapPerPixelEffect
    {
        #region High Level Processing Methods

        /// <summary>
        /// Gets the HSL lookup tables for applying a HSL color.
        /// </summary>
        /// <param name="hueLevel">The amount of change in hue in degrees. -180 to 180. 0 is no change.</param>
        /// <param name="satLevel">The amount of change in saturation. -100 to 100. 0 is no change.</param>
        /// <param name="lumLevel">The amount of change in lightness. -100 to 100. 0 is no change.</param>
        /// <param name="hueLUT">The hue lookup table.</param>
        /// <param name="satLUT">The saturation lookup table.</param>
        /// <param name="lumLUT">The luminence lookup table.</param>
        public static void GetHSLLookupTables(
            int hueLevel, int satLevel, int lumLevel,
            ref byte[] hueLUT, ref byte[] satLUT, ref byte[] lumLUT)
        {
            double ldH = HSLColorConverter.Roll(hueLevel / 360.0);
            double ldS = 1.0 + (satLevel / 100.0);
            double ldL = lumLevel / 100.0;

            int liHOffset = 255 - (int)(255.0 * ldH);

            Proc.AllocLUT(ref hueLUT);
            Proc.AllocLUT(ref satLUT);
            Proc.AllocLUT(ref lumLUT);

            for (int liCounter = 0; liCounter <= 255; liCounter++)
            {
                double ldCounter = (double)liCounter / 255.0;
                hueLUT[(liCounter + liHOffset) % 256] = (byte)liCounter;
                satLUT[liCounter] = (byte)(HSLColorConverter.Clamp(ldS * ldCounter) * 255.0);
                lumLUT[liCounter] = (byte)(HSLColorConverter.Clamp(ldL + ldCounter) * 255.0);
            }
        }

        /// <summary>
        /// Gets the lookup tables for this effect when in legacy mode.
        /// </summary>
        /// <param name="adjustment">The adjustment type.</param>
        /// <param name="adjustValue">The adjustment value. Actual range depends on adjustment type. -255 to 255.</param>
        /// <param name="lut">The lookup table to be applied.</param>
        public static void GetColorCurveLUT(
            Interop11.GpCurveAdjustments adjustment, int adjustValue, ref byte[] lut)
        {
            Proc.AllocLUT(ref lut);

            QuadraticBezierCurve.Point[] ldInValues = null;
            QuadraticBezierCurve.Point[] ldOutValues = null;
            float lfLen;

            switch (adjustment)
            {
                case Interop11.GpCurveAdjustments.AdjustContrast:
                    lfLen = (float)(adjustValue / 100f) * 0.5f;

                    ldInValues = new QuadraticBezierCurve.Point[] {
                        new QuadraticBezierCurve.Point(0, 0),
                        new QuadraticBezierCurve.Point(0.1f, 0.1f),
                        new QuadraticBezierCurve.Point(0.25f + lfLen, 0.25f - lfLen),
                        new QuadraticBezierCurve.Point(0.5f, 0.5f),
                        new QuadraticBezierCurve.Point(0.75f - lfLen, 0.75f + lfLen),
                        new QuadraticBezierCurve.Point(0.9f, 0.9f),
                        new QuadraticBezierCurve.Point(1.0f, 1.0f)
                    };
                    break;

                case Interop11.GpCurveAdjustments.AdjustShadow:
                    lfLen = (float)(adjustValue / 100f) * 0.25f;

                    ldInValues = new QuadraticBezierCurve.Point[] {
                        new QuadraticBezierCurve.Point(0, 0),
                        new QuadraticBezierCurve.Point(0.10f, 0.10f + lfLen * 0.8f),
                        new QuadraticBezierCurve.Point(0.25f - lfLen, 0.25f + lfLen),
                        new QuadraticBezierCurve.Point(0.3f, 0.3f + lfLen * 0.8f),
                        new QuadraticBezierCurve.Point(0.4f, 0.4f),
                        new QuadraticBezierCurve.Point(0.5f, 0.5f),
                        new QuadraticBezierCurve.Point(0.75f, 0.75f),
                        new QuadraticBezierCurve.Point(1, 1)
                    };
                    break;

                case Interop11.GpCurveAdjustments.AdjustHighlight:
                    lfLen = (float)(adjustValue / 100f) * 0.25f;

                    ldInValues = new QuadraticBezierCurve.Point[] {
                        new QuadraticBezierCurve.Point(0, 0),
                        new QuadraticBezierCurve.Point(0.25f, 0.25f),
                        new QuadraticBezierCurve.Point(0.5f, 0.5f),
                        new QuadraticBezierCurve.Point(0.6f, 0.6f),
                        new QuadraticBezierCurve.Point(0.70f, 0.70f + lfLen * 0.8f),
                        new QuadraticBezierCurve.Point(0.75f - lfLen, 0.75f + lfLen),
                        new QuadraticBezierCurve.Point(0.90f, 0.90f + lfLen * 0.8f),
                        new QuadraticBezierCurve.Point(1.0f, 1.0f)
                    };
                    break;

                case Interop11.GpCurveAdjustments.AdjustMidtone:
                    lfLen = (float)(adjustValue / 100f) * 0.9f;

                    ldInValues = new QuadraticBezierCurve.Point[] {
                        new QuadraticBezierCurve.Point(0, 0),
                        new QuadraticBezierCurve.Point(0.1f, 0.1f),
                        new QuadraticBezierCurve.Point(0.5f - lfLen, 0.5f + lfLen),
                        new QuadraticBezierCurve.Point(0.9f, 0.9f),
                        new QuadraticBezierCurve.Point(1.0f, 1.0f)
                    };
                    break;

                case Interop11.GpCurveAdjustments.AdjustBlackSaturation:
                    lfLen = (float)(adjustValue / 100f) * 0.39f;
                    ldInValues = new QuadraticBezierCurve.Point[] {
                        new QuadraticBezierCurve.Point(lfLen, 0.0f),
                        new QuadraticBezierCurve.Point(1.0f, 1.0f)
                    };
                    break;

                case Interop11.GpCurveAdjustments.AdjustWhiteSaturation:
                    lfLen = (float)(adjustValue / 100f) * 0.39f;
                    ldInValues = new QuadraticBezierCurve.Point[] {
                        new QuadraticBezierCurve.Point(0.0f, 0.0f),
                        new QuadraticBezierCurve.Point(lfLen, 1.0f)
                    };
                    break;
            }

            if (ldInValues != null)
            {
                ldOutValues = new QuadraticBezierCurve.Point[256];
                QuadraticBezierCurve.Bezier(ldInValues, 256, ldOutValues);
            }

            for (int liCounter = 0; liCounter < 256; liCounter++)
            {
                byte lbValue = 0;

                switch (adjustment)
                {
                    case Interop11.GpCurveAdjustments.AdjustExposure:
                    case Interop11.GpCurveAdjustments.AdjustDensity:
                        lbValue = BitmapPerPixelProcessing.TruncateChannel(
                            liCounter + adjustValue);
                        break;

                    default:
                        lbValue = BitmapPerPixelProcessing.TruncateChannel(
                            (float)(QuadraticBezierCurve.EstimatedYValue(ldOutValues, liCounter / 255.0) * 255.0)
                        );
                        break;
                }

                lut[liCounter] = lbValue;
            }
        }

        /// <summary>
        /// Gets the lookup tables for the color balance effect.
        /// </summary>
        /// <param name="cyanRed">The cyan to red level for the effect -100 to 100. 0 is no change.</param>
        /// <param name="magentaGreen">The magenta to green level for the effect -100 to 100. 0 is no change.</param>
        /// <param name="yellowBlue">The yellow to blue level for the effect -100 to 100. 0 is no change.</param>
        /// <param name="b">The blue lookup table.</param>
        /// <param name="g">The green lookup table.</param>
        /// <param name="r">The red lookup table.</param>
        public static void GetColorBalanceLUTs(
            int cyanRed, int magentaGreen, int yellowBlue,
            ref byte[] b, ref byte[] g, ref byte[] r)
        {
            Proc.AllocLUT(ref r);
            Proc.AllocLUT(ref g);
            Proc.AllocLUT(ref b);

            int liRedMul = 1024 + (cyanRed * 1024 / 100);
            int liGreenMul = 1024 + (magentaGreen * 1024 / 100);
            int liBlueMul = 1024 + (yellowBlue * 1024 / 100);

            for (int liCounter = 0; liCounter <= 255; liCounter++)
            {
                r[liCounter] = BitmapPerPixelProcessing.TruncateChannel((liCounter * liRedMul) >> 10);
                g[liCounter] = BitmapPerPixelProcessing.TruncateChannel((liCounter * liGreenMul) >> 10);
                b[liCounter] = BitmapPerPixelProcessing.TruncateChannel((liCounter * liBlueMul) >> 10);
            }
        }

        /// <summary>
        /// Gets the lookup table for the brightness contrast effect to be applied to the R, G and B channels.
        /// </summary>
        /// <param name="brightness">The brightness level for the effect -255 to 255. 0 is no change.</param>
        /// <param name="contrast">The contrast level for the effect -100 to 100. 0 is no change.</param>
        /// <param name="lut">The lookup table data.</param>
        public static void GetBrightnessContrastLUT(
            int brightness, int contrast, ref byte[] lut)
        {
            Proc.AllocLUT(ref lut);

            int liContrast;
            int liBrightnessBefore = 0;
            int liBrightnessAfter = 0;

            if (contrast >= 0)
            {
                liBrightnessBefore =
                    (int)((float)brightness *
                    (1.0f - ((float)contrast / 200.0f)));
                liContrast = (101 * 1024) / (101 - contrast);
            }
            else
            {
                liContrast = 1024 * (100 + contrast) / 101;
                liBrightnessAfter =
                    (int)((float)brightness *
                    (1.0f - ((float)contrast / -200.0f)));
            }

            for (int liCounter = 0; liCounter <= 255; liCounter++)
            {
                byte lbValue = (byte)liCounter;
                lbValue = Proc.TruncateChannel(lbValue + liBrightnessBefore);
                lbValue = Proc.TruncateChannel((127 + (((lbValue - 127) * liContrast) >> 10)));
                lbValue = Proc.TruncateChannel(lbValue + liBrightnessAfter);
                lut[liCounter] = lbValue;
            }   
        }

        /// <summary>
        /// Gets the lookup table for the levels effect to be applied to the R, G and B channels.
        /// </summary>
        /// <param name="highlights">the Lighten level for highlights. 0 to 100. 100 is no change.</param>
        /// <param name="midtones">The adjustment level for midtones. -100 to 100. 0 is no change.</param>
        /// <param name="shadows">The darken level for shadows. 0 to 100. 0 is no change.</param>
        /// <param name="lut">The lookup table data.</param>
        public static void GetLevelsLUT(
            int highlights, int midtones, int shadows,
            ref byte[] lut)
        {
            Proc.AllocLUT(ref lut);

            QuadraticBezierCurve.Point[] ldOutValues = null;
            float lfLen;

            lfLen = (float)(midtones / 100f);

            float ldEnd = (float)(highlights / 100.0);
            float ldStart = (float)(shadows / 100.0);
            ldOutValues = new QuadraticBezierCurve.Point[256];
            QuadraticBezierCurve.Point[] ldInValues;

            float ldOne = ldEnd - ldStart;
            if (ldEnd < ldStart) lfLen = -lfLen;

            ldInValues =
                new QuadraticBezierCurve.Point[] {
                    new QuadraticBezierCurve.Point(ldStart, 0),
                    new QuadraticBezierCurve.Point(ldStart + ldOne * 0.05f, 0.05f),
                    new QuadraticBezierCurve.Point(ldStart + ldOne * 0.5f - (lfLen * ldOne), 0.65f),
                    new QuadraticBezierCurve.Point(ldEnd - ldOne * 0.05f, 0.95f),
                    new QuadraticBezierCurve.Point(ldEnd, 1.0f)
                };

            QuadraticBezierCurve.Bezier(ldInValues, 256, ldOutValues);

            for (int liCounter = 0; liCounter < 256; liCounter++)
            {
                lut[liCounter] = Proc.TruncateChannel(
                    (float)(QuadraticBezierCurve.EstimatedYValue(ldOutValues, liCounter / 255.0, ldEnd > ldStart) * 255.0)
                );
            }
        }

        /// <summary>
        /// Applys AHSL lookup tables to an area of a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to alter.</param>
        /// <param name="a">The alpha lookup table.</param>
        /// <param name="h">The Hue lookup table (value range 0 to 255)</param>
        /// <param name="s">The Saturation lookup table (value range 0 to 255)</param>
        /// <param name="l">The Luminance lookup table (value range 0 to 255)</param>
        /// <param name="rect">The rectangle to process.</param>
        /// <param name="threads">The number of threads to use (in addition to the current thread).</param>
        /// <param name="pixelFormat">The pixel format to use (must be 32-bit ARGB pixel format).</param>
        public static void ApplyHSLLookupTables(
            Bitmap bitmap,
            byte[] a, byte[] h, byte[] s, byte[] l,
            Rectangle rect, int threads, PixelFormat pixelFormat)
        {
            if (rect.IsEmpty) rect = new Rectangle(new Point(), bitmap.Size);

            // Pre normalise the lookup table values to 0 - 1.0
            double[] ldaH = new double[256];
            double[] ldaS = new double[256];
            double[] ldaL = new double[256];

            for (int liCounter = 0; liCounter < 256; liCounter++)
            {
                ldaH[liCounter] = h[liCounter] / 255.0;
                ldaS[liCounter] = s[liCounter] / 255.0;
                ldaL[liCounter] = l[liCounter] / 255.0;
            }

#if Unsafe
            unsafe
            {
                Proc.ApplyPerPixel(
                    new Bitmap[] { bitmap },
                    new ImageLockMode[] { ImageLockMode.ReadWrite },
                    new PixelFormat[] { pixelFormat },
                    new Point[] { rect.Location },
                    rect.Size,
                    threads,
                    (d, t) =>
                    {
                        double ldH, ldS, ldL;

                        // Convert to HSL
                        HSLColorConverter.RGB2HSL(
                            (*(C32*)d).R, (*(C32*)d).G, (*(C32*)d).B,
                            out ldH, out ldS, out ldL);

                        // Apply lookup table
                        ldH = ldaH[(int)(255 * ldH)];
                        ldS = ldaS[(int)(255 * ldS)];
                        ldL = ldaL[(int)(255 * ldL)];

                        // Convert back to RGB
                        HSLColorConverter.HSL2RGB(
                            out (*(C32*)d).R, out (*(C32*)d).G, out (*(C32*)d).B,
                            ldH, ldS, ldL);

                        // Apply alpha lookup table
                        (*(C32*)d).A = a[(*(C32*)d).A];
                    }
                );
            }
#else
            Proc.ApplyPerPixel(
                new Bitmap[] { bitmap },
                new ImageLockMode[] { ImageLockMode.ReadWrite },
                new PixelFormat[] { pixelFormat },
                new Point[] { rect.Location },
                rect.Size,
                threads,
                (d, t) =>
                {
                    double ldH, ldS, ldL;

                    // Get the color channels
                    byte lbR = (byte)((d[0] >> 16) & 0xFF);
                    byte lbG = (byte)((d[0] >> 8) & 0xFF);
                    byte lbB = (byte)(d[0] & 0xFF);

                    // Convert to HSL
                    HSLColorConverter.RGB2HSL(
                        lbR, lbG, lbB,
                        out ldH, out ldS, out ldL);

                    // Apply lookup table
                    ldH = ldaH[(int)(255 * ldH)];
                    ldS = ldaS[(int)(255 * ldS)];
                    ldL = ldaL[(int)(255 * ldL)];

                    // Convert back to RGB
                    HSLColorConverter.HSL2RGB(
                        out lbR, out lbG, out lbB,
                        ldH, ldS, ldL);

                    // Set the color and apply alpha lookup table
                    d[0] =
                        (uint)
                        (
                            (uint)lbB |
                            (uint)(lbG << 8) |
                            (uint)(lbR << 16) |
                            (uint)(a[(byte)(d[0] >> 24)] << 24)
                        );
                }
            );
#endif
        }

        /// <summary>
        /// Applies a color matrix.
        /// </summary>
        /// <param name="bitmap">The bitmap to apply the color matrix to.</param>
        /// <param name="rIn">The red in value containing out values.</param>
        /// <param name="gIn">The green in value containing out values.</param>
        /// <param name="bIn">The blue in value containing out values.</param>
        /// <param name="aIn">The alpha in value containing out values.</param>
        /// <param name="rect">The rectangle to apply the matrix to.</param>
        /// <param name="threads">The number of threads to use.</param>
        /// <param name="pixelFormat">The pixel format to use (must be 32-bit ARGB pixel format).</param>
        public static void ApplyColorMatrix(
            Bitmap bitmap,
            float[] rIn, float[] gIn, float[] bIn, float[] aIn,
            Rectangle rect, int threads, PixelFormat pixelFormat)
        {
            if (rect.IsEmpty) rect = new Rectangle(new Point(), bitmap.Size);

#if Unsafe
            unsafe
            {
                Proc.ApplyPerPixel(
                    new Bitmap[] { bitmap },
                    new ImageLockMode[] { ImageLockMode.ReadWrite },
                    new PixelFormat[] { pixelFormat },
                    new Point[] { rect.Location },
                    rect.Size,
                    threads,
                    (d, t) =>
                    {
                        UInt64 luiTemp = d[0];
                        byte* lbpTemp = (byte*)&luiTemp;

                        // First pixel
                        ((byte*)d)[0] = Proc.TruncateChannel((bIn[2] * lbpTemp[0]) + (gIn[2] * lbpTemp[1]) + (rIn[2] * lbpTemp[2]) + (aIn[2] * lbpTemp[3]));
                        ((byte*)d)[1] = Proc.TruncateChannel((bIn[1] * lbpTemp[0]) + (gIn[1] * lbpTemp[1]) + (rIn[1] * lbpTemp[2]) + (aIn[1] * lbpTemp[3]));
                        ((byte*)d)[2] = Proc.TruncateChannel((bIn[0] * lbpTemp[0]) + (gIn[0] * lbpTemp[1]) + (rIn[0] * lbpTemp[2]) + (aIn[0] * lbpTemp[3]));
                        ((byte*)d)[3] = Proc.TruncateChannel((bIn[3] * lbpTemp[0]) + (gIn[3] * lbpTemp[1]) + (rIn[3] * lbpTemp[2]) + (aIn[3] * lbpTemp[3]));

                        // Second pixel
                        ((byte*)d)[4] = Proc.TruncateChannel((bIn[2] * lbpTemp[4]) + (gIn[2] * lbpTemp[5]) + (rIn[2] * lbpTemp[6]) + (aIn[2] * lbpTemp[7]));
                        ((byte*)d)[5] = Proc.TruncateChannel((bIn[1] * lbpTemp[4]) + (gIn[1] * lbpTemp[5]) + (rIn[1] * lbpTemp[6]) + (aIn[1] * lbpTemp[7]));
                        ((byte*)d)[6] = Proc.TruncateChannel((bIn[0] * lbpTemp[4]) + (gIn[0] * lbpTemp[5]) + (rIn[0] * lbpTemp[6]) + (aIn[0] * lbpTemp[7]));
                        ((byte*)d)[7] = Proc.TruncateChannel((bIn[3] * lbpTemp[4]) + (gIn[3] * lbpTemp[5]) + (rIn[3] * lbpTemp[6]) + (aIn[3] * lbpTemp[7]));
                    },
                    (d, t) =>
                    {
                        UInt32 luiTemp = d[0];
                        byte* lbpTemp = (byte*)&luiTemp;

                        // Apply the pixel
                        ((byte*)d)[0] = Proc.TruncateChannel((bIn[2] * lbpTemp[0]) + (gIn[2] * lbpTemp[1]) + (rIn[2] * lbpTemp[2]) + (aIn[2] * lbpTemp[3]));
                        ((byte*)d)[1] = Proc.TruncateChannel((bIn[1] * lbpTemp[0]) + (gIn[1] * lbpTemp[1]) + (rIn[1] * lbpTemp[2]) + (aIn[1] * lbpTemp[3]));
                        ((byte*)d)[2] = Proc.TruncateChannel((bIn[0] * lbpTemp[0]) + (gIn[0] * lbpTemp[1]) + (rIn[0] * lbpTemp[2]) + (aIn[0] * lbpTemp[3]));
                        ((byte*)d)[3] = Proc.TruncateChannel((bIn[3] * lbpTemp[0]) + (gIn[3] * lbpTemp[1]) + (rIn[3] * lbpTemp[2]) + (aIn[3] * lbpTemp[3]));
                    }
                );
            }
#else
            Proc.ApplyPerPixel(
                new Bitmap[] { bitmap },
                new ImageLockMode[] { ImageLockMode.ReadWrite },
                new PixelFormat[] { PixelFormat.Format32bppPArgb },
                new Point[] { rect.Location },
                rect.Size,
                threads,
                (d, t) =>
                {
                    byte B1 = (byte)(d[0] & 0xFF);
                    byte G1 = (byte)((d[0] >> 8) & 0xFF);
                    byte R1 = (byte)((d[0] >> 16) & 0xFF);
                    byte A1 = (byte)((d[0] >> 24) & 0xFF);
                    byte B2 = (byte)((d[0] >> 32) & 0xFF);
                    byte G2 = (byte)((d[0] >> 40) & 0xFF);
                    byte R2 = (byte)((d[0] >> 48) & 0xFF);
                    byte A2 = (byte)((d[0] >> 56) & 0xFF);

                    d[0] =
                        ((ulong)Proc.TruncateChannel((bIn[2] * B1) + (gIn[2] * G1) + (rIn[2] * R1) + (aIn[2] * A1))) |
                        ((ulong)Proc.TruncateChannel((bIn[1] * B1) + (gIn[1] * G1) + (rIn[1] * R1) + (aIn[1] * A1)) << 8) |
                        ((ulong)Proc.TruncateChannel((bIn[0] * B1) + (gIn[0] * G1) + (rIn[0] * R1) + (aIn[0] * A1)) << 16) |
                        ((ulong)Proc.TruncateChannel((bIn[3] * B1) + (gIn[3] * G1) + (rIn[3] * R1) + (aIn[3] * A1)) << 24) |
                        ((ulong)Proc.TruncateChannel((bIn[2] * B2) + (gIn[2] * G2) + (rIn[2] * R2) + (aIn[2] * A2)) << 32) |
                        ((ulong)Proc.TruncateChannel((bIn[1] * B2) + (gIn[1] * G2) + (rIn[1] * R2) + (aIn[1] * A2)) << 40) |
                        ((ulong)Proc.TruncateChannel((bIn[0] * B2) + (gIn[0] * G2) + (rIn[0] * R2) + (aIn[0] * A2)) << 48) |
                        ((ulong)Proc.TruncateChannel((bIn[3] * B2) + (gIn[3] * G2) + (rIn[3] * R2) + (aIn[3] * A2)) << 56);
                },
                (d, t) =>
                {
                    byte B = (byte)(d[0] & 0xFF);
                    byte G = (byte)((d[0] >> 8) & 0xFF);
                    byte R = (byte)((d[0] >> 16) & 0xFF);
                    byte A = (byte)((d[0] >> 24) & 0xFF);

                    d[0] = 
                        ((uint)Proc.TruncateChannel((bIn[2] * B) + (gIn[2] * G) + (rIn[2] * R) + (aIn[2] * A))) |
                        ((uint)Proc.TruncateChannel((bIn[1] * B) + (gIn[1] * G) + (rIn[1] * R) + (aIn[1] * A)) << 8) |
                        ((uint)Proc.TruncateChannel((bIn[0] * B) + (gIn[0] * G) + (rIn[0] * R) + (aIn[0] * A)) << 16) |
                        ((uint)Proc.TruncateChannel((bIn[3] * B) + (gIn[3] * G) + (rIn[3] * R) + (aIn[3] * A)) << 24);
                }
            );
#endif
        }

        /// <summary>
        /// Applies red eye reduction to an area of a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to apply the red eye reduction to.</param>
        /// <param name="rectOfInterest">The rectangle to apply at (intersected with each rect parameter).</param>
        /// <param name="rects">The rects to apply to red eye reduction to (should be limited to entire eye area).</param>
        /// <param name="threads">The number of threads to use for the effect.</param>
        /// <param name="pixelFormat">The pixel format to use (must be 32-bit ARGB pixel format).</param>
        public static void ApplyRedEyeReduction(
            Bitmap bitmap,
            Rectangle rectOfInterest, Rectangle[] rects,
            int threads, PixelFormat pixelFormat)
        {
            if (rectOfInterest.IsEmpty)
                rectOfInterest = new Rectangle(new Point(), bitmap.Size);

            foreach (Rectangle lrRect in rects)
            {
                // Intersect the rectangle
                Rectangle lrBlockRect = lrRect;
                lrBlockRect.Intersect(rectOfInterest);

                if (!lrBlockRect.IsEmpty)
                {
#if Unsafe
                    unsafe
                    {
#endif
                        Proc.ApplyPerPixel(
                            new Bitmap[] { bitmap },
                            new ImageLockMode[] { ImageLockMode.ReadWrite },
                            new PixelFormat[] { pixelFormat },
                            new Point[] { lrBlockRect.Location },
                            lrBlockRect.Size,
                            threads,
                            (d, t) =>
                            {
#if Unsafe
                                int liGBInt = (*(C32*)d).G + (*(C32*)d).B;

                                // If there is no green or blue, or there is more red then combined green and blue
                                if (liGBInt == 0 || (((int)(*(C32*)d).R << 8) / liGBInt) > 256)
                                    // Reduce saturation and luminosity
                                    HSLColorConverter.ApplyHSLToRGB(
                                        ref (*(C32*)d).R, ref (*(C32*)d).G, ref (*(C32*)d).B,
                                        0, 0.15, -0.1);
#else
                                byte lbR = (byte)((d[0] >> 16) & 0xFF);
                                byte lbG = (byte)((d[0] >> 8) & 0xFF);
                                byte lbB = (byte)(d[0] & 0xFF);
                                int liGBInt = lbB + lbG;

                                // If there is no green or blue, or there is more red then combined green and blue
                                if (liGBInt == 0 || (((int)lbR << 8) / liGBInt) > 256)
                                {
                                    // Reduce saturation and luminosity
                                    HSLColorConverter.ApplyHSLToRGB(ref lbR, ref lbG, ref lbB, 0, 0.15, -0.1);

                                    // Set the pixel
                                    d[0] =
                                         (uint)
                                         (
                                             (uint)lbB |
                                             (uint)(lbG << 8) |
                                             (uint)(lbR << 16) |
                                             (d[0] & 0xFF000000)
                                         );
                                }
#endif
                            }
                        );
#if Unsafe
                    }
#endif
                }
            }
        }

        /// <summary>
        /// Applies Hue, Saturation, Luminence alterations to an area of a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to apply the HSL to.</param>
        /// <param name="hue">The hue offset value in degrees.</param>
        /// <param name="saturation">The saturation multiplier times 100.</param>
        /// <param name="luminosity">The luminosity offset value, times 100.</param>
        /// <param name="rect">The rectangle to apply the effect.</param>
        /// <param name="threads">The number of threads to use for the effect.</param>
        /// <param name="pixelFormat">The pixel format to use (must be 32-bit ARGB pixel format).</param>
        public static void ApplyHSL(
            Bitmap bitmap,
            int hue, int saturation, int luminosity,
            Rectangle rect, int threads, PixelFormat pixelFormat)
        {
            if (rect.IsEmpty) rect = new Rectangle(new Point(), bitmap.Size);

            double ldH = HSLColorConverter.Roll(hue / 360.0);
            double ldS = 1.0 + (saturation / 100.0);
            double ldL = luminosity / 100.0;

#if Unsafe
            unsafe
            {
                Proc.ApplyPerPixel(
                    new Bitmap[] { bitmap },
                    new ImageLockMode[] { ImageLockMode.ReadWrite },
                    new PixelFormat[] { pixelFormat },
                    new Point[] { rect.Location },
                    rect.Size,
                    threads,
                    (d, t) =>
                    {
                        HSLColorConverter.ApplyHSLToRGB(
                            ref (*(C32*)d).R, ref (*(C32*)d).G, ref (*(C32*)d).B,
                            ldH, ldS, ldL);
                    }
                );
            }
#else
            Proc.ApplyPerPixel(
                new Bitmap[] { bitmap },
                new ImageLockMode[] { ImageLockMode.ReadWrite },
                new PixelFormat[] { pixelFormat },
                new Point[] { rect.Location },
                rect.Size,
                threads,
                (d, t) =>
                {
                    byte lbR = (byte)((d[0] >> 16) & 0xFF);
                    byte lbG = (byte)((d[0] >> 8) & 0xFF);
                    byte lbB = (byte)(d[0] & 0xFF);

                    HSLColorConverter.ApplyHSLToRGB(
                        ref lbR, ref lbG, ref lbB,
                        ldH, ldS, ldL);

                    d[0] =
                        (uint)
                        (
                            (uint)lbB |
                            (uint)(lbG << 8) |
                            (uint)(lbR << 16) |
                            (d[0] & 0xFF000000)
                        );
                }
            );
#endif
        }

        /// <summary>
        /// Applies RGBA lookup tables to an area of a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to apply the HSL to.</param>
        /// <param name="a">The alpha lookup table.</param>
        /// <param name="r">The red channel lookup table.</param>
        /// <param name="g">The green channel lookup table.</param>
        /// <param name="b">The blue channel lookup table.</param>
        /// <param name="rect">The rectangle to apply the effect.</param>
        /// <param name="threads">The number of threads to use for the effect.</param>
        /// <param name="pixelFormat">The pixel format to use (must be 32-bit ARGB pixel format).</param>
        public static void ApplyLookupTables(
            Bitmap bitmap,
            byte[] a, byte[] r, byte[] g, byte[] b,
            Rectangle rect, int threads, PixelFormat pixelFormat)
        {
            if (rect.IsEmpty) rect = new Rectangle(new Point(), bitmap.Size);

#if Unsafe
            unsafe
            {
                Proc.ApplyPerPixel(
                    new Bitmap[] { bitmap },
                    new ImageLockMode[] { ImageLockMode.ReadWrite },
                    new PixelFormat[] { pixelFormat },
                    new Point[] { rect.Location },
                    rect.Size,
                    threads,
                    (d, t) =>
                    {
                        // First pixel
                        (*(C64*)d).B0 = b[(*(C64*)d).B0];
                        (*(C64*)d).G0 = g[(*(C64*)d).G0];
                        (*(C64*)d).R0 = r[(*(C64*)d).R0];
                        (*(C64*)d).A0 = a[(*(C64*)d).A0];

                        // Second pixel
                        (*(C64*)d).B1 = b[(*(C64*)d).B1];
                        (*(C64*)d).G1 = g[(*(C64*)d).G1];
                        (*(C64*)d).R1 = r[(*(C64*)d).R1];
                        (*(C64*)d).A1 = a[(*(C64*)d).A1];
                    },
                    (d, t) =>
                    {
                        // Apply pixel
                        (*(C32*)d).B = b[(*(C32*)d).B];
                        (*(C32*)d).G = g[(*(C32*)d).G];
                        (*(C32*)d).R = r[(*(C32*)d).R];
                        (*(C32*)d).A = a[(*(C32*)d).A];
                    }
                );
            }
#else
            Proc.ApplyPerPixel(
                new Bitmap[] { bitmap },
                new ImageLockMode[] { ImageLockMode.ReadWrite },
                new PixelFormat[] { pixelFormat },
                new Point[] { rect.Location },
                rect.Size,
                threads,
                (d, t) =>
                {
                    // Apply first and second pixel
                    d[0] =
                        (ulong)(b[d[0] & 0xFF]) |
                        (((ulong)g[(d[0] >> 8) & 0xFF]) << 8) |
                        (((ulong)r[(d[0] >> 16) & 0xFF]) << 16) |
                        (((ulong)a[(d[0] >> 24) & 0xFF]) << 24) |
                        (((ulong)b[(d[0] >> 32) & 0xFF]) << 32) |
                        (((ulong)g[(d[0] >> 40) & 0xFF]) << 40) |
                        (((ulong)r[(d[0] >> 48) & 0xFF]) << 48) |
                        (((ulong)a[(d[0] >> 56) & 0xFF]) << 56);
                },
                (d, t) =>
                {
                    // Apply pixel
                    d[0] =
                        ((uint)b[d[0] & 0xFF]) |
                        ((uint)(g[(d[0] >> 8) & 0xFF]) << 8) |
                        ((uint)(r[(d[0] >> 16) & 0xFF]) << 16) |
                        ((uint)(a[(d[0] >> 24) & 0xFF]) << 24);
                }
            );
#endif
        }

        /// <summary>
        /// Applys a tinting effect to an area of a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to apply the effect to.</param>
        /// <param name="hue">The hue in degrees for the tint color.</param>
        /// <param name="amount">The amount of tinting to apply (0 - 100).</param>
        /// <param name="rect">The rectangular area to apply the tinting to.</param>
        /// <param name="threads">The number of threads to use for the effect.</param>
        /// <param name="pixelFormat">The pixel format to use (must be 32-bit ARGB pixel format).</param>
        public static void ApplyTint(
            Bitmap bitmap, int hue, int amount,
            Rectangle rect, int threads, PixelFormat pixelFormat)
        {
            if (rect.IsEmpty) rect = new Rectangle(new Point(), bitmap.Size);

            HSLColor lcHSLColor = new HSLColor(hue, 100.0f, 50.0f);
            Color lcColor = (Color)lcHSLColor;

            // Convert amount for faster divide
            amount = amount * 256 / 100;

#if Unsafe
            unsafe
            {
                Proc.ApplyPerPixel(
                    new Bitmap[] { bitmap },
                    new ImageLockMode[] { ImageLockMode.ReadWrite },
                    new PixelFormat[] { pixelFormat },
                    new Point[] { rect.Location },
                    rect.Size,
                    threads,
                    (d, t) =>
                    {
                        // Calculate original luminosity
                        int liLum = Interop11.Luminosity((*(C32*)d).R, (*(C32*)d).G, (*(C32*)d).B);

                        // Calculate new color by lerping from grayscale to tint color
                        int liB = ((*(C32*)d).B + (((Proc.FastDivBy255(liLum * lcColor.B) - (*(C32*)d).B) * amount) >> 8));
                        int liG = ((*(C32*)d).G + (((Proc.FastDivBy255(liLum * lcColor.G) - (*(C32*)d).G) * amount) >> 8));
                        int liR = ((*(C32*)d).R + (((Proc.FastDivBy255(liLum * lcColor.R) - (*(C32*)d).R) * amount) >> 8));

                        // Get luminosity difference
                        liLum -= Interop11.Luminosity(liR, liG, liB);

                        // Offset pixel for extra luminosity
                        (*(C32*)d).B = Proc.TruncateChannel(liB + liLum);
                        (*(C32*)d).G = Proc.TruncateChannel(liG + liLum);
                        (*(C32*)d).R = Proc.TruncateChannel(liR + liLum);
                    }
                );
            }
#else
            Proc.ApplyPerPixel(
                new Bitmap[] { bitmap },
                new ImageLockMode[] { ImageLockMode.ReadWrite },
                new PixelFormat[] { pixelFormat },
                new Point[] { rect.Location },
                rect.Size,
                threads,
                (d, t) =>
                {
                    int liR = (byte)((d[0] >> 16) & 0xFF);
                    int liG = (byte)((d[0] >> 8) & 0xFF);
                    int liB = (byte)(d[0] & 0xFF);

                    // Calculate original luminosity
                    int liLum = Interop11.Luminosity(liR, liG, liB);

                        // Calculate new color by lerping from grayscale to tint color
                    liB = (liB + (((Proc.FastDivBy255(liLum * lcColor.B) - liB) * amount) >> 8));
                    liG = (liG + (((Proc.FastDivBy255(liLum * lcColor.G) - liG) * amount) >> 8));
                    liR = (liR + (((Proc.FastDivBy255(liLum * lcColor.R) - liR) * amount) >> 8));

                    // Get luminosity difference
                    liLum -= Interop11.Luminosity(liR, liG, liB);

                    // Offset pixel for extra luminosity
                    d[0] =
                        (uint)
                        (
                            (uint)Proc.TruncateChannel(liB + liLum) |
                            (uint)(Proc.TruncateChannel(liG + liLum) << 8) |
                            (uint)(Proc.TruncateChannel(liR + liLum) << 16) |
                            (d[0] & 0xFF000000)
                        );
                }
            );
#endif
        }

        #endregion
    }
}
