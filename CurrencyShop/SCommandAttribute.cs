using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyShop
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	sealed class SCommandAttribute : Attribute
	{
		public string Name;
		public string[] Names;

		public SCommandAttribute(params string[] Names)
		{
			this.Name = Names[0];
			this.Names = Names;
		}

		public string[] Permissions { get; set; }

		public string[] HelpText { get; set; }

		public string[] Parameters { get; set; }

		public string[] Values { get; set; }
	}
}
