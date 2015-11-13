using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyShop
{
	public class Permissions
	{
		//public static Dictionary<string, string> Dict = new Dictionary<string, string>()
		//{
		//	{ "shop", Shop },
		//	{ "buy", Buy },
		//	{ "kitsbuy", KitsBuy },
		//	{ "help", Help },
		//	{ "reload", Reload },
		//	{ "search", Search }
		//};
		public static readonly Dictionary<string, string> Dict = new Dictionary<string, string>
		{
			["shop"] = Shop,
			["buy"] = Buy,
			["kitsbuy"] = KitsBuy,
			["help"] = Help,
			["reload"] = Reload,
			["search"] = Search
		};

		public static readonly string Shop = "cshop";

		public static readonly string Buy = "cshop.buy";

		public static readonly string KitsBuy = "cshop.buy.kits";

		public static readonly string Help = "cshop.help";

		public static readonly string Reload = "cshop.reload";

		public static readonly string Search = "cshop.search";
	}
}
