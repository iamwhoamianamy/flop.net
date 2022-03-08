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
            Assert.Equal(triangle.Points[2], new Point(2, 1));
        }

        [Fact]
        public void CreateTriangleThreePointsTest()
        {
            var pointA = new Point(0, 0);
            var pointB = new Point(2, 0);
            var pointC = new Point(2, 1);
            var triangle = PolygonBuilder.CreateTriangle(pointA, pointB, pointC);
            Assert.Equal(triangle.Points[0], new Point(0, 0));
            Assert.Equal(triangle.Points[1], new Point(2, 0));
            Assert.Equal(triangle.Points[2], new Point(2, 1));
        }
    }
}
