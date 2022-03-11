using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using flop.net.Annotations;
using flop.net.Model;

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
   public Figure FigureOnCreating { get; set; }

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
         return new RelayCommand(_ =>
        {
           RedoStack.Clear();
           Random r = new Random();
           int lwidth = r.Next(0, 500);
           int lheight = r.Next(0, 500);
           Point a = new Point(lwidth, lheight);
           Point b = new Point(lwidth + 20, lheight + 20);
           Polygon rectangle = PolygonBuilder.CreateRectangle(a, b);
           Figure figure = new Figure(rectangle, null);
           ActiveLayer.Figures.Add(figure);
               //UndoStack.Push(new UserCommands( _ => { figure.CreateFigure(); }, _ => { figure.DeleteFigure(); }));
            });
      }
   }

   public RelayCommand RotateFigure
   {
      get
      {
         return new RelayCommand(_ =>
         {
            RedoStack.Clear();
            Figure figure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
            figure.Geometric.Rotate(30);
            ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = figure;
            UndoStack.Push(new UserCommands(_ => { figure.Geometric.Rotate(30); },
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
            RedoStack.Clear();
            Figure figure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
            figure.Geometric.Scale(new Point(2, 2));
            ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = figure;
            UndoStack.Push(new UserCommands(
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
           RedoStack.Clear();
           Figure figure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
           Random r = new Random();
           double x = r.Next(-10, 11);
           double y = r.Next(-10, 11);
           Vector v = new Vector(x, y);
           figure.Geometric.Move(v);
           ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = figure;
           UndoStack.Push(new UserCommands(_ => { figure.Geometric.Move(v); },
               _ => { figure.Geometric.Move(-v); }));
        });
      }
   }

   // public RelayCommand DeleteFigure
   //{
   //    get
   //    {
   //        return new RelayCommand( _ =>
   //        {
   //            RedoStack.Clear();
   //            Figure figure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
   //            figure.DeleteFigure();
   //            ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = figure;
   //            UndoStack.Push(new UserCommands( _ => { figure.DeleteFigure(); }, _ => { figure.CreateFigure(); }));
   //        });
   //    }
   //}

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