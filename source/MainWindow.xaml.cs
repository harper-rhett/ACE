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
			SelectInputFolderItem.Click += SelectInputFolder;
			RunItem.Click += Run;
		}

		private void SelectInputFolder(object sender, RoutedEventArgs e)
		{
			// Initialize folder dialog
			Microsoft.Win32.OpenFolderDialog folderDialog = new();
			folderDialog.Multiselect = false;
			folderDialog.Title = "Select Debris Video Folder";
			bool? selectedFolder = folderDialog.ShowDialog();

			// Check if selected or cancelled
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
				videoContainer.Button.Click += (object sender, RoutedEventArgs e) => SelectVideo(VideosContainer.Children.IndexOf(videoContainer));

				// Add thumbnail
				videoContainer.Preview.Source = new Uri(filePath, UriKind.Absolute);
				videoContainer.Preview.Play();
				videoContainer.Preview.MediaOpened += (object sender, RoutedEventArgs e) => videoContainer.Preview.Pause();
			}
		}

		private void SelectVideo(int pathIndex)
		{
			// Load playback
			string videoPath = videoPaths[pathIndex];
			Debug.Print($"Selecting video at path {videoPath}");
			VideoPlayer.Source = new Uri(videoPath, UriKind.Absolute);

			// Load video properties
			FileInfo fileInfo = new FileInfo(videoPath);
			PropertiesTitle.Content = $"{fileInfo.Name} Properties";
			PropertiesText.Text = $"Path: {fileInfo.FullName}\n"
				+ $"Creation Date: {fileInfo.CreationTime}\n"
				+ $"Modification Date: {fileInfo.LastWriteTime}\n"
				+ $"Size: ~{fileInfo.Length / 1_000_000} megabytes";
		}

		private void Run(object sender, RoutedEventArgs e)
		{
			// Initialize folder dialog
			Microsoft.Win32.OpenFolderDialog folderDialog = new();
			folderDialog.Multiselect = false;
			folderDialog.Title = "Select Output Folder";
			bool? selectedFolder = folderDialog.ShowDialog();

			// Check if selected or cancelled
			string folderPath;
			if (selectedFolder == true) folderPath = folderDialog.FolderName;
			else return;

			// Process videos
			VideoProcessor videoProcessor = new(videoPaths);
			videoProcessor.Process(folderPath);

			// YO
			// Input and output folder locations should be saved
			// Right now the dialog uses the last location
			// So the user might accidentally save their output
			// to their input folder
		}
	}
}