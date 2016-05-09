//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// The exception that is thrown when an effect fails validation against an image.
    /// </summary>
    public class EffectValidateException : Exception
    {
        /// <summary>
        /// Creates a new exception.
        /// </summary>
        public EffectValidateException()
            : base()
        {
        }

        /// <summary>
        /// Creats a new exception with a message.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public EffectValidateException(string message)
            : base(message)
        {
        }
    }
}
