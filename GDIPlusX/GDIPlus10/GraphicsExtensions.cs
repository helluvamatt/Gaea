//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using GDIPlusX.GDIPlus10.Internal;

namespace GDIPlusX.GDIPlus10
{
    /// <summary>
    /// Provides internal extensions for the Graphics class.
    /// </summary>
    public static class GraphicsExtensions
    {
        /// <summary>
        /// Draws a polygon defined by an array of System.Drawing.PointF structures, a starting index and a count.
        /// </summary>
        /// <param name="graphics">The graphics to draw on.</param>
        /// <param name="pen">System.Drawing.Pen that determines the color, width, and style of the polygon.</param>
        /// <param name="points">Array of System.Drawing.Point structures that represent all or some of the vertices of the polygon.</param>
        /// <param name="startIndex">The starting index within the array of points for the first vertex of the polygon.</param>
        /// <param name="count">The number of points to use in the points array from startIndex for the vertexes of the polygon.</param>
        /// <exception cref="GDIPlusX.GDIPlus10.GDIPlus10NotAvailableException">GDI Plus 1.0 not available.</exception>
        /// <exception cref="System.ArgumentNullException">graphics is null.  -or- pen is null.  -or- points are null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex is not within array bounds.  -or-
        /// startIndex + count - 1 is not within array bounds.  -or-
        /// count does not contain enough elements (2).
        /// </exception>
        public static void DrawPolygon(this Graphics graphics, Pen pen, PointF[] points, int startIndex, int count)
        {
            Utils10.CheckAvailable();
            if (graphics == null) throw new ArgumentNullException("graphics");
            if (pen == null) throw new ArgumentNullException("pen");
            if (points == null) throw new ArgumentNullException("points");
            if (startIndex >= points.Length) throw new ArgumentOutOfRangeException("startIndex");
            if (startIndex + count > points.Length) throw new ArgumentOutOfRangeException("count", count, "startIndex + count must be <= points.Length");
            if (count < 2) throw new ArgumentOutOfRangeException("count", count, "must be >= 2");

#if Unsafe
            unsafe
            {
                fixed (PointF* lipPoints = points)
                {
                    int status = Utils10.GdipDrawPolygon(
                        new HandleRef(graphics, graphics.NativeHandle()),
                        new HandleRef(pen, pen.NativeHandle()),
                        (IntPtr)(&(lipPoints[startIndex])), count);

                    Utils10.CheckErrorStatus(status);
                }
            }
#else
            GCHandle lhHandle = GCHandle.Alloc(points, GCHandleType.Pinned);

            try
            {
                IntPtr lipPoints = lhHandle.AddrOfPinnedObject();
                lipPoints = (IntPtr)(((Int64)lipPoints) + (Marshal.SizeOf(typeof(PointF)) * startIndex));

                int status = Utils10.GdipDrawPolygon(
                    new HandleRef(graphics, graphics.NativeHandle()),
                    new HandleRef(pen, pen.NativeHandle()),
                    lipPoints, count);

                Utils10.CheckErrorStatus(status);
            }
            finally
            {
                lhHandle.Free();
            }
#endif
        }

