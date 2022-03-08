using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace flop.net.ViewModel.Models
{
    public class Layer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Figure> figures { get; set; }
        public ObservableCollection<Figure> Figures 
        {
            get => this.figures; 
            set 
            { 
                this.figures = value;
                OnPropertyChanged();
            } 
        }
        // TODO: Добавить св-ва необходимые бригаде GUI
        public void GetFigure()
        {
            // TODO: Обговорить с биргадой IO, в каком формате необходимо передавать фигуру
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
