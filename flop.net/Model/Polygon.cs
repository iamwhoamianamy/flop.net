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

      public Polygon BoundingBox
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
      
      public bool IsIn(Point position, double eps = 0.001)
      {
         var result = false;
         int i1, i2, n;
         double S, S1, S2, S3;
         var pointsSize = Points.Count;
         for (n = 0; n < pointsSize; n++)
         {
            result = false;
            i1 = n < pointsSize - 1 ? n + 1 : 0;
            while (result == false)
            {
               i2 = i1 + 1;
               if (i2 >= pointsSize)
                  i2 = 0;
               if (i2 == (n < pointsSize - 1 ? n + 1 : 0))
                  break;
               S = Math.Abs(Points[i1].X * (Points[i2].Y - Points[n].Y) +
                        Points[i2].X * (Points[n].Y - Points[i1].Y) +
                        Points[n].X * (Points[i1].Y - Points[i2].Y));
               S1 = Math.Abs(Points[i1].X * (Points[i2].Y - position.Y) +
                         Points[i2].X * (position.Y - Points[i1].Y) +
                         position.X * (Points[i1].Y - Points[i2].Y));
               S2 = Math.Abs(Points[n].X * (Points[i2].Y - position.Y) +
                         Points[i2].X * (position.Y - Points[n].Y) +
                          position.X * (Points[n].Y - Points[i2].Y));
               S3 = Math.Abs(Points[i1].X * (Points[n].Y - position.Y) +
                         Points[n].X * (position.Y - Points[i1].Y) +
                          position.X * (Points[i1].Y - Points[n].Y));
               if (Math.Abs(S - (S1 + S2 + S3)) <= eps)
               {
                  result = true;
                  break;
               }
               i1++;
               if (i1 >= pointsSize)
                  i1 = 0;
            }
            if (result == true)
               break;
         }
         return result;
      }

      public void Move(Vector delta)
      {
         for (var i = 0; i < Points.Count; i++)
         {
            Points[i] = Point.Add(Points[i], delta);
         }
      }

      public void Rotate(double angle, Point? rotationCenter=null)
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

      public void Scale(Point scale, Point? scalePoint=null)
      {
         var shift = scalePoint.HasValue ? scalePoint : Center;
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
