using FirstFloor.ModernUI.Presentation;
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
using System.Reflection;

namespace ShopManager.Pages
{
	/// <summary>
	/// Interaction logic for ConfigPage.xaml
	/// </summary>
	public partial class ConfigPage : UserControl
	{
		public ConfigPage()
		{
			InitializeComponent();
			if (App.Config != null)
			{
				//optionsPanel.Links.Clear();
				//optionsPanel.Links.Add(new Link() { DisplayName = "UseGiveItemSSC" });
				//optionsPanel.Links.Add(new Link() { DisplayName = "Items" });
				//optionsPanel.Links.Add(new Link() { DisplayName = "Kits" });
			}
		}
	}
}
