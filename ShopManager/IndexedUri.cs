using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopManager
{
	public class IndexedUri : Uri
	{
		public int Index { get; set; }

		public IndexedUri(string uri, int index)
			: base(uri)
		{
			Index = index;
		}

		public IndexedUri(string uri, int index, UriKind kind)
			: base(uri, kind)
		{
			Index = index;
		}
	}
}
