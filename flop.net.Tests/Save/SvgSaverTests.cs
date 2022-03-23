using flop.net.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xunit;
using System.IO;
using flop.net.Model;

namespace flop.net.Tests.Save
{
   public class SvgSaverTests
   {
      [Fact]
      public void CreateSvgFileTest()
      {
         var svgSaver = new SvgSaver("test.svg", new Layer(), 100, 100);
         svgSaver.Save();
         Assert.True(File.Exists("test.svg"));
      }

      [Fact]
      public void WriteToSvgFileRectangleTest()
      {
         var layer = new Layer();
         var rectangle = PolygonBuilder.CreateRectangle(new Point(100, 100), new Point(250, 250));
         layer.Figures.Add(new Figure(rectangle, new DrawingParameters()));
         var svgSaver = new SvgSaver("test.svg", layer, 1000, 1000);
         svgSaver.Save();
         var expected = File.ReadAllText("Save/Answers/SvgAnswerRectangle.txt");
         var received = File.ReadAllText("test.svg");
         Assert.Equal(expected, received);
      }

      [Fact]
      public void WriteToSvgFileTriangleTest()
      {
         var layer = new Layer();
         var triangle = PolygonBuilder.CreateTriangle(new Point(100, 100), new Point(250, 250));
         layer.Figures.Add(new Figure(triangle, new DrawingParameters()));
         var svgSaver = new SvgSaver("test.svg", layer, 1000, 1000);
         svgSaver.Save();
         var expected = File.ReadAllText("Save/Answers/SvgAnswerTriangle.txt");
         var received = File.ReadAllText("test.svg");
         Assert.Equal(expected, received);
      }

      [Fact]
      public void WriteToSvgFileEllipseTest()
      {
         var layer = new Layer();
         var ellipse = PolygonBuilder.CreateEllipse(new Point(-20, 10), new Point(20, -10), 4);
         layer.Figures.Add(new Figure(ellipse, new DrawingParameters()));
         var svgSaver = new SvgSaver("test.svg", layer, 1000, 1000);
         svgSaver.Save();
         var expected = File.ReadAllText("Save/Answers/SvgAnswerEllipse.txt");
         var received = File.ReadAllText("test.svg");
         Assert.Equal(expected, received);
      }
   }
}
