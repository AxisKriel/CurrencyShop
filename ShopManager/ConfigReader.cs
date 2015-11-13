using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CurrencyShop;
using System.IO;

namespace ShopManager
{
	public class ConfigReader
	{
		public delegate void ConfigReadHandler(ConfigReadEventArgs e);

		public event ConfigReadHandler Read;

		public Config Config { get; private set; }

		public ConfigReader()
		{

		}

		public bool ReadFile(string path)
		{
			try
			{
				if (Path.GetExtension(path) != ".json")
					return false;
				Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
				if (Read != null)
					Read(new ConfigReadEventArgs(path, Config));
				return true;
			}
			catch (Exception ex)
			{
				throw new JsonReaderException(ex.Message, ex);
				return false;
			}
		}
	}

	public class ConfigReadEventArgs
	{
		public Config Config { get; set; }

		public bool Handled { get; set; }

		public string Path { get; set; }

		public ConfigReadEventArgs(string path, Config config)
		{
			Config = config;
			Path = path;
		}
	}
}
