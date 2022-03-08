using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using flop.net.Annotations;
using flop.net.Model;
using flop.net.View;
using System.Windows.Input;
using flop.net.ViewModel.Models;
using System.Diagnostics;
using System.Windows.Media;

namespace flop.net.ViewModel;

public class UserCommands
{
    private Action<object> execute;
    private Action<object> unexecute;
    public UserCommands(Action<object> execute, Action<object> unexecute)
    {
        this.execute = execute;
        this.unexecute = unexecute;
    }

    public RelayCommand Execute
    {
       get => new RelayCommand(execute);
    }

    public RelayCommand UnExecute
    {
        get => new RelayCommand(unexecute);
    }
}

public class MainWindowVM : INotifyPropertyChanged
{
    public ObservableCollection<Figure> Figures { get; set; }
    public Stack<UserCommands> undoStack;
    public Stack<UserCommands> redoStack;

    private void undoFunc()
    {
        if (undoStack.Count > 0)
        {
            var undoCommand = undoStack.Pop();
            redoStack.Push(undoCommand);
            undoCommand.UnExecute.Execute(null);
            if (Figures.Count != 0)
                Figures.Move(0, 0); // simulation of a collection change 
        }
    }

    private void redoFunc()
    {
        if (redoStack.Count > 0)
        {
            var redoCommand = redoStack.Pop();
            undoStack.Push(redoCommand);
            redoCommand.Execute.Execute(null);
            if (Figures.Count != 0)
                Figures.Move(0, 0); // simulation of a collection change 
        }
    }

    public RelayCommand Undo
    {
        get => new RelayCommand( _ => undoFunc());
        set => OnPropertyChanged();
    }

    public RelayCommand Redo
    {
        get => new RelayCommand( _ => redoFunc());
        set => OnPropertyChanged();
    }


    public RelayCommand DrawRectangle
    {
        get
        {
            return new RelayCommand( _ =>
            {
                redoStack.Clear();
                Point a = new Point(0, 0);
                Point b = new Point(0, 0);
                Polygon rectangle = PolygonBuilder.CreateRectangle(a, b);
                Figure figure = new Figure(rectangle, null, rectangle.Points, a); 
                Figures.Add(figure);
                //undoStack.Push(new UserCommands( _ => { figure.CreateFigure(); }, _ => { figure.DeleteFigure(); }));
            });
        }
    }
    public void Draw(Point a, Point b)
    {
        Point A = new Point(0, 0);
        Point B = new Point(b.X - a.X, b.Y - a.Y);

        Polygon rectangle = PolygonBuilder.CreateRectangle(A, B);
        Figures[Figures.Count - 1] = new Figure(rectangle, null, rectangle.Points, a);
        //Figures[Figures.Count - 1].Position = new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
    }

    public RelayCommand DoRotate
    {
        get
        {
            return new RelayCommand( _ =>
            {
                redoStack.Clear();
                Figure figure = Figures[Figures.Count - 1];
                figure.ModifyFigure(Figure.FigureAction.ROTATE, 30);
                Figures[Figures.Count - 1] = figure;
                undoStack.Push(new UserCommands( _ => { figure.ModifyFigure(Figure.FigureAction.ROTATE, 30); }, 
                                                 _ => { figure.ModifyFigure(Figure.FigureAction.ROTATE, -30); }));
            });
        }
    }

    public RelayCommand DoScale
    {
        get
        {
            return new RelayCommand( _ =>
            {
                redoStack.Clear();
                Figure figure = Figures[Figures.Count - 1];
                figure.ModifyFigure(Figure.FigureAction.SCALE, new Point(2,2));
                Figures[Figures.Count - 1] = figure;
                undoStack.Push(new UserCommands( _ => { figure.ModifyFigure(Figure.FigureAction.SCALE, new Point(2, 2)); },
                                                 _ => { figure.ModifyFigure(Figure.FigureAction.SCALE, new Point(0.5, 0.5)); }));
            });
        }
    }

    public RelayCommand DoMove
    {
        get
        {
            return new RelayCommand( _ =>
            {
                redoStack.Clear();
                Figure figure = Figures[Figures.Count - 1];
                Random r = new Random();
                double x =  r.Next(-10, 11);
                double y = r.Next(-10, 11);
                Vector v = new Vector(x, y);
                figure.ModifyFigure(Figure.FigureAction.MOVE, v);
                Figures[Figures.Count - 1] = figure;
                undoStack.Push(new UserCommands( _ => { figure.ModifyFigure(Figure.FigureAction.MOVE, v); },
                                                 _ => { figure.ModifyFigure(Figure.FigureAction.MOVE, -v); }));

            });
        }
    }

     public RelayCommand DeleteFigure
    {
        get
        {
            return new RelayCommand( _ =>
            {
                redoStack.Clear();
                Figure figure = Figures[Figures.Count - 1];
                figure.DeleteFigure();
                Figures[Figures.Count - 1] = figure;
                undoStack.Push(new UserCommands( _ => { figure.DeleteFigure(); }, _ => { figure.CreateFigure(); }));
            });
        }
    }

    public MainWindowVM()
    {
        Figures = new ObservableCollection<Figure>();
        redoStack = new Stack<UserCommands>();
        undoStack = new Stack<UserCommands>();
    }
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}