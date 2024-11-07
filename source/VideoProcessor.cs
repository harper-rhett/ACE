using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing;
using System.Windows.Controls;
using System.Diagnostics;

namespace ACE
{
	class VideoProcessor
	{
		private string[] videoPaths;

		public VideoProcessor(string[] videoPaths)
		{
			this.videoPaths = videoPaths;
		}

		public void ProcessVideos(string folderSavePath)
		{
			foreach (string videoPath in videoPaths)
				ProcessVideo(videoPath, folderSavePath);
		}

		public void ProcessVideo(string videoPath, string folderSavePath)
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
}
