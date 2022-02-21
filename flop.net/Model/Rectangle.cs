﻿using System;
using System.Windows;
using System.Windows.Media;

namespace flop.net.Model
{
   public class Rectangle : IGeometric
   {
      public PointCollection Points { get; }
      public Point Center { get; }
      public double Height { get; }
      public double Width { get; }

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

         Center = new Point((pointA.X + pointB.X) / 2, (pointA.Y + pointB.Y) / 2);
         Height = Math.Abs(pointA.Y - pointB.Y);
         Width = Math.Abs(pointA.X - pointB.X);
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
         throw new NotImplementedException();
      }

      public void Scale(Point scale)
      {
         throw new NotImplementedException();
      }
   }
}
