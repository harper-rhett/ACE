// Namespaces
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

// Current namespace
namespace ACE;
using ACE.Properties;
using System.Security.Policy;

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
		LoadInputVideosItem.Click += (object sender, RoutedEventArgs e) => SelectInputFolder();
		LoadLastInputVideosItem.Click += (object sender, RoutedEventArgs e) => SelectLastInputFolder();
		ToNewOutputFolderItem.Click += (object sender, RoutedEventArgs e) => SelectOutputFolder();
		ToLastOutputFolderItem.Click += (object sender, RoutedEventArgs e) => SelectLastOutputFolder();
		HelpItem.Click += (object sender, RoutedEventArgs e) => Help();
	}

	private void SelectInputFolder()
	{
		// Initialize folder dialog
		Microsoft.Win32.OpenFolderDialog folderDialog = new();
		folderDialog.Multiselect = false;
		folderDialog.Title = "Select Debris Video Folder";
		string? parentDirectory = Path.GetDirectoryName(Settings.Default.InputPath);
		folderDialog.InitialDirectory = parentDirectory;
		folderDialog.FolderName = Settings.Default.InputPath;
		bool? selectedFolder = folderDialog.ShowDialog();

		// Check if selected or cancelled
		if (selectedFolder == true)
		{
			// Load the selected folder
			string folderPath = folderDialog.FolderName;
			LoadVideos(folderPath);
			Settings.Default.InputPath = folderPath;
			Settings.Default.Save();
		}
	}

	private void SelectLastInputFolder()
	{
		// Check if exists
		string folderPath = Settings.Default.InputPath;
		bool hasPath = folderPath.Length > 3;
		if (!hasPath) return;
		LoadVideos(folderPath);
	}

	private void LoadVideos(string folderPath)
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

	private void SelectOutputFolder()
	{
		// Check if input has been loaded
		if (videoPaths.Length == 0)
		{
			string warningText = "No input videos loaded.";
			MessageBox.Show(warningText, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		// Initialize folder dialog
		Microsoft.Win32.OpenFolderDialog folderDialog = new();
		folderDialog.Multiselect = false;
		folderDialog.Title = "Select Output Folder";
		string? parentDirectory = Path.GetDirectoryName(Settings.Default.OutputPath);
		folderDialog.InitialDirectory = parentDirectory;
		folderDialog.FolderName = Settings.Default.OutputPath;
		bool? selectedFolder = folderDialog.ShowDialog();

		// Check if selected or cancelled
		string folderSavePath;
		if (selectedFolder == true)
		{
			folderSavePath = folderDialog.FolderName;
			Settings.Default.OutputPath = folderSavePath;
			Settings.Default.Save();
		}
		else return;

		// Process videos
		VideoProcessor.Process(videoPaths, folderSavePath);

		// Load videos
		LoadVideos(folderSavePath);
	}

	private void SelectLastOutputFolder()
	{
		// Check if input has been loaded
		if (videoPaths.Length == 0)
		{
			string warningText = "No input videos loaded.";
			MessageBox.Show(warningText, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		// Check if exists
		string folderPath = Settings.Default.OutputPath;
		bool hasPath = folderPath.Length > 3;
		if (!hasPath) return;

		// Process videos
		VideoProcessor.Process(videoPaths, folderPath);

		// Load videos
		LoadVideos(folderPath);
	}

	private void Help()
	{
		Process.Start(new ProcessStartInfo { FileName = "https://github.com/harper-rhett/ACE/wiki/User-Guide", UseShellExecute = true });
	}
}