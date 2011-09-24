using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows;

namespace VncSharp
{
  static class Extensions
  {
    /// <summary>
    /// Converts a <see cref="System.Drawing.Bitmap"/> into a WPF <see cref="BitmapSource"/>.
    /// </summary>
    /// <remarks>Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
    /// </remarks>
    /// <param name="source">The source bitmap.</param>
    /// <returns>A BitmapSource</returns>
    public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
    {
      BitmapSource bitSrc = null;

      var hBitmap = source.GetHbitmap();

      try
      {
        bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            hBitmap,
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
      }
      catch (Win32Exception)
      {
        bitSrc = null;
      }
      finally
      {
        NativeMethods.DeleteObject(hBitmap);
      }

      return bitSrc;
    }
  }
}
