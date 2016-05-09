//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using GDIPlusX.GDIPlus10.Internal;

namespace GDIPlusX.GDIPlus11.Internal
{
    /// <summary>
    /// Encapsulates all API and utility functions for GDI 1.1 interop.
    /// </summary>
    internal static class Interop11
    {
        #region Constants

        /// <summary>
        /// The balance of Red that GDI+ uses for luminosity.
        /// </summary>
        /// <remarks>
        /// Do these change with the color profile?
        /// </remarks>
        const float cfGrayscaleRBalance = 0.2126f;

        /// <summary>
        /// The balance of Green that GDI+ uses for luminosity.
        /// </summary>
        /// <remarks>
        /// Do these change with the color profile?
        /// </remarks>
        const float cfGrayscaleGBalance = 0.71345f;

        /// <summary>
        /// The balance of Blue that GDI+ uses for luminosity.
        /// </summary>
        /// <remarks>
        /// Do these change with the color profile?
        /// </remarks>
        const float cfGrayscaleBBalance = 0.07085f;

        /// <summary>
        /// The balance of Red that GDI+ uses for luminosity.
        /// </summary>
        const int ciGrayscaleRBalance = (int)(256.0f * cfGrayscaleRBalance);

        /// <summary>
        /// The balance of Green that GDI+ uses for luminosity.
        /// </summary>
        const int ciGrayscaleGBalance = (int)(256.0f * cfGrayscaleGBalance);

        /// <summary>
        /// The balance of Blue that GDI+ uses for luminosity.
        /// </summary>
        const int ciGrayscaleBBalance = (int)(256.0f * cfGrayscaleBBalance);

        #endregion

        #region Enumerations

        /// <summary>
        /// Specifies the curve adjustment to make for the ColorCurve effect.
        /// </summary>
        public enum GpCurveAdjustments
        {
            /// <summary>
            /// Simulates increasing or decreasing the exposure of a photograph. 
            /// Adjust value range: -255 to 255.
            /// </summary>
            AdjustExposure = 0,

            /// <summary>
            /// Simulates increasing or decreasing the film density of a photograph. 
            /// Adjust value range: -255 to 255.
            /// </summary>
            AdjustDensity = 1,

            /// <summary>
            /// Increases or decreases the contrast of a bitmap. 
            /// Adjust value range: -100 to 100.
            /// </summary>
            AdjustContrast = 2,

            /// <summary>
            /// Increases or decreases the value of a color channel if that channel already has a 
            /// value that is above half intensity. 
            /// Adjust value range: -100 to 100.
            /// </summary>
            AdjustHighlight = 3,

            /// <summary>
            /// Increases or decreases the value of a color channel if that channel already has a 
            /// value that is below half intensity. 
            /// Adjust value range: -100 to 100.
            /// </summary>
            AdjustShadow = 4,

            /// <summary>
            /// Lightens or darkens an images midtones. 
            /// Adjust value range: -100 to 100.
            /// </summary>
            AdjustMidtone = 5,

            /// <summary>
            /// Adjusts white saturation levels of a bitmap. 
            /// Adjust value range: 0 to 255.
            /// </summary>
            AdjustWhiteSaturation = 6,

            /// <summary>
            /// Adjusts black saturation levels of a bitmap. 
            /// Adjust value range: 0 to 255.
            /// </summary>
            AdjustBlackSaturation = 7
        }

        /// <summary>
        /// Specifies which color channels are affected by the ColorCurve effect.
        /// </summary>
        public enum GpCurveChannel
        {
            /// <summary>
            /// Specifies that the color adjustment applies to all channels.
            /// </summary>
            CurveChannelAll = 0,

            /// <summary>
            /// Specifies that the color adjustment applies only to the red channel.
            /// </summary>
            CurveChannelRed = 1,

            /// <summary>
            /// Specifies that the color adjustment applies only to the green channel.
            /// </summary>
            CurveChannelGreen = 2,

