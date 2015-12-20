using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace CurrencyShop.DB
{
	public class ShopManager : IShopManager
	{
		private IDbConnection db;
		private List<SItem> inventory = new List<SItem>();
		private List<Kit> kits = new List<Kit>();

		public ShopManager(IDbConnection db)
		{
			this.db = db;

			var creator = new SqlTableCreator(db, CShop.Config.StorageType == "mysql"
				? new MysqlQueryCreator()
				: new MysqlQueryCreator());

			SqlTable itemTable = new SqlTable("Items",
				new SqlColumn("ID", MySqlDbType.Int32) { AutoIncrement = true, Primary = true },
				new SqlColumn("NetID", MySqlDbType.Int32),
				new SqlColumn("Stack", MySqlDbType.Int32),
				new SqlColumn("Prefix", MySqlDbType.Int32),
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

			Task.Run(async () =>
			{
				int count = await ReloadAsync();
				TShock.Log.ConsoleInfo($"currencyshop: loaded {count} items.");
			});
		}

		public async Task<int> ReloadAsync()
		{
			return await Task.Run(() =>
				{
					int count = 0;
					inventory.Clear();
					using (QueryResult r = db.QueryReader("SELECT * FROM Items"))
					{
						while (r.Read())
						{
							try
							{
								inventory.Add(new SItem(r.Get<int>("NetID"), r.Get<int>("Stack"), r.Get<int>("Cost"), (byte)r.Get<int>("Prefix")));
								count++;
							}
							catch (InvalidItemException e)
							{
								TShock.Log.ConsoleError($"currencyshop: {e.Message}");
							}
							catch (InvalidPrefixException e)
							{
								TShock.Log.ConsoleError($"currencyshop: {e.Message}");
							}
							catch (Exception e)
							{
								TShock.Log.Error($"currencyshop: {e.Message} (RowID: {r.Get<int>("ID")})");
							}
						}
					}
					kits.Clear();
					using (QueryResult r = db.QueryReader("SELECT * FROM Kits"))
					{
						while (r.Read())
						{
							try
							{
								kits.Add(new Kit(r.Get<string>("Name"), r.Get<int>("Cost"), JsonConvert.DeserializeObject<SItem[]>(r.Get<string>("Items"))));
							}
							catch (InvalidItemException e)
							{
								TShock.Log.ConsoleError($"currencyshop: {e.Message}");
							}
							catch (InvalidPrefixException e)
							{
								TShock.Log.ConsoleError($"currencyshop: {e.Message}");
							}
							catch (Exception e)
							{
								TShock.Log.Error($"currencyshop: {e.Message} (RowID: {r.Get<int>("ID")})");
							}
						}
					}
					return count;
				});
		}

		/// <summary>
		/// Returns the full item inventory.
		/// </summary>
		/// <returns>The list of items on sale.</returns>
		public List<SItem> GetItems()
		{
			return inventory;
		}

		/// <summary>
		/// Returns the full list of kits.
		/// </summary>
		/// <returns>The list of kits on sale.</returns>
		public List<Kit> GetKits()
		{
			return kits;
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
					if (inventory.Any(n => n.netID == items[i].netID))
						list.Add(inventory.Find(n => n.netID == items[i].netID));
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

		/// <summary>
		/// Reloads the manager to take into account outside changes.
		/// </summary>
		public void Reload()
		{
			Task.Run(() => ReloadAsync());
		}
	}
}
