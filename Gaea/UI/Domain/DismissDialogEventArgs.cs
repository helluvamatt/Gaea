using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaea.UI.Domain
{
	internal class DismissDialogEventArgs : EventArgs
	{
		public bool Result { get; set; }
	}
}
