//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;

namespace GDIPlusX.GDIPlus11.EffectsInternal
{
    /// <summary>
    /// Provides min max data for a curve adjustment field value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false)] 
    internal class CurveAdjustmentValueRangeAttribute : Attribute
    {
        #region Protected Locals

        /// <summary>
        /// Minimum value
        /// </summary>
        protected int miMin;

        /// <summary>
        /// Maximum value
        /// </summary>
        protected int miMax;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new CurveAdjustmentValueRangeAttribute attribute.
        /// </summary>
        /// <param name="min">The minimum adjust value.</param>
        /// <param name="max">The maximum adjust value.</param>
        public CurveAdjustmentValueRangeAttribute(int min, int max)
        {
            miMin = min;
            miMax = max;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the minimum adjust value.
        /// </summary>
        public int Min
        {
            get
            {
                return miMin;
            }
        }

        /// <summary>
        /// Gets the maximum adjust value.
        /// </summary>
        public int Max
        {
            get
            {
                return miMax;
            }
        }

        #endregion
    }
}
