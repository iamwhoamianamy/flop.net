using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using flop.net.Enums;

namespace flop.net.Model
{
    public class Figure : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private IGeometric geometric;
        public FigureCreationMode FigureType { get; private set; }
        public IGeometric Geometric 
        {
            get => geometric;
            set
            {
                geometric = value;
                OnPropertyChanged();
            }
        }

        private DrawingParameters drawingParameters;
        public DrawingParameters DrawingParameters
        {
            get => drawingParameters;
            set
            {
                drawingParameters = value;
                OnPropertyChanged();
            }
        }

        public Figure(IGeometric geometric, DrawingParameters drawingParameters, FigureCreationMode figureType)
        {
            Geometric = geometric;
            DrawingParameters = drawingParameters;
            FigureType = figureType;
        }
    }
}