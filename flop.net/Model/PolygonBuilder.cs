using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
               new Point(pointA.Y, (pointB.X + pointA.X) / 2)
            };
         }  
         else
         {
            points = new PointCollection()
            {
               pointA,
               new Point(pointB.X, pointA.Y),
               new Point(pointB.Y, (pointB.X + pointA.X) / 2)
            };
         }
         return new Polygon(points, true);
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
         return new Polygon(points, true);
      }
   }
}
