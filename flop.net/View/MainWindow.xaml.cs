using flop.net.Annotations;
using flop.net.View;
using flop.net.ViewModel;
using Fluent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using flop.net.Model;
using flop.net.Enums;
using flop.net.Save;
using System.Windows.Controls.Primitives;
using Figure = flop.net.Model.Figure;
using System.Collections.Specialized;

namespace flop.net
{
   /// <summary>
   /// Логика взаимодействия для MainWindow.xaml
   /// </summary>   
   public partial class MainWindow : RibbonWindow, INotifyPropertyChanged
   {
      private MainWindowVM mainWindowVM;
      public MainWindowVM MainWindowVM
      {
         get { return mainWindowVM; }
         set
         {
            mainWindowVM = value;
            OnPropertyChanged();
         }
      }
      private Graphic graphic;
      public Graphic Graphic
      {
         get
         {
            return graphic;
         }
         set
         {
            graphic = value;
            OnPropertyChanged();
         }
      }
      public ViewMode WorkingMode { get; set; }
      public FigureCreationMode СurrentFigureType { get; set; }
      public Point MousePosition1;
      public Point MousePosition2;
      public MainWindow()
      {
         InitializeComponent();

         MainWindowVM = new MainWindowVM();

         DataContext = MainWindowVM;
         MainWindowVM.ActiveLayer.Figures.CollectionChanged += Figures_CollectionChanged;
         Graphic = new Graphic(MainCanvas);        
         

         Save.MouseLeftButtonDown += SaveOnMouseLeftButtonDown; 
         DrawAll();
      }      

      private void SaveOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         MainWindowVM.Save.Execute(new SaveParameters { Format = "svg" , Width = (int)MainCanvas.ActualWidth, Height = (int)MainCanvas.ActualHeight });
      }

