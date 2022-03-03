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

public class MainWindowVM : INotifyPropertyChanged
{
    public ObservableCollection<Figure> Figures { get; set; }

    //public RelayCommand Undo
    //{
    //    get => new (obj => undoFunc());
    //    set => OnPropertyChanged();
    //}

    //public RelayCommand Redo
    //{
    //    get => new (obj => redoFunc());
    //    set => OnPropertyChanged();
    //}

    private CommandFlop createRectangle;
    public CommandFlop CreateRectangle
    {
        get
        {
            createRectangle ??= new CommandFlop(obj =>
            {
                Random r = new Random();
                int lwidth = r.Next(0, 500);
                int lheight = r.Next(0, 500);
                Point a = new(lwidth, lheight);
                Point b = new(lwidth + 20, lheight + 20);
                Polygon rectangle = PolygonBuilder.CreateRectangle(a, b);
                Figure figure = new(rectangle, null);
                Figures.Add(figure);
             });
            createRectangle.ExecuteCommand += CommandsList.Instance.CommandExecuted;
            return createRectangle;
        }
    }

    private void CreateRectangleOnExecuteCommand(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    public RelayCommand RotateFigure
    {
        get
        {
            return new RelayCommand(obj =>
            {
               Figure figure = Figures[Figures.Count - 1];
               figure.Geometric.Rotate(30);
               Figures[Figures.Count - 1] = figure;
                //undoStack.Push(new CommandFlop(_ => { figure.Geometric.Rotate(30); },
                //   _ => { figure.Geometric.Rotate(-30); }));
            });
        }
    }

    public RelayCommand ScaleFigure
    {
        get
        {
            return new RelayCommand(obj =>
            {
                Figure figure = Figures[Figures.Count - 1];
                figure.Geometric.Scale(new Point(2, 2));
                Figures[Figures.Count - 1] = figure;
                //undoStack.Push(new CommandFlop(
                //    _ => { figure.Geometric.Scale(new Point(2, 2)); },
                //    _ => { figure.Geometric.Scale(new Point(0.5, 0.5)); }));
            });
        }
    }

    public RelayCommand MoveFigure
    {
        get
        {
            return new RelayCommand(_ =>
           {
               Figure figure = Figures[Figures.Count - 1];
               Random r = new Random();
               double x = r.Next(-10, 11);
               double y = r.Next(-10, 11);
               Vector v = new Vector(x, y);
               figure.Geometric.Move(v);
               Figures[Figures.Count - 1] = figure;
               //undoStack.Push(new CommandFlop(_ => { figure.Geometric.Move(v); },
               //    _ => { figure.Geometric.Move(-v); }));
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
    //           //undoStack.Push(new CommandFlop(_ => { figure.DeleteFigure(); }, _ => { figure.CreateFigure(); }));
    //       });
    //    }
    //}

    public MainWindowVM()
    {
        Figures = new ObservableCollection<Figure>();
    }
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}