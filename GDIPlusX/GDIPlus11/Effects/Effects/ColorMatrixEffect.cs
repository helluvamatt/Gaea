//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Imaging;
using GDIPlusX.GDIPlus11.EffectsInternal;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates an effect which allows for a color matrix to be applied to an area.
    /// </summary>
    /// <see>http://msdn.microsoft.com/en-us/library/ms534431(v=vs.85).aspx</see>
    public class ColorMatrixEffect : LegacyEffect
    {
        #region Protected Static Locals

        /// <summary>
        /// GUID for the GDI+ color matrix effect.
        /// </summary>
        protected static Guid mgEffectGuid = new Guid("{718F2615-7933-40E3-A511-5F68FE14DD74}");

        #endregion

        #region Private Locals

        /// <summary>
        /// Holds the parameters for the color matrix effect.
        /// </summary>
        private Internal.Interop11.ColorMatrixParams mcmParams;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new color matrix effect with an identity matrix.
        /// </summary>
        public ColorMatrixEffect()
            : base(mgEffectGuid)
        {
            RIn = GetNewIdentityRow(0);
            GIn = GetNewIdentityRow(1);
            BIn = GetNewIdentityRow(2);
            AIn = GetNewIdentityRow(3);
            WIn = GetNewIdentityRow(4);
        }

        /// <summary>
        /// Creates a new color matrix effect by specifying each row of the matrix.
        /// </summary>
        /// <param name="rIn">The red color matrix row in RGBAw order.</param>
        /// <param name="gIn">The green color matrix row in RGBAw order.</param>
        /// <param name="bIn">The blue color matrix row in RGBAw order.</param>
        /// <param name="aIn">The alpha color matrix row in RGBAw order.</param>
        /// <param name="wIn">The w color matrix row in RGBAw order.</param>
        public ColorMatrixEffect(float[] rIn, float[] gIn, float[] bIn, float[] aIn, float[] wIn)
            : this()
        {
            RIn = rIn;
            GIn = gIn;
            BIn = bIn;
            AIn = aIn;
            WIn = wIn;
        }

        /// <summary>
        /// Creates a new color matrix effect by specifying the matrix as a 5x5 array.
        /// </summary>
        /// <param name="matrix5x5">The [row][column] matrix in RGBAw order.</param>
        public ColorMatrixEffect(float[][] matrix5x5)
            : this()
        {
            Matrix5x5 = matrix5x5;
        }

        /// <summary>
        /// Creates a new color matrix effect from a ColorMatrix object.
        /// </summary>
        /// <param name="colorMatrix">The color matrix structure to base the ColorMatrixEffect on.</param>
        public ColorMatrixEffect(ColorMatrix colorMatrix)
            : this()
        {
            ColorMatrix = colorMatrix;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Gets whether the parameters object is pinnable for this effect
        /// </summary>
        /// <returns>True if its pinnable, false to copy to other memory first</returns>
        protected override bool ParametersPinnable()
        {
            return false;
        }

        /// <summary>
        /// Returns the parameter data for this effect.
        /// </summary>
        /// <returns>An object containing the parameter data.</returns>
        protected override object InitialiseParameterData()
        {
            return mcmParams;
        }

        /// <summary>
        /// Applys an effect to a Bitmap using legacy code.
        /// </summary>
        /// <param name="bitmap">The Bitmap to apply the effect to.</param>
        /// <param name="rectOfInterest">
        /// The rectangle to apply the Effect or Rectangle.Empty 
        /// for entire bitmap.
        /// </param>
        /// <exception cref="GDIPlusX.GDIPlus11.GDIPlus11NotAvailableException">GDI Plus 1.1 functions not available.</exception>
        /// <exception cref="System.ArgumentNullException">bitmap is null or effect is null.</exception>
        /// <exception cref="GDIPlusX.GDIPlus11.Effects.EffectValidateException">Effect validation with bitmap failed.</exception>
        /// <remarks>Auxillary data is calculated if the effect supports it.</remarks>
        protected override void LegacyApplyToBitmap(Bitmap bitmap, Rectangle rectOfInterest)
        {
            LegacyBitmapPerPixelEffect.ApplyColorMatrix(
                bitmap,
                mcmParams.Mr, mcmParams.Mg, mcmParams.Mb, mcmParams.Ma,
                rectOfInterest, Effect.LegacyThreads, LegacyCloneApplyPixelFormat());
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates a 5 element array representing an identity row of the matrix.
        /// </summary>
        /// <param name="row">The index of the identity row to create.</param>
        /// <returns>A float[5] array.</returns>
        protected float[] GetNewIdentityRow(int row)
        {
            float[] lfRow = new float[5];
            lfRow[row] = 1f;
            return lfRow;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the color matrix for this effect.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">ColorMatrix is null.</exception>
        public ColorMatrix ColorMatrix
        {
            get
            {
                return new ColorMatrix(Matrix5x5);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("ColorMatrix");

                float[][] lmMatrix5x5 = new float[5][];
                for (int liRow = 0; liRow < 5; liRow++)
                {
                    lmMatrix5x5[liRow] = new float[5];

                    for (int liCol = 0; liCol < 5; liCol++)
                        lmMatrix5x5[liRow][liCol] = value[liRow, liCol];
                }

                Matrix5x5 = lmMatrix5x5;
            }
        }

        /// <summary>
        /// Gets or sets an array reference the matrix for this effect as a jagged 5x5 [row][column] RGBAw ordered array.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Matrix5x5 is null.</exception>
        /// <exception cref="System.ArgumentException">Matrix5x5 is not a 5x5 jagged array.</exception>
        public float[][] Matrix5x5
        {
            get
            {
                float[][] lmMatrix = new float[5][];

                lmMatrix[0] = RIn;
                lmMatrix[1] = GIn;
                lmMatrix[2] = BIn;
                lmMatrix[3] = AIn;
                lmMatrix[4] = WIn;

                return lmMatrix;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Matrix5x5"); 

                if (value.Length != 5)
                    throw new ArgumentException("Must be a 5 x 5 jagged array", "Matrix5x5");

                for (int liCounter = 0; liCounter < 5; liCounter++)
                    if (value[liCounter].Length != 5)
                        throw new ArgumentException("Must be a 5 x 5 jagged array", "Matrix5x5");

                RIn = value[0];
                GIn = value[1];
                BIn = value[2];
                AIn = value[3];
                WIn = value[4];
            }
        }

        /// <summary>
        /// Gets or sets the red color matrix row in RGBAw order.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">RIn is null.</exception>
        /// <exception cref="System.ArgumentException">ROut must have 5 elements.</exception>
        public float[] RIn
        {
            get
            {
                return mcmParams.Mr;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("ROut");

                if (value.Length != 5)
                    throw new ArgumentException("Must have 5 elements", "ROut");

                mcmParams.Mr = value;
            }
        }

        /// <summary>
        /// Gets or sets the green color matrix row in RGBAw order.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">GIn is null.</exception>
        /// <exception cref="System.ArgumentException">GIn must have 5 elements.</exception>
        public float[] GIn
        {
            get
            {
                return mcmParams.Mg;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("GIn");

                if (value.Length != 5)
                    throw new ArgumentException("Must have 5 elements", "GIn");

                mcmParams.Mg = value;
            }
        }

        /// <summary>
        /// Gets or sets the blue color matrix row in RGBAw order.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">BIn is null.</exception>
        /// <exception cref="System.ArgumentException">BIn must have 5 elements.</exception>
        public float[] BIn
        {
            get
            {
                return mcmParams.Mb;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("BIn");

                if (value.Length != 5)
                    throw new ArgumentException("Must have 5 elements", "BIn");

                mcmParams.Mb = value;
            }
        }

        /// <summary>
        /// Gets or sets the alpha color matrix row in RGBAw order.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">AIn is null.</exception>
        /// <exception cref="System.ArgumentException">AIn must have 5 elements.</exception>
        public float[] AIn
        {
            get
            {
                return mcmParams.Ma;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("AIn");

                if (value.Length != 5)
                    throw new ArgumentException("Must have 5 elements", "AIn");

                mcmParams.Ma = value;
            }
        }

        /// <summary>
        /// Gets or sets the w color matrix row in RGBAw order.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">WIn is null.</exception>
        /// <exception cref="System.ArgumentException">WIn must have 5 elements.</exception>
        public float[] WIn
        {
            get
            {
                return mcmParams.Mw;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("WIn");

                if (value.Length != 5)
                    throw new ArgumentException("Must have 5 elements", "WIn");

                mcmParams.Mw = value;
            }
        }

        #endregion   
    }
}
