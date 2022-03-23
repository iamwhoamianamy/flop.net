using System;
using System.Windows;
using System.Windows.Media;

namespace flop.net.Model
{
   public static class PolygonBuilder
   {
      public static Polygon CreateRectangle(Point pointA, Point pointB)
      {
         PointCollection points = new PointCollection()
         {
            pointA,
            new Point(pointA.X, pointB.Y),
            pointB,
            new Point(pointB.X, pointA.Y)
         };
         return new Polygon(points, true);
      }

      public static Polygon CreateTriangle(Point pointA, Point pointB)
      {
         PointCollection points;
         if (pointA.Y > pointB.Y)
         {
            points = new PointCollection()
            {
               pointB,
               new Point(pointA.X, pointB.Y),
               new Point((pointB.X + pointA.X) / 2, pointA.Y)
            };
         }  
         else
         {
            points = new PointCollection()
            {
               pointA,
               new Point(pointB.X, pointA.Y),
               new Point((pointB.X + pointA.X) / 2, pointB.Y)
            };
         }
         return new Polygon(points, true);
      }


      public static Polygon CreateTriangle(Point pointA, Point pointB, Point pointC)
      {
         var points = new PointCollection()
         {
            pointA,
            pointB,
            pointC
         };

         return new Polygon(points, true);
      }
  

      public static Polygon CreateEllipse(Point pointA, Point pointB, int? pointCount = null)
      {
         PointCollection points = new PointCollection() { };
         Point center = new Point((pointA.X + pointB.X) / 2, (pointA.Y + pointB.Y) / 2);
         double h = Math.Abs(pointA.Y - pointB.Y);
         double w = Math.Abs(pointA.X - pointB.X);
         if(pointCount == null)
         {
            pointCount = (int)Math.Round(4 * (Math.PI * h * w + (w - h) * (w - h)) / (w + h));
         }

         if (pointCount.Value < 3)
            pointCount = 3;
   
         for (var i = 0; i < pointCount; i ++)
         {
            double x = Math.Cos(2 * Math.PI * i / Convert.ToDouble(pointCount)) * w / 2 + center.X;
            double y = Math.Sin(2 * Math.PI * i / Convert.ToDouble(pointCount)) * h / 2 + center.Y;
            points.Add(new Point(x, y));
         }
         return new Ellipse(points);
      }

      public static Polygon CreateCircle(Point center, double radius)
      {
         int pointCount = (int)Math.Round(2 * Math.PI * radius);   
         PointCollection points = new PointCollection() { };
         for (var i = 0; i < pointCount; i++)
         {
            double x = Math.Cos(2 * Math.PI * i / pointCount) * radius + center.X;
            double y = Math.Sin(2 * Math.PI * i / pointCount) * radius + center.Y;
            points.Add(new Point(x, y));
         }
         return new Ellipse(points);
      }
   }
}
