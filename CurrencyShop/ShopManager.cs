using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;
using Newtonsoft.Json;
using Terraria;

namespace CurrencyShop.DB
{
	public class ShopManager
	{
		private IDbConnection db;
		private List<SItem> items = new List<SItem>();
		private List<Kit> kits = new List<Kit>();

		public ShopManager(IDbConnection db)
		{
			this.db = db;

			var creator = new SqlTableCreator(db, CShop.Config.StorageType == "mysql"
				? new MysqlQueryCreator()
				: new MysqlQueryCreator());

			SqlTable itemTable = new SqlTable("Items",
				new SqlColumn("ID", MySqlDbType.Int32) { AutoIncrement = true, Primary = true },
				new SqlColumn("NetID", MySqlDbType.Int16),
				new SqlColumn("Stack", MySqlDbType.Int16),
				new SqlColumn("Prefix", MySqlDbType.Byte),
				new SqlColumn("Cost", MySqlDbType.Int64));

			SqlTable kitTable = new SqlTable("Kits",
				new SqlColumn("ID", MySqlDbType.Int32) { AutoIncrement = true, Primary = true },
				new SqlColumn("Name", MySqlDbType.Text),
				new SqlColumn("Cost", MySqlDbType.Int64),
				new SqlColumn("Items", MySqlDbType.Text));

			if (creator.EnsureTableStructure(itemTable))
				TShock.Log.ConsoleInfo("currencyshop: created table 'Items'");
			if (creator.EnsureTableStructure(kitTable))
				TShock.Log.ConsoleInfo("currencyshop: created table 'Kits'");

			Task.Run(() => ReloadAsync());
		}

		public Task ReloadAsync()
		{
			return Task.Run(() =>
				{
					items.Clear();
					using (QueryResult r = db.QueryReader("SELECT * FROM Items"))
					{
						while (r.Read())
							items.Add(new SItem(r.Get<int>("NetID"), r.Get<int>("Stack"), r.Get<int>("Cost"), r.Get<byte>("Prefix")));
					}
					kits.Clear();
					using (QueryResult r = db.QueryReader("SELECT * FROM Kits"))
					{
						while (r.Read())
							kits.Add(new Kit(r.Get<string>("Name"), r.Get<int>("Cost"), JsonConvert.DeserializeObject<SItem[]>(r.Get<string>("Items"))));
					}
				});
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
				List<Item> _items = TShock.Utils.GetItemByIdOrName(value);
				if (_items.Count < 0)
					throw new InvalidItemException(-1);
				var list = new List<SItem>();
				for (int i = 0; i < _items.Count; i++)
					if (items.Any(n => n.netID == _items[i].netID))
						list.Add(items.Find(n => n.netID == _items[i].netID));
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
			return Task.Run(() => kits.FindAll(k => k.name.StartsWith(value)));
		}

		/// <summary>
		/// Returns all kits containing an item with a matching netID.
		/// </summary>
		/// <param name="netID">The netID to look for.</param>
		/// <returns>The list of kits containing the item.</returns>
		public Task<List<Kit>> KitContains(int netID)
		{
			return Task.Run(() => kits.FindAll(k => k.items.Any(i => i.netID == netID)));
		}
	}
}
