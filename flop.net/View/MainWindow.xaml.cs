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
using Microsoft.Win32;
using System.IO;
using flop.net.Model;
using flop.net.Enums;
using flop.net.Save;
using System.Windows.Controls.Primitives;

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
      public Point MousePosition1 { get; set; }
      public Point MousePosition2 { get; set; }
      private double previousHorizontal;
      private double previousVertical;
      private double previousAngle;
       
      Thumb MovingThumb { get; set; }
      Thumb RotatingThumb { get; set; }
      ThumbsBox scalingThumbs;
      public MainWindow()
      {
         InitializeComponent();

         MainWindowVM = new MainWindowVM();
         ReloadData();
      }

      private void ReloadData()
      {
         DataContext = MainWindowVM;
         MainWindowVM.ActiveLayer.Figures.CollectionChanged += Figures_CollectionChanged;
         MainWindowVM.PropertyChanged += MainWindowVM_PropertyChanged;
         MainWindowVM.TempDrawingParameters.PropertyChanged += TempDrawingParametrs_PropertyChanged;

         Graphic = new Graphic(MainCanvas);

         Save.MouseLeftButtonDown += SaveOnMouseLeftButtonDown;
         NewDocument.MouseLeftButtonDown += NewDocumentOnMouseLeftButtonDown;

         InitMovingThumb();
         InitRotatingThumb();
         InitScalingThumbs();

         Open.MouseLeftButtonDown += OpenOnMouseLeftButtonDown;

         WorkingMode = ViewMode.Default;

         DrawAll();
      }

      private void MainWindowVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (MainWindowVM.ActiveFigure is not null)
         {
            if (MainWindowVM.WorkingMode == ViewMode.Moving)
            {
               var center = mainWindowVM.ActiveFigure.Geometric.BoundingBox.Center;
               MovingThumb.Margin = new Thickness(center.X, center.Y, -center.X, -center.Y);
               MovingThumb.Visibility = Visibility.Visible;
            }
            else
            {
               MovingThumb.Visibility = Visibility.Hidden;
            }

            if (MainWindowVM.WorkingMode == ViewMode.Rotating)
            {
               var center = mainWindowVM.ActiveFigure.Geometric.BoundingBoxRotated.BotCenter;
               RotatingThumb.Margin = new Thickness(center.X, center.Y, -center.X, -center.Y);
               RotatingThumb.Visibility = Visibility.Visible;
            }
            else
            {
               RotatingThumb.Visibility = Visibility.Hidden;
            }

            if (MainWindowVM.WorkingMode == ViewMode.Scaling)
            {
               scalingThumbs.Visibility = Visibility.Visible;
               scalingThumbs.Box = MainWindowVM.ActiveFigure.Geometric.BoundingBox;
            }
            else
            {
               scalingThumbs.Visibility = Visibility.Hidden;
            }
         }
         else
         {
            MovingThumb.Visibility = Visibility.Hidden;
            RotatingThumb.Visibility = Visibility.Hidden;
            scalingThumbs.Visibility = Visibility.Hidden;
         }

         DrawAll();
      }
      private void TempDrawingParametrs_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (FigureEditor.Visibility == Visibility.Visible) 
         {
            MainWindowVM.UpdateActiveFigureDrawingParameters.Execute(null);
         }
      }
      private void InitScalingThumbs()
      {
         scalingThumbs = new ThumbsBox();

         foreach (var thumb in scalingThumbs.Thumbs)
         {
            thumb.PreviewMouseDown += ScalingThumb_PreviewMouseDown;
            thumb.PreviewMouseMove += ScalingThumb_PreviewMouseMove;
            thumb.DragCompleted += ScalingThumb_DragCompleted;
         }
      }
      private void ScalingThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         if (mainWindowVM.WorkingMode == ViewMode.Scaling &&
            WorkingMode == ViewMode.Default)
         {
            mainWindowVM.BeginActiveFigureScaling.Execute(null);
            WorkingMode = ViewMode.Scaling;

            MousePosition1 = e.GetPosition(MainCanvas);
         }
      }
      private void ScalingThumb_PreviewMouseMove(object sender, MouseEventArgs e)
      {
         if(WorkingMode == ViewMode.Scaling)
         {
            MousePosition2 = e.GetPosition(MainCanvas);

            Point delta = new Point();
            Point scalePoint = new Point();

            double h = (MousePosition2.X - MousePosition1.X) - previousHorizontal;
            double v = (MousePosition2.Y - MousePosition1.Y) - previousVertical;            

            var box = MainWindowVM.ActiveFigure.Geometric.BoundingBox;

            Point size = new Point();
            var thumb = e.Source as Thumb;

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
               delta.X = h;
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

            double newWidth = (box.RightCenter.X - box.LeftCenter.X) * scale.X;
            double newHeight = (box.TopCenter.Y - box.BotCenter.Y) * scale.Y;

            if (MainWindowVM.WorkingMode == ViewMode.Scaling)
            {
               if (newWidth > 50 && newHeight > 50)
               {
                  MainWindowVM.OnActiveFigureScaling.Execute((scale, scalePoint));
                  previousHorizontal += h;
                  previousVertical += v;
               }
               else
               {
                  MainWindowVM.OnActiveFigureScaling.Execute((new Point(1, 1), scalePoint));
               }
            }
         }
      }
      private void ScalingThumb_DragCompleted(object sender, DragCompletedEventArgs e)
      {
         if (WorkingMode == ViewMode.Scaling)
         {
            MainWindowVM.OnFigureScalingFinished.Execute(null);
            previousHorizontal = 0;
            previousVertical = 0;
            WorkingMode = ViewMode.Default;
         }
      }      
      private void InitMovingThumb()
      {
         MovingThumb = new Thumb();

         MovingThumb.Width = 10;
         MovingThumb.Height = 10;
         MovingThumb.Cursor = Cursors.SizeAll;

         MovingThumb.Background = Brushes.LightGreen;
         MovingThumb.BorderBrush = Brushes.DarkGreen;

         MovingThumb.Visibility = Visibility.Hidden;

         MovingThumb.PreviewMouseMove += MovingThumb_PreviewMouseMove;
         MovingThumb.PreviewMouseDown += MovingThumb_PreviewMouseDown;
      }
      private void MovingThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         MousePosition1 = e.GetPosition(MainCanvas);

         if (mainWindowVM.WorkingMode == ViewMode.Moving &&
            WorkingMode == ViewMode.Default)
         {
            mainWindowVM.BeginActiveFigureMoving.Execute(null);
            WorkingMode = ViewMode.Moving;

            MousePosition2 = e.GetPosition(MainCanvas);
         }
      }
      private void MovingThumb_PreviewMouseMove(object sender, MouseEventArgs e)
      {
         if (mainWindowVM.WorkingMode == ViewMode.Moving &&
            WorkingMode == ViewMode.Moving)
         {
            mainWindowVM.OnActiveFigureMoving.Execute(e.GetPosition(MainCanvas) - MousePosition2);
            MousePosition2 = e.GetPosition(MainCanvas);
         }

         if (e.LeftButton == MouseButtonState.Released &&
            mainWindowVM.WorkingMode == ViewMode.Moving &&
            WorkingMode == ViewMode.Moving)
         {
            WorkingMode = ViewMode.Default;
            mainWindowVM.OnFigureMovingFinished.Execute(null);
         }
      }
      private void InitRotatingThumb()
      {
         RotatingThumb = new Thumb();

         RotatingThumb.Width = 10;
         RotatingThumb.Height = 10;
         RotatingThumb.Cursor = Cursors.Hand;

         RotatingThumb.Background = Brushes.LightSkyBlue;
         RotatingThumb.BorderBrush = Brushes.SkyBlue;

         RotatingThumb.Visibility = Visibility.Hidden;

         RotatingThumb.PreviewMouseMove += RotatingThumb_PreviewMouseMove;
         RotatingThumb.PreviewMouseDown += RotatingThumb_PreviewMouseDown;
         RotatingThumb.DragCompleted += RotatingThumb_DragCompleted;
      }      

      private void RotatingThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         if (mainWindowVM.WorkingMode == ViewMode.Rotating &&
            WorkingMode == ViewMode.Default)
         {
            mainWindowVM.BeginActiveFigureRotating.Execute(null);
            WorkingMode = ViewMode.Rotating;

            MousePosition1 = e.GetPosition(MainCanvas);
         }
      }
      private void RotatingThumb_PreviewMouseMove(object sender, MouseEventArgs e)
      {
         if (WorkingMode == ViewMode.Rotating)
         {
            MousePosition2 = e.GetPosition(MainCanvas);

            var center = MainWindowVM.ActiveFigure.Geometric.Center;

            Vector a = MousePosition1 - center;
            Vector b = MousePosition2 - center;

            double angle = Math.Acos(Math.Round((a.X * b.X + a.Y * b.Y) / (a.Length * b.Length), 6));

            if (MainWindowVM.WorkingMode == ViewMode.Rotating) 
            {
               MainWindowVM.OnActiveFigureRotating.Execute(new double?((angle - previousAngle) * Math.Sign(Vector.CrossProduct(a, b))));

               previousAngle = angle;
            }
         }
      }
      private void RotatingThumb_DragCompleted(object sender, DragCompletedEventArgs e)
      {
         if (WorkingMode == ViewMode.Rotating) 
         {
            MainWindowVM.OnFigureRotatingFinished.Execute(null);
            previousAngle = 0;
            WorkingMode = ViewMode.Default;
         }
      }
      private void NewDocumentOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         if (MainWindowVM.ActiveLayer.Figures.Count > 0) 
         {
            var res = MessageBox.Show("Вы уверены, что хотите начать с нуля?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (res == MessageBoxResult.OK) 
            {
               MainWindowVM.NewDocument.Execute(null);
            }
            else
            {
               return;
            }
         }
         else
         {
            MainWindowVM.NewDocument.Execute(null);
            return;
         }
      }
      private void SaveOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {

         var saveDialog = new SaveFileDialog
         {
            Filter = SaveDialogFilter.GetFilter(),
            RestoreDirectory = true,
            FileName = "Новая шлёпа"
         };
         if(saveDialog.ShowDialog() != true) return;

         MainWindowVM.Save.Execute(new SaveParameters { Format = System.IO.Path.GetExtension(saveDialog.FileName).Replace(".", "") , 
            Width = (int)MainCanvas.ActualWidth, Height = (int)MainCanvas.ActualHeight, Canv = MainCanvas,
            FileName = saveDialog.FileName});
      }
      private void OpenOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {

         var openDialog = new OpenFileDialog
         {
            Filter = OpenDialogFilter.GetFilter(),
            RestoreDirectory = true,
            FileName = string.Empty
         };
         if (openDialog.ShowDialog() != true) return;

         MainWindowVM.Open.Execute(new OpenParameters
         {
            Format = System.IO.Path.GetExtension(openDialog.FileName).Replace(".", ""),
            FileName = openDialog.FileName
         });

         ReloadData();
      }
      private void Figures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         DrawAll();
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

         MainCanvas.Children.Add(MovingThumb);
         MainCanvas.Children.Add(RotatingThumb);

         foreach (var thumb in scalingThumbs.Thumbs)
         {
            MainCanvas.Children.Add(thumb);
         }
      }
      private bool polylineStarted;
      private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         MousePosition1 = e.GetPosition(MainCanvas);

         if (mainWindowVM.WorkingMode == ViewMode.Creation &&
            WorkingMode == ViewMode.Default)
         {
            mainWindowVM.BeginFigureCreation.Execute(null);
            WorkingMode = ViewMode.Creation;
         }
         if (mainWindowVM.WorkingMode == ViewMode.PolylineCreation &&
            WorkingMode == ViewMode.Default && !polylineStarted)
         {
            mainWindowVM.BeginFigureCreation.Execute(MousePosition1);
            WorkingMode = ViewMode.PolylineCreation;
            polylineStarted = false;
         }
         if (mainWindowVM.WorkingMode == ViewMode.PencilDrawing &&
            WorkingMode == ViewMode.Default)
         {
            mainWindowVM.BeginFigureCreation.Execute(MousePosition1);
            WorkingMode = ViewMode.PencilDrawing;
         }
         if (WorkingMode == ViewMode.PolylineCreation && polylineStarted)
         {
            mainWindowVM.OnPolylineCreation.Execute(e.GetPosition(MainCanvas));
         }
      }
      private void OnPreviewMouseMove(object sender, MouseEventArgs e)
      {
         if (mainWindowVM.WorkingMode == ViewMode.Creation &&
            WorkingMode == ViewMode.Creation)
         {
            mainWindowVM.OnFigureCreation.Execute((MousePosition1, e.GetPosition(MainCanvas)));
         }
         if (mainWindowVM.WorkingMode == ViewMode.PolylineCreation &&
            WorkingMode == ViewMode.PolylineCreation)
         {            
            polylineStarted = true;
         }
         if (mainWindowVM.WorkingMode == ViewMode.PencilDrawing &&
            WorkingMode == ViewMode.PencilDrawing)
         {
            mainWindowVM.OnFigureCreation.Execute((MousePosition1, e.GetPosition(MainCanvas)));
         }

         if (e.LeftButton == MouseButtonState.Released && 
            ((mainWindowVM.WorkingMode == ViewMode.Creation && WorkingMode == ViewMode.Creation) ||
            (mainWindowVM.WorkingMode == ViewMode.PencilDrawing && WorkingMode == ViewMode.PencilDrawing)))
         {
            WorkingMode = ViewMode.Default;
            mainWindowVM.OnFigureCreationFinished.Execute(null);
         }

      }
      private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
      {
         if (WorkingMode == ViewMode.PolylineCreation) 
         {
            mainWindowVM.OnFigureCreationFinished.Execute(null);
            WorkingMode = ViewMode.Default;
            polylineStarted = false;
         }
         else
         {
            mainWindowVM.SetActiveFigure.Execute(e.GetPosition(MainCanvas));
         }
      }
      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
