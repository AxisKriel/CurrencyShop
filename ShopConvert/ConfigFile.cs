using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopConvert
{
	public class ConfigFile
	{
		public bool UseGiveItemSSC { get; set; }

		public List<SItem> Items { get; set; }

		public List<Kit> Kits { get; set; }

		public string StorageType { get; set; }

		public string MySqlHost { get; set; }

		public string MySqlDbName { get; set; }

		public string MySqlUsername { get; set; }

		public string MySqlPassword { get; set; }
	}

	public class SItem
	{
		public int netID { get; set; }
		public int stack { get; set; }
		public byte prefix { get; set; }
		public int cost { get; set; }
	}

	public class Kit
	{
		public string name { get; set; }
		public int cost { get; set; }
		public List<SItem> items { get; set; }
	}
}
