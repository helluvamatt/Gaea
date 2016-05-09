//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using GDIPlusX.GDIPlus10.Internal;
using GDIPlusX.GDIPlus11.Internal;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Reflection;
using GDIPlusX.GDIPlus11.EffectsInternal;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates a bese effect.
    /// </summary>
    public abstract class Effect : IDisposable
    {
        #region Private Static Properties

        /// <summary>
        /// The number of threads to use as well as the current thread
        /// </summary>
        private static int miLegacyThreads = Environment.ProcessorCount - 1;

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets or sets the number of threads to use for legacy operations,
        /// this value does not include the current thread which is used as well.
        /// (-1 = Auto).
        /// </summary>
        public static int LegacyThreads
        {
            get
            {
                return miLegacyThreads;
            }
            set
            {
                miLegacyThreads = value;

                if (miLegacyThreads < 0)
                    miLegacyThreads = Environment.ProcessorCount - 1;
            }
        }

        #endregion

        #region Protected Locals

        /// <summary>
        /// Holds the point to the native effect object.
        /// </summary>
        protected IntPtr mipNativeHandle = IntPtr.Zero;

        /// <summary>
        /// Holds the Guid for this effect object.
        /// </summary>
        protected Guid mgGuid;

        /// <summary>
        /// True if the effect parameters have been invalidated.
        /// </summary>
        protected bool mbInvalid;

        /// <summary>
        /// True if this effect has been disposed.
        /// </summary>
        protected bool mbDisposed = false;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new effect from a Guid.
        /// </summary>
        /// <param name="guid">The Guid for the effect.</param>
        public Effect(Guid guid)
        {
            if(this is ILegacyEffect) Utils10.CheckAvailable();
            mbInvalid = true;
            mgGuid = guid;
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Returns the parameter data for this effect.
        /// </summary>
        /// <returns>An object containing the parameter data.</returns>
        protected abstract object InitialiseParameterData();

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Finalises parameter data.
        /// </summary>
        /// <param name="data">The structure containing the parameters to finalise.</param>
        protected virtual void FinaliseParameterData(object data)
        {
        }

        /// <summary>
        /// Gets the parameter size to send to the SetEffectParameterSize GDI+ function.
        /// </summary>
        /// <param name="structSize">The structure size of the effect parameters.</param>
        /// <param name="data">The data object for the structure of the effect parameters.</param>
        /// <returns>A uint value for the size.</returns>
        protected virtual uint GetParameterDataSize(int structSize, object data)
        {
            return (uint)structSize;
        }

        /// <summary>
        /// Gets whether the parameters object is pinnable for this effect
        /// </summary>
        /// <returns>True if its pinnable, false to copy to other memory first</returns>
        protected virtual bool ParametersPinnable()
        {
            return true;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Copies an area bitmap and applys an effect
        /// </summary>
        /// <param name="bitmap">The Bitmap to apply the effect to.</param>
        /// <param name="rect">
        /// The rectangle to apply the Effect or Rectangle.Empty for 
        /// entire bitmap, on out the area actually applied.
        /// </param>
        /// <returns>A new bitmap object with the effect applied.</returns>
        /// <exception cref="GDIPlusX.GDIPlus11.GDIPlus11NotAvailableException">GDI Plus 1.1 functions not available.</exception>
        /// <exception cref="System.ArgumentNullException">bitmap is null or effect is null.</exception>
        /// <exception cref="GDIPlusX.GDIPlus11.Effects.EffectValidateException">Effect validation with bitmap failed.</exception>
        /// <remarks>Auxillary data is calculated if the effect supports it.</remarks>
        internal Bitmap CloneApply(Bitmap bitmap, ref Rectangle rect)
        {
            if (bitmap == null) throw new ArgumentNullException("bitmap");
            this.Validate(bitmap);

            if (this is ILegacyEffectApply && ((ILegacyEffectApply)this).RunningLegacy)
                return ((ILegacyEffectApply)this).LegacyCloneApply(bitmap, ref rect);

            Interop11.CheckAvailable();

            // Initialise values
            Utils10.RECT lrRect = Utils10.RECT.FromRectangle(rect);
            Utils10.RECT lrOutRect = new Utils10.RECT();
            IntPtr lipBitmap = IntPtr.Zero;
            bool lbUseAuxData = (this is IAuxDataEffect) && ((IAuxDataEffect)this).UseAuxData;

            IntPtr lipAuxData;
            int liAuxDataSize;
            int liStatus;

            HandleRef lhrRef = new HandleRef(bitmap, bitmap.NativeHandle());
            IntPtr lipAddr = bitmap.NativeHandle();

            // Call function
            if (rect.IsEmpty)
                liStatus = Interop11.GdipBitmapCreateApplyEffect(
                    ref lipAddr, 1,
                    new HandleRef(this, this.NativeHandle()),
                    IntPtr.Zero, ref lrOutRect, out lipBitmap,
                    lbUseAuxData, out lipAuxData, out liAuxDataSize);
            else
                liStatus = Interop11.GdipBitmapCreateApplyEffect(
                    ref lipAddr, 1,
                    new HandleRef(this, this.NativeHandle()),
                    ref lrRect, ref lrOutRect, out lipBitmap,
                    lbUseAuxData, out lipAuxData, out liAuxDataSize);

            // Check for errors
            Utils10.CheckErrorStatus(liStatus);

            // Create bitmap object for return value
            Bitmap lbmpBitmap = null;
            if (lipBitmap != IntPtr.Zero)
                lbmpBitmap = lipBitmap.NativeBitmapPtrToBitmap();

            // Set rectangle output
            rect = lrOutRect.ToRectangle();

            // Set aux data if required
            if (liAuxDataSize > 0 && lipAuxData != IntPtr.Zero && lbUseAuxData)
                ((IAuxDataEffect)this).SetAuxData(lipAuxData, liAuxDataSize);

            // Free aux data if required
            if (lipAuxData != IntPtr.Zero)
                Utils10.GdipFree(lipAuxData);

            return lbmpBitmap;
        }

        /// <summary>
        /// Applys an effect to a Bitmap.
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
        internal void ApplyToBitmap(Bitmap bitmap, Rectangle rectOfInterest)
        {
            if (bitmap == null) throw new ArgumentNullException("bitmap");
            this.Validate(bitmap);

            if (this is ILegacyEffectApply && ((ILegacyEffectApply)this).RunningLegacy)
            {
                ((ILegacyEffectApply)this).LegacyApplyToBitmap(bitmap, rectOfInterest);
                return;
            }

            Interop11.CheckAvailable();

            // Initialise values
            Utils10.RECT lrRect = Utils10.RECT.FromRectangle(rectOfInterest);
            bool lbUseAuxData = (this is IAuxDataEffect) && ((IAuxDataEffect)this).UseAuxData;

            IntPtr lipAuxData;
            int liAuxDataSize;
            int liStatus;

            // Call function
            if (rectOfInterest.IsEmpty)
                liStatus = Interop11.GdipBitmapApplyEffect(
                    new HandleRef(bitmap, bitmap.NativeHandle()),
                    new HandleRef(this, this.NativeHandle()),
                    IntPtr.Zero, lbUseAuxData, out lipAuxData,
                    out liAuxDataSize);
            else
                liStatus = Interop11.GdipBitmapApplyEffect(
                    new HandleRef(bitmap, bitmap.NativeHandle()),
                    new HandleRef(this, this.NativeHandle()),
                    ref lrRect, lbUseAuxData, out lipAuxData,
                    out liAuxDataSize);

            // Check for errors
            Utils10.CheckErrorStatus(liStatus);

            // Set aux data if required
            if (liAuxDataSize > 0 && lipAuxData != IntPtr.Zero && lbUseAuxData)
                ((IAuxDataEffect)this).SetAuxData(lipAuxData, liAuxDataSize);

            // Free aux data if required
            if (lipAuxData != IntPtr.Zero)
                Utils10.GdipFree(lipAuxData);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Validates the effect parameters
        /// </summary>
        protected void ValidateParameters()
        {
            // Check to ensure this effect hasnt been disposed.
            CheckDisposed();

            // If legacy mode
            if (this is ILegacyEffect && ((ILegacyEffect)this).RunningLegacy)
            {
                mbInvalid = false;
                return;
            }

            Interop11.CheckAvailable();

            int liStatus;

            // Create the effect if it hasnt been already
            if (mipNativeHandle == IntPtr.Zero)
            {
                liStatus = Interop11.GdipCreateEffect(mgGuid, out mipNativeHandle);
                Utils10.CheckErrorStatus(liStatus);
            }

            // Get the parameter data
            object loData = InitialiseParameterData();

            try
            {
                // Set the parameter data
                SetParameters(loData);
            }
            finally
            {
                // Finalise the parameter data
                FinaliseParameterData(loData);
            }

            // Validate the effect
            mbInvalid = false;
        }

        /// <summary>
        /// Sets the parameters for the effect.
        /// </summary>
        /// <param name="structure">The structure containing the parameters.</param>
        protected void SetParameters(object structure)
        {
            int liStatus;

            // Get the structure size
            int liSize = Marshal.SizeOf(structure);

            // If the structure for the parameters is pinnable
            if (ParametersPinnable())
            {
                // Pin the structure
                GCHandle lhHandle = GCHandle.Alloc(structure, GCHandleType.Pinned);

                try
                {
                    // Get the parameter size to pass to the function
                    uint luiSize = GetParameterDataSize(liSize, structure);

                    // Set the parameters
                    liStatus = Interop11.GdipSetEffectParameters(mipNativeHandle, lhHandle.AddrOfPinnedObject(), luiSize);

                    // Check for errors
                    Utils10.CheckErrorStatus(liStatus);
                }
                finally
                {
                    // Free the pinned handle
                    lhHandle.Free();
                }
            }
            else
            {
                // Otherwise copy the memory first
                IntPtr lipParams = Marshal.AllocHGlobal(liSize);

                try
                {
                    Marshal.StructureToPtr(structure, lipParams, true);
                    uint luiSize = GetParameterDataSize(liSize, structure);
                    liStatus = Interop11.GdipSetEffectParameters(mipNativeHandle, lipParams, luiSize);
                    Utils10.CheckErrorStatus(liStatus);
                }
                finally
                {
                    Marshal.FreeHGlobal(lipParams);
                }
            }
        }

        /// <summary>
        /// Checks to see if the object is disposed and throws exception if it is.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Effect is disposed.</exception>
        protected void CheckDisposed()
        {
            if (mbDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
        }

        /// <summary>
        /// Invalidates the parameters for the effect.
        /// </summary>
        protected void InvalidateParameters()
        {
            CheckDisposed();
            mbInvalid = true;
        }

        /// <summary>
        /// Gets the size of the parameters structure in bytes.
        /// </summary>
        /// <returns>The size of the structure in bytes.</returns>
        protected uint GetParametersSize()
        {
            ValidateParameters();

            uint luiSize;
            int liStatus = Interop11.GdipGetEffectParameterSize(mipNativeHandle, out luiSize);
            Utils10.CheckErrorStatus(liStatus);

            return luiSize;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets (after validating) the pointer to the native GDI+ object for this effect.
        /// </summary>
        /// <returns>An IntPtr.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal IntPtr NativeHandle()
        {
            if (mbInvalid) ValidateParameters();

            return mipNativeHandle;
        }

        #endregion

        #region Public Virtual Methods

        /// <summary>
        /// Validates the effect with an image and raises an exception if it is not compatible.
        /// </summary>
        /// <param name="image">The image to validate with.</param>
        public virtual void Validate(Image image)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether the effect can be applied in the current environment.
        /// </summary>
        public virtual bool CanApply
        {
            get
            {
                return Interop11.Ver11Available;
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Returns true if an effect type is applicable to current environment.
        /// </summary>
        /// <param name="effectType">The type of the effect.</param>
        /// <returns>True if effect can be applied, false otherwise.</returns>
        public static bool EffectAvailable(Type effectType)
        {
            if (typeof(Effect).IsAssignableFrom(effectType))
            {
                if (Info.Ver11Available)
                    return true;
                else
                    return typeof(ILegacyEffect).IsAssignableFrom(effectType);
            }

            return false;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Disposes of the effect.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the native effect.
        /// </summary>
        protected void DisposeEffect()
        {
            if (mipNativeHandle != IntPtr.Zero)
            {
                int liStatus = Interop11.GdipDeleteEffect(mipNativeHandle);
                Utils10.CheckErrorStatus(liStatus);
                mipNativeHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Disposes of the effect.
        /// </summary>
        /// <param name="disposeManaged">True to dispose managed objects as well.</param>
        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                // Dispose managed
            }

            // Free Native
            DisposeEffect();

            mbDisposed = true;
        }

        /// <summary>
        /// Static finalizer.
        /// </summary>
        ~Effect() 
        {
            Dispose(false);
        }

        #endregion
    }
}
