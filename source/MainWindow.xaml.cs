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
			VideosContainer.Children.Clear();
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
			// Get videos
			string[] filePaths = Directory.GetFiles(folderPath, "*.mp4", SearchOption.AllDirectories);
			Debug.Print($"Loading {filePaths.Length} videos at {folderPath}");

			// Load previews
			VideosContainer.Children.Clear();
			foreach (string filePath in filePaths)
			{
				// Get file info
				string fileName = Path.GetFileName(filePath);

				// Add video preview to container
				VideoContainer videoContainer = new();
				videoContainer.Label.Content = fileName;
				VideosContainer.Children.Add(videoContainer);
			}
		}
	}
}