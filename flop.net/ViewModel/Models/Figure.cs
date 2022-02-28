using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using flop.net.Model;
using System.Windows;

namespace flop.net.ViewModel.Models
{
    public class Figure : INotifyPropertyChanged
    {
        public enum FigureAction { MOVE, ROTATE, SCALE };

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public PointCollection Points { get; set; }

        private static IGeometric emptyGeometric = new Polygon(new PointCollection {new(-1, -1), new(-1, -1)}, true);
        private IGeometric saveGeometric;
        private IGeometric geometric;
        public IGeometric Geometric 
        {
            get => geometric;
            set
            {
                geometric = value;
                OnPropertyChanged();
            }
        }

        public DrawingParameters DrawingParameters { get; set; }

        public Figure(IGeometric geometric, DrawingParameters drawingParameters, PointCollection points)
        {
            Points = points;
            Geometric = geometric;
            DrawingParameters = drawingParameters;
        }

        public void CreateFigure()
        {
            Geometric = saveGeometric;
        }

        public void DeleteFigure()
        {
            saveGeometric = Geometric;
            Geometric = emptyGeometric;
        }

        public void ModifyFigure(FigureAction action, object parameter)
        {
            switch (action)
            {
                case FigureAction.MOVE:
                {
                    var vector = (Vector)parameter;
                    Geometric.Move(vector);
                    break;
                }
                case FigureAction.ROTATE:
                {
                    var angle = Convert.ToSingle(parameter);
                    Geometric.Rotate(angle);
                    break;
                }
                case FigureAction.SCALE:
                {
                    var p = (Point)parameter;
                    Geometric.Scale(p);
                    break;
                }
            }
        }
    }
}