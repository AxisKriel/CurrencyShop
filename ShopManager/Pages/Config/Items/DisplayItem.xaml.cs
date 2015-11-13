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
using ShopManager.ID;
using CurrencyShop;
using FirstFloor.ModernUI.Windows.Controls;

namespace ShopManager.Pages.Config.Items
{
	/// <summary>
	/// Interaction logic for DisplayItem.xaml
	/// </summary>
	public partial class DisplayItem : UserControl
	{
		public static DisplayItem Instance { get; private set; }

		public DisplayItem()
		{
			InitializeComponent();
			ItemsPage.Instance.itemTab.SelectedSourceChanged += SelectedSourceChanged;
			Instance = this;
		}

		public void UpdateItem(SItem item)
		{
			Instance.UpdateSelectedItem(item);
		}

		private void SelectedSourceChanged(object sender, SourceEventArgs e)
		{
			UpdateSelectedItem(App.Config.Items[((IndexedUri)e.Source).Index]);
		}

		private void UpdateSelectedItem(SItem item)
		{
			this.itemSelector.SelectedIndex = ItemID.NetIdToName.Keys.ToList().IndexOf(item.netID);
			this.stackText.Text = item.netID.ToString();
			this.costText.Text = item.cost.ToString();
		}

		private void itemSelector_Loaded(object sender, RoutedEventArgs e)
		{
			itemSelector.ItemsSource = new List<string>(ItemID.NetIdToName.Select(p => p.Key + ": " + p.Value));
		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			int result;
			if (!int.TryParse(e.Text, out result))
				e.Handled = true;
		}
	}
}
