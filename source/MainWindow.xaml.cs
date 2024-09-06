// Default namespaces
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// Added namespaces
using System.Diagnostics;

namespace ACE
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Run_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Run clicked");
		}

		private void Run_Training_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Run_Model_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}