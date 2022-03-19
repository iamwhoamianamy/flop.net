using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using flop.net.Annotations;

namespace flop.net.Model
{
    public class DrawingParameters : INotifyPropertyChanged
    {
        private Color fill;
        public Color Fill
        {
            get => fill;
            set
            {
                fill = value;
                OnPropertyChanged();
            }
        }

        private Color stroke;
        public Color Stroke
        {
            get => stroke;
            set
            {
                stroke = value;
                OnPropertyChanged();
            }
        }

        private int strokeThickness;
        public int StrokeThickness
        {
            get => strokeThickness;
            set
            {
                strokeThickness = value;
                OnPropertyChanged();
            }
        }

        private PenLineCap penLineCap;
        public PenLineCap PenLineCap
        {
            get => penLineCap;
            set
            {
                penLineCap = value;
                OnPropertyChanged();
            }
        }

        private List<double> strokeDashArray;
        public List<double> StrokeDashArray
        {
            get => strokeDashArray;
            set
            {
                strokeDashArray = value;
                OnPropertyChanged();
            }
        }

        private double opacity;
        public double Opacity
        {
            get => opacity;
            set
            {
                opacity = value;
                OnPropertyChanged();
            }
        }

        private int zIndex;
        public int ZIndex
        {
            get => zIndex;
            set
            {
                zIndex = value;
                OnPropertyChanged();
            }
        }
      public DrawingParameters()
      {
         this.Fill = Colors.Black;
         this.Stroke = Colors.White;
         this.StrokeThickness = 0;
         this.StrokeDashArray = new List<double>();
         this.Opacity = 1;
         this.PenLineCap = PenLineCap.Flat;
      }
      public DrawingParameters(DrawingParameters parameters)
      {
         this.Fill = parameters.Fill;
         this.Stroke = parameters.Stroke;
         this.StrokeThickness = parameters.StrokeThickness;
         this.StrokeDashArray = parameters.StrokeDashArray;
         this.Opacity = parameters.Opacity;
         this.ZIndex = parameters.ZIndex;
         this.PenLineCap = parameters.PenLineCap;
      }

        

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}