            /// <summary>
            /// Specifies that the color adjustment applies only to the blue channel.
            /// </summary>
            CurveChannelBlue = 3
        }

        /// <summary>
        /// Specifies the number and type of histograms that represent the color channels of a bitmap.
        /// </summary>
        public enum GpHistogramFormat
        {
            /// <summary>
            /// Four histograms, Alpha, Red, Green and Blue.
            /// </summary>
            HistogramFormatARGB,

            /// <summary>
            /// Four histograms, Alpha, Red, Green, Blue, but the Red, Green and Blue channels 
            /// are premultiplied by the Alpha.
            /// </summary>
            HistogramFormatPARGB,

            /// <summary>
            /// Three histograms, Red, Green, Blue.
            /// </summary>
            HistogramFormatRGB,

            /// <summary>
            /// One histogram, Grayscale.
            /// </summary>
            HistogramFormatGray,

            /// <summary>
            /// One histogram, Blue.
            /// </summary>
            HistogramFormatB,

            /// <summary>
            /// One histogram, Green.
            /// </summary>
            HistogramFormatG,

            /// <summary>
            /// One histogram, Red.
            /// </summary>
            HistogramFormatR,

            /// <summary>
            /// One histogram, Alpha.
            /// </summary>
            HistogramFormatA 
        }

        #endregion

        #region Structures

        /// <summary>
        /// Contains members that specify the nature of a Gaussian blur.
        /// </summary>
        /// <remarks>Cannot be pinned with GCHandle due to bool value.</remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BlurParams
        {
            /// <summary>
            /// Real number that specifies the blur radius (the radius of the Gaussian convolution kernel) in 
            /// pixels. The radius must be in the range 0 through 255. As the radius increases, the resulting 
            /// bitmap becomes more blurry.
            /// </summary>
            public float Radius;

            /// <summary>
            /// Boolean value that specifies whether the bitmap expands by an amount equal to the blur radius. 
            /// If TRUE, the bitmap expands by an amount equal to the radius so that it can have soft edges. 
            /// If FALSE, the bitmap remains the same size and the soft edges are clipped.
            /// </summary>
            public bool ExpandEdges;
        };

        /// <summary>
        /// Contains members that specify the nature of a brightness or contrast adjustment.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BrightnessContrastParams
        {
            /// <summary>
            /// Integer in the range -255 through 255 that specifies the brightness level. If the value is 0, 
            /// the brightness remains the same. As the value moves from 0 to 255, the brightness of the image 
            /// increases. As the value moves from 0 to -255, the brightness of the image decreases.
            /// </summary>
            public int BrightnessLevel;

            /// <summary>
            /// Integer in the range -100 through 100 that specifies the contrast level. If the value is 0, 
            /// the contrast remains the same. As the value moves from 0 to 100, the contrast of the image 
            /// increases. As the value moves from 0 to -100, the contrast of the image decreases.
            /// </summary>
            public int ContrastLevel;
        }

        /// <summary>
        /// Contains members that specify the nature of a color balance adjustment.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ColorBalanceParams
        {
            /// <summary>
            /// Integer in the range -100 through 100 that specifies a change in the amount of red in the 
            /// image. If the value is 0, there is no change. As the value moves from 0 to 100, the amount 
            /// of red in the image increases and the amount of cyan decreases. As the value moves from 0 to 
            /// -100, the amount of red in the image decreases and the amount of cyan increases.
            /// </summary>
            public int CyanRed;

            /// <summary>
            /// Integer in the range -100 through 100 that specifies a change in the amount of green in the 
            /// image. If the value is 0, there is no change. As the value moves from 0 to 100, the amount 
            /// of green in the image increases and the amount of magenta decreases. As the value moves from 
            /// 0 to -100, the amount of green in the image decreases and the amount of magenta increases.
            /// </summary>
            public int MagentaGreen;

            /// <summary>
            /// Integer in the range -100 through 100 that specifies a change in the amount of blue in the 
            /// image. If the value is 0, there is no change. As the value moves from 0 to 100, the amount 
            /// of blue in the image increases and the amount of yellow decreases. As the value moves from 
            /// 0 to -100, the amount of blue in the image decreases and the amount of yellow increases.
            /// </summary>
            public int YellowBlue;
        }

