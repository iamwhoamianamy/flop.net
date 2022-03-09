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
               break;
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
