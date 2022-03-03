﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using flop.net.Model;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System;

namespace flop.net.ViewModel.Models
{
    public class Figure : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public enum FigureAction { MOVE, ROTATE, SCALE };

        public PointCollection Points { get; set; }
        Point position;
        public Point Position
        {
            get { return position; }
            set 
            {
                if (position != value)
                {
                    position = value; 
                    OnPropertyChanged();
                }
            }
        }
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

        public ICommand RequestMove { get; }
        public Figure()
        {            
        }
        void MoveTo(Point newPosition)
        {
            Position = newPosition;
        }
        static Figure()
        {
            EmptyGeometric = new Polygon(new PointCollection(){ new Point(-1, -1), new Point(-1, -1) }, true);
        }
        public Figure(IGeometric geometric, DrawingParameters drawingParameters, PointCollection points)
        {
            Points = points;
            Geometric = geometric;
            DrawingParameters = drawingParameters;
            Random r = new Random();
            Position = new Point(r.Next(0, 500), r.Next(0, 500));
            RequestMove = new SimpleCommand<Point>(MoveTo);
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