//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;

namespace GDIPlusX.GDIPlus11.EffectsInternal
{
    /// <summary>
    /// Encapsulates a static class which can calculate the points
    /// for smooth quadratic bezier curves.
    /// </summary>
    /// <see>http://en.wikipedia.org/wiki/B%C3%A9zier_curve</see>
    /// <seealso>http://www.codeproject.com/KB/recipes/BezirCurves.aspx</seealso>
    /// <remarks>Code originally taken from seealso above.</remarks>
    internal static class QuadraticBezierCurve
    {
        #region Public Structures

        /// <summary>
        /// A 2D point using Double fields.
        /// </summary>
        public struct Point
        {
            #region Public Fields

            /// <summary>
            /// The X coordinate.
            /// </summary>
            public Double X;

            /// <summary>
            /// The Y coordinate.
            /// </summary>
            public Double Y;

            #endregion

            #region Initialisation

            /// <summary>
            /// Creates a new Point.
            /// </summary>
            /// <param name="x">The x coordinate of the point.</param>
            /// <param name="y">The y coordinate of the point.</param>
            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Sets botht the X and the Y to 0.
            /// </summary>
            public void Zero()
            {
                X = 0.0;
                Y = 0.0;
            }

            #endregion
        }

        #endregion

        #region Private Static Constants

        /// <summary>
        /// Cache values containing the factorial value of its index.
        /// </summary>
        private static readonly double[] mdFactorials = new double[]
        {
            1.0,
            1.0,
            2.0,
            6.0,
            24.0,
            120.0,
            720.0,
            5040.0,
            40320.0,
            362880.0,
            3628800.0,
            39916800.0,
            479001600.0,
            6227020800.0,
            87178291200.0,
            1307674368000.0,
            20922789888000.0,
            355687428096000.0,
            6402373705728000.0,
            121645100408832000.0,
            2432902008176640000.0,
            51090942171709440000.0,
            1124000727777607680000.0,
            25852016738884976640000.0,
            620448401733239439360000.0,
            15511210043330985984000000.0,
            403291461126605635584000000.0,
            10888869450418352160768000000.0,
            304888344611713860501504000000.0,
            8841761993739701954543616000000.0,
            265252859812191058636308480000000.0,
            8222838654177922817725562880000000.0,
            263130836933693530167218012160000000.0
        };

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Gets the factorial of a number.
        /// </summary>
        /// <param name="n">The factorial to get. Range is 0 to 32.</param>
        /// <returns>The factorial of n.</returns>
        private static double factorial(int n)
        {
            if (n < 0 || n > 32) 
                throw new ArgumentOutOfRangeException("n", n, "Must be in range 0 to 32");
            return mdFactorials[n]; 
        }

        /// <summary>
        /// Calculates the Ni value for a Benstein value.
        /// </summary>
        /// <param name="n">The n value for the Bernstein formula.</param>
        /// <param name="i">The i value fot the Bernstein formula.</param>
        /// <returns>The Ni value.</returns>
        private static double Ni(int n, int i)
        {
            double ldA1 = factorial(n);
            double ldA2 = factorial(i);
            double ldA3 = factorial(n - i);
            return ldA1 / (ldA2 * ldA3);
        }

        /// <summary>
        /// Calculates the Bernstein basis value.
        /// </summary>
        /// <param name="n">Last point index.</param>
        /// <param name="i">Point index to calculate Bernstein basis value for.</param>
        /// <param name="t">Time value for all points to get the value for. 0 to 1.0.</param>
        /// <returns>The Bernstein basis value.</returns>
        private static double Bernstein(int n, int i, double t)
        {
            // t^i
            double ldTI;

            // (1 - t)^i
            double ldTNI; 

            // Prevent problems with pow (0 values)
            if (t == 0.0 && i == 0) ldTI = 1.0; else ldTI = Math.Pow(t, i);
            if (n == i && t == 1.0) ldTNI = 1.0; else ldTNI = Math.Pow((1 - t), (n - i));

            // Return Bernstein basis
            return Ni(n, i) * ldTI * ldTNI;
        }

