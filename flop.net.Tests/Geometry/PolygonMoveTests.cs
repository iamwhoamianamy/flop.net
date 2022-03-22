using flop.net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Xunit;

namespace flop.net.Tests.Geometry
{
   public class PolygonMoveTest
   {
      [Fact]
      public void MoveTriangle()
      {
         var pointA = new Point(0, 0);
         var pointB = new Point(0, 2);
         var pointC = new Point(2, 2);
         var triangle = PolygonBuilder.CreateTriangle(pointA, pointB, pointC);
         var delta = new Vector(10, 10);
         triangle.Move(delta);
         var points = new PointCollection()
            {
                new Point(10, 10),
                new Point(10, 12),
                new Point(12, 12),
            };
         Assert.True(points.SequenceEqual(triangle.Points));
      }

      [Fact]
      public void MoveRectangle()
      {
         var pointA = new Point(0, 0);
         var pointB = new Point(2, 2);
         var rectangle = PolygonBuilder.CreateRectangle(pointA, pointB);
         var delta = new Vector(10, 10);
         rectangle.Move(delta);
         var points = new PointCollection()
            {
                new Point(10, 10),
                new Point(10, 12),
                new Point(12, 12),
                new Point(12, 10)
            };
         Assert.True(points.SequenceEqual(rectangle.Points));
      }

      private const double Eps = 1E-10;

      [Fact]
      public void MoveEllipse()
      {
         var pointA = new Point(-2, 1);
         var pointB = new Point(2, -1);
         var pointCount = 4;
         var ellipse = PolygonBuilder.CreateEllipse(pointA, pointB, pointCount);
         for (var i = 0; i < pointCount; i++)
         {
            if (Math.Abs(ellipse.Points[i].X) < Eps)
               ellipse.Points[i] = new Point(0, ellipse.Points[i].Y);
            if (Math.Abs(ellipse.Points[i].Y) < Eps)
               ellipse.Points[i] = new Point(ellipse.Points[i].X, 0);
         }
         var delta = new Vector(10, 10);
         ellipse.Move(delta);
         var points = new PointCollection()
            {
                new Point(12, 10),
                new Point(10, 11),
                new Point(8, 10),
                new Point(10, 9)
            };
         Assert.True(points.SequenceEqual(ellipse.Points));
      }
   }
}
