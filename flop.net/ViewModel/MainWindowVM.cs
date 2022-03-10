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

public enum ViewModelMode
{
   Default,
   Creation,
   Moving,
   Rotating,
   Scaling
}

public class MainWindowVM : INotifyPropertyChanged
{
   public Stack<UserCommands> UndoStack { get; set; }
   public Stack<UserCommands> RedoStack { get; set; }
   public Layer ActiveLayer { get; private set; }
   public Figure ActiveFigure { get; private set; }
   public ObservableCollection<Layer> Layers { get; set; }
   public ViewModelMode Mode { get; set; } = ViewModelMode.Default;
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

   public RelayCommand toggleRectangleCreation;
   public RelayCommand ToggleRectangleCreation => toggleRectangleCreation = new RelayCommand(_ =>
   {
      if (Mode == ViewModelMode.Default)
      {
         Mode = ViewModelMode.Creation;
      }
      else
      {
         Mode = ViewModelMode.Default;
      }
   });

   public RelayCommand ToggleMoving
   {
      get
      {
         return new RelayCommand(_ =>
         {
            if (Mode == ViewModelMode.Default)
            {
               Mode = ViewModelMode.Moving;
            }
            else
            {
               Mode = ViewModelMode.Default;
            }
         });
      }
   }

   public RelayCommand BeginRectangleCreation
   {
      get
      {
         return new RelayCommand(_ =>
         {
            ActiveLayer.Figures.Add(new Figure(PolygonBuilder.CreateRectangle(new Point(0, 0), new Point(0, 0)), null));
         });
      }
   }

   public RelayCommand BeginActiveFigureMoving
   {
      get
      {
         return new RelayCommand(_ =>
         {

         });
      }
   }

   public RelayCommand OnActiveFigureMoving
   {
      get
      {
         return new RelayCommand(obj =>
         {
            var coords = obj as (Point, Point)?;

            var mousePreviousCoords = coords.Value.Item1;
            var mouseCurrentCoords = coords.Value.Item2;

            var delta = mouseCurrentCoords - mousePreviousCoords;

            ActiveFigure.Geometric.Move(delta);

            ActiveLayer.Figures.Add(null);
            ActiveLayer.Figures.RemoveAt(ActiveLayer.Figures.Count - 1);
         });
      }
   }

   public RelayCommand EndActiveFigureMoving
   {
      get
      {
         return new RelayCommand(_ =>
         {

         });
      }
   }

   public RelayCommand OnRectangleCreation
   {
      get
      {
         return new RelayCommand(obj =>
         {
            var points = obj as (Point, Point)?;
            ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = new Figure(PolygonBuilder.CreateRectangle(points.Value.Item1, points.Value.Item2), null);
         });
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

   public RelayCommand SetActiveFigure
   {
      get
      {
         return new RelayCommand(obj =>
         {
            var mouseCoords = obj as Point?;

            foreach (var figure in ActiveLayer.Figures)
            {
               if (figure.Geometric.IsIn(mouseCoords.Value, 1e-1))
               {
                  ActiveFigure = figure;
                  break;
               }
            }
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