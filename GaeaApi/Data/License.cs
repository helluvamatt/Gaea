using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaea.Api.Data
{
	/// <summary>
	/// Represents a license that is applied to an image
	/// </summary>
	public class License
	{
		/// <summary>
		/// URL to the web page for the license
		/// </summary>
		public string URL { get; set; }
		
		/// <summary>
		/// Name of the license
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Description or full text of the license
		/// </summary>
		public string Description { get; set; }
	}
}