        /// <summary>
        /// Draws a polygon defined by an array of System.Drawing.Point structures, a starting index and a count.
        /// </summary>
        /// <param name="graphics">The graphics to draw on.</param>
        /// <param name="pen">System.Drawing.Pen that determines the color, width, and style of the polygon.</param>
        /// <param name="points">Array of System.Drawing.PointF structures that represent all or some of the vertices of the polygon.</param>
        /// <param name="startIndex">The starting index within the array of points for the first vertex of the polygon.</param>
        /// <param name="count">The number of points to use in the points array from startIndex for the vertexes of the polygon.</param>
        /// <exception cref="GDIPlusX.GDIPlus10.GDIPlus10NotAvailableException">GDI Plus 1.0 not available.</exception>
        /// <exception cref="System.ArgumentNullException">graphics is null.  -or- pen is null.  -or- points are null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex is not within array bounds.  -or-
        /// startIndex + count - 1 is not within array bounds.  -or-
        /// count does not contain enough elements (2).
        /// </exception>        
        public static void DrawPolygon(this Graphics graphics, Pen pen, Point[] points, int startIndex, int count)
        {
            Utils10.CheckAvailable();
            if (graphics == null) throw new ArgumentNullException("graphics");
            if (pen == null) throw new ArgumentNullException("pen");
            if (points == null) throw new ArgumentNullException("points");
            if (startIndex >= points.Length) throw new ArgumentOutOfRangeException("startIndex");
            if (startIndex + count > points.Length) throw new ArgumentOutOfRangeException("count", count, "startIndex + count must be <= points.Length");
            if (count < 2) throw new ArgumentOutOfRangeException("count", count, "must be >= 2");

#if Unsafe
            unsafe
            {
                fixed (Point* lipPoints = points)
                {
                    int liStatus = Utils10.GdipDrawPolygonI(
                        new HandleRef(graphics, graphics.NativeHandle()),
                        new HandleRef(pen, pen.NativeHandle()),
                        (IntPtr)(&(lipPoints[startIndex])), count);

                    Utils10.CheckErrorStatus(liStatus);
                }
            }
#else
            GCHandle lhHandle = GCHandle.Alloc(points, GCHandleType.Pinned);

            try
            {
                IntPtr lipPoints = lhHandle.AddrOfPinnedObject();
                lipPoints = (IntPtr)(((Int64)lipPoints) + (Marshal.SizeOf(typeof(Point)) * startIndex));

                int status = Utils10.GdipDrawPolygonI(
                    new HandleRef(graphics, graphics.NativeHandle()),
                    new HandleRef(pen, pen.NativeHandle()),
                    lipPoints, count);

                Utils10.CheckErrorStatus(status);
            }
            finally
            {
                lhHandle.Free();
            }
#endif
        }

        /// <summary>
        /// Fills the interior of a polygon defined by an array of points specified by
        ///  System.Drawing.PointF structures.
        /// </summary>
        /// <param name="graphics">The graphics to draw on.</param>
        /// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
        /// <param name="points">Array of System.Drawing.PointF structures that represent all or some of the vertices of the polygon to fill.</param>
        /// <param name="startIndex">The starting index within the array of points for the first vertex of the polygon.</param>
        /// <param name="count">The number of points to use in the points array from startIndex for the vertexes of the polygon.</param>
        /// <param name="fillMode">Member of the System.Drawing.Drawing2D.FillMode enumeration that determines the style of the fill.</param>
        /// <exception cref="GDIPlusX.GDIPlus10.GDIPlus10NotAvailableException">GDI Plus 1.0 not available.</exception>
        /// <exception cref="System.ArgumentNullException">graphics is null.  -or- brush is null.  -or- points are null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex is not within array bounds.  -or-
        /// startIndex + count - 1 is not within array bounds.  -or-
        /// count does not contain enough elements (2).
        /// </exception>     
        public static void FillPolygon(this Graphics graphics, Brush brush, PointF[] points, int startIndex, int count, FillMode fillMode)
        {
            Utils10.CheckAvailable();
            if (graphics == null) throw new ArgumentNullException("graphics");
            if (brush == null) throw new ArgumentNullException("brush");
            if (points == null) throw new ArgumentNullException("points");
            if (startIndex >= points.Length) throw new ArgumentOutOfRangeException("startIndex");
            if (startIndex + count > points.Length) throw new ArgumentOutOfRangeException("count", count, "startIndex + count must be <= points.Length");
            if (count < 2) throw new ArgumentOutOfRangeException("count", count, "must be >= 2");

#if Unsafe
            unsafe
            {
                fixed (PointF* lipPoints = points)
                {
                    int liStatus = Utils10.GdipFillPolygon(
                        new HandleRef(graphics, graphics.NativeHandle()),
                        new HandleRef(brush, brush.NativeHandle()),
                        (IntPtr)(&(lipPoints[startIndex])), count, (Utils10.GpFillMode)fillMode);

                    Utils10.CheckErrorStatus(liStatus);
                }
            }
#else
            GCHandle lhHandle = GCHandle.Alloc(points, GCHandleType.Pinned);

            try
            {
                IntPtr lipPoints = lhHandle.AddrOfPinnedObject();
                lipPoints = (IntPtr)(((Int64)lipPoints) + (Marshal.SizeOf(typeof(PointF)) * startIndex));

                int status = Utils10.GdipFillPolygon(
                    new HandleRef(graphics, graphics.NativeHandle()),
                    new HandleRef(brush, brush.NativeHandle()),
                    lipPoints, count, (Utils10.GpFillMode)fillMode);

                Utils10.CheckErrorStatus(status);
            }
            finally
            {
                lhHandle.Free();
            }
#endif
        }

