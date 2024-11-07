using System;
using System.Collections.Generic;
using System.IO;
// Default namespaces
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// Added namespaces
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing;

// Current namespace
namespace ACE;

static class VideoProcessor
{
	public static void Process(string[] videoPaths, string folderSavePath)
	{
		foreach (string videoPath in videoPaths)
			Process(videoPath, folderSavePath);
	}

	public static void Process(string videoPath, string folderSavePath)
	{
		// Create video capture with memory safety
		using VideoCapture videoCapture = new(videoPath);

		// Get properties of video
		int fps = (int)videoCapture.Get(CapProp.Fps);
		int frameCount = (int)videoCapture.Get(CapProp.FrameCount);
		Size size = new(videoCapture.Width, videoCapture.Height);

		// Determine new path for video output
		string videoName = Path.GetFileNameWithoutExtension(videoPath) + "_out" + ".mp4";
		string videoSavePath = Path.Combine(folderSavePath, videoName);

		// Create video writer with memory safety
		using VideoWriter videoWriter = new(videoSavePath, VideoWriter.Fourcc('H', '2', '6', '4'), fps, size, true);

		// Read frames from video capture
		Mat[] frames = new Mat[frameCount];
		Mat currentFrame = new();
		while (videoCapture.Read(currentFrame))
		{
			// Convert frame to grayscale
			Mat grayFrame = new Mat();
			CvInvoke.CvtColor(currentFrame, grayFrame, ColorConversion.Bgr2Gray);
			videoWriter.Write(grayFrame);
		}
	}
}