        /// <summary>
        /// Contains members that specify an adjustment to the colors of a bitmap.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ColorCurveParams
        {
            /// <summary>
            /// Element of the GpCurveAdjustments enumeration that specifies the adjustment to be applied.
            /// </summary>
            public GpCurveAdjustments Adjustment;

            /// <summary>
            /// Element of the GpCurveChannel enumeration that specifies the color channel to which the 
            /// adjustment applies.
            /// </summary>
            public GpCurveChannel Channel;

            /// <summary>
            /// Integer that specifies the intensity of the adjustment. The range of acceptable values 
            /// depends on which adjustment is being applied. To see the range of acceptable values for a 
            /// particular adjustment, see the GpCurveAdjustments enumeration.
            /// </summary>
            public int AdjustValue;
        }

        /// <summary>
        /// Contains members (color lookup tables) that specify color adjustments to a bitmap.
        /// </summary>
        /// <remarks>Cannot be pinned with GCHandle due to arrays.</remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ColorLUTParams
        {
            /// <summary>
            /// Array of 256 bytes that specifies the adjustment for the blue channel.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] B;

            /// <summary>
            /// Array of 256 bytes that specifies the adjustment for the green channel.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] G;

            /// <summary>
            /// Array of 256 bytes that specifies the adjustment for the red channel.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] R;

            /// <summary>
            /// Array of 256 bytes that specifies the adjustment for the alpha channel.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] A;
        }

        /// <summary>
        /// Cntains 5 matrix rows to make up a 5×5 matrix of real numbers.
        /// </summary>
        /// <remarks>
        /// A 5×5 color matrix is a homogeneous matrix for a 4-space transformation. The element 
        /// in the fifth row and fifth column of a 5×5 homogeneous matrix must be 1, and all of 
        /// the other elements in the fifth column must be 0. Color matrices are used to transform 
        /// color vectors. The first four components of a color vector hold the red, green, blue, 
        /// and alpha components (in that order) of a color. The fifth component of a color vector 
        /// is always 1.
        /// </remarks>
        /// <remarks>Cannot be pinned with GCHandle due to arrays.</remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ColorMatrixParams
        {
            /// <summary>
            /// Row for Red input of matrix in RGBAw order.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public float[] Mr;

            /// <summary>
            /// Row for Green input of matrix in RGBAw order.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public float[] Mg;

            /// <summary>
            /// Row for Blue input of matrix in RGBAw order.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public float[] Mb;

            /// <summary>
            /// Row for Alpha input of matrix in RGBAw order.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public float[] Ma;

            /// <summary>
            /// Row for w input of matrix in RGBAw order.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public float[] Mw;
        }

        /// <summary>
        /// Contains members that specify hue, saturation and lightness adjustments to a bitmap.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HueSaturationLightnessParams
        {
            /// <summary>
            /// Integer in the range -180 through 180 that specifies the change in hue. A value 
            /// of 0 specifies no change. Positive values specify counterclockwise rotation on the 
            /// color wheel. Negative values specify clockwise rotation on the color wheel.
            /// </summary>
            public int HueLevel;

            /// <summary>
            /// Integer in the range -100 through 100 that specifies the change in saturation. A 
            /// value of 0 specifies no change. Positive values specify increased saturation and 
            /// negative values specify decreased saturation.
            /// </summary>
            public int SaturationLevel;

            /// <summary>
            /// Integer in the range -100 through 100 that specifies the change in lightness. A 
            /// value of 0 specifies no change. Positive values specify increased lightness and 
            /// negative values specify decreased lightness.
            /// </summary>
            public int LightnessLevel;
        }

