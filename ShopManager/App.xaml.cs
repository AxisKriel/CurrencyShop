using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CurrencyShop;
using ShopManager.ID;
using ShopManager.Pages;
using ShopManager.Pages.Config;
using ShopManager.Pages.Config.Items;

namespace ShopManager
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{

		}

		public static Config Config
		{
			get { return Reader.Config ?? new Config(); }
		}

		public static ConfigReader Reader = new ConfigReader();

		public static string FormatItemName(SItem item, bool displayName = false)
		{
			var sb = new StringBuilder();
			sb.Append(item.netID);
			if (displayName && ItemID.NetIdToName.ContainsKey(item.netID))
				// This won't work, needs an alternative design
				sb.Append(": ").Append(ItemID.NetIdToName[item.netID]);
			return sb.ToString();
		}

		public static List<FieldInfo> GetConstants(Type type)
		{
			FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
				 BindingFlags.Static | BindingFlags.FlattenHierarchy);

			return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
		}
	}
}
