using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SurfaceRabbit.Capture
{

  public class VideoCaptureOpenCV
  {

    private VideoWriter writer = null;

    public bool Started { get; private set; }

    public VideoCaptureOpenCV()
    {
    }

    public bool StartCapture(String fileName, int width, int height)
    {
      try
      {
        writer = new VideoWriter(fileName, CvInvoke.CV_FOURCC('D', 'I', 'V', 'X'), 25, width, height, false);
        Started = true;
        return true;
      }
      catch (Exception e)
      {
        Console.WriteLine("Exception capturing video: {0}", e.Message);
        return false;
      }
    }

    public bool AddFrame(Bitmap cImage)
    {
      try
      {
        Image<Gray, byte> frame = new Image<Gray, byte>(cImage);
        writer.WriteFrame<Gray, byte>(frame);
        //Both codes trigger the same exception
        //IntPtr writerPtr = writer.Ptr;
        //IntPtr framePtr = frame.Ptr;
        //CvInvoke.cvWriteFrame(writerPtr, framePtr);
        return true;
      }
      catch(Exception e) 
      {
        Console.WriteLine("Exception capturing video: {0}", e.Message);
        return false; 
      }
    }

    public bool StopCapture()
    {
      try
      {
        writer.Dispose();
        Started = false;
        return true;
      }
      catch (Exception e)
      {
        Console.WriteLine("Exception capturing video: {0}", e.Message);
        return false;
      }
    }

  }

}