        /// <summary>
        /// Lerps a value to between another control value in a linear fashion.
        /// </summary>
        /// <param name="min">The minimum resulting value.</param>
        /// <param name="max">The maximum resulting value.</param>
        /// <param name="controlMin">The control minimum value.</param>
        /// <param name="controlMax">The control maximum value.</param>
        /// <param name="controlValue">The control value to base the new value on.</param>
        /// <returns>A value between min and max if controlValue is in range controlMin controlMax.</returns>
        private static double Lerp(double min, double max, double controlMin, double controlMax, double controlValue)
        {
            return min + ((max - min) * ((controlValue - controlMin) / (controlMax - controlMin)));
        }

        #endregion

        #region Public Static Method

        /// <summary>
        /// Calculates the estimated Y value for non-overlapping curve.
        /// </summary>
        /// <param name="points">The points to use for the calculation.</param>
        /// <param name="x">The X value to get the Y value for.</param>
        /// <returns>The Y value, or Y curve start if x is too small, or Y curve end if x is too big.</returns>
        public static double EstimatedYValue(Point[] points, double x)
        {
            return EstimatedYValue(points, x, points[points.Length-1].X > points[0].X);
        }

        /// <summary>
        /// Calculates the estimated Y value for non-overlapping curve.
        /// </summary>
        /// <param name="points">The points to use for the calculation.</param>
        /// <param name="x">The X value to get the Y value for.</param>
        /// <param name="forwardDirection">True if to search forward in points, false otherwise.</param>
        /// <returns>The Y value, or Y curve start if x is too small, or Y curve end if x is too big.</returns>
        public static double EstimatedYValue(Point[] points, double x, bool forwardDirection)
        {
            int liIndex = 0;

            // While the value is still not in range
            while (
                liIndex < points.Length && 
                (
                    (forwardDirection && x > points[liIndex].X) || 
                    (!forwardDirection && x < points[liIndex].X)
                )
            )
                liIndex++;

            // Set to end if too big or too small
            if (liIndex >= points.Length) return points[points.Length-1].Y;
            if (liIndex < 2) return points[0].Y;

            // Return linear lerped Y value
            return Lerp(points[liIndex - 1].Y, points[liIndex].Y, points[liIndex - 1].X, points[liIndex].X, x);
        }

        /// <summary>
        /// Calculates the qudratic bezier curve points for a set of points.
        /// </summary>
        /// <param name="source">The points to base the curve on. Must not be anymore than 32 points, and at least 2 points.</param>
        /// <param name="outPoints">The number of output points to calculate.</param>
        /// <param name="dest">The points to contain the curve. Must be at least outPoints in length.</param>
        public static void Bezier(Point[] source, int outPoints, Point[] dest)
        {
            // Cache points
            int liPoints = source.Length;

            // Check for errors
            if (liPoints > 32 || liPoints < 2)
                throw new ArgumentOutOfRangeException("source", source.Length, "Cannot be more than 32 or less than 2 points.");

            if (outPoints < 2)
                throw new ArgumentOutOfRangeException("outPoints", outPoints, "Cannot be less than 2 points.");

            if (dest == null || dest.Length < outPoints)
                throw new ArgumentOutOfRangeException("dest", dest.Length, "Must be at least outPoints in length.");

            // Current point position
            double ldPoint = 0;

            // Step point position
            double ldStep = (double)1.0 / (outPoints - 1);

            // Destination points
            for (int liDest = 0; liDest != outPoints; liDest++)
            {
                // If point position is really close to end then clamp it to the end
                if ((1.0 - ldPoint) < double.Epsilon) ldPoint = 1.0;

                // Zero the total
                dest[liDest].Zero();

                // For each source
                for (int liSource = 0; liSource != liPoints; liSource++)
                {
                    // Calcualte the basis
                    double ldBasis = Bernstein(liPoints - 1, liSource, ldPoint);

                    // Multiply the X and Y from source and add to dest
                    dest[liDest].X += ldBasis * source[liSource].X;
                    dest[liDest].Y += ldBasis * source[liSource].Y;
                }

                // Move to next output point
                ldPoint += ldStep;
            }
        }

        #endregion
    }
}
