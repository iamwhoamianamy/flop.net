using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using flop.net.Model;

namespace flop.net.ViewModel.Models
{
    public class Figure : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private PointCollection Points { get; }
        private IGeometric Geometric { get; }
        private DrawingParameters DrawingParameters { get; }

        public Figure()
        {
        }
        
        public Figure(IGeometric geometric, DrawingParameters drawingParameters, PointCollection points)
        {
            Points = points;
            Geometric = geometric;
            DrawingParameters = drawingParameters;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}