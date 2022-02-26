using System.Collections.Generic;
using System.Windows.Media;
using flop.net.ViewModel.Enums;

namespace flop.net.ViewModel.Models
{
    public abstract class DrawingParameters
    {
        public Brush Fill { get; set; }
        public Brush Stroke { get; set; }
        public int StrokeThickness { get; set; }
        public PenLineCap PenLineCap { get; set; }
        public List<double> StrokeDashArray { get; set; }
        public double Opacity { get; set; }
        public int ZIndex { get; set; }
    }
}