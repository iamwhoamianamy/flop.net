using System.ComponentModel;
using System.Runtime.CompilerServices;
using flop.net.Model;

namespace flop.net.ViewModel.Models
{
    public class Figure : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private PointsCollection Points { get; }
        private IGeometric Geometric { get; }
        private DrawingParameters DrawingParameters { get; }

        public Figure()
        {
        }
        
        public Figure(IGeometric geometric, DrawingParameters drawingParameters, PointsCollection points)
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