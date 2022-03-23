﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using flop.net.Annotations;
using flop.net.Model;
using System.Windows.Media;
using System.Xml;
using flop.net.Enums;
using ControlzEx.Theming;
using flop.net.Save;
using Microsoft.Win32;

namespace flop.net.ViewModel;

public class MainWindowVM : INotifyPropertyChanged
{
   private const double EpsIsIn = double.MinValue;

   private List<RelayCommand> palletCommands;
   public Stack<UserCommands> UndoStack { get; set; }
   public Stack<UserCommands> RedoStack { get; set; }
   public Stack<Figure> DeletedFigures;
   private Figure activeFigure;
   public Figure ActiveFigure
   {
      get { return activeFigure; }
      set
      {
         activeFigure = value;
         OnPropertyChanged();
      }
   }
   public Layer ActiveLayer { get; set; }
   public TrulyObservableCollection<Layer> Layers { get; set; }
   public Figure FigureOnCreating { get; set; }
   public ViewMode WorkingMode { get; set; }
   public FigureCreationMode СurrentFigureType { get; set; }
   public ViewMode Mode { get; set; } = ViewMode.Default;

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
      get => creationDrawingParameters;
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

         OnPropertyChanged();
      }
   }

   public void RedoFunc()
   {
      if (RedoStack.Count > 0)
      {
         var redoCommand = RedoStack.Pop();
         UndoStack.Push(redoCommand);
         redoCommand.Execute.Execute(null);

         OnPropertyChanged();
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
         if (selectedCommand != command)
            command.IsPressed = false;
      }
   }
   public RelayCommand SetActiveFigure
   {
      get
      {
         return new RelayCommand(obj =>
         {
            var mouseCoords = obj as Point?;

            ActiveFigure = null;

            var ReverseFiguresList = ActiveLayer.Figures.Reverse<Figure>();
            foreach (var figure in ReverseFiguresList)
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

   private RelayCommand toggleMoving;
   public RelayCommand ToggleMoving
   {
      get
      {
         toggleMoving ??= new RelayCommand(_ =>
         {
            switchButtonSelection(toggleMoving, palletCommands);
            if (WorkingMode != ViewMode.Moving)
               WorkingMode = ViewMode.Moving;
            else if (WorkingMode == ViewMode.Moving)
               WorkingMode = ViewMode.Default;
            СurrentFigureType = FigureCreationMode.None;
            OnPropertyChanged();
         }, isModifyingAvailable);
         if (!palletCommands.Contains(toggleMoving))
            palletCommands.Add(toggleMoving);
         return toggleMoving;
      }
   }

   private RelayCommand toggleScaling;
   public RelayCommand ToggleScaling
   {
      get
      {
         toggleScaling ??= new RelayCommand(_ =>
         {
            switchButtonSelection(toggleScaling, palletCommands);
            if (WorkingMode != ViewMode.Scaling)
               WorkingMode = ViewMode.Scaling;
            else if (WorkingMode == ViewMode.Scaling)
               WorkingMode = ViewMode.Default;
            СurrentFigureType = FigureCreationMode.None;
            OnPropertyChanged();
         }, isModifyingAvailable);
         if (!palletCommands.Contains(toggleScaling))
            palletCommands.Add(toggleScaling);
         return toggleScaling;
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
   public RelayCommand BeginFigureCreation
   {
      get
      {
         return new RelayCommand(_ =>
         {
            switch (СurrentFigureType)
            {
               case FigureCreationMode.Triangle:
                  ActiveLayer.Figures.Add(new Figure(PolygonBuilder.CreateTriangle(new Point(0, 0), new Point(0, 0)), new DrawingParameters(CreationDrawingParameters)));
                  break;
               case FigureCreationMode.Ellipse:
                  ActiveLayer.Figures.Add(new Figure(PolygonBuilder.CreateEllipse(new Point(0, 0), new Point(0, 0)), new DrawingParameters(CreationDrawingParameters)));
                  break;
               case FigureCreationMode.Polygon:
                  break;
               case FigureCreationMode.Polyline:
                  break;
               case FigureCreationMode.Rectangle:
                  ActiveLayer.Figures.Add(new Figure(PolygonBuilder.CreateRectangle(new Point(0, 0), new Point(0, 0)), new DrawingParameters(CreationDrawingParameters)));
                  break;
               case FigureCreationMode.None:
                  break;
            }
         });
      }
   }
   public RelayCommand BeginActiveFigureScaling
   {
      get
      {
         return new RelayCommand(_ =>
         {
            summary_scale_value = new Point(1, 1);
         });
      }
   }
   public RelayCommand OnActiveFigureMoving
   {
      get
      {
         return new RelayCommand(obj =>
         {
            var delta = obj as Vector?;
            summary_moving_delta += (Vector)delta;
            if (ActiveFigure != null)
               ActiveFigure.Move((Vector)delta);
         });
      }
   }

   public RelayCommand OnActiveFigureScaling
   {
      get
      {
         return new RelayCommand(obj =>
         {
            var scale = obj as (Point, Point)?;

            ActiveFigure.Geometric.Scale(scale.Value.Item1, scale.Value.Item2);

            summary_scale_value.X *= scale.Value.Item1.X;
            summary_scale_value.Y *= scale.Value.Item1.Y;
            scalePoint = scale.Value.Item2;

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
            var selfDrawingParametrs = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1].DrawingParameters;

            switch (СurrentFigureType)
            {
               case FigureCreationMode.Triangle:
                  ActiveLayer.Figures[ActiveLayer.Figures.Count - 1]
                  = new Figure(PolygonBuilder.CreateTriangle(points.Value.Item1, points.Value.Item2), selfDrawingParametrs);
                  ActiveFigure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
                  break;
               case FigureCreationMode.Ellipse:
                  ActiveLayer.Figures[ActiveLayer.Figures.Count - 1]
                  = new Figure(PolygonBuilder.CreateEllipse(points.Value.Item1, points.Value.Item2), selfDrawingParametrs);
                  ActiveFigure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
                  break;
               case FigureCreationMode.Polygon:
                  break;
               case FigureCreationMode.Polyline:
                  break;
               case FigureCreationMode.Rectangle:
                  ActiveLayer.Figures[ActiveLayer.Figures.Count - 1]
                  = new Figure(PolygonBuilder.CreateRectangle(points.Value.Item1, points.Value.Item2), selfDrawingParametrs);
                  ActiveFigure = ActiveLayer.Figures[ActiveLayer.Figures.Count - 1];
                  break;
               case FigureCreationMode.None:
                  break;
            }
         });
      }
   }

   public RelayCommand OnFigureCreationFinished
   {
      get
      {
         return new RelayCommand(obj =>
         {
            RedoStack.Clear();
            Figure figure = activeFigure;

            Action<object> redo = (_) =>
            {
               if (DeletedFigures.Count > 0)
               {
                  ActiveFigure = DeletedFigures.Pop();
                  ActiveLayer.Figures.Add(figure);
               }
               else
                  throw new InvalidOperationException();
            };

            Action<object> undo = (_) =>
            {
               if (!ActiveLayer.Figures.Contains(figure))
                  throw new InvalidOperationException();
               DeletedFigures.Push(figure);
               ActiveLayer.Figures.Remove(figure);

               activeFigure = null;
            };
            UndoStack.Push(new UserCommands(redo, undo));
         });
      }
   }

   private Vector summary_moving_delta;
   private Point summary_scale_value;
   private Point scalePoint;
   public RelayCommand OnFigureMovingFinished
   {
      get
      {
         return new RelayCommand(obj =>
         {
            RedoStack.Clear();
            Figure figure = activeFigure;
            Vector delta = summary_moving_delta;
            Action<object> redo = (_) =>
            {
               figure.Geometric.Move(delta);
            };

            Action<object> undo = (_) =>
            {
               figure.Geometric.Move(-delta);
            };
            UndoStack.Push(new UserCommands(redo, undo));
            summary_moving_delta = new Vector();
         });
      }
   }
   public RelayCommand OnFigureScalingFinished
   {
      get
      {
         return new RelayCommand(obj =>
         {
            RedoStack.Clear();
            Figure figure = activeFigure;
            Point scale = summary_scale_value;
            Point currentScalePoint = scalePoint;

            Action<object> redo = (_) =>
            {
               figure.Geometric.Scale(scale, currentScalePoint);
            };

            Action<object> undo = (_) =>
            {
               figure.Geometric.Scale(new Point(1 / scale.X, 1 / scale.Y), currentScalePoint);
            };
            UndoStack.Push(new UserCommands(redo, undo));
            summary_scale_value = new Point();
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
            switchButtonSelection(toggleRectangleCreation, palletCommands);
            СurrentFigureType = СurrentFigureType == FigureCreationMode.Rectangle ? FigureCreationMode.None : FigureCreationMode.Rectangle;
            WorkingMode = СurrentFigureType == FigureCreationMode.Rectangle ? ViewMode.Creation : ViewMode.Default;
         });
         if (!palletCommands.Contains(toggleRectangleCreation))
            palletCommands.Add(toggleRectangleCreation);
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
            switchButtonSelection(toggleTriangleCreation, palletCommands);
            СurrentFigureType = СurrentFigureType == FigureCreationMode.Triangle ? FigureCreationMode.None : FigureCreationMode.Triangle;
            WorkingMode = СurrentFigureType == FigureCreationMode.Triangle ? ViewMode.Creation : ViewMode.Default;
         });
         if (!palletCommands.Contains(toggleTriangleCreation))
            palletCommands.Add(toggleTriangleCreation);
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
            switchButtonSelection(toggleEllipseCreation, palletCommands);
            СurrentFigureType = СurrentFigureType == FigureCreationMode.Ellipse ? FigureCreationMode.None : FigureCreationMode.Ellipse;
            WorkingMode = СurrentFigureType == FigureCreationMode.Ellipse ? ViewMode.Creation : ViewMode.Default;
         });
         if (!palletCommands.Contains(toggleEllipseCreation))
            palletCommands.Add(toggleEllipseCreation);
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
            switchButtonSelection(togglePolylineCreation, palletCommands);
            СurrentFigureType = СurrentFigureType == FigureCreationMode.Polyline ? FigureCreationMode.None : FigureCreationMode.Polyline;
            WorkingMode = СurrentFigureType == FigureCreationMode.Polyline ? ViewMode.Creation : ViewMode.Default;
         });
         if (!palletCommands.Contains(togglePolylineCreation))
            palletCommands.Add(togglePolylineCreation);
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
            switchButtonSelection(togglePolygonCreation, palletCommands);
            СurrentFigureType = СurrentFigureType == FigureCreationMode.Polygon ? FigureCreationMode.None : FigureCreationMode.Polygon;
            WorkingMode = СurrentFigureType == FigureCreationMode.Polygon ? ViewMode.Creation : ViewMode.Default;
         });
         if (!palletCommands.Contains(togglePolygonCreation))
            palletCommands.Add(togglePolygonCreation);
         return togglePolygonCreation;
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
               switchButtonSelection(scaleFigure, palletCommands);

               activeFigure.Geometric.Scale(new Point(2, 2));
               UndoStack.Push(new UserCommands(
                      _ => { activeFigure.Geometric.Scale(new Point(2, 2)); },
                      _ => { activeFigure.Geometric.Scale(new Point(0.5, 0.5)); }));
               //if (ActiveLayer.Figures.Count != 0)
               //   ActiveLayer.Figures.Move(0, 0); // simulation of a collection change 
            }
         }, isModifyingAvailable);
         palletCommands.Add(scaleFigure);
         return scaleFigure;
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

               switchButtonSelection(rotateFigure, palletCommands);

               activeFigure.Geometric.Rotate(30);
               ActiveLayer.Figures[ActiveLayer.Figures.Count - 1] = activeFigure;
               UndoStack.Push(new UserCommands(_ => { activeFigure.Geometric.Rotate(30); },
                      _ => { activeFigure.Geometric.Rotate(-30); }));
               if (ActiveLayer.Figures.Count != 0)
                  ActiveLayer.Figures.Move(0, 0); // simulation of a collection change 
            }
         }, isModifyingAvailable);
         palletCommands.Add(rotateFigure);
         return rotateFigure;
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
               switchButtonSelection(deleteFigure, palletCommands);
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
         palletCommands.Add(deleteFigure);
         return deleteFigure;
      }
   }

   public string CurrentBaseColor
   {
      get => this.CurrentTheme.BaseColorScheme;

      set
      {
         if (value is null)
         {
            return;
         }

         ThemeManager.Current.ChangeThemeBaseColor(Application.Current, value);
         this.OnPropertyChanged();
         this.OnPropertyChanged(nameof(this.CurrentTheme));
      }
   }

   public Theme CurrentTheme
   {
      get => ThemeManager.Current.DetectTheme(Application.Current);

      set
      {
         if (value is null)
         {
            return;
         }

         ThemeManager.Current.ChangeTheme(Application.Current, value);
         this.OnPropertyChanged();
         this.OnPropertyChanged(nameof(this.CurrentBaseColor));
      }
   }

   private RelayCommand save;

   public RelayCommand Save
   {
      get
      {
         save ??= new RelayCommand(o =>
         {
            var parameters = (SaveParameters)o;
            switch (Enum.Parse(typeof(SaveTypes), parameters.Format, true))
            {
               case SaveTypes.Svg:
                  var saver = new SvgSaver(parameters.FileName, ActiveLayer, parameters.Width, parameters.Height);
                  saver.Save();
                  break;
               case SaveTypes.Flp:
                  var jsonSaver = new JsonSaver(parameters.FileName);
                  //TODO: Проблема при сохранении эллипса
                  jsonSaver.Save(Layers);
                  break;
               case SaveTypes.Png:
                  var saverPng = new PngSaver(parameters.FileName, parameters.Canv, parameters.Width, parameters.Height);
                  saverPng.Save();
                  break;
               case SaveTypes.Pdf:
                  var pdfSaver = new PdfSaver(parameters.FileName, parameters.Width, parameters.Height);
                  pdfSaver.SaveLayersToPdf(Layers);
                  break;
            }
         });
         return save;
      }
   }

   private RelayCommand open;

   public RelayCommand Open
   {
      get
      {
         open ??= new RelayCommand(o =>
         {
            var parameters = (OpenParameters)o;
            switch (Enum.Parse(typeof(OpenTypes), parameters.Format, true))
            {
               case OpenTypes.Svg:
                  //TODO: Открыть svg
                  break;
               case OpenTypes.Flp:
                  var flpSaver = new JsonSaver(parameters.FileName);
                  Layers = flpSaver.Restore();
                  ActiveLayer = Layers[0];
                  break;
               //case OpenTypes.Json:
               //   var jsonSaver = new JsonSaver(parameters.FileName);
               //   Layers = jsonSaver.RestoreLayersFromJson();
               //   break;
            }
            OnPropertyChanged();
         });
         return open;
      }
   }

   public MainWindowVM()
   {
      Layers = new TrulyObservableCollection<Layer>();
      Layers.Add(new Layer());
      ActiveLayer = Layers.Last();

      RedoStack = new Stack<UserCommands>();
      UndoStack = new Stack<UserCommands>();
      DeletedFigures = new Stack<Figure>();
      activeFigure = null;
      СurrentFigureType = FigureCreationMode.None;
      palletCommands = new List<RelayCommand> { };
      CreationDrawingParameters = new DrawingParameters();
      summary_moving_delta = new Vector();

      Layers.CollectionChanged += Layers_CollectionChanged;
   }

   private void Layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
   {
      OnPropertyChanged();
   }

   public event PropertyChangedEventHandler PropertyChanged;

   [NotifyPropertyChangedInvocator]
   protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }
}