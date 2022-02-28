using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using flop.net.Model;
using System.Windows;
using System.Diagnostics;
namespace flop.net.ViewModel.Models
{
    public class Figure : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public enum FigureAction { MOVE, ROTATE, SCALE };

        public PointCollection Points { get; set; }
        private IGeometric geometric;
        public IGeometric Geometric 
        {
            get
            {
                return geometric;
            }
            set
            {
                geometric = value;
                OnPropertyChanged();
            }
        }
        public DrawingParameters DrawingParameters { get; set; }

        public Figure()
        {
        }
        static Figure()
        {
            EmptyGeometric = new Rectangle(new System.Windows.Point(-1, -1), new System.Windows.Point(-1, -1));
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

        private static IGeometric EmptyGeometric;
        private IGeometric SaveGeometric;

        public void CreateFigure()
        {
            Geometric = SaveGeometric;
        }

        public void DeleteFigure()
        {
            SaveGeometric = Geometric;
            Geometric = EmptyGeometric;
        }

        public void ModifyFigure(FigureAction action, object parameter)
        {
            if (action == FigureAction.MOVE)
            {
                Vector vector = (Vector)parameter;
                if (vector != null)
                    Geometric.Move(vector);
            }
            if (action == FigureAction.ROTATE)
            {
                double angle;
                double.TryParse(parameter.ToString(), out angle);
                Geometric.Rotate(angle);
            }
            if (action == FigureAction.SCALE)
            {
                Point p = (Point)parameter;
                if (p != null)
                    Geometric.Scale(p);
            }
        }
    }
}