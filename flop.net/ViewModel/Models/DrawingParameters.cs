using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using flop.net.Annotations;
using flop.net.ViewModel.Enums;

namespace flop.net.ViewModel.Models
{
    public abstract class DrawingParameters : INotifyPropertyChanged
    {
        public Brush Fill { get; set; }
        public Brush Stroke { get; set; }
        public int StrokeThickness { get; set; }
        public PenLineCap PenLineCap { get; set; }
        public List<double> StrokeDashArray { get; set; }
        public double Opacity { get; set; }
        public int ZIndex { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}