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
using flop.net.ViewModel;

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
    // public ObservableCollection<Figure> Figures { get; set; }
    public Stack<UserCommands> UndoStack { get; set; }
    public Stack<UserCommands> RedoStack { get; set; }
    public Layer ActiveLayer { get; set; }
    public ObservableCollection<Layer> Layers { get; set; }
    public RelayCommand Undo
    {
        get => new RelayCommand(_ => UndoFunc());
        set => OnPropertyChanged();
    }

    public RelayCommand Redo
    {
        get => new RelayCommand(_ => RedoFunc());
        set => OnPropertyChanged();
    }

    public void SetActiveLayer(Layer layer)
    {
        ActiveLayer = layer;
    }

    public void AddLayer(Layer layer)
    {
        Layers.Add(layer);
    }

    public void RemoveLayer(Layer layer)
    {
        Layers.Remove(layer);
    }

    public void UndoFunc()
    {
        if (UndoStack.Count > 0)
        {
            var undoCommand = UndoStack.Pop();
            RedoStack.Push(undoCommand);
            undoCommand.UnExecute.Execute(null);
            if (ActiveLayer.Figures.Count != 0)
                ActiveLayer.Figures.Move(0, 0); // simulation of a collection change 
        }
    }

    public void RedoFunc()
    {
        if (RedoStack.Count > 0)
        {
            var redoCommand = RedoStack.Pop();
            UndoStack.Push(redoCommand);
            redoCommand.Execute.Execute(null);
            if (ActiveLayer.Figures.Count != 0)
                ActiveLayer.Figures.Move(0, 0); // simulation of a collection change 
        }
    }


    public RelayCommand AddRectangle
    {
        get
        {
            return new RelayCommand( _ =>
            {
                RedoStack.Clear();
                Random r = new Random();
                int lwidth = r.Next(0, 500);
                int lheight = r.Next(0, 500);
                Point a = new Point(lwidth, lheight);
                Point b = new Point(lwidth + 20, lheight + 20);
                Polygon rectangle = PolygonBuilder.CreateRectangle(a, b);
                Figure figure = new Figure(rectangle, null, null);
                ActiveLayer.Figures.Add(figure);
                UndoStack.Push(new UserCommands( _ => { figure.CreateFigure(); }, _ => { figure.DeleteFigure(); }));
            });
        }
    }

    public RelayCommand Rotate
    {
        get
        {
            return new RelayCommand( _ =>
            {
                RedoStack.Clear();
                Figure figure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
                figure.ModifyFigure(Figure.FigureAction.ROTATE, 30);
                ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = figure;
                UndoStack.Push(new UserCommands( _ => { figure.ModifyFigure(Figure.FigureAction.ROTATE, 30); }, 
                                                 _ => { figure.ModifyFigure(Figure.FigureAction.ROTATE, -30); }));
            });
        }
    }

    public RelayCommand Scale
    {
        get
        {
            return new RelayCommand( _ =>
            {
                RedoStack.Clear();
                Figure figure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
                figure.ModifyFigure(Figure.FigureAction.SCALE, new Point(2,2));
                ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = figure;
                UndoStack.Push(new UserCommands( _ => { figure.ModifyFigure(Figure.FigureAction.SCALE, new Point(2, 2)); },
                                                 _ => { figure.ModifyFigure(Figure.FigureAction.SCALE, new Point(0.5, 0.5)); }));
            });
        }
    }

    public RelayCommand Move
    {
        get
        {
            return new RelayCommand( _ =>
            {
                RedoStack.Clear();
                Figure figure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
                Random r = new Random();
                double x =  r.Next(-10, 11);
                double y = r.Next(-10, 11);
                Vector v = new Vector(x, y);
                figure.ModifyFigure(Figure.FigureAction.MOVE, v);
                ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = figure;
                UndoStack.Push(new UserCommands( _ => { figure.ModifyFigure(Figure.FigureAction.MOVE, v); },
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
                RedoStack.Clear();
                Figure figure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
                figure.DeleteFigure();
                ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = figure;
                UndoStack.Push(new UserCommands( _ => { figure.DeleteFigure(); }, _ => { figure.CreateFigure(); }));
            });
        }
    }

    public MainWindowVM()
    {
        ActiveLayer = new Layer();
        ActiveLayer.Figures = new ObservableCollection<Figure>();
        RedoStack = new Stack<UserCommands>();
        UndoStack = new Stack<UserCommands>();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}