        /// <summary>
        /// Contains members that specify adjustments to the light, midtone, or dark areas of a bitmap.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LevelsParams
        {
            /// <summary>
            /// Integer in the range 0 through 100 that specifies which pixels should be lightened. 
            /// You can use this adjustment to lighten pixels that are already lighter than a certain 
            /// threshold. Setting highlight to 100 specifies no change. Setting highlight to t 
            /// specifies that a color channel value is increased if it is already greater than t 
            /// percent of full intensity. For example, setting highlight to 90 specifies that all 
            /// color channel values greater than 90 percent of full intensity are increased.
            /// </summary>
            public int Highlight;

            /// <summary>
            /// Integer in the range -100 through 100 that specifies how much to lighten or darken 
            /// an image. Color channel values in the middle of the intensity range are altered more 
            /// than color channel values near the minimum or maximum intensity. You can use this 
            /// adjustment to lighten (or darken) an image without loosing the contrast between the 
            /// darkest and lightest portions of the image. A value of 0 specifies no change. 
            /// Positive values specify that the midtones are made lighter, and negative values 
            /// specify that the midtones are made darker.
            /// </summary>
            public int Midtone;

            /// <summary>
            /// Integer in the range 0 through 100 that specifies which pixels should be darkened. 
            /// You can use this adjustment to darken pixels that are already darker than a certain 
            /// threshold. Setting shadow to 0 specifies no change. Setting shadow to t specifies 
            /// that a color channel value is decreased if it is already less than t percent of 
            /// full intensity. For example, setting shadow to 10 specifies that all color channel 
            /// values less than 10 percent of full intensity are decreased.
            /// </summary>
            public int Shadow;
        }

        /// <summary>
        /// Contains members that specify the areas of a bitmap to which a red-eye correction is applied.
        /// </summary>
        /// <remarks>This is the 64-bit version of the structure.</remarks>
        [StructLayout(LayoutKind.Explicit)]
        public struct RedEyeCorrectionParams64Bit
        {
            /// <summary>
            /// Number of areas to filter
            /// </summary>
            [FieldOffset(0)]
            public uint NumberOfAreas;

            /// <summary>
            /// Memory address of RECT structs
            /// </summary>
            /// <remarks>
            /// Must be aligned to 64-bit boundary due to
            /// 64-bit cpu environment and this being a pointer.
            /// Im assuming C++ does this by default.
            /// </remarks>
            [FieldOffset(8)]
            public IntPtr Areas;
        }

        /// <summary>
        /// Contains members that specify the areas of a bitmap to which a red-eye correction is applied.
        /// </summary>
        /// <remarks>This is the 32-bit version of the structure.</remarks>
        [StructLayout(LayoutKind.Explicit)]
        public struct RedEyeCorrectionParams32Bit
        {
            /// <summary>
            /// Number of areas to filter
            /// </summary>
            [FieldOffset(0)]
            public uint NumberOfAreas;

            /// <summary>
            /// Memory address of RECT structs
            /// </summary>
            [FieldOffset(4)]
            public IntPtr Areas;
        }

        /// <summary>
        /// Contains members that specify the nature of a sharpening adjustment to a bitmap.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SharpenParams
        {
            /// <summary>
            /// Real number that specifies the sharpening radius (the radius of the convolution 
            /// kernel) in pixels. The radius must be in the range 0 through 255. As the radius 
            /// increases, more surrounding pixels are involved in calculating the new value of 
            /// a given pixel.
            /// </summary>
            public float Radius;

            /// <summary>
            /// Real number in the range 0 through 100 that specifies the amount of sharpening 
            /// to be applied. A value of 0 specifies no sharpening. As the value of amount 
            /// increases, the sharpness increases.
            /// </summary>
            public float Amount;
        }

