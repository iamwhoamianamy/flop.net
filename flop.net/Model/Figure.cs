﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

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

        public Figure(IGeometric geometric, DrawingParameters drawingParameters)
        {
            Geometric = geometric;
            DrawingParameters = drawingParameters;
        }
    }
}