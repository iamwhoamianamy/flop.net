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
   public class PolygonIsInTest
   {
      [Fact]
      public void IncludePointTriang()
      {
         var pointA = new Point(0, 0);
         var pointB = new Point(0, 2);
         var pointC = new Point(2, 2);
         var triangle = PolygonBuilder.CreateTriangle(pointA, pointB, pointC);
         var includePoin = new Point(1, 1);
         Assert.True(triangle.IsIn(includePoin));
      }

      [Fact]
      public void NotIncludePointTringle()
      {
         var pointA = new Point(0, 0);
         var pointB = new Point(0, 2);
         var pointC = new Point(1, 2);
         var triangle = PolygonBuilder.CreateTriangle(pointA, pointB, pointC);
         var notIncludePoin = new Point(4, 4);
         Assert.False(triangle.IsIn(notIncludePoin));
      }

      [Fact]
      public void IncludePointRectangle()
      {
         var pointA = new Point(0, 0);
         var pointB = new Point(2, 2);
         var rectangle = PolygonBuilder.CreateRectangle(pointA, pointB);
         var includePoin = new Point(1, 1);
         Assert.True(rectangle.IsIn(includePoin));
      }

      [Fact]
      public void NotIncludePointRectangle()
      {
         var pointA = new Point(0, 0);
         var pointB = new Point(2, 2);
         var rectangle = PolygonBuilder.CreateRectangle(pointA, pointB);
         var notIncludePoin = new Point(4, 4);
         Assert.False(rectangle.IsIn(notIncludePoin));
      }

      [Fact]
      public void IncludePointEllipse()
      {
         var pointA = new Point(-2, 1);
         var pointB = new Point(2, -1);
         var pointCount = 4;
         var ellipse = PolygonBuilder.CreateEllipse(pointA, pointB, pointCount);
         var includePoin = new Point(0, 0);
         Assert.True(ellipse.IsIn(includePoin));
      }

      [Fact]
      public void NotIncludePointEllipse()
      {
         var pointA = new Point(-2, 1);
         var pointB = new Point(2, -1);
         var pointCount = 4;
         var ellipse = PolygonBuilder.CreateEllipse(pointA, pointB, pointCount);
         var notIncludePoin = new Point(4, 4);
         Assert.False(ellipse.IsIn(notIncludePoin));
      }
   }
}
