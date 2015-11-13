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
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Win32;
using ShopManager;
using ShopManager.ID;

namespace ShopManager.Pages
{
	/// <summary>
	/// Interaction logic for Home.xaml
	/// </summary>
	public partial class Home : UserControl
	{
		public Home()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog()
			{
				Filter = "JSON Files (*.json)|*.json"
			};
			if (dialog.ShowDialog() == true)
			{
				if (!App.Reader.ReadFile(dialog.FileName))
				{
					ModernDialog errorDialog = new ModernDialog();
					errorDialog.Title = "Error";
					errorDialog.Content = "The given file has an invalid format. Only .JSON files are accepted.";
					errorDialog.ShowDialog();
				}
				else
				{
					MainWindow.Instance.editingLink.DisplayName = dialog.FileName;
					openButton.Command = NavigationCommands.GoToPage;
					openButton.CommandParameter = "/Pages/ConfigPage.xaml";
				}
			}
		}

		private void Debug_NetIdToName(object sender, RoutedEventArgs e)
		{
			ModernDialog dialog = new ModernDialog() { Title = "DEBUG" };
			var dict = ItemID.NetIdToName;
			dialog.Content = "NetIdToName count = " + dict.Count + "\n" + string.Join("\n", dict.Take(5).Select(s => s.Value));
			dialog.ShowDialog();
		}
	}
}