        /// <summary>
        /// Fills the interior of a polygon defined by an array of points specified by
        /// System.Drawing.PointF structures.
        /// </summary>
        /// <param name="graphics">The graphics to draw on.</param>
        /// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
        /// <param name="points">Array of System.Drawing.PointF structures that represent all or some of the vertices of the polygon to fill.</param>
        /// <param name="startIndex">The starting index within the array of points for the first vertex of the polygon.</param>
        /// <param name="count">The number of points to use in the points array from startIndex for the vertexes of the polygon.</param>
        /// <param name="fillMode">Member of the System.Drawing.Drawing2D.FillMode enumeration that determines the style of the fill.</param>
        /// <exception cref="GDIPlusX.GDIPlus10.GDIPlus10NotAvailableException">GDI Plus 1.0 not available.</exception>
        /// <exception cref="System.ArgumentNullException">graphics is null.  -or- brush is null.  -or- points are null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex is not within array bounds.  -or-
        /// startIndex + count - 1 is not within array bounds.  -or-
        /// count does not contain enough elements (2).
        /// </exception>     
        public static void FillPolygon(this Graphics graphics, Brush brush, Point[] points, int startIndex, int count, FillMode fillMode)
        {
            if (graphics == null) throw new ArgumentNullException("graphics");
            if (brush == null) throw new ArgumentNullException("brush");
            if (points == null) throw new ArgumentNullException("points");
            if (startIndex >= points.Length) throw new ArgumentOutOfRangeException("startIndex");
            if (startIndex + count > points.Length) throw new ArgumentOutOfRangeException("count", count, "startIndex + count must be <= points.Length");
            if (count < 2) throw new ArgumentOutOfRangeException("count", count, "must be >= 2");

#if Unsafe
            unsafe
            {
                fixed (Point* lipPoints = points)
                {
                    int status = Utils10.GdipFillPolygonI(
                        new HandleRef(graphics, graphics.NativeHandle()),
                        new HandleRef(brush, brush.NativeHandle()),
                        (IntPtr)(&(lipPoints[startIndex])), count, (Utils10.GpFillMode)fillMode);

                    Utils10.CheckErrorStatus(status);
                }
            }
#else
            GCHandle lhHandle = GCHandle.Alloc(points, GCHandleType.Pinned);

            try
            {
                IntPtr lipPoints = lhHandle.AddrOfPinnedObject();
                lipPoints = (IntPtr)(((Int64)lipPoints) + (Marshal.SizeOf(typeof(Point)) * startIndex));

                int status = Utils10.GdipFillPolygonI(
                    new HandleRef(graphics, graphics.NativeHandle()),
                    new HandleRef(brush, brush.NativeHandle()),
                    lipPoints, count, (Utils10.GpFillMode)fillMode);

                Utils10.CheckErrorStatus(status);
            }
            finally
            {
                lhHandle.Free();
            }
#endif
        }
    }
}
