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
    public abstract class LUTTablesAuxDataEffectBGRA : LUTTablesAuxDataEffect, IAuxDataEffectBGRA
    {
        #region Initialisation

        /// <summary>
        /// Creates a new lookup table effect.
        /// </summary>
        /// <param name="guid">The Guid for the effect.</param>
        public LUTTablesAuxDataEffectBGRA(Guid guid)
            : base(guid)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The last red channel lookup table data.
        /// </summary>
        public byte[] LUTInfoR
        {
            get
            {
                return mbLUTInfo2;
            }
        }

        /// <summary>
        /// The last green channel lookup table data.
        /// </summary>
        public byte[] LUTInfoG
        {
            get
            {
                return mbLUTInfo1;
            }
        }

        /// <summary>
        /// The last blue channel lookup table data.
        /// </summary>
        public byte[] LUTInfoB
        {
            get
            {
                return mbLUTInfo0;
            }
        }

        /// <summary>
        /// The last alpha channel lookup table data.
        /// </summary>
        public byte[] LUTInfoA
        {
            get
            {
                return mbLUTInfo3;
            }
        }

        #endregion
    }
}
