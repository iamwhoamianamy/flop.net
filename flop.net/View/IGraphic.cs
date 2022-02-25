using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using flop.net.ViewModel.Models;

namespace flop.net.View
{
    public interface IGraphic
    {
        void DrawPolyline(List<Point> points, DrawingParameters drawingParametrs);
        void DrawPolygon(PointCollection points);
    }
    public class Graphic : IGraphic
    {
        Canvas canvas;
        public Graphic(Canvas canvas)
        {
            this.canvas = canvas;
        }
        public void DrawPolygon(PointCollection points)
        {
            Polygon polygon = new Polygon();

            polygon.Fill = Brushes.Green;
            //polygon.Stroke = drawingParametrs.Fill;
            //polygon.StrokeThickness = drawingParametrs.Fill;
            //polygon.StrokeDashCap = drawingParametrs.Fill;
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

        public void DrawPolyline(List<Point> points, DrawingParameters drawingParametrs)
        {
            Polyline polyline = new Polyline();

            //polyline.Fill = drawingParametrs.Fill;
            //polyline.Stroke = drawingParametrs.Fill;
            //polyline.StrokeThickness = drawingParametrs.Fill;
            //polyline.StrokeDashCap = drawingParametrs.Fill;
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