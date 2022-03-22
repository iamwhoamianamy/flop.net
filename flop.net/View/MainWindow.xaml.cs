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
      public Point MousePosition1;
      public Point MousePosition2;
      Thumb MovingThumb { get; set; }
      ThumbsBox scalingThumbs;
      public MainWindow()
      {
         InitializeComponent();

         MainWindowVM = new MainWindowVM();

         DataContext = MainWindowVM;
         MainWindowVM.ActiveLayer.Figures.CollectionChanged += Figures_CollectionChanged;
         MainWindowVM.PropertyChanged += MainWindowVM_PropertyChanged;
         Graphic = new Graphic(MainCanvas);

         Save.MouseLeftButtonDown += SaveOnMouseLeftButtonDown;

         InitMovingThumb();
         InitScalingThumbs();

         DrawAll();
      }

      private void InitScalingThumbs()
      {
         scalingThumbs = new ThumbsBox();

         foreach (var thumb in scalingThumbs.Thumbs)
         {
            thumb.PreviewMouseDown += ScalingThumb_PreviewMouseDown;
         }

      }

      private void ScalingThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         var oppositVec = scalingThumbs.OppositToThumb((Thumb)sender);
      }

      private void MainWindowVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if(MainWindowVM.ActiveFigure is not null)
         {
            if (MainWindowVM.WorkingMode == ViewMode.Moving)
            {
               var center = mainWindowVM.ActiveFigure.Geometric.BoundingBox.Center;
               MovingThumb.Margin = new Thickness(center.X, center.Y, center.X, center.Y);
               MovingThumb.Visibility = Visibility.Visible;
               return;
            }
            else
            {
               MovingThumb.Visibility = Visibility.Hidden;
            }

            if (MainWindowVM.WorkingMode == ViewMode.Scaling)
            {
               scalingThumbs.Visibility = Visibility.Visible;
               scalingThumbs.Box = MainWindowVM.ActiveFigure.Geometric.BoundingBox;
               return;
            }
            else
            {
               scalingThumbs.Visibility = Visibility.Hidden;
            }
         }
      }

      private void InitMovingThumb()
      {
         MovingThumb = new Thumb();

         MovingThumb.Width = 10;
         MovingThumb.Height = 10;
         MovingThumb.Cursor = Cursors.SizeNS;

         MovingThumb.Background = Brushes.LightGreen;
         MovingThumb.BorderBrush = Brushes.DarkGreen;

         MovingThumb.Visibility = Visibility.Hidden;

         MovingThumb.PreviewMouseMove += MovingThumb_PreviewMouseMove;
         MovingThumb.PreviewMouseDown += MovingThumb_PreviewMouseDown;

         MovingThumb.HorizontalAlignment = HorizontalAlignment.Center;
         MovingThumb.VerticalAlignment = VerticalAlignment.Center;
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
            OnPreviewMouseUp(sender, null);
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

      private void Figures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

         MainCanvas.Children.Add(MovingThumb);

         foreach (var thumb in scalingThumbs.Thumbs)
         {
            MainCanvas.Children.Add(thumb);
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
      }

      private void OnPreviewMouseMove(object sender, MouseEventArgs e)
      {
         if (mainWindowVM.WorkingMode == ViewMode.Creation &&
            WorkingMode == ViewMode.Creation)
         {
            mainWindowVM.OnFigureCreation.Execute((MousePosition1, e.GetPosition(MainCanvas)));
         }

         if (e.LeftButton == MouseButtonState.Released &&
            mainWindowVM.WorkingMode == ViewMode.Creation &&
            WorkingMode == ViewMode.Creation)
         {
            WorkingMode = ViewMode.Default;
            mainWindowVM.OnFigureCreationFinished.Execute(null);
            OnPreviewMouseUp(sender, null);
         }

         //if (e.LeftButton == MouseButtonState.Released)
         //{
         //   WorkingMode = ViewMode.Default;
         //   OnPreviewMouseUp(sender, null);
         //}
      }

      private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
      {
         // Место для рекламы ваших заканчивающих команд
      }

      private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
      {
         mainWindowVM.SetActiveFigure.Execute(e.GetPosition(MainCanvas));
      }
   }
}
