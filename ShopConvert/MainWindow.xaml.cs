using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace ShopConvert
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OpenFileButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog()
			{
				Filter = "JSON Files (*.json)|*.json",
				Title = "Opening a JSON-encoded text file..."
			};
			if (dialog.ShowDialog() == true)
			{
				QueryBox.Text = App.WriteQueries(File.ReadAllText(dialog.FileName));
			}
		}

		private void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
		{
			if (!String.IsNullOrWhiteSpace(QueryBox.Text))
			{
				Clipboard.SetText(QueryBox.Text);
				new ClipboardPopup().ShowDialog();
			}
		}
	}
}
