using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using flop.net.Model;

namespace flop.net.Save;

public class PngSaver
{
   private string _fullFilename;
   private Canvas _canvas;
   private int _width;
   private int _height;
   public string FullFilename => _fullFilename;

   public PngSaver(string fullFilename, Canvas canvas, int width, int height)
   {
      _canvas = canvas;
      _width = width;
      _height = height;
      _fullFilename = fullFilename;
   }

   public void Save()
   {
      RenderTargetBitmap rtb = new RenderTargetBitmap(_width, _height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
      rtb.Render(_canvas);

      var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, _width, _height));

      BitmapEncoder pngEncoder = new PngBitmapEncoder();
      pngEncoder.Frames.Add(BitmapFrame.Create(crop));

      using (var fs = System.IO.File.OpenWrite(_fullFilename))
      {
          pngEncoder.Save(fs);
      }
  }

}