      private void Figures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         DrawAll();
      }
      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
      public void DrawAll()
      {
         Graphic.CleanCanvas();
         foreach (var figure in MainWindowVM.ActiveLayer.Figures)
         {
            if(figure != null)
            {
               switch (figure.Geometric.IsClosed)
               {
                  case true:
                     Graphic.DrawPolygon(figure.Geometric.Points, figure.DrawingParameters);
                     break;
                  case false:
                     Graphic.DrawPolyline(figure.Geometric.Points, figure.DrawingParameters);
                     break;
               }
            }            
         }
      }            
      private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         MousePosition1 = e.GetPosition(MainCanvas);

         if (mainWindowVM.WorkingMode == ViewMode.Creation &&
            WorkingMode == ViewMode.Default)
         {
            mainWindowVM.BeginFigureCreation.Execute(null);
            WorkingMode = ViewMode.Creation;
         }

         if (mainWindowVM.WorkingMode == ViewMode.Moving &&
            WorkingMode == ViewMode.Default)
         {
            mainWindowVM.BeginActiveFigureMoving.Execute(null);
            WorkingMode = ViewMode.Moving;

            MousePosition2 = e.GetPosition(MainCanvas);
         }
      }
      private void OnPreviewMouseMove(object sender, MouseEventArgs e)
      {
         if (mainWindowVM.WorkingMode == ViewMode.Creation &&
            WorkingMode == ViewMode.Creation)
         {
            mainWindowVM.OnFigureCreation.Execute((MousePosition1, e.GetPosition(MainCanvas)));
         }

         if (mainWindowVM.WorkingMode == ViewMode.Moving &&
            WorkingMode == ViewMode.Moving)
         {
            mainWindowVM.OnActiveFigureMoving.Execute(e.GetPosition(MainCanvas) - MousePosition2);
            MousePosition2 = e.GetPosition(MainCanvas);
         }

         if (e.LeftButton == MouseButtonState.Released &&
            mainWindowVM.WorkingMode == ViewMode.Creation &&
            WorkingMode == ViewMode.Creation)
         {
            WorkingMode = ViewMode.Default;
            mainWindowVM.OnFigureCreationFinished.Execute(null);
            OnPreviewMouseUp(sender, null);
         }

         if (e.LeftButton == MouseButtonState.Released &&
            mainWindowVM.WorkingMode == ViewMode.Moving &&
            WorkingMode == ViewMode.Moving)
         {
            WorkingMode = ViewMode.Default;
            mainWindowVM.OnFigureMovingFinished.Execute(null);
            OnPreviewMouseUp(sender, null);
         }

         if (e.LeftButton == MouseButtonState.Released)
         {
            WorkingMode = ViewMode.Default;
            OnPreviewMouseUp(sender, null);
         }
      }
      private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
      {
         // Место для рекламы ваших заканчивающих команд
      }
      private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
      {
         mainWindowVM.SetActiveFigure.Execute(e.GetPosition(MainCanvas));
      }

      private double previousHorizontal;
      private double previousVertical;
      private void OnDragDelta(object sender, DragDeltaEventArgs e)
      {
         Point delta = new Point();
         Point scalePoint = new Point();

         double h = e.HorizontalChange - previousHorizontal;
         double v = e.VerticalChange - previousVertical;

         var box = MainWindowVM.ActiveFigure.Geometric.BoundingBox;

         Point size = new Point();
         var thumb = e.OriginalSource as Thumb;

         // Top Left Corner
         if (thumb.VerticalAlignment == VerticalAlignment.Top &&
            thumb.HorizontalAlignment == HorizontalAlignment.Left)
         {
            size = (Point)(box.TopLeft - box.BotRight);
            delta.X = h;
            delta.Y = v;
            scalePoint = box.BotRight;
         }
         // Top Edge
         if (thumb.VerticalAlignment == VerticalAlignment.Top &&
            thumb.HorizontalAlignment == HorizontalAlignment.Stretch)
         {
            size = (Point)(box.TopCenter - box.BotCenter);
            delta.X = 0;
            delta.Y = v;
            scalePoint = box.BotCenter;
         }
         // Top Right Corner
         if (thumb.VerticalAlignment == VerticalAlignment.Top &&
            thumb.HorizontalAlignment == HorizontalAlignment.Right)
         {
            size = (Point)(box.TopRight - box.BotLeft);
            delta.X =h;
            delta.Y = v;
            scalePoint = box.BotLeft;
         }
         // Left Edge
         if (thumb.VerticalAlignment == VerticalAlignment.Stretch &&
            thumb.HorizontalAlignment == HorizontalAlignment.Left)
         {
            size = (Point)(box.LeftCenter - box.RightCenter);
            delta.X = h;
            delta.Y = 0;
            scalePoint = box.RightCenter;
         }
         // Right Edge
         if (thumb.VerticalAlignment == VerticalAlignment.Stretch &&
            thumb.HorizontalAlignment == HorizontalAlignment.Right)
         {
            size = (Point)(box.RightCenter - box.LeftCenter);
            delta.X = h;
            delta.Y = 0;
            scalePoint = box.LeftCenter;
         }
         // Bottom Left Corner
         if (thumb.VerticalAlignment == VerticalAlignment.Bottom &&
            thumb.HorizontalAlignment == HorizontalAlignment.Left)
         {
            size = (Point)(box.BotLeft - box.TopRight);
            delta.X = h;
            delta.Y = v;
            scalePoint = box.TopRight;
         }
         // Bottom Edge
         if (thumb.VerticalAlignment == VerticalAlignment.Bottom &&
            thumb.HorizontalAlignment == HorizontalAlignment.Stretch)
         {
            size = (Point)(box.BotCenter - box.TopCenter);
            delta.X = 0;
            delta.Y = v;
            scalePoint = box.TopCenter;
         }
         // Bottom Right Corner
         if (thumb.VerticalAlignment == VerticalAlignment.Bottom &&
            thumb.HorizontalAlignment == HorizontalAlignment.Right)
         {
            size = (Point)(box.BotRight - box.TopLeft);
            delta.X = h;
            delta.Y = v;
            scalePoint = box.TopLeft;
         }

         Point scale = new Point((size.X + delta.X) / size.X, (size.Y + delta.Y) / size.Y);

         if (thumb.HorizontalAlignment == HorizontalAlignment.Stretch) scale.X = 1;
         if (thumb.VerticalAlignment == VerticalAlignment.Stretch) scale.Y = 1;

         if (MainWindowVM.WorkingMode == ViewMode.Scaling)
         {
            MainWindowVM.OnActiveFigureScaling.Execute((scale, scalePoint));
            previousHorizontal += h;
            previousVertical += v;
         }
      }      
      private void OnDragCompleted(object sender, DragCompletedEventArgs e)
      {         
         if (MainWindowVM.WorkingMode == ViewMode.Scaling)
         {
            MainWindowVM.OnFigureScalingFinished.Execute(null);
         }
      }

      private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         if (MainWindowVM.WorkingMode == ViewMode.Scaling &&
            MainWindowVM.ActiveFigure != null)
         {
            MainWindowVM.BeginActiveFigureScaling.Execute(null);
            previousHorizontal = 0;
            previousVertical = 0;
         }
      }
   }
}
