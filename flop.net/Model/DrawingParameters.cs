using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using flop.net.Annotations;

namespace flop.net.ViewModel.Models
{
    public class DrawingParameters : INotifyPropertyChanged
    {
        private Brush fill;

        public Brush Fill
        {
            get => fill;
            set
            {
                fill = value;
                OnPropertyChanged();
            }
        }

        private Brush stroke;
        public Brush Stroke
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}