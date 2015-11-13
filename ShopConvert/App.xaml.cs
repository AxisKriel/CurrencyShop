using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace ShopConvert
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static string WriteQueries(string json)
		{
			var sb = new StringBuilder();
			ConfigFile config = JsonConvert.DeserializeObject<ConfigFile>(json);

			// Items
			if (config.Items != null)
			{
				foreach (SItem i in config.Items)
				{
					if (sb.Length > 0)
						sb.Append("\n");
					sb.AppendFormat("INSERT INTO Items (NetID, Stack, Prefix, Cost) VALUES ('{0}', '{1}', '{2}', '{3}');", i.netID, i.stack, i.prefix, i.cost);
				}
			}

			// Kits
			if (config.Kits != null)
			{
				foreach (Kit k in config.Kits)
				{
					if (sb.Length > 0)
						sb.Append("\n");
					sb.AppendFormat("INSERT INTO Kits (Name, Cost, Items) VALUES ('{0}', '{1}', '{2}');", k.name, k.cost, JsonConvert.SerializeObject(k.items));
				}
			}

			return sb.ToString();
		}
	}
}
