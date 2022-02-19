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
      void Rotate(double angle);
      void Scale(Point scale);
      PointCollection GetPoints(double minstep);
      PointCollection Points { get; }
      bool IsClosed { get; }
      string TypeName { get; }
      bool IsIn(Point position, double eps);
   }
}
