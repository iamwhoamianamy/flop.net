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

         //if(MainWindowVM.ActiveFigure != null && 
         //   MainWindowVM.WorkingMode == ViewMode.Scaling)
         //{
         //   DrawBoundaryBox(MainWindowVM.ActiveFigure);
         //}
      }
      public void DrawBoundaryBox(Figure figure)
      {
         //  0----1----2
         //  |         |
         //  3         4
         //  |         |
         //  5----6----7
         List<Thumb> thumbs = new List<Thumb>();
         Thumb TopLeft = new Thumb();
         Thumb TopCenter = new Thumb();
         Thumb TopRight = new Thumb();
         Thumb LeftCenter = new Thumb();
         Thumb RightCenter = new Thumb();
         Thumb BotLeft = new Thumb();
         Thumb BotCenter = new Thumb();
         Thumb BotRight = new Thumb();

         TopLeft.Margin = new Thickness(
            figure.Geometric.BoundingBox.TopLeft.X - 10,
            figure.Geometric.BoundingBox.TopLeft.Y - 10,
            -figure.Geometric.BoundingBox.TopLeft.X + 10,
            -figure.Geometric.BoundingBox.TopLeft.Y + 10
            );
         TopCenter.Margin = new Thickness(
            figure.Geometric.BoundingBox.TopCenter.X - 5,
            figure.Geometric.BoundingBox.TopCenter.Y - 10,
            -figure.Geometric.BoundingBox.TopCenter.X + 5,
            -figure.Geometric.BoundingBox.TopCenter.Y + 10
            );
         TopRight.Margin = new Thickness(
            figure.Geometric.BoundingBox.TopRight.X,
            figure.Geometric.BoundingBox.TopRight.Y - 10,
            -figure.Geometric.BoundingBox.TopRight.X,
            -figure.Geometric.BoundingBox.TopRight.Y + 10
            );
         LeftCenter.Margin = new Thickness(
            figure.Geometric.BoundingBox.LeftCenter.X - 10,
            figure.Geometric.BoundingBox.LeftCenter.Y - 5,
            -figure.Geometric.BoundingBox.LeftCenter.X + 10,
            -figure.Geometric.BoundingBox.LeftCenter.Y + 5
            );
         RightCenter.Margin = new Thickness(
            figure.Geometric.BoundingBox.RightCenter.X,
            figure.Geometric.BoundingBox.RightCenter.Y - 5,
            -figure.Geometric.BoundingBox.RightCenter.X,
            -figure.Geometric.BoundingBox.RightCenter.Y + 5
            );
         BotLeft.Margin = new Thickness(
            figure.Geometric.BoundingBox.BotLeft.X - 10,
            figure.Geometric.BoundingBox.BotLeft.Y,
            -figure.Geometric.BoundingBox.BotLeft.X + 10,
            -figure.Geometric.BoundingBox.BotLeft.Y
            );
         BotCenter.Margin = new Thickness(
            figure.Geometric.BoundingBox.BotCenter.X - 5,
            figure.Geometric.BoundingBox.BotCenter.Y,
            -figure.Geometric.BoundingBox.BotCenter.X + 5,
            -figure.Geometric.BoundingBox.BotCenter.Y
            );
         BotRight.Margin = new Thickness(
           figure.Geometric.BoundingBox.BotRight.X,
           figure.Geometric.BoundingBox.BotRight.Y,
           -figure.Geometric.BoundingBox.BotRight.X,
           -figure.Geometric.BoundingBox.BotRight.Y
           );

         TopLeft.Cursor = Cursors.SizeNESW;
         TopCenter.Cursor = Cursors.SizeNS;
         TopRight.Cursor = Cursors.SizeNWSE;
         LeftCenter.Cursor = Cursors.SizeWE;
         RightCenter.Cursor = Cursors.SizeWE;
         BotLeft.Cursor = Cursors.SizeNWSE;
         BotCenter.Cursor = Cursors.SizeNS;
         BotRight.Cursor = Cursors.SizeNESW;

         TopLeft.VerticalAlignment = VerticalAlignment.Top;
         TopCenter.VerticalAlignment = VerticalAlignment.Top;
         TopRight.VerticalAlignment = VerticalAlignment.Top;
         LeftCenter.VerticalAlignment = VerticalAlignment.Center;
         RightCenter.VerticalAlignment = VerticalAlignment.Center;
         BotLeft.VerticalAlignment = VerticalAlignment.Bottom;
         BotCenter.VerticalAlignment = VerticalAlignment.Bottom;
         BotRight.VerticalAlignment = VerticalAlignment.Bottom;

         TopLeft.HorizontalAlignment = HorizontalAlignment.Left;
         TopCenter.HorizontalAlignment = HorizontalAlignment.Center;
         TopRight.HorizontalAlignment = HorizontalAlignment.Right;
         LeftCenter.HorizontalAlignment = HorizontalAlignment.Left;
         RightCenter.HorizontalAlignment = HorizontalAlignment.Right;
         BotLeft.HorizontalAlignment = HorizontalAlignment.Left;
         BotCenter.HorizontalAlignment = HorizontalAlignment.Center;
         BotRight.HorizontalAlignment = HorizontalAlignment.Right;

         thumbs.Add(TopLeft);
         thumbs.Add(TopCenter);
         thumbs.Add(TopRight);
         thumbs.Add(LeftCenter);
         thumbs.Add(RightCenter);
         thumbs.Add(BotLeft);
         thumbs.Add(BotCenter);
         thumbs.Add(BotRight);

         foreach (var thumb in thumbs)
         {
            thumb.Width = 10;
            thumb.Height = 10;
            thumb.Background = Brushes.LightGreen;
            thumb.BorderBrush = Brushes.DarkGreen;

            MainCanvas.Children.Add(thumb);
         }
      }
      private void OnDragDelta(object sender, DragDeltaEventArgs e)
      {
         
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
         //if (mainWindowVM.ActiveFigure != null)
         //{
         //   DrawBoundaryBox(mainWindowVM.ActiveFigure);
         //}
      }

      private void TopEdgeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
      {
         Point delta = new Point(e.HorizontalChange, e.VerticalChange);
         Point scalePoint = new Point(0, 0);

         var box = MainWindowVM.ActiveFigure.Geometric.BoundingBox;

         Point size = (Point)(box.TopLeft - box.BotRight);
         var thumb = e.OriginalSource as Thumb;

         Point scale = new Point((size.X + delta.X) / size.X, (size.Y + delta.Y) / size.Y);

         switch (thumb.VerticalAlignment)
         {
            case VerticalAlignment.Top:
               scalePoint.Y = box.BotCenter.Y;
               break;
            case VerticalAlignment.Center:
               scale.Y = 1;
               break;
            case VerticalAlignment.Bottom:
               scalePoint.Y = box.TopCenter.Y;
               break;
         }

         switch (thumb.HorizontalAlignment)
         {
            case HorizontalAlignment.Left:
               scalePoint.X = box.RightCenter.X;
               break;
            case HorizontalAlignment.Center:
               scale.X = 1;
               break;
            case HorizontalAlignment.Right:
               scalePoint.Y = box.LeftCenter.Y;
               break;
         }

         if (MainWindowVM.WorkingMode == ViewMode.Scaling)
         {
            MainWindowVM.OnActiveFigureScaling.Execute((scale, scalePoint));
         }
      }

   }
}
