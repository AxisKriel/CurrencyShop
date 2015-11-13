using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CurrencyShop;
using FirstFloor.ModernUI.Presentation;
using ShopManager.Pages.Config.Items;

namespace ShopManager.Pages.Config
{
	/// <summary>
	/// Interaction logic for Items.xaml
	/// </summary>
	public partial class ItemsPage : UserControl
	{
		public static ItemsPage Instance { get; private set; }

		public List<SItem> Items
		{
			get { return App.Config.Items; }
		}

		public List<string> ItemNames
		{
			get { return ShopManager.ID.ItemID.NetIdToName.Values.ToList(); }
		}

		public ItemsPage()
		{
			InitializeComponent();
			this.itemGridNameColumn.ItemsSource = ShopManager.ID.ItemID.NetIdToName.Values;
			Instance = this;
		}

		private void onLoad(object sender, RoutedEventArgs e)
		{
			onConfigRead(new ConfigReadEventArgs("", App.Config));
			App.Reader.Read += onConfigRead;
		}

		private void onUnload(object sender, RoutedEventArgs e)
		{
			App.Reader.Read -= onConfigRead;
			var x = ShopManager.ID.PrefixID[1];
		}

		//private Uri MakeSource(SItem item)
		//{
		//	return new Uri("Placeholder");
		//}

		private void onConfigRead(ConfigReadEventArgs e)
		{
			if (!e.Handled && e.Config != null && e.Config.Items != null)
			{
				this.itemGrid.ItemsSource = e.Config.Items;
				//for (int i = 0; i < e.Config.Items.Count; i++)
				//{
				//	Instance.itemTab.Links.Add(new Link()
				//	{
				//		DisplayName = App.FormatItemName(e.Config.Items[i], true),
				//		Source = new IndexedUri("/Pages/Config/Items/DisplayItem.xaml", i, UriKind.Relative)
				//	});
				//}
				//if (Instance.itemTab.Links.Count > 0)
				//{
				//	Instance.itemTab.SelectedSource = Instance.itemTab.Links[0].Source;
				//	DisplayItem.Instance.UpdateItem(e.Config.Items[0]);
				//}
			}
					//e.Config.Items.ForEach(i => itemTab.Links.Add(new Link()
					//{
					//	DisplayName = App.FormatItemName(i, true),
					//	Source = new Uri("/Pages/Config/Items/DisplayItem.xaml")
					//}));
		}
	}
}
