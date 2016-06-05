using Prism.Commands;

namespace Gaea.UI.Domain
{
	public static class GlobalCommands
	{
		public static readonly CompositeCommand ShutdownCommand = new CompositeCommand();

		public static readonly CompositeCommand ShowConfigCommand = new CompositeCommand();
	}
}
