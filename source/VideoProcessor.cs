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
	//public static async void ProcessAsync(string[] videoPaths, string folderSavePath)
	//{
	//	await Task.Run(() => Process(videoPaths, folderSavePath));
	//}

	public static void Process(string[] videoPaths, string folderSavePath)
	{
		// Start a stopwatch to measure time
		Debug.Print("Started processing videos.");
		Stopwatch stopwatch = new();
		stopwatch.Start();

		// Process the videos in threads
		List<Thread> threads = new();
		foreach (string videoPath in videoPaths)
		{
			Thread thread = new(() => Process(videoPath, folderSavePath));
			thread.Start();
			threads.Add(thread);
		}
		foreach (Thread thread in threads) thread.Join();

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
		using BackgroundSubtractorMOG2 backgroundSubtractor = new();
		backgroundSubtractor.History = 30;
		backgroundSubtractor.VarThreshold = 40;

		// Set up blob detection
		using SimpleBlobDetectorParams blobDetectorParams = new()
		{
			FilterByArea = true,
			FilterByCircularity = false,
			FilterByConvexity = false,
			FilterByInertia = false,
			MinArea = 50,
			MaxArea = 100_000
		};
		using SimpleBlobDetector blobDetector = new(blobDetectorParams);
		int blobs = 0;

		// Read frames from video capture
		using Mat sourceFrame = new();
		while (videoCapture.Read(sourceFrame))
		{
			// Resize frame
			using Mat downsizedFrame = new();
			CvInvoke.Resize(sourceFrame, downsizedFrame, processSize, interpolation: Inter.Linear);

			// Subtract the background
			using Mat workingFrame = new();
			backgroundSubtractor.Apply(downsizedFrame, workingFrame);

			// Remove noise
			CvInvoke.MedianBlur(workingFrame, workingFrame, 5);

			// Convert to binary video
			CvInvoke.Threshold(workingFrame, workingFrame, 127, 255, ThresholdType.BinaryInv);

			// Blob detection
			using VectorOfKeyPoint keyPoints = new VectorOfKeyPoint();
			blobDetector.DetectRaw(workingFrame, keyPoints);

			// Draw blobs
			foreach (MKeyPoint keyPoint in keyPoints.ToArray())
			{
				Point point = new Point((int)keyPoint.Point.X, (int)keyPoint.Point.Y);
				CvInvoke.Circle(downsizedFrame, point, (int)keyPoint.Size, new MCvScalar(0, 255, 0));
			}
			blobs += keyPoints.Size;

			// Resize again
			using Mat upsizedFrame = new();
			CvInvoke.Resize(downsizedFrame, upsizedFrame, size, interpolation: Inter.Cubic);

			// Write frame to video
			videoWriter.Write(upsizedFrame);
		}

		Debug.Print($"Detected {blobs} blobs.");
	}
}
