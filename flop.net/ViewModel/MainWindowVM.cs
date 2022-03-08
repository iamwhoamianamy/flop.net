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
using System.Linq;
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
        get => new RelayCommand(_ => undoFunc());
        set => OnPropertyChanged();
    }

    public RelayCommand Redo
    {
        get => new RelayCommand(_ => redoFunc());
        set => OnPropertyChanged();
    }


    public RelayCommand CreateRectangle
    {
        get
        {
            return new RelayCommand(_ =>
           {
               redoStack.Clear();
               Random r = new Random();
               int lwidth = r.Next(0, 500);
               int lheight = r.Next(0, 500);
               Point a = new(lwidth, lheight);
               Point b = new(lwidth + 20, lheight + 20);
               Polygon rectangle = PolygonBuilder.CreateRectangle(a, b);
               Figure figure = new(rectangle, null);
               Figures.Add(figure);
               //undoStack.Push(new UserCommands(_ => { figure.CreateFigure(); }, _ => { figure.DeleteFigure(); }));
           });
        }
    }

    public RelayCommand RotateFigure
    {
        get
        {
            return new RelayCommand(_ =>
            {
               redoStack.Clear();
               Figure figure = Figures[Figures.Count - 1];
               figure.Geometric.Rotate(30);
               Figures[Figures.Count - 1] = figure;
                undoStack.Push(new UserCommands(_ => { figure.Geometric.Rotate(30); },
                   _ => { figure.Geometric.Rotate(-30); }));
            });
        }
    }

    public RelayCommand ScaleFigure
    {
        get
        {
            return new RelayCommand(_ =>
            {
                redoStack.Clear();
                Figure figure = Figures[Figures.Count - 1];
                figure.Geometric.Scale(new Point(2, 2));
                Figures[Figures.Count - 1] = figure;
                undoStack.Push(new UserCommands(
                    _ => { figure.Geometric.Scale(new Point(2, 2)); },
                    _ => { figure.Geometric.Scale(new Point(0.5, 0.5)); }));
            });
        }
    }

    public RelayCommand MoveFigure
    {
        get
        {
            return new RelayCommand(_ =>
           {
               redoStack.Clear();
               Figure figure = Figures[Figures.Count - 1];
               Random r = new Random();
               double x = r.Next(-10, 11);
               double y = r.Next(-10, 11);
               Vector v = new Vector(x, y);
               figure.Geometric.Move(v);
               Figures[Figures.Count - 1] = figure;
               undoStack.Push(new UserCommands(_ => { figure.Geometric.Move(v); },
                   _ => { figure.Geometric.Move(-v); }));
           });
        }
    }

    //public RelayCommand DeleteFigure
    //{
    //    get
    //    {
    //        return new RelayCommand(_ =>
    //       {
    //           redoStack.Clear();
    //           Figure figure = Figures.Last();
    //           Figures[Figures.Count - 1] = figure;
    //           //undoStack.Push(new UserCommands(_ => { figure.DeleteFigure(); }, _ => { figure.CreateFigure(); }));
    //       });
    //    }
    //}

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