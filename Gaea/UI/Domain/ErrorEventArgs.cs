using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaea.UI.Domain
{
	internal class ErrorEventArgs : EventArgs
	{
		public string Subject { get; set; }
		public string Message { get; set; }
	}
}
