using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;

namespace flop.net.Model
{
   public class Polygon : IGeometric
   {
      public double RotationAngle { get; private set; }
      
      public PointCollection Points { get; private set; }

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

      public Polygon(PointCollection points, bool isClosed, double rotationAngle=0)
      {
         Points = points.Clone();
         IsClosed = isClosed;
         RotationAngle = rotationAngle;
      }

      public Rectangle BoundingBoxRotated
      {
         get
         {
            var defaultPoints = Points.Clone();
            // Разворот фигуры к осям параллельным X и Y
            for (var i = 0; i < Points.Count; i++)
            {
               var point = Point.Subtract(defaultPoints[i], (Vector)Center);
               var oldX = point.X;
               var oldY = point.Y;
               point.X = oldX * Math.Cos(RotationAngle) + oldY * Math.Sin(RotationAngle);
               point.Y = -oldX * Math.Sin(RotationAngle) + oldY * Math.Cos(RotationAngle);
               defaultPoints[i] = Point.Add(point, (Vector)Center);
            }

            var maxX = defaultPoints.Max(x => x.X);
            var maxY = defaultPoints.Max(y => y.Y);
            var minX = defaultPoints.Min(x => x.X);
            var minY = defaultPoints.Min(y => y.Y);
            var points = new PointCollection()
            {
               new Point(minX, maxY),
               new Point(maxX, maxY),
               new Point(maxX, minY),
               new Point(minX, minY)
            };

            // Разворот BoundingBox обратно к исходному положению
            Point rectangleCenter = new((points[0].X + points[2].X) / 2, (points[0].Y + points[2].Y) / 2);
            for (var i = 0; i < points.Count; i++)
            {
               var point = Point.Subtract(points[i], (Vector)rectangleCenter);
               var oldX = point.X;
               var oldY = point.Y;
               point.X = oldX * Math.Cos(RotationAngle) - oldY * Math.Sin(RotationAngle);
               point.Y = oldX * Math.Sin(RotationAngle) + oldY * Math.Cos(RotationAngle);
               points[i] = Point.Add(point, (Vector)rectangleCenter);
            }
            return new Rectangle(points);
         }
      }

      public Rectangle BoundingBox 
      {
         get
         {
            var maxX = Points.Max(x => x.X);
            var maxY = Points.Max(y => y.Y);
            var minX = Points.Min(x => x.X);
            var minY = Points.Min(y => y.Y);
            var points = new PointCollection()
            {
               new Point(minX, maxY),
               new Point(maxX, maxY),
               new Point(maxX, minY),
               new Point(minX, minY)
            };
            return new Rectangle(points);
         }
      }

      public bool IsClosed { get; set; }
      
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
         RotationAngle += degToRad;
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

      public virtual void Scale(Point scale, Point? scalePoint=null)
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

      public virtual Polygon AddPoint(Point newPoint)
      {
         var minPoint = Points.OrderBy(x => Math.Sqrt((x.X - newPoint.X) * (x.X - newPoint.X) + (x.Y - newPoint.Y) * (x.Y - newPoint.Y))).First();
         var index = Points.IndexOf(minPoint);
         int index_prev = index - 1;
         int index_next = index + 1;
         int indexNewPoint = 0;
         var newPoints = Points.Clone();

         if (IsClosed)
         {
            if (index == 0)
            {
               index_prev = Points.Count - 1;
            }
            else if (index == Points.Count - 1)
            {
               index_next = 0;
            }

            var points_distance = new List<double>()
            {
               Math.Sqrt((Points[index_prev].X - newPoint.X) * (Points[index_prev].X - newPoint.X) + (Points[index_prev].Y - newPoint.Y) * (Points[index_prev].Y - newPoint.Y)),
               Math.Sqrt((Points[index_next].X - newPoint.X) * (Points[index_next].X - newPoint.X) + (Points[index_next].Y - newPoint.Y) * (Points[index_next].Y - newPoint.Y))
            };

            if (points_distance.IndexOf(points_distance.Min(x=>x)) == 0)
            {
               indexNewPoint = index;
            }  
            else
            {
               indexNewPoint = index_next;
            }
            newPoints.Insert(indexNewPoint, newPoint);
         }
         else
         {
            newPoints.Add(newPoint);
         }
         
         return new Polygon(newPoints, IsClosed, RotationAngle);
      }

      public void MovePoint(Point pointToMove, Vector delta)
      {
         var index = Points.IndexOf(pointToMove);
         if (index != -1)
         {
            Points[index] = Point.Add(Points[index], delta);
         }
      }
   }
}
