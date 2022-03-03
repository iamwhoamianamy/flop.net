using System;
using System.Windows;
using System.Windows.Media;

namespace flop.net.Model
{
   public class Polygon : IGeometric
   {
      public PointCollection Points { get; }
      public Point Center { get 
         {
            var sumPoints = new Point(0, 0);
            var pointsSize = Points.Count;
            foreach (var point in Points)
            {
               sumPoints = Point.Add(sumPoints, (Vector)point);
            }
            return new Point(sumPoints.X / pointsSize, sumPoints.Y / pointsSize);
         }
      }

      public Polygon() { Points = new PointCollection(); }

      public Polygon(PointCollection points, bool isClosed)
      {
         Points = points;
         IsClosed = isClosed;
      }

      public bool IsClosed { get; }
      
      public bool IsIn(Point position, double eps)
      {
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
