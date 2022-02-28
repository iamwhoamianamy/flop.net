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
        public Stack<UserCommands> UndoStack { get; set; }
        public Stack<UserCommands> RedoStack { get; set; }
        public void UndoFunc()
        {
            if (UndoStack.Count > 0)
            {
                var undoCommand = UndoStack.Pop();
                RedoStack.Push(undoCommand);
                undoCommand.UnExecute.Execute(null);
                if (Figures.Count != 0)
                    Figures.Move(0, 0); // simulation of a collection change 
            }
        }

        public void RedoFunc()
        {
            if (RedoStack.Count > 0)
            {
                var redoCommand = RedoStack.Pop();
                UndoStack.Push(redoCommand);
                redoCommand.Execute.Execute(null);
                if (Figures.Count != 0)
                    Figures.Move(0, 0); // simulation of a collection change 
            }
        }

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
