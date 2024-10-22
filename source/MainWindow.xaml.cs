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
		private string[] videoPaths = Array.Empty<string>();

		public MainWindow()
		{
			// Initialize
			InitializeComponent();

			// Clear videos container of example previews
			VideosContainer.Children.Clear();

			// Assign click events
			SelectTrainingDataItem.Click += SelectTrainingData;
			SelectDebrisVideosItem.Click += SelectDebrisVideos;
		}

		private void SelectTrainingData(object sender, RoutedEventArgs e)
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

		private void SelectDebrisVideos(object sender, RoutedEventArgs e)
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

		private void LoadVideos(string folderName, string folderPath)
		{
			// Get videos
			videoPaths = Directory.GetFiles(folderPath, "*.mp4", SearchOption.AllDirectories);
			Debug.Print($"Loading {videoPaths.Length} videos at {folderPath}");

			// Load previews
			VideosContainer.Children.Clear();
			foreach (string filePath in videoPaths)
			{
				// Get file info
				string fileName = Path.GetFileName(filePath);

				// Add video preview to container
				VideoContainer videoContainer = new();
				videoContainer.Label.Content = fileName;
				VideosContainer.Children.Add(videoContainer);
				videoContainer.Button.Click += (object sender, RoutedEventArgs e) => ClickVideo(VideosContainer.Children.IndexOf(videoContainer));

				// Add thumbnail
				videoContainer.Preview.Source = new Uri(filePath, UriKind.Absolute);
				videoContainer.Preview.Play();
				videoContainer.Preview.MediaOpened += (object sender, RoutedEventArgs e) => videoContainer.Preview.Pause();
			}
		}

		private void ClickVideo(int pathIndex)
		{
			string videoPath = videoPaths[pathIndex];
			Debug.Print($"Selecting video at path {videoPath}");
			VideoPlayer.Source = new Uri(videoPath, UriKind.Absolute);
		}
	}
}