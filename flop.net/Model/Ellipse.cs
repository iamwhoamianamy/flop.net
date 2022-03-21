using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace flop.net.Model
{
   public class Ellipse : Polygon
   {
      private double Height { get; set; }
      private double Width { get; set; }
      public Ellipse() : base() { }

      public Ellipse(PointCollection points, bool isClosed) : base(points, isClosed) 
      {
         Height = points.Max(x => x.Y) - points.Min(x => x.Y);
         Width = points.Max(x => x.X) - points.Min(x => x.X);
      }

      public void Scale(Point scale, Point? scalePoint = null)
      {
         var shift = scalePoint.HasValue ? scalePoint.Value : Center;
         Height *= scale.X;
         Width *= scale.Y;
         Points.Clear();
         var pointCount = (int)Math.Round(4 * (Math.PI * Height * Width + (Width - Height) * (Width - Height)) / (Width + Height));
         for (var i = 0; i < pointCount; i++)
         {
            double x = Math.Cos(2 * Math.PI * i / Convert.ToDouble(pointCount)) * Width / 2 + shift.X;
            double y = Math.Sin(2 * Math.PI * i / Convert.ToDouble(pointCount)) * Height / 2 + shift.Y;
            Points.Add(new Point(x * Math.Cos(RotationAngle) - y * Math.Sin(RotationAngle),
               x * Math.Sin(RotationAngle) + y * Math.Cos(RotationAngle)));
         }
      }
   }
}
