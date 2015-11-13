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

namespace ShopManager.Pages.Config
{
	/// <summary>
	/// Interaction logic for UseGiveItemSSC.xaml
	/// </summary>
	public partial class UseGiveItemSSC : UserControl
	{
		public UseGiveItemSSC()
		{
			InitializeComponent();
			App.Reader.Read += onConfigRead;
		}

		private void onConfigRead(ConfigReadEventArgs e)
		{
			if (!e.Handled && e.Config != null)
				checkGiveItem.IsChecked = e.Config.UseGiveItemSSC;
		}
	}
}
