using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Terraria;
using TShockAPI;

namespace CurrencyShop
{
	public class Config
	{
		[Description("This will place purchased items directly into a player's inventory instead of dropping them. Requires SSC.")]
		public bool UseGiveItemSSC = false;

		[Description("The list of items to be sold separately.")]
		public List<SItem> Items = new List<SItem>();

		[Description("The list of item kits to be sold.")]
		public List<Kit> Kits = new List<Kit>();

		[Description("The storage type for storing the shop inventory. Can be either `json`, `sqlite` or `mysql`.")]
		public string StorageType = "json";

		[Description("The IP address / hostname of the MySQL database and the listening port, separated by `:`. In case a port" +
			" isn't provided, the default port of `3306` will be used.")]
		public string MySqlHost = "";

		[Description("The MySQL database name, in case StorageType is set to `mysql`.")]
		public string MySqlDbName = "";

		[Description("The username of a MySQL user with read/write access to the database.")]
		public string MySqlUsername = "";

		[Description("The password for the MySQL user whose name was entered in MySqlUsername.")]
		public string MySqlPassword = "";

		public static Config Read(string path)
		{
			try
			{
				Config config;
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				if (!File.Exists(path))
				{
					File.WriteAllText(path, JsonConvert.SerializeObject((config = new Config()), Formatting.Indented));
					return config;
				}
				return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError(ex.ToString());
				return new Config();
			}
		}

		public class JsonShopManager : IShopManager
		{
			private List<SItem> _items;
			private List<Kit> _kits;

			/// <summary>
			/// Creates a new instance of the Json Shop Manager with the given items and kits.
			/// </summary>
			/// <param name="items">The list of items to be on sale.</param>
			/// <param name="kits">The list of kits to be on sale.</param>
			public JsonShopManager(List<SItem> items, List<Kit> kits)
			{
				_items = items;
				_kits = kits;
			}

			/// <summary>
			/// Returns the full item inventory.
			/// </summary>
			/// <returns>The list of items on sale.</returns>
			public List<SItem> GetItems()
			{
				return _items;
			}

			/// <summary>
			/// Returns the full list of kits.
			/// </summary>
			/// <returns>The list of kits on sale.</returns>
			public List<Kit> GetKits()
			{
				return _kits;
			}

			/// <summary>
			/// Returns all items which name starts with the input value.
			/// </summary>
			/// <param name="value">The input value to match.</param>
			/// <returns>The list of items which name starts with value.</returns>
			public Task<List<SItem>> GetMatchingItems(string value)
			{
				return Task.Run(() =>
				{
					List<Item> items = TShock.Utils.GetItemByIdOrName(value);
					if (items.Count < 0)
						throw new InvalidItemException(-1);
					var list = new List<SItem>();
					for (int i = 0; i < items.Count; i++)
						if (_items.Any(n => n.netID == items[i].netID))
							list.Add(_items.Find(n => n.netID == items[i].netID));
					return list;
				});
			}

			/// <summary>
			/// Returns all kits which name starts with the input value.
			/// </summary>
			/// <param name="value">The input value to match.</param>
			/// <returns>The list of kits which name starts with value.</returns>
			public Task<List<Kit>> GetMatchingKits(string value)
			{
				return Task.Run(() => _kits.FindAll(k => k.name.StartsWith(value)));
			}

			/// <summary>
			/// Returns all kits containing an item with a matching netID.
			/// </summary>
			/// <param name="netID">The netID to look for.</param>
			/// <returns>The list of kits containing the item.</returns>
			public Task<List<Kit>> KitContains(int netID)
			{
				return Task.Run(() => _kits.FindAll(k => k.items.Any(i => i.netID == netID)));
			}

			/// <summary>
			/// Reloads the manager to take into account outside changes.
			/// </summary>
			public void Reload()
			{
				_items = CShop.Config.Items;
				_kits = CShop.Config.Kits;
			}
		}
	}

	public class Kit
	{
		public string name;
		public int cost;
		public List<SItem> items;

		public Kit()
		{

		}

		public Kit(string name, int cost, params SItem[] items)
		{
			this.name = name;
			this.cost = cost;
			this.items = new List<SItem>(items);
		}

		public override string ToString()
		{
			return name;
		}
	}

	[Serializable]
	public class ShopConfigException : Exception
	{
		public ShopConfigException()
		{

		}

		public ShopConfigException(string message)
			: base(message)
		{

		}

		public ShopConfigException(string message, Exception inner)
			: base(message, inner)
		{

		}
	}

	[Serializable]
	public class InvalidItemException : ShopConfigException
	{
		public InvalidItemException(int netID)
			: base("Item '" + netID + "' doesn't exist.")
		{

		}
	}

	[Serializable]
	public class InvalidPrefixException : ShopConfigException
	{
		public InvalidPrefixException(int prefix)
			: base("Prefix '" + prefix + "' doesn't exist.")
		{

		}
	}
}
