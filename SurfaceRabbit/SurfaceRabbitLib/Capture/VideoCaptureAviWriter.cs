	using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SurfaceRabbit.Capture
{

	/// <summary>Create AVI files from bitmaps</summary>
	public class VideoCaptureAviWriter {

		private int aviFile = 0;
		private IntPtr aviStream = IntPtr.Zero;
		private UInt32	frameRate = 0;
		private int countFrames = 0;
		private int width = 0;
		private int height = 0;
		private UInt32 stride = 0;
		private UInt32 fccType = Avi.StreamtypeVIDEO; // vids
		private UInt32 fccHandler = 1668707181; //"Microsoft Video 1" - Use CVID for default codec: (UInt32)Avi.mmioStringToFOURCC("CVID", 0);

    public bool Started { get; private set; }

		/// <summary>Creates a new AVI file</summary>
		/// <param name="fileName">Name of the new AVI file</param>
		/// <param name="frameRate">Frames per second</param>
		/// <param name="width">Width</param><param name="height">Height</param>
		public void Open(string fileName, UInt32 frameRate) {
			this.frameRate = frameRate;

			Avi.AVIFileInit();
			
			int hr = Avi.AVIFileOpen(
				ref aviFile, fileName, 
				4097 /* OF_WRITE | OF_CREATE (winbase.h) */, 0);
			if (hr != 0)
				throw new Exception("Error in AVIFileOpen: "+hr.ToString());

      Started = true;
		}

		/// <summary>Adds a new frame to the AVI stream</summary>
		/// <param name="bmp">The image to add</param>
		public void AddFrame(Bitmap bmp) {

			bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

			BitmapData bmpDat = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height),
				ImageLockMode.ReadOnly,PixelFormat.Format24bppRgb);

			if (countFrames == 0) {
				//this is the first frame - get size and create a new stream
				this.stride = (UInt32)bmpDat.Stride;
				this.width = bmp.Width;
				this.height = bmp.Height;
				CreateStream();
			}

			int result = Avi.AVIStreamWrite(aviStream,
				countFrames, 1, 
				bmpDat.Scan0, //pointer to the beginning of the image data
				(Int32) (stride  * height), 
				0, 0, 0); 

			if (result != 0) {
				throw new Exception("Error in AVIStreamWrite: "+result.ToString());
			}

			bmp.UnlockBits(bmpDat);
			countFrames ++;
		}

		/// <summary>Closes stream, file and AVI library</summary>
		public void Close() {
			if(aviStream != IntPtr.Zero){
				Avi.AVIStreamRelease(aviStream);
				aviStream = IntPtr.Zero;
			}
			if(aviFile != 0){
				Avi.AVIFileRelease(aviFile);
				aviFile = 0;
			}
			Avi.AVIFileExit();
      Started = false;
		}

		/// <summary>Creates a new video stream in the AVI file</summary>
		private void CreateStream() {
			Avi.AVISTREAMINFO strhdr = new Avi.AVISTREAMINFO();
			strhdr.fccType = fccType;
			strhdr.fccHandler = fccHandler;
			strhdr.dwScale = 1;
			strhdr.dwRate = frameRate;
			strhdr.dwSuggestedBufferSize = (UInt32)(height * stride);
			strhdr.dwQuality             = 10000; //highest quality! Compression destroys the hidden message
			strhdr.rcFrame.bottom        = (UInt32)height;
			strhdr.rcFrame.right         = (UInt32)width;
			strhdr.szName				 = new UInt16[64];
			
			int result = Avi.AVIFileCreateStream(aviFile, out aviStream, ref strhdr);
			if(result != 0){ throw new Exception("Error in AVIFileCreateStream: "+result.ToString()); }

			//define the image format

			Avi.BITMAPINFOHEADER bi = new Avi.BITMAPINFOHEADER();
			bi.biSize      = (UInt32)Marshal.SizeOf(bi);
			bi.biWidth     = (Int32) width;
			bi.biHeight    = (Int32) height;
			bi.biPlanes    = 1;
			bi.biBitCount  = 24;
			bi.biSizeImage = (UInt32)(stride*height);

			result = Avi.AVIStreamSetFormat(aviStream, 0, ref bi, Marshal.SizeOf(bi));
			if(result != 0){ throw new Exception("Error in AVIStreamSetFormat: "+result.ToString()); }
		}
	}
}
