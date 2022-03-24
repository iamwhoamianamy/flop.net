using flop.net.Save;
using System.Windows;
using Xunit;
using System.IO;
using flop.net.Model;

namespace flop.net.Tests.Save
{
   public class PdfSaverTests
   {
      [Fact]
      public void CreatePdfFileTest()
      {
         var pdfSaver = new PdfSaver("test.pdf", 1000, 1000);
         pdfSaver.SaveLayersToPdf(new Layer());
         Assert.True(File.Exists("test.pdf"));
      }

   }
}
