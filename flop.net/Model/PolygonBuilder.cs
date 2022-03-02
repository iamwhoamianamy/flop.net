﻿using System;
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

      public static Polygon CreateEllipse(Point pointA, Point pointB, double amount)
      {
         PointCollection points = new PointCollection() { };
         Point center = new Point((pointA.X + pointB.X) / 2, (pointA.Y + pointB.Y) / 2);
         double h = Math.Abs(pointA.Y) + Math.Abs(pointB.Y);
         double w = Math.Abs(pointA.X) + Math.Abs(pointB.X);
         for (var i = 0; i < amount; i ++)
         {
            double x = Math.Cos(2 * Math.PI * i / amount) * w / 2 + center.X;
            double y = Math.Sin(2 * Math.PI * i / amount) * h / 2 + center.Y;
            points.Add(new Point(x, y));
         }
         return new Polygon(points, true);
      }
   }
}
