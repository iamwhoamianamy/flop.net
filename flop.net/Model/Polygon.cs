using System;
using System.Windows;
using System.Windows.Media;
using System.Linq;

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
         Points = points.Clone();
         IsClosed = isClosed;
      }

      public Polygon CircumscribingRectangle
      {
         get
         {
            var maxX = Points.Max(x => x.X);
            var maxY = Points.Max(y => y.Y);
            var minX = Points.Min(x => x.X);
            var minY = Points.Min(y => y.Y);
            var points = new PointCollection()
            {
               new Point(minX, minY),
               new Point(minX, maxY),
               new Point(maxX, maxY),
               new Point(maxX, minY)
            };
            return new Polygon(points, true);
         }
      }

      public bool IsClosed { get; private set; }
      
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

      public void Rotate(double angle, Point? rotationCenter)
      {
         if (!rotationCenter.HasValue)
         {
            rotationCenter = Center;
         }
         var degToRad = angle * Math.PI / 180;
         for (var i = 0; i < Points.Count; i++)
         {
            var point = Point.Subtract(Points[i], (Vector)rotationCenter);
            var oldX = point.X;
            var oldY = point.Y;
            point.X = oldX * Math.Cos(degToRad) - oldY * Math.Sin(degToRad);
            point.Y = oldX * Math.Sin(degToRad) + oldY * Math.Cos(degToRad);
            Points[i] = Point.Add(point, (Vector)rotationCenter);
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
