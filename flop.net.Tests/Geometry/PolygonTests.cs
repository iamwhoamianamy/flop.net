using flop.net.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xunit;

namespace flop.net.Tests.Geometry
{
   public class PolygonTest
   {
      private const double Eps = 1E-10;

      [Fact]
      public void RotateRectangleTest()
      {
         var pointA = new Point(0, 0);
         var pointB = new Point(2, 2);
         var rectangle = PolygonBuilder.CreateRectangle(pointA, pointB);

         rectangle.Rotate(90);

         for (var i = 0; i < rectangle.Points.Count; i++)
         {
            if (Math.Abs(rectangle.Points[i].X) < Eps)
               rectangle.Points[i] = new Point(0, rectangle.Points[i].Y);
            if (Math.Abs(rectangle.Points[i].Y) < Eps)
               rectangle.Points[i] = new Point(rectangle.Points[i].X, 0);
         }

         var points = new PointCollection()
         {
            new Point(2, 0),
            new Point(0, 0),
            new Point(0, 2),
            new Point(2, 2)
         };

         Assert.True(points.SequenceEqual(rectangle.Points));
      }

      [Fact]
      public void RotateTriangleTest()
      {
         var pointA = new Point(0, 0);
         var pointB = new Point(3, 0);
         var pointC = new Point(0, 3);
         var triangle = PolygonBuilder.CreateTriangle(pointA, pointB, pointC);

         triangle.Rotate(90);

         for (var i = 0; i < triangle.Points.Count; i++)
         {
            if (Math.Abs(triangle.Points[i].X) < Eps)
               triangle.Points[i] = new Point(0, triangle.Points[i].Y);
            if (Math.Abs(triangle.Points[i].Y) < Eps)
               triangle.Points[i] = new Point(triangle.Points[i].X, 0);
         }

         var points = new PointCollection()
         {
            new Point(2, 0),
            new Point(2, 3),
            new Point(-1, 0)
         };

         Assert.True(points.SequenceEqual(triangle.Points));
      }

      [Fact]
      public void RotateEllipseTest()
      {
         var pointA = new Point(-2, 1);
         var pointB = new Point(2, -1);
         var pointCount = 4;
         var ellipse = PolygonBuilder.CreateEllipse(pointA, pointB, pointCount);

         ellipse.Rotate(90);

         for (var i = 0; i < pointCount; i++)
         {
            if (Math.Abs(ellipse.Points[i].X) < Eps)
               ellipse.Points[i] = new Point(0, Math.Round(ellipse.Points[i].Y));
            if (Math.Abs(ellipse.Points[i].Y) < Eps)
               ellipse.Points[i] = new Point(Math.Round(ellipse.Points[i].X), 0);
         }

         var points = new PointCollection()
         {
            new Point(0, 2),
            new Point(-1, 0),
            new Point(0, -2),
            new Point(1, 0),
         };

         Assert.True(points.SequenceEqual(ellipse.Points));

      }
      [Fact]
      public void ScaleEllipseTest()
      {
         var pointA = new Point(-2, 1);
         var pointB = new Point(2, -1);
         var pointCount = 4;
         var scale = new Point(0.5,0.075);
         var scalePoint = new Point(1, 1);
         var ellipse = PolygonBuilder.CreateEllipse(pointA, pointB, pointCount);

         ellipse.Scale(scale, scalePoint);

         for (var i = 0; i < pointCount; i++)
         {
            if (Math.Abs(ellipse.Points[i].X) < Eps)
               ellipse.Points[i] = new Point(0, Math.Round(ellipse.Points[i].Y));
            if (Math.Abs(ellipse.Points[i].Y) < Eps)
               ellipse.Points[i] = new Point(Math.Round(ellipse.Points[i].X), 0);
         }

         var points = new PointCollection()
         {
            new Point(1.15, 1),
            new Point(1, 1.5),
            new Point(0.85, 1),
            new Point(1, 0.5),
         };

         Assert.True(points.SequenceEqual(ellipse.Points));
      }
   }
}
