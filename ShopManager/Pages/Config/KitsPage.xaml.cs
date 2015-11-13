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

namespace ShopManager.Pages.Config
{
	/// <summary>
	/// Interaction logic for Kits.xaml
	/// </summary>
	public partial class KitsPage : UserControl
	{
		public KitsPage()
		{
			InitializeComponent();
			App.Reader.Read += onConfigRead;
		}

		private void onConfigRead(ConfigReadEventArgs e)
		{
			if (!e.Handled && e.Config != null && e.Config.Kits != null)
				e.Config.Kits.ForEach(k => kitsTab.Items.Add(k.name));
		}
	}
}
