//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GDIPlusX.GDIPlus10.Internal
{
    /// <summary>
    /// Provides utility and import operations for GDI+ 1.0
    /// </summary>
    internal static class Utils10
    {
        #region Private Enumerations

        /// <summary>
        /// Enumeration for status of GDI+ Native function
        /// </summary>
        private enum GpStatus
        {
            /// <summary>
            /// No error.
            /// </summary>
            Ok = 0,

            /// <summary>
            /// Generic error.
            /// </summary>
            GenericError = 1,

            /// <summary>
            /// Invalidate argument.
            /// </summary>
            InvalidParameter = 2,

            /// <summary>
            /// Out of memory.
            /// </summary>
            OutOfMemory = 3,

            /// <summary>
            /// Object busy.
            /// </summary>
            ObjectBusy = 4,

            /// <summary>
            /// Insufficient buffer.
            /// </summary>
            InsufficientBuffer = 5,

            /// <summary>
            /// Not implemented
            /// </summary>
            NotImplemented = 6,

            /// <summary>
            /// Windows standard error.
            /// </summary>
            Win32Error = 7,

            /// <summary>
            /// Invalid object state.
            /// </summary>
            WrongState = 8,

            /// <summary>
            /// Aborted.
            /// </summary>
            Aborted = 9,

            /// <summary>
            /// File not found.
            /// </summary>
            FileNotFound = 10,

            /// <summary>
            /// Value overflow.
            /// </summary>
            ValueOverflow = 11,

            /// <summary>
            /// Access denied.
            /// </summary>
            AccessDenied = 12,

            /// <summary>
            /// Unknown image format.
            /// </summary>
            UnknownImageFormat = 13,

            /// <summary>
            /// Font family not found.
            /// </summary>
            FontFamilyNotFound = 14,

            /// <summary>
            /// Font style not found.
            /// </summary>
            FontStyleNotFound = 15,

            /// <summary>
            /// Not a true type font.
            /// </summary>
            NotTrueTypeFont = 16,

            /// <summary>
            /// Unsupported GDI Plus version.
            /// </summary>
            UnsupportedGdiplusVersion = 17,

            /// <summary>
            /// GDI Plus not initialized.
            /// </summary>
            GdiplusNotInitialized = 18,

            /// <summary>
            /// Property not found
            /// </summary>
            PropertyNotFound = 19,

            /// <summary>
            /// Property not supported.
            /// </summary>
            PropertyNotSupported = 20
        }

        /// <summary>
        /// Windows errors returned by GDI Plus native functions which are ignored.
        /// </summary>
        private enum Win32Error : int
        {
            /// <summary>
            /// Access was denied.
            /// </summary>
            AccessDenied = 5,

            /// <summary>
            /// Procedure not found.
            /// </summary>
            ProcNotFound = 127
        }

        #endregion

        #region Internal Constants

        /// <summary>
        /// The GDI Plus DLL file name.
        /// </summary>
        public const string GDIPlusDll = "gdiplus.dll";

        #endregion

        #region Enumerations

        /// <summary>
        /// GDI Plus unit description.
        /// </summary>
        public enum GpUnit
        {
            /// <summary>
            /// World coordinate (non-physical unit).
            /// </summary>
            UnitWorld,      

            /// <summary>
            /// Variable - for PageTransform only.
            /// </summary>
            UnitDisplay,    

            /// <summary>
            /// Each unit is one device pixel.
            /// </summary>
            UnitPixel,      

            /// <summary>
            /// Each unit is a printer's point, or 1/72 inch.
            /// </summary>
            UnitPoint,      

            /// <summary>
            /// Each unit is 1 inch.
            /// </summary>
            UnitInch,       

            /// <summary>
            /// Each unit is 1/300 inch.
            /// </summary>
            UnitDocument,   

            /// <summary>
            /// Each unit is 1 millimeter.
            /// </summary>
            UnitMillimeter  
        };

        /// <summary>
        /// GDI Plus polygon fill mode.
        /// </summary>
        public enum GpFillMode : int
        {
            /// <summary>
            /// Alternate fill mode.
            /// </summary>
            FillModeAlternate,

            /// <summary>
            /// Winding fill mode.
            /// </summary>
            FillModeWinding
        }

        #endregion

        #region Structures

        /// <summary>
        /// A floating point GDI Plus width/hight based rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct GpRectF
        {
            /// <summary>
            /// The X corner location of the rectangle.
            /// </summary>
            public float X;

            /// <summary>
            /// The Y corner location of the rectangle.
            /// </summary>
            public float Y;

            /// <summary>
            /// The width of the rectangle.
            /// </summary>
            public float Width;

            /// <summary>
            /// The height of the rectangle.
            /// </summary>
            public float Height;

            /// <summary>
            /// Creates a new GDI Plus rectangle.
            /// </summary>
            /// <param name="x">The X corner location of the rectangle.</param>
            /// <param name="y">The Y corner location of the rectangle.</param>
            /// <param name="width">The width of the rectangle.</param>
            /// <param name="height">The height of the rectangle.</param>
            public GpRectF(float x, float y, float width, float height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            /// <summary>
            /// Creates a new GDI Plus rectangle from a System.Drawing.RectangleF.
            /// </summary>
            /// <param name="rect">The rectangle to base this GDI Plus rectangle on.</param>
            public GpRectF(RectangleF rect)
            {
                X = rect.X;
                Y = rect.Y;
                Width = rect.Width;
                Height = rect.Height;
            }

            /// <summary>
            /// Creates a new GDI Plus rectangle from a System.Drawing.Rectangle.
            /// </summary>
            /// <param name="rect">The rectangle to base this GDI Plus rectangle on.</param>
            public GpRectF(Rectangle rect)
            {
                X = rect.X;
                Y = rect.Y;
                Width = rect.Width;
                Height = rect.Height;
            }

            /// <summary>
            /// Returns a RectangleF for this GDI Plus rectangle.
            /// </summary>
            /// <returns>A System.Drawing.RectangleF structure.</returns>
            public RectangleF ToRectangle()
            {
                return new RectangleF(X, Y, Width, Height);
            }

            /// <summary>
            /// Returns a RectangleF for a GDI Plus rectangle.
            /// </summary>
            /// <param name="rect">The GDI Plus rectangle to get the RectangleF for.</param>
            /// <returns>A System.Drawing.RectangleF structure.</returns>
            public static RectangleF ToRectangle(GpRectF rect)
            {
                return rect.ToRectangle();
            }

            /// <summary>
            /// Returns a GDI Plus rectangle for a RectangleF structure.
            /// </summary>
            /// <param name="rect">The RectangleF to get the GDI Plus rectangle for.</param>
            /// <returns>A GDI Plus rectangle structure.</returns>
            public static GpRectF FromRectangle(RectangleF rect)
            {
                return new GpRectF(rect);
            }

            /// <summary>
            /// Returns a GDI Plus rectangle for a Rectangle structure.
            /// </summary>
            /// <param name="rect">The Rectangle to get the GDI Plus rectangle for.</param>
            /// <returns>A GDI Plus rectangle structure.</returns>
            public static GpRectF FromRectangle(Rectangle rect)
            {
                return new GpRectF(rect);
            }
        }

        /// <summary>
        /// An integer GDI Plus width/hight based rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct GpRect
        {
            /// <summary>
            /// The X corner location of the rectangle.
            /// </summary>
            public int X;

            /// <summary>
            /// The Y corner location of the rectangle.
            /// </summary>
            public int Y;

            /// <summary>
            /// The width of the rectangle.
            /// </summary>
            public int Width;

            /// <summary>
            /// The height of the rectangle.
            /// </summary>
            public int Height;

            /// <summary>
            /// Creates a new GDI Plus rectangle.
            /// </summary>
            /// <param name="x">The X corner location of the rectangle.</param>
            /// <param name="y">The Y corner location of the rectangle.</param>
            /// <param name="width">The width of the rectangle.</param>
            /// <param name="height">The height of the rectangle.</param>
            public GpRect(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            /// <summary>
            /// Creates a new GDI Plus rectangle from a System.Drawing.Rectangle.
            /// </summary>
            /// <param name="rect">The rectangle to base this GDI Plus rectangle on.</param>
            public GpRect(Rectangle rect)
            {
                X = rect.X;
                Y = rect.Y;
                Width = rect.Width;
                Height = rect.Height;
            }

            /// <summary>
            /// Returns a Rectangle for a GDI Plus rectangle.
            /// </summary>
            /// <returns>A System.Drawing.Rectangle structure.</returns>
            public Rectangle ToRectangle()
            {
                return new Rectangle(X, Y, Width, Height);
            }

            /// <summary>
            /// Returns a Rectangle for a GDI Plus rectangle.
            /// </summary>
            /// <param name="rect">The GDI Plus rectangle to get the Rectangle for.</param>
            /// <returns>A System.Drawing.Rectangle structure.</returns>
            public static Rectangle ToRectangle(GpRect rect)
            {
                return rect.ToRectangle();
            }

            /// <summary>
            /// Converts an array of GDI Plus rectangles to an array of System.Drawing.Rectangles.
            /// </summary>
            /// <param name="rects">The GDI Plus rectangle to convert.</param>
            /// <returns>An array of System.Drawing.Rectangle.</returns>
            public static Rectangle[] ToRectangles(GpRect[] rects)
            {
                Rectangle[] lrReturn = new Rectangle[rects.Length];

                for (int liCounter = 0; liCounter < rects.Length; liCounter++)
                    lrReturn[liCounter] = rects[liCounter].ToRectangle();

                return lrReturn;
            }

            /// <summary>
            /// Converts an array of System.Drawing.Rectangles to GDI Plus rectangles.
            /// </summary>
            /// <param name="rects">The System.Drawing.Rectangles to convert.</param>
            /// <returns>An array of GDI Plus rectangles.</returns>
            public static GpRect[] FromRectangles(Rectangle[] rects)
            {
                GpRect[] lrReturn = new GpRect[rects.Length];

                for (int liCounter = 0; liCounter < rects.Length; liCounter++)
                    lrReturn[liCounter] = FromRectangle(rects[liCounter]);

                return lrReturn;
            }

            /// <summary>
            /// Converts a System.Drawing.Rectangle to a GDI Plus Rectangle.
            /// </summary>
            /// <param name="rect">The System.Drawing.Rectangle to convert.</param>
            /// <returns>A GDI Plus Rectangle.</returns>
            public static GpRect FromRectangle(Rectangle rect)
            {
                return new GpRect(rect);
            }
        }

        /// <summary>
        /// A windows API integer LTRB based rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RECT
        {
            /// <summary>
            /// The left edge of the rectangle.
            /// </summary>
            public int Left;

            /// <summary>
            /// The top edge of the rectangle.
            /// </summary>
            public int Top;

            /// <summary>
            /// The exclusive right edge of the rectangle.
            /// </summary>
            public int Right;

            /// <summary>
            /// The exclusive bottom edge of the rectangle.
            /// </summary>
            public int Bottom;

            /// <summary>
            /// Creates a new rectangle.
            /// </summary>
            /// <param name="left">The left edge of the rectangle.</param>
            /// <param name="top">The top edge of the rectangle.</param>
            /// <param name="right">The exclusive right edge of the rectangle.</param>
            /// <param name="bottom">The exclusive bottom edge of the rectangle.</param>
            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            /// <summary>
            /// Creates a new rectangle from a System.Drawing.Rectangle.
            /// </summary>
            /// <param name="rect">The rectangle to create from.</param>
            public RECT(Rectangle rect)
            {
                Left = rect.X;
                Top = rect.Y;
                Right = rect.Right;
                Bottom = rect.Bottom;
            }

            /// <summary>
            /// Converts this rectangle to a System.Drawin.Rectangle.
            /// </summary>
            /// <returns>A System.Drawing.Rectangle structure.</returns>
            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }

            /// <summary>
            /// Converts an array of System.Drawing.Rectangles to an array of Windows API Rectangles.
            /// </summary>
            /// <param name="rects">The rectangles to convert.</param>
            /// <returns>An array of Windows API Rectangles.</returns>
            public static RECT[] FromRectangles(Rectangle[] rects)
            {
                RECT[] lrReturn = new RECT[rects.Length];

                for (int liCounter = 0; liCounter < rects.Length; liCounter++)
                    lrReturn[liCounter] = FromRectangle(rects[liCounter]);

                return lrReturn;
            }

            /// <summary>
            /// Converts a System.Drawing.Rectangle to a Windows API Rectangle.
            /// </summary>
            /// <param name="rect">The rectangle to convert.</param>
            /// <returns>A new Windows API Rectangle.</returns>
            public static RECT FromRectangle(Rectangle rect)
            {
                return new RECT(rect);
            }
        }

        #endregion

        #region Locals

        /// <summary>
        /// True if GDI Plus 1.0 functions are available.
        /// </summary>
        private static bool mbGDIPlus10Available = IsGDIPlus10Available();

        /// <summary>
        /// True if running in a 64-bit environment.
        /// </summary>
        private static bool mbIs64Bit =
            (System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").Contains("64")) &&
            (IntPtr.Size == 8);

        #endregion

        #region Imports

        /// <summary>
        /// Loads the specified module into the address space of the calling process. 
        /// </summary>
        /// <param name="fileName">
        /// The name of the module. This can be either a library module (a .dll file) or an 
        /// executable module (an .exe file). The name specified is the file name of the 
        /// module and is not related to the name stored in the library module itself, as 
        /// specified by the LIBRARY keyword in the module-definition (.def) file.
        /// <para />
        /// If the string specifies a full path, the function searches only that path for the module.
        /// <para />
        /// If the string specifies a relative path or a module name without a path, the function 
        /// uses a standard search strategy to find the module; for more information, see the Remarks.
        /// <para />
        /// If the function cannot find the module, the function fails. When specifying a path, be 
        /// sure to use backslashes (\), not forward slashes (/). For more information about paths, 
        /// see Naming a File or Directory.
        /// <para />
        /// If the string specifies a module name without a path and the file name extension is omitted, 
        /// the function appends the default library extension .dll to the module name. To prevent the 
        /// function from appending .dll to the module name, include a trailing point character (.) in 
        /// the module name string.
        /// </param>
        /// <returns></returns>
        /// <remarks>The specified module may cause other modules to be loaded.</remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string fileName);

        /// <summary>
        /// Retrieves the address of an exported function or variable from the specified 
        /// dynamic-link library (DLL).
        /// </summary>
        /// <param name="hModule">
        /// A handle to the DLL module that contains the function or variable. 
        /// The LoadLibrary, LoadLibraryEx, or GetModuleHandle function returns this handle.
        /// </param>
        /// <param name="procedureName">
        /// The function or variable name, or the function's ordinal value. If this 
        /// parameter is an ordinal value, it must be in the low-order word; the high-order 
        /// word must be zero.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the address of the exported 
        /// function or variable. If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        /// <summary>
        /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements 
        /// its reference count.
        /// </summary>
        /// <param name="hModule">
        /// A handle to the loaded library module. The LoadLibrary function returns this handle.
        /// </param>
        /// <returns>
        /// True on success, false on failure.
        /// </returns>
        /// <remarks>
        /// When the reference count reaches zero, the module is unloaded from the address space 
        /// of the calling process and the handle is no longer valid.
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

#if Unsafe
        /// <summary>
        /// Moves a block of memory from one location to another.
        /// </summary>
        /// <param name="dest">A memory pointer to the starting address of the move destination.</param>
        /// <param name="src">A memory pointer to the starting address of the block of memory to be moved.</param>
        /// <param name="size">The size of the block of memory to move, in bytes.</param>
        /// <remarks>The source and destination blocks may overlap.</remarks>
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void MoveMemory(IntPtr dest, IntPtr src, int size);
#endif

        /// <summary>
        /// Draws a polygon.
        /// </summary>
        /// <param name="graphics">Pointer to the Graphics object.</param>
        /// <param name="pen">Pointer to a pen that used to draw the polygon.</param>
        /// <param name="pointFs">
        /// Pointer to an array of PointF structures that specify the vertices of 
        /// the polygon.
        /// </param>
        /// <param name="count">
        /// Long integer value that specifies the number of 
        /// elements in the points array.
        /// </param>
        /// <returns>A standard GDI Plus status flag (see GpStatus).</returns>
        /// <remarks>
        /// If the first and last coordinates in the points array are not identical, a 
        /// line is drawn between them to close the polygon.
        /// </remarks>
        [DllImport(
            GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipDrawPolygon(HandleRef graphics, HandleRef pen, IntPtr pointFs, int count);

        /// <summary>
        /// Uses uses a brush to fill the interior of a polygon.
        /// </summary>
        /// <param name="graphics">Pointer to the Graphics object.</param>
        /// <param name="brush">Pointer to a Brush object that is used to paint the interior of the polygon.</param>
        /// <param name="pointFs">
        /// Pointer to an array of PointsF structures that make up the vertices of the polygon. The 
        /// first two points in the array specify the first side of the polygon. Each additional 
        /// point specifies a new side, the vertices of which include the point and the previous point. 
        /// </param>
        /// <param name="count">
        /// Long integer value that specifies the number of 
        /// elements in the points array.
        /// </param>
        /// <param name="fillmode">
        /// Element of the GpFillMode enumeration that specifies how to fill a closed area 
        /// that is within another closed area and that is created when the curve or 
        /// path passes over itself.
        /// </param>
        /// <returns>A standard GDI Plus status flag (see GpStatus).</returns>
        /// <remarks>
        /// If the last point and the first point do not coincide, they specify the last 
        /// side of the polygon.
        /// </remarks>        
        [DllImport(
            GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipFillPolygon(HandleRef graphics, HandleRef brush, IntPtr pointFs, int count, GpFillMode fillmode);

        /// <summary>
        /// Draws a polygon.
        /// </summary>
        /// <param name="graphics">Pointer to the Graphics object.</param>
        /// <param name="pen">Pointer to a pen that used to draw the polygon.</param>
        /// <param name="points">
        /// Pointer to an array of Point structures that specify the vertices of 
        /// the polygon.
        /// </param>
        /// <param name="count">
        /// Long integer value that specifies the number of 
        /// elements in the points array.
        /// </param>
        /// <returns>A standard GDI Plus status flag (see GpStatus).</returns>
        /// <remarks>
        /// If the first and last coordinates in the points array are not identical, a 
        /// line is drawn between them to close the polygon.
        /// </remarks>
        [DllImport(
            GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipDrawPolygonI(HandleRef graphics, HandleRef pen, IntPtr points, int count);

        /// <summary>
        /// Uses uses a brush to fill the interior of a polygon.
        /// </summary>
        /// <param name="graphics">Pointer to the Graphics object.</param>
        /// <param name="brush">Pointer to a Brush object that is used to paint the interior of the polygon.</param>
        /// <param name="points">
        /// Pointer to an array of Point structures that make up the vertices of the polygon. The 
        /// first two points in the array specify the first side of the polygon. Each additional 
        /// point specifies a new side, the vertices of which include the point and the previous point. 
        /// </param>
        /// <param name="count">
        /// Long integer value that specifies the number of 
        /// elements in the points array.
        /// </param>
        /// <param name="fillmode">
        /// Element of the GpFillMode enumeration that specifies how to fill a closed area 
        /// that is within another closed area and that is created when the curve or 
        /// path passes over itself.
        /// </param>
        /// <returns>A standard GDI Plus status flag (see GpStatus).</returns>
        /// <remarks>
        /// If the last point and the first point do not coincide, they specify the last 
        /// side of the polygon.
        /// </remarks> 
        [DllImport(
            GDIPlusDll,
            ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern int GdipFillPolygonI(HandleRef graphics, HandleRef brush, IntPtr points, int count, GpFillMode fillmode);

        /// <summary>
        /// Allocates memory for GDI+ objects.
        /// </summary>
        /// <param name="size">The number of to be allocated.</param>
        /// <returns>A pointer to the memory block on success, or IntPtr.Zero on failure</returns>
        [DllImport(
            GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern IntPtr GdipAlloc(uint size);

        /// <summary>
        /// Frees memory allocated for GDI+ objects.
        /// </summary>
        /// <param name="ptr">Pointer to the start of the memory block to free.</param>
        [DllImport(
            GDIPlusDll,
            SetLastError = true, ExactSpelling = true,
            CharSet = CharSet.Unicode)]
        public static extern void GdipFree(IntPtr ptr);

        #endregion

        #region Private Utility Methods

        /// <summary>
        /// Tests to ensure that GDI Plus 1.0 is available
        /// </summary>
        /// <returns>True if GDI Plus 1.0 is available, false otherwise.</returns>
        private static bool IsGDIPlus10Available()
        {
            return GDIPlusProcAvailable("GdipAlloc");
        }

        /// <summary>
        /// Returns an exception for a GDI Plus status value.
        /// </summary>
        /// <param name="status">The status value to throw the exception for.</param>
        /// <returns>The exception which is to be thrown.</returns>
        private static Exception StatusException(int status)
        {
            switch ((GpStatus)status)
            {
                case GpStatus.GenericError: return new ExternalException("External Error: E_FAIL");
                case GpStatus.InvalidParameter: return new ArgumentException();
                case GpStatus.OutOfMemory: return new OutOfMemoryException();
                case GpStatus.ObjectBusy: return new InvalidOperationException();
                case GpStatus.InsufficientBuffer: return new OutOfMemoryException();
                case GpStatus.NotImplemented: return new NotImplementedException();
                case GpStatus.Win32Error: return new ExternalException("Win32Error: E_FAIL");
                case GpStatus.WrongState: return new InvalidOperationException();
                case GpStatus.Aborted: return new ExternalException("Aborted: E_ABORT");
                case GpStatus.FileNotFound: return new FileNotFoundException();
                case GpStatus.ValueOverflow: return new OverflowException();
                case GpStatus.AccessDenied: return new ExternalException("Access Denied");
                case GpStatus.UnknownImageFormat: return new ArgumentException();
                case GpStatus.PropertyNotFound: return new ArgumentException();
                case GpStatus.PropertyNotSupported: return new ArgumentException();

                case GpStatus.FontFamilyNotFound:
                    return new ArgumentException("GDI Plus Font Family Not Found", "?");

                case GpStatus.FontStyleNotFound:
                    return new ArgumentException("GDI Plus Font Style Not Found", "?");

                case GpStatus.NotTrueTypeFont:
                    return new ArgumentException("GDI Plus Not True Type Font");

                case GpStatus.UnsupportedGdiplusVersion:
                    return new ExternalException("GDI Plus - Unsupported GDI Version");

                case GpStatus.GdiplusNotInitialized:
                    return new ExternalException("GDI Plus Not Initialised : E_FAILE");
            }

            return new ExternalException("GDI Plus Unknown: E_UNEXPECTED", status);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Pins a set of objects and returns the GC Handles. Fails if any pins fail.
        /// </summary>
        /// <param name="objs">The objects to pin.</param>
        /// <returns>An array of GCHandles.</returns>
        public static GCHandle[] PinObjects(params object[] objs)
        {
            GCHandle[] lhHandles = new GCHandle[objs.Length];

            try
            {
                for (int liCounter = 0; liCounter < objs.Length; liCounter++)
                    lhHandles[liCounter] = GCHandle.Alloc(objs[liCounter], GCHandleType.Pinned);
            }
            catch
            {
                UnpinObjects(lhHandles);
                throw;
            }

            return lhHandles;
        }

        /// <summary>
        /// Unpins an array of GCHandles.
        /// </summary>
        /// <param name="handles">The handles to unpin.</param>
        public static void UnpinObjects(params GCHandle[] handles)
        {
            foreach (GCHandle lhHandle in handles)
                if(lhHandle.IsAllocated) lhHandle.Free();
        }

        /// <summary>
        /// Tests to see if a GDI Plus procedure is available.
        /// </summary>
        /// <param name="procName">The case sensitive name of the procedure.</param>
        /// <returns>True if the procedure is available, false otherwise.</returns>
        /// <remarks>This method is intended only for light use, results should be cached.</remarks>
        public static bool GDIPlusProcAvailable(string procName)
        {
            // Load the GDI Plus DLL
            IntPtr lipModule = LoadLibrary(GDIPlusDll);
            if (lipModule == IntPtr.Zero) return false;

            try
            {
                // Attempt to get the procedure address
                IntPtr lipProcAddress = GetProcAddress(lipModule, procName);
                if (lipProcAddress == IntPtr.Zero) return false;
            }
            finally
            {
                // Free the library.
                FreeLibrary(lipModule);
            }

            return true;
        }

        /// <summary>
        /// Checks to see if GDI Plus 1.0 is available
        /// </summary>
        /// <exception cref="GDIPlusX.GDIPlus10.GDIPlus10NotAvailableException">
        /// GDI Plus 1.0 not available.
        /// </exception>
        public static void CheckAvailable()
        {
            if (!Ver10Available)
                throw new GDIPlus10NotAvailableException();
        }

        /// <summary>
        /// Checks an enumeration parameter value to ensure it is within range and raises an exception if it isnt.
        /// </summary>
        /// <typeparam name="T">The type for enumeration.</typeparam>
        /// <param name="enumValue">The value for the enumeration.</param>
        /// <param name="minValue">The minimum value for the enumeration.</param>
        /// <param name="maxValue">The maximum value for the numeration.</param>
        /// <param name="paramName">The name of the parameter / variable to check.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Value is out of range.</exception>
        public static void CheckEnumRange<T>(T enumValue, T minValue, T maxValue, string paramName) where T : IComparable, IConvertible 
        {
            if (enumValue.CompareTo(minValue) < 0 || enumValue.CompareTo(maxValue) > 0)
                throw new ArgumentOutOfRangeException(
                    paramName, enumValue,
                    String.Format(
                        "Must be from {0}.{1} to {0}.{2}", 
                        typeof(T).FullName,
                        minValue, 
                        maxValue));
        }

        /// <summary>
        /// Checks the GDI Plus status for a GDI Plus native function return value.
        /// </summary>
        /// <param name="status">The status to check.</param>
        /// <exception>Any GDI Plus exception.</exception>
        public static void CheckErrorStatus(int status)
        {
            if (status != (int)GpStatus.Ok)
            {
                // Generic error from GDI+ can be GenericError or Win32Error.
                if (status == (int)GpStatus.GenericError || status == (int)GpStatus.Win32Error)
                {
                    int liError = Marshal.GetLastWin32Error();
                    if (liError == (int)Win32Error.AccessDenied || liError == (int)Win32Error.ProcNotFound ||
                       (((SystemInformation.TerminalServerSession && (liError == 0)))))
                        return;
                }

                //legitimate error, throw our status exception 
                throw Utils10.StatusException(status);
            }
        }

        #endregion

        #region Utility Properties

        /// <summary>
        /// Gets whether GDI Plus 1.0 is available.
        /// </summary>
        public static bool Ver10Available
        {
            get
            {
                return mbGDIPlus10Available;
            }
        }

        /// <summary>
        /// Gets whether we are running in a 64-bit environment.
        /// </summary>
        public static bool Is64BitOS
        {
            get
            {
                return mbIs64Bit;
            }
        }

        #endregion
    }
}