        /// <summary>
        /// Contains members that specify the nature of a tint adjustment to a bitmap.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TintParams
        {
            /// <summary>
            /// From MSDN:
            /// Integer in the range -180 through 180 that specifies the hue to be strengthened 
            /// or weakened. A value of 0 specifies blue. A positive value specifies a clockwise 
            /// angle on the color wheel. For example, positive 60 specifies cyan and positive 
            /// 120 specifies green. A negative value specifies a counter-clockwise angle on the 
            /// color wheel. For example, negative 60 specifies magenta and negative 120 
            /// specifies red.
            /// --- WRONG AGAIN MICROSOFT...
            /// Actual values are:
            /// -180 is cyan. -120 is blue. -60 is magenta. 0 is red. 60 is yellow. 120 is green. 180 is cyan.
            /// One would think they would document this properly.
            /// </summary>
            public int Hue;

            /// <summary>
            /// Integer in the range -100 through 100 that specifies how much the hue (given by 
            /// the hue parameter) is strengthened or weakened. A value of 0 specifies no change. 
            /// Positive values specify that the hue is strengthened and negative values specify 
            /// that the hue is weakened.
            /// </summary>
            public int Amount;
        }

        #endregion

        #region Locals

        /// <summary>
        /// Holds whether GDI Plus 1.1 is available
        /// </summary>
        private static bool mbGDIPlus11Available = IsGDIPlus11Available();

        #endregion

        #region Imports

        /// <summary>
        /// Creates a new GDI Plus Effect.
        /// </summary>
        /// <param name="guid">The Guid for the effect.</param>
        /// <param name="effect">On out the pointer or handle to the effect.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipCreateEffect(Guid guid, out IntPtr effect);

        /// <summary>
        /// Deletes a GDI Plus Effect created with GdipCreateEffect.
        /// </summary>
        /// <param name="effect">The pointer or handle of the effect to delete.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipDeleteEffect(IntPtr effect);

        /// <summary>
        /// Gets the parameter size in bytes for an effect.
        /// </summary>
        /// <param name="effect">A pointer or handle to the effect.</param>
        /// <param name="size">On out the size in bytes of the effects parameters.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipGetEffectParameterSize(IntPtr effect, out uint size);

        /// <summary>
        /// Sets the parameters for an effect.
        /// </summary>
        /// <param name="effect">A pointer or handle to the effect.</param>
        /// <param name="parameters">A pointer to the parameters to set.</param>
        /// <param name="size">The size in bytes of the parameters.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipSetEffectParameters(IntPtr effect, IntPtr parameters, uint size);

        /// <summary>
        /// Gets the parameters for an effect.
        /// </summary>
        /// <param name="effect">A pointer or handle to the effect.</param>
        /// <param name="size">The size in bytes of the parameters.</param>
        /// <param name="parameters">A pointer to the parameters to set.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipGetEffectParameters(IntPtr effect, ref uint size, IntPtr parameters);

        /// <summary>
        /// Applys an effect to a bitmap.
        /// </summary>
        /// <param name="bitmap">A pointer or handle to the bitmap.</param>
        /// <param name="effect">A pointer or handle to the effect.</param>
        /// <param name="rectOfInterest">The rectangle to apply the effect, on out the area applied.</param>
        /// <param name="useAuxData">True to return effect auxillary data.</param>
        /// <param name="auxData">Contains pointer to auxillary data on out.</param>
        /// <param name="auxDataSize">Contains the size in bytes of the auxillary data.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipBitmapApplyEffect(
            HandleRef bitmap, HandleRef effect, 
            ref Utils10.RECT rectOfInterest, 
            bool useAuxData, out IntPtr auxData, out int auxDataSize);

