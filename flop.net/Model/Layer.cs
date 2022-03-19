using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace flop.net.Model
{
    public class Layer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Figure> figures;
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
