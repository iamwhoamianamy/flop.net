using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using flop.net.Annotations;
using flop.net.Model;
using System.Windows.Media;
using flop.net.Enums;

namespace flop.net.ViewModel;

public class MainWindowVM : INotifyPropertyChanged
{
   public Stack<UserCommands> UndoStack { get; set; }
   public Stack<UserCommands> RedoStack { get; set; } 
   public Stack<Figure> DeletedFigures;
   private Figure activeFigure;
   public Layer ActiveLayer { get; set; }
   public ObservableCollection<Layer> Layers { get; set; }
   public Figure FigureOnCreating { get; set; }
   public ViewMode WorkingMode { get; set; }
   public FigureCreationMode СurrentFigureType { get; set; }

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

   private RelayCommand addRectangle;
   public RelayCommand AddRectangle
   {
      get
      {
         Action<object> redo = (_) =>
         {
            if (DeletedFigures.Count > 0)
            {
               activeFigure = DeletedFigures.Pop();
               ActiveLayer.Figures.Add(activeFigure);
            }
            else
               throw new InvalidOperationException();
         };

         Action<object> undo = (_) =>
         {
            if (!ActiveLayer.Figures.Contains(activeFigure))
               throw new InvalidOperationException();
            DeletedFigures.Push(activeFigure);
            ActiveLayer.Figures.Remove(activeFigure);
            if (ActiveLayer.Figures.Count > 0)
               activeFigure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
            else
               activeFigure = null;
         };

         return addRectangle ??= new RelayCommand(_ =>
         {
            RedoStack.Clear();
            Random r = new Random();
            int lwidth = r.Next(0, 500);
            int lheight = r.Next(0, 500);
            Point a = new Point(lwidth, lheight);
            Point b = new Point(lwidth + 20, lheight + 20);
            Polygon rectangle = PolygonBuilder.CreateRectangle(a, b);
            activeFigure = new Figure(rectangle, null);
            ActiveLayer.Figures.Add(activeFigure);
            UndoStack.Push(new UserCommands(redo, undo));
         });
      }
   }
   private RelayCommand rotateFigure;
   public RelayCommand RotateFigure
   {
      get
      {
         return rotateFigure ??= new RelayCommand(_ =>
         {
            RedoStack.Clear();
            if (activeFigure != null)
            {
               activeFigure.Geometric.Rotate(30);
               ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = activeFigure;
               UndoStack.Push(new UserCommands(_ => { activeFigure.Geometric.Rotate(30); },
                      _ => { activeFigure.Geometric.Rotate(-30); }));
               if (ActiveLayer.Figures.Count != 0)
                  ActiveLayer.Figures.Move(0, 0); // simulation of a collection change 
               }
         });
      }
   }

   private RelayCommand toggleRectangleCreation;
   public RelayCommand ToggleRectangleCreation
   {
      get
      {
         return toggleRectangleCreation ??= new RelayCommand(_ =>
         {
            WorkingMode = WorkingMode == ViewMode.Creation ? ViewMode.Default : ViewMode.Creation;
            СurrentFigureType = FigureCreationMode.Rectangle;
         });
      }
   }

   private RelayCommand toggleTriangleCreation;
   public RelayCommand ToggleTriangleCreation
   {
      get
      {
         return toggleTriangleCreation ??= new RelayCommand(_ =>
         {
            WorkingMode = WorkingMode == ViewMode.Creation ? ViewMode.Default : ViewMode.Creation;
            СurrentFigureType = FigureCreationMode.Triangle;
         });
      }
   }

   private RelayCommand toggleEllipseCreation;
   public RelayCommand ToggleEllipseCreation
   {
      get
      {
         return toggleEllipseCreation ??= new RelayCommand(_ =>
         {
            WorkingMode = WorkingMode == ViewMode.Creation ? ViewMode.Default : ViewMode.Creation;
            СurrentFigureType = FigureCreationMode.Ellipse;
         });
      }
   }

