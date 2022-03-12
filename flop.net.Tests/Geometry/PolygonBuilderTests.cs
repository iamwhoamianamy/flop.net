using flop.net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xunit;

namespace flop.net.Tests
{
    public class PolygonBuilderTest
    {
        [Fact]
        public void CreateTriangleTwoPointsTest()
        {
            var pointA = new Point(0, 0);
            var pointB = new Point(2, 2);
            var triangle = PolygonBuilder.CreateTriangle(pointA, pointB);
            Assert.Equal(triangle.Points[0], new Point(0, 0));
            Assert.Equal(triangle.Points[1], new Point(2, 0));
            Assert.Equal(triangle.Points[2], new Point(1, 2));
        }

        [Fact]
        public void CreateTriangleThreePointsTest()
        {
            var pointA = new Point(0, 0);
            var pointB = new Point(2, 0);
            var pointC = new Point(1, 2);
            var triangle = PolygonBuilder.CreateTriangle(pointA, pointB, pointC);
            Assert.Equal(triangle.Points[0], new Point(0, 0));
            Assert.Equal(triangle.Points[1], new Point(2, 0));
            Assert.Equal(triangle.Points[2], new Point(1, 2));
        }

        private const double Eps = 1E-10;

        //[Fact]
        //public void CreateEllipseTest()
        //{
        //    var pointA = new Point(-2, 1);
        //    var pointB = new Point(2, -1);
        //    var pointCount = 4;
        //    var ellipse = PolygonBuilder.CreateEllipse(pointA, pointB, pointCount);

        //    for (var i = 0; i < pointCount; i++)
        //    {
        //        if (Math.Abs(ellipse.Points[i].X) < Eps)
        //            ellipse.Points[i] = new Point(0, ellipse.Points[i].Y);
        //        if (Math.Abs(ellipse.Points[i].Y) < Eps)
        //            ellipse.Points[i] = new Point(ellipse.Points[i].X, 0);
        //    }

        //    Assert.Equal(ellipse.Points[0], new Point(2, 0));
        //    Assert.Equal(ellipse.Points[1], new Point(0, 1));
        //    Assert.Equal(ellipse.Points[2], new Point(-2, 0));
        //    Assert.Equal(ellipse.Points[3], new Point(0, -1));
        //}
    }
}