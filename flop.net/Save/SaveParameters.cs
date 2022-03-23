using System;
using System.Windows.Controls;

namespace flop.net.Save
{
   public class SaveParameters
   {
      public string Format { get; set; }
      public int Width { get; set; }
      public int Height { get; set; }
      public Canvas Canv { get; set; }
      public string FileName { get; set; }
   }

   public class OpenParameters
   {
      public string Format { get; set; }
      public string FileName { get; set; }
   }
}