   private RelayCommand togglePolylineCreation;
   public RelayCommand TogglePolylineCreation
   {
      get
      {
         return togglePolylineCreation ??= new RelayCommand(_ =>
         {
            WorkingMode = WorkingMode == ViewMode.Creation ? ViewMode.Default : ViewMode.Creation;
            СurrentFigureType = FigureCreationMode.Polyline;
         });
      }
   }

   private RelayCommand togglePolygonCreation;
   public RelayCommand TogglePolygonCreation
   {
      get
      {
         return togglePolygonCreation ??= new RelayCommand(_ =>
         {
            WorkingMode = WorkingMode == ViewMode.Creation ? ViewMode.Default : ViewMode.Creation;
            СurrentFigureType = FigureCreationMode.Polygon;
         });
      }
   }

   private RelayCommand scaleFigure;
   public RelayCommand ScaleFigure
   {
      get
      {
         return scaleFigure ??= new RelayCommand(_ =>
         {
            RedoStack.Clear();
            if (activeFigure != null)
            {
               activeFigure.Geometric.Scale(new Point(2, 2));
               UndoStack.Push(new UserCommands(
                      _ => { activeFigure.Geometric.Scale(new Point(2, 2)); },
                      _ => { activeFigure.Geometric.Scale(new Point(0.5, 0.5)); }));
               if (ActiveLayer.Figures.Count != 0)
                  ActiveLayer.Figures.Move(0, 0); // simulation of a collection change 
               }
         });
      }
   }

   private RelayCommand moveFigure;
   public RelayCommand MoveFigure
   {
      get
      {
         return moveFigure ??= new RelayCommand(_ =>
         {
            RedoStack.Clear();
            if (activeFigure != null)
            {
               Random r = new Random();
               double x = r.Next(-10, 11);
               double y = r.Next(-10, 11);
               Vector v = new Vector(x, y);
               activeFigure.Geometric.Move(v);
               UndoStack.Push(new UserCommands(_ => { activeFigure.Geometric.Move(v); },
                      _ => { activeFigure.Geometric.Move(-v); }));
               if (ActiveLayer.Figures.Count != 0)
                  ActiveLayer.Figures.Move(0, 0); // simulation of a collection change 
               }
         });
      }
   }
   private RelayCommand deleteFigure;
   public RelayCommand DeleteFigure
   {
      get
      {
         Action<object> redo = (_) =>
         {
            if (!ActiveLayer.Figures.Contains(activeFigure))
               throw new InvalidOperationException();
            DeletedFigures.Push(activeFigure);
            ActiveLayer.Figures.Remove(activeFigure);
            if (ActiveLayer.Figures.Count > 0)
               activeFigure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
            else
               activeFigure = null;
         };

         Action<object> undo = (_) =>
         {
            if (DeletedFigures.Count > 0)
            {
               activeFigure = DeletedFigures.Pop();
               ActiveLayer.Figures.Add(activeFigure);
            }
            else
               throw new InvalidOperationException();
         };

         return deleteFigure ??= new RelayCommand(_ =>
         {
            RedoStack.Clear();
            if (activeFigure != null)
            {
               DeletedFigures.Push(activeFigure);
               if (ActiveLayer.Figures.Contains(activeFigure))
                  ActiveLayer.Figures.Remove(activeFigure);
               else
                  throw new InvalidOperationException();
               UndoStack.Push(new UserCommands(redo, undo));
               if (ActiveLayer.Figures.Count > 0)
                  activeFigure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
               else
                  activeFigure = null;
               if (ActiveLayer.Figures.Count != 0)
                  ActiveLayer.Figures.Move(0, 0); // simulation of a collection change 
               }

         });
      }
   }

   public MainWindowVM()
   {
      ActiveLayer = new Layer();
      ActiveLayer.Figures = new ObservableCollection<Figure>();
      RedoStack = new Stack<UserCommands>();
      UndoStack = new Stack<UserCommands>();
      DeletedFigures = new Stack<Figure>();
      activeFigure = null;
   }

   public event PropertyChangedEventHandler PropertyChanged;

   [NotifyPropertyChangedInvocator]
   protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }
}