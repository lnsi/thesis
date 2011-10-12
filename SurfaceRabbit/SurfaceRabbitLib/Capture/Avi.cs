using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SurfaceRabbit.Capture
{

	public class Avi{

		public const int StreamtypeVIDEO = 1935960438; //mmioStringToFOURCC("vids", 0)
		public const int OF_SHARE_DENY_WRITE = 32;
		public const int BMP_MAGIC_COOKIE = 19778; //ascii string "BM"
		
		#region structure declarations

		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct RECT{ 
			public UInt32 left; 
			public UInt32 top; 
			public UInt32 right; 
			public UInt32 bottom; 
		} 		

		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct BITMAPINFOHEADER {
			public UInt32 biSize;
			public  Int32 biWidth;
			public  Int32 biHeight;
			public  Int16 biPlanes;
			public  Int16 biBitCount;
			public UInt32 biCompression;
			public UInt32 biSizeImage;
			public  Int32 biXPelsPerMeter;
			public  Int32 biYPelsPerMeter;
			public UInt32 biClrUsed;
			public UInt32 biClrImportant;
		}

		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct AVISTREAMINFO {
			public UInt32    fccType;
			public UInt32    fccHandler;
			public UInt32    dwFlags;
			public UInt32    dwCaps;
			public UInt16    wPriority;
			public UInt16    wLanguage;
			public UInt32    dwScale;
			public UInt32    dwRate;
			public UInt32    dwStart;
			public UInt32    dwLength;
			public UInt32    dwInitialFrames;
			public UInt32    dwSuggestedBufferSize;
			public UInt32    dwQuality;
			public UInt32    dwSampleSize;
			public RECT		 rcFrame;
			public UInt32    dwEditCount;
			public UInt32    dwFormatChangeCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
			public UInt16[]    szName;
		}
		[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct BITMAPFILEHEADER{
			public Int16 bfType; //"magic cookie" - must be "BM"
			public Int32 bfSize;
			public Int16 bfReserved1;
			public Int16 bfReserved2;
			public Int32 bfOffBits;
		}

		#endregion structure declarations

		#region method declarations
	
		//Initialize the AVI library
		[DllImport("avifil32.dll")]
		public static extern void AVIFileInit();

		//Open an AVI file
		[DllImport("avifil32.dll", PreserveSig=true)]
		public static extern int AVIFileOpen(
			ref int ppfile,
			String szFile,
			int uMode,
			int pclsidHandler);

		//Get a stream from an open AVI file
		[DllImport("avifil32.dll")]
		public static extern int AVIFileGetStream(
			int pfile,
			out IntPtr ppavi,  
			int fccType,       
			int lParam);

		//Get the start position of a stream
		[DllImport("avifil32.dll", PreserveSig=true)]
		public static extern int AVIStreamStart(int pavi);

		//Get the length of a stream in frames
		[DllImport("avifil32.dll", PreserveSig=true)]
		public static extern int AVIStreamLength(int pavi);

		//Get information about an open stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamInfo(
			int pAVIStream,
			ref AVISTREAMINFO psi,
			int lSize);

		//Get a pointer to a GETFRAME object (returns 0 on error)
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamGetFrameOpen(
			IntPtr pAVIStream,
			ref BITMAPINFOHEADER bih);

		/*[DllImport("avifil32.dll")]
		public static extern int AVIStreamGetFrameOpen(
			IntPtr pAVIStream,
			int dummy);*/

		//Get a pointer to a packed DIB (returns 0 on error)
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamGetFrame(
			int pGetFrameObj,
			int lPos);

		//Create a new stream in an open AVI file
		[DllImport("avifil32.dll")]
		public static extern int AVIFileCreateStream(
			int pfile,
			out IntPtr ppavi, 
			ref AVISTREAMINFO ptr_streaminfo);
 
		//Set the format for a new stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamSetFormat(
			IntPtr aviStream, Int32 lPos, 
			ref BITMAPINFOHEADER lpFormat, Int32 cbFormat);

		//Write a sample to a stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamWrite(
			IntPtr aviStream, Int32 lStart, Int32 lSamples, 
			IntPtr lpBuffer, Int32 cbBuffer, Int32 dwFlags, 
			Int32 dummy1, Int32 dummy2);

		//Release the GETFRAME object
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamGetFrameClose(
			int pGetFrameObj);

		//Release an open AVI stream
		[DllImport("avifil32.dll")]
		public static extern int AVIStreamRelease(IntPtr aviStream);

		//Release an open AVI file
		[DllImport("avifil32.dll")]
		public static extern int AVIFileRelease(int pfile);

		//Close the AVI library
		[DllImport("avifil32.dll")]
		public static extern void AVIFileExit();
		
		#endregion methos declarations

		#region other useful avi functions

		//public const int StreamtypeAUDIO = 1935963489; //mmioStringToFOURCC("auds", 0)
		//public const int StreamtypeMIDI = 1935960429;  //mmioStringToFOURCC("mids", 0)
		//public const int StreamtypeTEXT = 1937012852;  //mmioStringToFOURCC("txts", 0)
	
		/*[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct AVIFILEINFO{
			public Int32 dwMaxBytesPerSecond;
			public Int32 dwFlags;
			public Int32 dwCaps;
			public Int32 dwStreams;
			public Int32 dwSuggestedBufferSize;
			public Int32 dwWidth;
			public Int32 dwHeight;
			public Int32 dwScale;
			public Int32 dwRate;
			public Int32 dwLength;
			public Int32 dwEditCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
			public char[] szFileType;
		}*/

		/*[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct AVICOMPRESSOPTIONS {
			public UInt32   fccType;
			public UInt32   fccHandler;
			public UInt32   dwKeyFrameEvery;  // only used with AVICOMRPESSF_KEYFRAMES
			public UInt32   dwQuality;
			public UInt32   dwBytesPerSecond; // only used with AVICOMPRESSF_DATARATE
			public UInt32   dwFlags;
			public IntPtr   lpFormat;
			public UInt32   cbFormat;
			public IntPtr   lpParms;
			public UInt32   cbParms;
			public UInt32   dwInterleaveEvery;
		}*/

		/*[DllImport("avifil32.dll")]
		public static extern int AVIMakeCompressedStream(
			out IntPtr ppsCompressed, IntPtr aviStream, 
			ref AVICOMPRESSOPTIONS ao, int dummy);*/

		/*[DllImport("avifil32.dll")]
		public static extern int AVISaveOptions(
			IntPtr hWnd,
			int uiFlags,
			int nStreams, 
			ref IntPtr ppavi,
			ref IntPtr ppOptions);*/

		/*[DllImport("avifil32.dll")]
		public static extern int AVIFileInfo(
			int pfile, 
			ref AVIFILEINFO pfi,
			int lSize);*/

		/*[DllImport("winmm.dll", EntryPoint="mmioStringToFOURCCA")]
		public static extern int mmioStringToFOURCC(String sz, int uFlags);*/

		/*[DllImport("avifil32.dll")]
		public static extern int AVIStreamRead(
			IntPtr pavi, 
			Int32 lStart,     
			Int32 lSamples,   
			IntPtr lpBuffer, 
			Int32 cbBuffer,   
			Int32  plBytes,  
			Int32  plSamples 
			);*/

		#endregion other useful avi functions

	}
}
