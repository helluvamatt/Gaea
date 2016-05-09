using System.Drawing;

namespace GDIPlusX.GDIPlus11.Effects
{
    /// <summary>
    /// Encapsulates the ability for an effect which has legacy capabilities.
    /// </summary>
    public interface ILegacyEffect 
    {
        /// <summary>
        /// Gets or sets whether to force legacy mode.
        /// </summary>
        bool ForceLegacy { get; set; }

        /// <summary>
        /// Gets whether effect will run in legacy mode.
        /// </summary>
        bool RunningLegacy { get; }
    }
}
