using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaea.Services.Data
{
	internal class ServiceMessage
	{
		public MessageType Type { get; set; }

		public object Data1 { get; set; }

		public object Data2 { get; set; }
	}

	internal enum MessageType
	{
		BeginNextWallpaper, EndNextWallpaper, ConfigChange, EndPostProcess
	}
}