        /// <summary>
        /// Applys an effect to a bitmap.
        /// </summary>
        /// <param name="bitmap">A pointer or handle to the bitmap.</param>
        /// <param name="effect">A pointer or handle to the effect.</param>
        /// <param name="rectOfInterest">IntPtr.Zero to specify entire bitmap.</param>
        /// <param name="useAuxData">True to return effect auxillary data.</param>
        /// <param name="auxData">Contains pointer to auxillary data on out.</param>
        /// <param name="auxDataSize">Contains the size in bytes of the auxillary data.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipBitmapApplyEffect(
            HandleRef bitmap, HandleRef effect, IntPtr rectOfInterest,
            bool useAuxData, out IntPtr auxData, out int auxDataSize);

        /// <summary>
        /// Applys an effect to a bitmap and returns a new bitmap.
        /// </summary>
        /// <param name="inputBitmaps">A pointer to a bitmap pointer or handle that contains the bitmap to apply the effect.</param>
        /// <param name="numInputs">The number of bitmaps to apply the effect to. (Always 1).</param>
        /// <param name="effect">A pointer or handle to the effect.</param>
        /// <param name="rectOfInterest">The rectangle to apply the effect, on out the area applied.</param>
        /// <param name="outputRect">The rectangle to output to.</param>
        /// <param name="outputBitmap">On out the new bitmap that was created.</param>
        /// <param name="useAuxData">True to return effect auxillary data.</param>
        /// <param name="auxData">Contains pointer to auxillary data on out.</param>
        /// <param name="auxDataSize">Contains the size in bytes of the auxillary data.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipBitmapCreateApplyEffect(
            ref IntPtr inputBitmaps, int numInputs, HandleRef effect, 
            ref Utils10.RECT rectOfInterest,
            ref Utils10.RECT outputRect, out IntPtr outputBitmap, 
            bool useAuxData, out IntPtr auxData, out int auxDataSize);

        /// <summary>
        /// Applys an effect to a bitmap and returns a new bitmap.
        /// </summary>
        /// <param name="inputBitmaps">A pointer to a bitmap pointer or handle that contains the bitmap to apply the effect.</param>
        /// <param name="numInputs">The number of bitmaps to apply the effect to. (Always 1).</param>
        /// <param name="effect">A pointer or handle to the effect.</param>
        /// <param name="rectOfInterest">IntPtr.Zero to specify entire bitmap.</param>
        /// <param name="outputRect">The rectangle to output to.</param>
        /// <param name="outputBitmap">On out the new bitmap that was created.</param>
        /// <param name="useAuxData">True to return effect auxillary data.</param>
        /// <param name="auxData">Contains pointer to auxillary data on out.</param>
        /// <param name="auxDataSize">Contains the size in bytes of the auxillary data.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int 
            GdipBitmapCreateApplyEffect(
            ref IntPtr inputBitmaps, int numInputs, HandleRef effect, IntPtr rectOfInterest,
            ref Utils10.RECT outputRect, out IntPtr outputBitmap,
            bool useAuxData, out IntPtr auxData, out int auxDataSize);

        /// <summary>
        /// Draws an image and apply an effect as its drawn.
        /// </summary>
        /// <param name="graphics">A pointer or handle to the graphics object to draw on.</param>
        /// <param name="image">A pointer or handle to the image to draw. (The image is not changed by the effect).</param>
        /// <param name="source">The source rectangle to draw from.</param>
        /// <param name="matrix">The matrix for the output transformation.</param>
        /// <param name="effect">The effect to apply or IntPtr.Zero for none.</param>
        /// <param name="imageAttributes"></param>
        /// <param name="srcUnit">The src units of measurement.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipDrawImageFX(
            HandleRef graphics,
            HandleRef image,
            ref Utils10.GpRectF source,
            HandleRef matrix,
            HandleRef effect,
            HandleRef imageAttributes,
            Utils10.GpUnit srcUnit
        );

        /// <summary>
        /// Gets histogram data for a bitmap.
        /// </summary>
        /// <param name="bitmap">A pointer or handle to the bitmap.</param>
        /// <param name="format">The format of the histogram data. This determines the number of channels of data returned.</param>
        /// <param name="numberOfEntries">The number of entries provided in the channel data.</param>
        /// <param name="uiChannel0">A pointer to the first channel data.</param>
        /// <param name="uiChannel1">A pointer to the second channel data or null if not needed.</param>
        /// <param name="uiChannel2">A pointer to the third channel data or null if not needed.</param>
        /// <param name="uiChannel3">A pointer to the forth channel data or null if not needed.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipBitmapGetHistogram(
            HandleRef bitmap, GpHistogramFormat format, 
            uint numberOfEntries, 
            IntPtr uiChannel0, IntPtr uiChannel1, IntPtr uiChannel2, IntPtr uiChannel3);

        /// <summary>
        /// Gets the histogram channel data size as a number of elements.
        /// </summary>
        /// <param name="format">The format of the histogram data.</param>
        /// <param name="numberOfEntries">On out the number of entires required per channel data.</param>
        /// <returns>A GpStatus value.</returns>
        [DllImport(
            Utils10.GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipBitmapGetHistogramSize(
            GpHistogramFormat format, out uint numberOfEntries);

        #endregion

        #region Private Utility Methods

        /// <summary>
        /// Returns true if GDI Plus 1.1 is available
        /// </summary>
        /// <returns>A bool.</returns>
        private static bool IsGDIPlus11Available()
        {
            // Test to see if the GdipCreateEffect procedure is available in the GDI Plus DLL
            return GDIPlus10.Internal.Utils10.GDIPlusProcAvailable("GdipCreateEffect");
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Checks to see if GDI Plus 1.0 is available
        /// </summary>
        /// <exception cref="GDIPlusX.GDIPlus11.GDIPlus11NotAvailableException">
        /// GDI Plus 1.1 not available.
        /// </exception>
        public static void CheckAvailable()
        {
            if (!Ver11Available)
                throw new GDIPlus11NotAvailableException();
        }

        /// <summary>
        /// Calculate the grayscale luminosity of a pixel value according to GDI+.
        /// </summary>
        /// <param name="red">The amount of red in the color (0 - 255).</param>
        /// <param name="green">The amount of green in the color (0 - 255).</param>
        /// <param name="blue">The amount of blue in the color (0 - 255).</param>
        /// <returns>An integer in the range of 0 to 255 for the grayscale value of the color.</returns>
        public static int Luminosity(int red, int green, int blue)
        {
            return
                (
                    red * ciGrayscaleRBalance +
                    green * ciGrayscaleGBalance +
                    blue * ciGrayscaleBBalance
                ) >> 8;
        }

        /// <summary>
        /// Calculate the grayscale luminosity of a pixel value according to GDI+.
        /// </summary>
        /// <param name="red">The amount of red in the color (0 - 255).</param>
        /// <param name="green">The amount of green in the color (0 - 255).</param>
        /// <param name="blue">The amount of blue in the color (0 - 255).</param>
        /// <returns>An integer in the range of 0 to 255 for the grayscale value of the color.</returns>
        public static int Luminosity(uint red, uint green, uint blue)
        {
            return
                (int)
                (
                    red * ciGrayscaleRBalance +
                    green * ciGrayscaleGBalance +
                    blue * ciGrayscaleBBalance
                ) >> 8;
        }

        /// <summary>
        /// Calculate the grayscale luminosity of a pixel value according to GDI+.
        /// </summary>
        /// <param name="red">The amount of red in the color (0 - 255).</param>
        /// <param name="green">The amount of green in the color (0 - 255).</param>
        /// <param name="blue">The amount of blue in the color (0 - 255).</param>
        /// <returns>An integer in the range of 0 to 255 for the grayscale value of the color.</returns>
        public static byte LuminosityF(byte red, byte green, byte blue)
        {
            return
                (byte)
                Math.Ceiling(
                    (float)red * cfGrayscaleRBalance +
                    (float)green * cfGrayscaleGBalance +
                    (float)blue * cfGrayscaleBBalance
                );
        }

        #endregion

        #region Utility Properties

        /// <summary>
        /// Gets whether GDI Plus 1.1 is available.
        /// </summary>
        public static bool Ver11Available
        {
            get
            {
                return mbGDIPlus11Available;
            }
        }

        #endregion
    }
}
