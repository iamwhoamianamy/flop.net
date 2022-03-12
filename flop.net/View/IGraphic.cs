using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using flop.net.Model;
using Polygon = System.Windows.Shapes.Polygon;

namespace flop.net.View
{
    public interface IGraphic
    {
        void DrawPolyline(PointCollection points, DrawingParameters drawingParametrs);
        void DrawPolygon(PointCollection points, DrawingParameters drawingParametrs);
    }
    public class Graphic : IGraphic
    {
        Canvas canvas;
        public Graphic(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void CleanCanvas()
        {
            canvas.Children.Clear();
        }

        public void DrawPolygon(PointCollection points, DrawingParameters drawingParametrs)
        {
            Polygon polygon = new Polygon();
          
            polygon.Fill = Brushes.Green;
            //polygon.Fill = drawingParametrs.Fill;
            //polygon.Stroke = drawingParametrs.Stroke;
            //polygon.StrokeThickness = drawingParametrs.StrokeThickness;
            //polygon.StrokeDashCap = drawingParametrs.StrokeDashCap;
            //polygon.Opacity = drawingParametrs.Opacity;
            //polygon.Name = "Polygon" + 1.ToString();

            //foreach (var x in drawingParametrs.StrokeDashArray)
            //{
            //    polygon.StrokeDashArray.Add(x);
            //}

            foreach (Point point in points)
            {
                polygon.Points.Add(point);
            }

            canvas.Children.Add(polygon);
        }

        public void DrawPolyline(PointCollection points, DrawingParameters drawingParametrs)
        {
            Polyline polyline = new Polyline();

            //polyline.Fill = drawingParametrs.Fill;
            //polyline.Stroke = drawingParametrs.Stroke;
            //polyline.StrokeThickness = drawingParametrs.StrokeThickness;
            //polyline.StrokeDashCap = drawingParametrs.StrokeDashCap;
            //polyline.Opacity = drawingParametrs.Opacity;

            //foreach (var x in drawingParametrs.StrokeDashArray)
            //{
            //    polyline.StrokeDashArray.Add(x);
            //}

            foreach (Point point in points)
            {
                polyline.Points.Add(point);
            }

            canvas.Children.Add(polyline);
        }
    }
}