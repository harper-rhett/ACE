// Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Features2D;
using Emgu.CV.Linemod;
using Emgu.CV.Util;
using System.Diagnostics;
using Emgu.CV.VideoStab;

// Current namespace
namespace ACE;

static class VideoProcessor
{
	public static void Process(string[] videoPaths, string folderSavePath)
	{
		// Start a stopwatch to measure time
		Debug.Print("Started processing videos.");
		Stopwatch stopwatch = new();
		stopwatch.Start();

		foreach (string videoPath in videoPaths)
		{
			Process(videoPath, folderSavePath);
		}

		//List<Thread> threads = new();
		//foreach (string videoPath in videoPaths)
		//{
		//	Thread thread = new(() => Process(videoPath, folderSavePath));
		//	thread.Start();
		//	threads.Add(thread);
		//}

		//foreach (Thread thread in threads) thread.Join();

		// Stop stopwatch and print results
		stopwatch.Stop();
		Debug.Print($"Completed processing videos in {stopwatch.Elapsed.TotalSeconds} seconds.");
	}

	public static void Process(string videoPath, string folderSavePath)
	{
		// Create video capture with memory safety
		using VideoCapture videoCapture = new(videoPath);

		// Get properties of video
		int fps = (int)videoCapture.Get(CapProp.Fps);
		int frameCount = (int)videoCapture.Get(CapProp.FrameCount);
		Size size = new(videoCapture.Width, videoCapture.Height);
		Size processSize = size / 4;

		// Determine new path for video output
		string videoName = Path.GetFileNameWithoutExtension(videoPath) + "_out" + ".mp4";
		string videoSavePath = Path.Combine(folderSavePath, videoName);

		// Create video writer with memory safety
		using VideoWriter videoWriter = new(videoSavePath, VideoWriter.Fourcc('H', '2', '6', '4'), fps, size, true);
		
		// Set up background subtraction
		BackgroundSubtractorMOG2 backgroundSubtractor = new();
		backgroundSubtractor.History = 30;
		backgroundSubtractor.VarThreshold = 40;

		// Set up blob detection
		SimpleBlobDetectorParams blobDetectorParams = new()
		{
			FilterByArea = true,
			FilterByCircularity = false,
			FilterByConvexity = false,
			FilterByInertia = false,
			MinArea = 1,
			MaxArea = 100_000
		};
		SimpleBlobDetector blobDetector = new(blobDetectorParams);
		int blobs = 0;

		// Read frames from video capture
		Mat[] frames = new Mat[frameCount];
		Mat sourceFrame = new();
		while (videoCapture.Read(sourceFrame))
		{
			// Resize frame
			Mat downsizedFrame = new();
			CvInvoke.Resize(sourceFrame, downsizedFrame, processSize, interpolation: Inter.Linear);

			// Subtract the background
			Mat workingFrame = new();
			backgroundSubtractor.Apply(downsizedFrame, workingFrame);

			// Remove noise
			CvInvoke.MedianBlur(workingFrame, workingFrame, 5);

			// Convert to binary video
			CvInvoke.Threshold(workingFrame, workingFrame, 127, 255, ThresholdType.Binary);

			// Blob detection
			VectorOfKeyPoint keypoints = new VectorOfKeyPoint();
			blobDetector.Detect(workingFrame, keypoints);

			// Draw blobs
			Features2DToolbox.DrawKeypoints(workingFrame, keypoints, workingFrame, new Bgr(0, 255, 0));
			blobs += keypoints.Size;
			
			// Resize again
			Mat upsizedFrame = new();
			CvInvoke.Resize(workingFrame, upsizedFrame, size, interpolation: Inter.Cubic);

			// Write frame to video
			videoWriter.Write(upsizedFrame);
		}

		Debug.Print($"Detected {blobs} blobs.");
	}
}
