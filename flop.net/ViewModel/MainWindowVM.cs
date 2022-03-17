using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using flop.net.Annotations;
using flop.net.Model;
using System.Windows.Media;
using flop.net.Enums;

namespace flop.net.ViewModel;

public class MainWindowVM : INotifyPropertyChanged
{
   private const double EpsIsIn = double.MinValue;

   private List<RelayCommand> drawingCommands;
   private List<RelayCommand> modifyingCommands;
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

   private DrawingParameters creationDrawingParameters;
   public DrawingParameters CreationDrawingParameters
   {
      get => creationDrawingParameters ??= new DrawingParameters();
      set
      {
         creationDrawingParameters = value;
         OnPropertyChanged();
      }
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

   private bool isModifyingAvailable(object obj)
   {
      return activeFigure != null;
   }

   private void switchButtonSelection(RelayCommand selectedCommand, List<RelayCommand> commandsInRibbon)
   {
      foreach (var command in commandsInRibbon)
      {
         if (selectedCommand == command)
            command.IsPressed = true;
         else
            command.IsPressed = false;
      }
   }

   public Figure ActiveFigure { get; set; }
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
            СurrentFigureType = FigureCreationMode.Rectangle;
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

   public RelayCommand BeginFigureCreation
   {
      get
      {
         return new RelayCommand(_ =>
         {
            switch (СurrentFigureType)
            {
               case FigureCreationMode.Triangle:
                  ActiveLayer.Figures.Add(new Figure(PolygonBuilder.CreateTriangle(new Point(0, 0), new Point(0, 0)), CreationDrawingParameters));
                  break;
               case FigureCreationMode.Ellipse:
                  ActiveLayer.Figures.Add(new Figure(PolygonBuilder.CreateEllipse(new Point(0, 0), new Point(0, 0)), CreationDrawingParameters));
                  break;
               case FigureCreationMode.Polygon:
                  break;
               case FigureCreationMode.Polyline:
                  break;
               case FigureCreationMode.Rectangle:
                  ActiveLayer.Figures.Add(new Figure(PolygonBuilder.CreateRectangle(new Point(0, 0), new Point(0, 0)), CreationDrawingParameters));
                  break;
               case FigureCreationMode.None:
                  break;
            }
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
   public RelayCommand OnFigureCreation
   {
      get
      {
         return new RelayCommand(obj =>
         {
            var points = obj as (Point, Point)?;
            switch (СurrentFigureType)
            {
               case FigureCreationMode.Triangle:
                  ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = new Figure(PolygonBuilder.CreateTriangle(points.Value.Item1, points.Value.Item2), CreationDrawingParameters);
                  break;
               case FigureCreationMode.Ellipse:
                  ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = new Figure(PolygonBuilder.CreateEllipse(points.Value.Item1, points.Value.Item2), CreationDrawingParameters);
                  break;
               case FigureCreationMode.Polygon:
                  break;
               case FigureCreationMode.Polyline:
                  break;
               case FigureCreationMode.Rectangle:
                  ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = new Figure(PolygonBuilder.CreateRectangle(points.Value.Item1, points.Value.Item2), CreationDrawingParameters);
                  break;
               case FigureCreationMode.None:
                  break;
            }
         });
      }
   }

   private RelayCommand toggleRectangleCreation;
   public RelayCommand ToggleRectangleCreation
   {
      get
      {
         toggleRectangleCreation ??= new RelayCommand(_ =>
         {
            switchButtonSelection(moveFigure, drawingCommands);
            WorkingMode = WorkingMode == ViewMode.Creation ? ViewMode.Default : ViewMode.Creation;
            СurrentFigureType = FigureCreationMode.Rectangle;
         });
         drawingCommands.Add(toggleRectangleCreation);
         return toggleRectangleCreation;
      }
   }

   private RelayCommand toggleTriangleCreation;
   public RelayCommand ToggleTriangleCreation
   {
      get
      {
         toggleTriangleCreation ??= new RelayCommand(_ =>
         {
            switchButtonSelection(toggleTriangleCreation, drawingCommands);

            WorkingMode = WorkingMode == ViewMode.Creation ? ViewMode.Default : ViewMode.Creation;
            СurrentFigureType = FigureCreationMode.Triangle;
         });
         drawingCommands.Add(toggleTriangleCreation);
         return toggleTriangleCreation;
      }
   }

   private RelayCommand toggleEllipseCreation;
   public RelayCommand ToggleEllipseCreation
   {
      get
      {
         toggleEllipseCreation ??= new RelayCommand(_ =>
         {
            WorkingMode = WorkingMode == ViewMode.Creation ? ViewMode.Default : ViewMode.Creation;
            СurrentFigureType = FigureCreationMode.Ellipse;
         });
         drawingCommands.Add(toggleEllipseCreation);
         return toggleEllipseCreation;
      }
   }

   private RelayCommand togglePolylineCreation;
   public RelayCommand TogglePolylineCreation
   {
      get
      {
         togglePolylineCreation ??= new RelayCommand(_ =>
         {
            WorkingMode = WorkingMode == ViewMode.Creation ? ViewMode.Default : ViewMode.Creation;
            СurrentFigureType = FigureCreationMode.Polyline;
         });
         drawingCommands.Add(togglePolylineCreation);
         return togglePolylineCreation;
      }
   }

   private RelayCommand togglePolygonCreation;
   public RelayCommand TogglePolygonCreation
   {
      get
      {
         togglePolygonCreation ??= new RelayCommand(_ =>
         {
            WorkingMode = WorkingMode == ViewMode.Creation ? ViewMode.Default : ViewMode.Creation;
            СurrentFigureType = FigureCreationMode.Polygon;
         });
         drawingCommands.Add(togglePolygonCreation);
         return togglePolygonCreation;
      }
   }

   private RelayCommand rotateFigure;
   public RelayCommand RotateFigure
   {
      get
      {
         rotateFigure ??= new RelayCommand(_ =>
         {
            if (activeFigure != null)
            {
               RedoStack.Clear();

               switchButtonSelection(rotateFigure, modifyingCommands);

               activeFigure.Geometric.Rotate(30);
               ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = activeFigure;
               UndoStack.Push(new UserCommands(_ => { activeFigure.Geometric.Rotate(30); },
                      _ => { activeFigure.Geometric.Rotate(-30); }));
               if (ActiveLayer.Figures.Count != 0)
                  ActiveLayer.Figures.Move(0, 0); // simulation of a collection change 
            }
         }, isModifyingAvailable);
         modifyingCommands.Add(rotateFigure);
         return rotateFigure;
      }
   }


   private RelayCommand scaleFigure;
   public RelayCommand ScaleFigure
   {
      get
      {
         scaleFigure ??= new RelayCommand(_ =>
         {
            if (activeFigure != null)
            {
               RedoStack.Clear();
               switchButtonSelection(scaleFigure, modifyingCommands);

               activeFigure.Geometric.Scale(new Point(2, 2));
               UndoStack.Push(new UserCommands(
                      _ => { activeFigure.Geometric.Scale(new Point(2, 2)); },
                      _ => { activeFigure.Geometric.Scale(new Point(0.5, 0.5)); }));
               if (ActiveLayer.Figures.Count != 0)
                  ActiveLayer.Figures.Move(0, 0); // simulation of a collection change 
            }
         }, isModifyingAvailable);
         modifyingCommands.Add(scaleFigure);
         return scaleFigure;
      }
   }

   private RelayCommand moveFigure;
   public RelayCommand MoveFigure
   {
      get
      {
         moveFigure ??= new RelayCommand(_ =>
         {
            if (activeFigure != null)
            {
               RedoStack.Clear();
               switchButtonSelection(moveFigure, modifyingCommands);
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
         }, isModifyingAvailable);
         modifyingCommands.Add(moveFigure);
         return moveFigure;
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

         deleteFigure ??= new RelayCommand(_ =>
         {
            if (activeFigure != null)
            {
               RedoStack.Clear();
               switchButtonSelection(deleteFigure, modifyingCommands);
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
         }, isModifyingAvailable);
         modifyingCommands.Add(deleteFigure);
         return deleteFigure;
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
      СurrentFigureType = FigureCreationMode.None;
      drawingCommands = new List<RelayCommand> { };
      modifyingCommands = new List<RelayCommand> { };
      CreationDrawingParameters.Fill = Colors.Green;
      CreationDrawingParameters.Stroke = Colors.Green;
   }

   public event PropertyChangedEventHandler PropertyChanged;

   [NotifyPropertyChangedInvocator]
   protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }
}