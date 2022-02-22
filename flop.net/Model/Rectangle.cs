using System;
using System.Windows;
using System.Windows.Media;

namespace flop.net.Model
{
   public class Rectangle : IGeometric
   {
      public PointCollection Points { get; }
      public Point Center => new ((Points[0].X + Points[2].X) / 2, (Points[0].Y + Points[2].Y) / 2);

      public Rectangle() { Points = new PointCollection(); }
      public Rectangle(Point pointA, Point pointB)
      {  
         // Точки хранятся начиная с точки А по порядку следования
         Points = new PointCollection
         {
            pointA,
            new Point(pointB.X, pointA.Y),
            pointB,
            new Point(pointA.X, pointB.Y)
         };
      }

      public bool IsClosed => true;
      
      public bool IsIn(Point position, double eps)
      {
         //return (position.X - Math.Min(Points[0].X, Points[2].X) < -eps &&
         //   position.X < Math.Max(Points[0].X, Points[2].X) &&
         //   position.Y > Math.Min(Points[0].Y, Points[2].Y) &&
         //   position.Y < Math.Max(Points[0].Y, Points[2].Y));
         throw new NotImplementedException();
      }

      public void Move(Vector delta)
      {
         for (var i = 0; i < Points.Count; i++)
         {
            Points[i] = Point.Add(Points[i], delta);
         }
      }

      public void Rotate(double angle)
      {
         var shift = Center;
         var degToRad = angle * Math.PI / 180;
         for (var i = 0; i < Points.Count; i++)
         {
            var point = Point.Subtract(Points[i], (Vector)shift);
            var oldX = point.X;
            var oldY = point.Y;
            point.X = oldX * Math.Cos(degToRad) - oldY * Math.Sin(degToRad);
            point.Y = oldX * Math.Sin(degToRad) + oldY * Math.Cos(degToRad);
            Points[i] = Point.Add(point, (Vector)shift);
         }
      }

      public void Scale(Point scale)
      {
         var shift = Center;
         for (var i = 0; i < Points.Count; i++)
         {
            var point = Point.Subtract(Points[i], (Vector)shift);
            point.X *= scale.X;
            point.Y *= scale.Y;
            point = Point.Add(point, (Vector)shift);
            Points[i] = point;
         }
      }
   }
}
