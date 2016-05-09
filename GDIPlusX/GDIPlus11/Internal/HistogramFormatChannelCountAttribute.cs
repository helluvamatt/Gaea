//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;

namespace GDIPlusX.GDIPlus11.Internal
{
    /// <summary>
    /// Provides an attribute for the HistogramFormat fields to specify the number of channels output.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false)] 
    internal class HistogramFormatChannelCountAttribute : Attribute
    {
        #region Protected Locals

        /// <summary>
        /// Contains the number of channels.
        /// </summary>
        protected int miChannelCount;

        #endregion

        #region Initialisation

        /// <summary>
        /// Creates a new channel count attribute.
        /// </summary>
        /// <param name="channelCount">The number of channels for this field.</param>
        public HistogramFormatChannelCountAttribute(int channelCount)
        {
            miChannelCount = channelCount;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        public int ChannelCount
        {
            get
            {
                return miChannelCount;
            }
        }

        #endregion
    }
}
