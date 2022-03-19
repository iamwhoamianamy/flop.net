using flop.net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xunit;

namespace flop.net.Tests.Geometry
{
   public class BoundingBoxTests
   {
      [Fact]
      public void BoundedRectangleTest()
      {
         var pointA = new Point(-1, 10.6);
         var pointB = new Point(5, -4.3);
         var rectangle = PolygonBuilder.CreateRectangle(pointA, pointB);
         var boundingBox = rectangle.BoundingBox;

         var rectangleArea = Math.Abs(pointB.Y - pointA.Y) *
            Math.Abs(pointB.X - pointA.X);

         var boxArea = (boundingBox.Points[1].Y - boundingBox.Points[0].Y) *
    (boundingBox.Points[3].X - boundingBox.Points[0].X);

         Assert.Equal(boxArea, rectangleArea);
      }

      [Fact]
      public void BoundedTriangle2pTest()
      {
         var pointA = new Point(-3.6, 0.1);
         var pointB = new Point(3.76, 3.33);
         var triangle = PolygonBuilder.CreateTriangle(pointA, pointB);
         var boundingBox = triangle.BoundingBox;

         var triangleArea = (Math.Abs(pointB.Y - pointA.Y) *
            (Math.Abs(pointB.X - pointA.X)) / 2);

         var boxArea = (boundingBox.Points[1].Y - boundingBox.Points[0].Y) *
             (boundingBox.Points[3].X - boundingBox.Points[0].X);

         Assert.Equal(boxArea, triangleArea * 2);
      }

      [Fact]
      public void BoundedTriangle3pTest()
      {
         var pointA = new Point(1, 4.5);
         var pointB = new Point(4.5, 3);
         var pointC = new Point(2, 1.5);
         var triangle = PolygonBuilder.CreateTriangle(pointA, pointB, pointC);
         var boundingBox = triangle.BoundingBox;

         var testBoxArea = (Math.Max(pointA.Y, Math.Max(pointB.Y, pointC.Y)) - Math.Min(pointA.Y, Math.Min(pointB.Y, pointC.Y))) *
            (Math.Max(pointA.X, Math.Max(pointB.X, pointC.X)) - Math.Min(pointA.X, Math.Min(pointB.X, pointC.X)));

         var boxArea = (boundingBox.Points[1].Y - boundingBox.Points[0].Y) *
             (boundingBox.Points[3].X - boundingBox.Points[0].X);

         Assert.Equal(boxArea, testBoxArea);
      }

      [Fact]
      public void BoundedEllipseTest()
      {
         var pointA = new Point(-1.11, -1.67);
         var pointB = new Point(3, 22);
         var pointCount = 4;
         var ellipse = PolygonBuilder.CreateEllipse(pointA, pointB, pointCount);
         var boundingBox = ellipse.BoundingBox;

         var maxX = ellipse.Points.Max(x => x.X);
         var maxY = ellipse.Points.Max(y => y.Y);
         var minX = ellipse.Points.Min(x => x.X);
         var minY = ellipse.Points.Min(y => y.Y);

         var testBoxArea = (maxX - minX) * (maxY - minY);

         var boxArea = (boundingBox.Points[1].Y - boundingBox.Points[0].Y) *
    (boundingBox.Points[3].X - boundingBox.Points[0].X);

         Assert.Equal(boxArea, testBoxArea);
      }
   }
}
