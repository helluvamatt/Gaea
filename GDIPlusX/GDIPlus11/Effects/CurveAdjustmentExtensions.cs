//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using GDIPlusX.GDIPlus11.EffectsInternal;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Provides extension methods for the CurveAdjustment enumeration.
    /// </summary>
    public static class CurveAdjustmentExtensions
    {
        /// <summary>
        /// Gets the minimum and maximum adjustment value for a CurveAdjustment member.
        /// </summary>
        /// <param name="curveAdjustment">The curve adjustment to get the min and max for.</param>
        /// <param name="min">On out the minimum adjustment value.</param>
        /// <param name="max">On out the maximum adjustment value.</param>
        /// <exception cref="System.InvalidOperationException">Attribute cannot be found.</exception>
        public static void MinMax(this CurveAdjustment curveAdjustment, out int min, out int max)
        {
            Type ltType = typeof(CurveAdjustment);

            // Get the member information
            MemberInfo[] lmiInfos = ltType.GetMember(curveAdjustment.ToString());

            if (lmiInfos.Length > 0)
            {
                // Get the attribute
                object[] loAtts =
                    lmiInfos[0].GetCustomAttributes(typeof(CurveAdjustmentValueRangeAttribute), false);

                if (loAtts.Length > 0)
                {
                    CurveAdjustmentValueRangeAttribute laAttr = 
                        (CurveAdjustmentValueRangeAttribute)loAtts[0];

                    // Set the min max
                    min = laAttr.Min;
                    max = laAttr.Max;
                    return;
                }
            }

            throw new InvalidOperationException();
        }
    }
}
