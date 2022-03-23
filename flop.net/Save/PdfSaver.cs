using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using flop.net.Model;
using System.Diagnostics;
namespace flop.net.Save
{
   public class PdfSaver
   {
      public string FullFileName { get; private set; }
      public int Width { get; private set; }
      public int Height { get; private set; }
      public PdfSaver(string fullFileName, int width, int height)
      {
         FullFileName = fullFileName;
         Width = width;
         Height = height;
      }

      public void SaveLayersToPdf(Collection<Layer> layers)
      {
         PdfDocument document = new PdfDocument();
         PdfPage page = document.AddPage();
         page.Width = Width;
         page.Height = Height;
         XGraphics gfx = XGraphics.FromPdfPage(page);
         foreach(var layer in layers)
         {
            foreach(var figure in layer.Figures)
            {
               if (figure.Geometric.IsClosed == true)
                  DrawPolygon(gfx, figure);
               else
                  DrawPolyline(gfx, figure);
            }
         }
         document.Save(FullFileName);

      }

      public void SaveLayersToPdf(Layer layer)
      {
         PdfDocument document = new PdfDocument();
         PdfPage page = document.AddPage();
         page.Width = Width;
         page.Height = Height;
         XGraphics gfx = XGraphics.FromPdfPage(page);
            foreach (var figure in layer.Figures)
            {
               DrawPolygon(gfx, figure);
            }
         document.Save(FullFileName);
         //Process.Start(FullFileName);
      }

      private void DrawPolygon(XGraphics gfx, Figure figure)
      {
         XBrush brush = new XSolidBrush(GetXColor(figure.DrawingParameters.Fill));
         if(figure.DrawingParameters.StrokeThickness > 0)
         {
            XPen pen = new XPen(GetXColor(figure.DrawingParameters.Stroke), figure.DrawingParameters.StrokeThickness);
            gfx.DrawPolygon(pen, brush, GetXPoints(figure.Geometric.Points), XFillMode.Alternate);
         }
         else
         {
            gfx.DrawPolygon(brush, GetXPoints(figure.Geometric.Points), XFillMode.Alternate);
         }
      }

      private void DrawPolyline(XGraphics gfx, Figure figure)
      {
         XPen pen = new XPen(GetXColor(figure.DrawingParameters.Fill), figure.DrawingParameters.StrokeThickness);
         gfx.DrawLines(pen, GetXPoints(figure.Geometric.Points));
      }


      private XPoint[] GetXPoints(PointCollection points)
      {
         XPoint[] result = new XPoint[points.Count];
         for (int i = 0; i < points.Count; i++)
            result[i] = new XPoint(points[i].X, points[i].Y);
         return result;
      }
      private XColor GetXColor(Color color)
      {
         return XColor.FromArgb(color.A, color.R, color.G, color.B);
      }

   }
}
