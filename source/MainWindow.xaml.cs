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
using System.IO;

namespace ACE
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void SelectTrainingData_Click(object sender, RoutedEventArgs e)
		{
			// Initialize folder dialog
			Microsoft.Win32.OpenFolderDialog folderDialog = new();
			folderDialog.Multiselect = false;
			folderDialog.Title = "Select Training Data";

			// Check if selected or cancelled
			bool? selectedFolder = folderDialog.ShowDialog();
			if (selectedFolder == true)
			{
				// Load the selected folder
				string folderName = folderDialog.SafeFolderName;
				string folderPath = folderDialog.FolderName;
				LoadVideos(folderName, folderPath);
			}
		}

		private void SelectModelData_Click(object sender, RoutedEventArgs e)
		{

		}

		private void SelectDebrisVideos_Click(object sender, RoutedEventArgs e)
		{
			// Initialize folder dialog
			Microsoft.Win32.OpenFolderDialog folderDialog = new();
			folderDialog.Multiselect = false;
			folderDialog.Title = "Select Debris Videos";

			// Check if selected or cancelled
			bool? selectedFolder = folderDialog.ShowDialog();
			if (selectedFolder == true)
			{
				// Load the selected folder
				string folderName = folderDialog.SafeFolderName;
				string folderPath = folderDialog.FolderName;
				LoadVideos(folderName, folderPath);
			}
		}

		private void TrainModel_Click(object sender, RoutedEventArgs e)
		{

		}

		private void RunModel_Click(object sender, RoutedEventArgs e)
		{

		}

		private void LoadVideos(string folderName, string folderPath)
		{
			string[] filePaths = Directory.GetFiles(folderPath, "*.mp4", SearchOption.AllDirectories);
			Debug.Print($"Loading {filePaths.Length} videos at {folderPath}");

		}
	}
}