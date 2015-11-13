using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using CurrencyShop.DB;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace CurrencyShop
{
	[ApiVersion(1, 22)]
	public class CShop : TerrariaPlugin
	{
		public override string Author
		{
			get { return "Enerdy"; }
		}

		public override string Description
		{
			get { return "Expanding CurrencyBank's horizons with an item shop"; }
		}

		public override string Name
		{
			get { return "CurrencyShop"; }
		}

		public override Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}

		public static Config Config { get; set; }

		public static CShop Instance { get; private set; }

		public static IDbConnection Db { get; private set; }

		public static IShopManager Manager { get; private set; }

		public static List<SItem> Inventory
		{
			get { return Manager.GetItems(); }
		}

		internal static string Tag { get; } = TShock.Utils.ColorTag("CurrencyShop:", new Color(90, 138, 211));

		public CShop(Main game)
			: base(game)
		{
			//Instance = new CShop(game);
			Order = 2;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
			}
		}

		public override void Initialize()
		{
			Instance = this;
			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
		}

		void OnInitialize(EventArgs e)
		{
			#region Commands

			TShockAPI.Commands.ChatCommands.Add(new Command(Permissions.Dict.Values.ToList(), Commands.Shop, "shop", "cshop", "currencyshop")
			{
				HelpText = $"Syntax: {TShockAPI.Commands.Specifier}shop [-b|-h|-k|-s|-r] <text> [digit]"
			});

			#endregion

			#region Config

			string path = Path.Combine(TShock.SavePath, "CurrencyBank", "Shop.json");
			Config = Config.Read(path);

			if (Config.StorageType.Equals("json", StringComparison.OrdinalIgnoreCase))
			{
				Manager = new Config.JsonShopManager(Config.Items, Config.Kits);
			}

			#endregion

			#region DB (StorageType == "mysql/sqlite")
			else if (Config.StorageType.Equals("mysql", StringComparison.OrdinalIgnoreCase))
			{
				string[] host = Config.MySqlHost.Split(':');
				Db = new MySqlConnection()
				{
					ConnectionString = String.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
					host[0],
					host.Length == 1 ? "3306" : host[1],
					Config.MySqlDbName,
					Config.MySqlUsername,
					Config.MySqlPassword)
				};
				Manager = new ShopManager(Db);
			}
			else if (Config.StorageType.Equals("sqlite", StringComparison.OrdinalIgnoreCase))
			{
				Db = new SqliteConnection(String.Format("uri=file://{0},Version=3",
					Path.Combine(TShock.SavePath, "CurrencyBank", "Shop.sqlite")));
				Manager = new ShopManager(Db);
			}
			else
				TShock.Log.ConsoleError($"currencyshop: Invalid storage type \"{Config.StorageType}\". Check your config file.");
		}

		#endregion
	}
}
