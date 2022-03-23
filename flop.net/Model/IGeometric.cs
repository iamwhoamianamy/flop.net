using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace flop.net.Model
{
   public interface IGeometric
   {
      void Move(Vector delta);
      void Rotate(double angle, Point? rotationCenter=null);
      void Scale(Point scale, Point? scalePoint=null);
      PointCollection Points { get; }
      bool IsClosed { get; set; }
      Point Center { get; }
      bool IsIn(Point position, double eps);
      Rectangle BoundingBoxRotated { get; }
      Rectangle BoundingBox { get; }
   }
}
