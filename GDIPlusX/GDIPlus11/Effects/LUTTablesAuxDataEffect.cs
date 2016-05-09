//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates an effect which has lookup table auxillary data.
    /// </summary>
    public abstract class LUTTablesAuxDataEffect : Effect, IAuxDataEffect
    {
        #region Protected Locals

        /// <summary>
        /// True to retrieve and process the lookup table data.
        /// </summary>
        protected bool mbProcessLUTInfo = false;

        /// <summary>
        /// The last blue lookup table data.
        /// </summary>
        protected byte[] mbLUTInfo0 = null;

        /// <summary>
        /// The last green lookup table data.
        /// </summary>
        protected byte[] mbLUTInfo1 = null;

        /// <summary>
        /// The last red lookup table data.
        /// </summary>
        protected byte[] mbLUTInfo2 = null;

        /// <summary>
        /// The last alpha lookup table data.
        /// </summary>
        protected byte[] mbLUTInfo3 = null;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new lookup table effect.
        /// </summary>
        /// <param name="guid">The Guid for the effect.</param>
        public LUTTablesAuxDataEffect(Guid guid)
            : base(guid)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears the lookup table data.
        /// </summary>
        public void ClearLUTInfo()
        {
            mbLUTInfo2 = null;
            mbLUTInfo1 = null;
            mbLUTInfo0 = null;
            mbLUTInfo3 = null;
        }

        #endregion

        #region Protected Virtual

        /// <summary>
        /// Sets the auxillary data after a call to ApplyEffect or DrawImageFX.
        /// </summary>
        /// <param name="data">The data to process.</param>
        /// <param name="size">The size in bytes of the data.</param>
        protected virtual void SetAuxData(IntPtr data, int size)
        {
            if (size != 1024 || data == IntPtr.Zero || !mbProcessLUTInfo)
            {
                ClearLUTInfo();
                return;
            }

            mbLUTInfo2 = new byte[256];
            mbLUTInfo1 = new byte[256];
            mbLUTInfo0 = new byte[256];
            mbLUTInfo3 = new byte[256];

            Marshal.Copy(data, mbLUTInfo0, 0, 256);
            data = (IntPtr)(((Int64)data) + 256);
            Marshal.Copy(data, mbLUTInfo1, 0, 256);
            data = (IntPtr)(((Int64)data) + 256);
            Marshal.Copy(data, mbLUTInfo2, 0, 256);
            data = (IntPtr)(((Int64)data) + 256);
            Marshal.Copy(data, mbLUTInfo3, 0, 256);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets whether to retrieve and process the lookup table data.
        /// </summary>
        public bool ProcessLUTInfo
        {
            get
            {
                return mbProcessLUTInfo;
            }
            set
            {
                mbProcessLUTInfo = value;
            }
        }

        /// <summary>
        /// The last red channel lookup table data.
        /// </summary>
        public byte[] LUTInfo2
        {
            get
            {
                return mbLUTInfo2;
            }
        }

        /// <summary>
        /// The last green channel lookup table data.
        /// </summary>
        public byte[] LUTInfo1
        {
            get
            {
                return mbLUTInfo1;
            }
        }

        /// <summary>
        /// The last blue channel lookup table data.
        /// </summary>
        public byte[] LUTInfo0
        {
            get
            {
                return mbLUTInfo0;
            }
        }

        /// <summary>
        /// The last alpha channel lookup table data.
        /// </summary>
        public byte[] LUTInfo3
        {
            get
            {
                return mbLUTInfo3;
            }
        }

        #endregion

        #region IAuxDataEffect Members

        /// <summary>
        /// Gets whether the effect should process and return auxillary data.
        /// </summary>
        bool IAuxDataEffect.UseAuxData
        {
            get 
            {
                return ProcessLUTInfo;
            }
        }

        /// <summary>
        /// Sets the data to the effect after an ApplyEffect or DrawImageFX call.
        /// </summary>
        /// <param name="data">A pointer to the data.</param>
        /// <param name="size">The size of the data.</param>
        /// <remarks>The data should NOT be freed, this is automatically done.</remarks>
        void IAuxDataEffect.SetAuxData(IntPtr data, int size)
        {
            SetAuxData(data, size);
        }

        #endregion
    }
}
