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

namespace flop.net
{
   public enum ViewMode
   {
      Default,
      Creation
   }
   /// <summary>
   /// Логика взаимодействия для MainWindow.xaml
   /// </summary>
   public partial class MainWindow : RibbonWindow, INotifyPropertyChanged
   {
      public ViewMode Mode { get; set; } = ViewMode.Default;

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
      private Brush currentFillColor;
      public Brush CurrentFillColor
      {
         get => currentFillColor;
         set
         {
            currentFillColor = value;
            OnPropertyChanged();
         }
      }
      public Point MousePressedCoords { get; set; }
      public MainWindow()
      {
         InitializeComponent();

         MainWindowVM = new MainWindowVM();

         DataContext = MainWindowVM;
         MainWindowVM.ActiveLayer.Figures.CollectionChanged += Figures_CollectionChanged;
         Graphic = new Graphic(MainCanvas);

         DrawAll();
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
            if(figure is not null)
               Graphic.DrawPolygon(figure.Geometric.Points, figure.DrawingParameters);
         }
      }

      private void MainCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         MousePressedCoords = e.GetPosition(MainCanvas);

         if (mainWindowVM.Mode == ViewModelMode.Creation &&
            Mode == ViewMode.Default)
         {
            mainWindowVM.BeginRectangleCreation.Execute();
            Mode = ViewMode.Creation;
         }
         //else
         //{
         //   if (mainWindowVM.Mode == ViewModelMode.Creation &&
         //      Mode == ViewMode.Creation)
         //   {
         //      Mode = ViewMode.Default;
         //   }
         //}
      }

      private void MainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
      {
         if (mainWindowVM.Mode == ViewModelMode.Creation &&
            Mode == ViewMode.Creation)
         {
            mainWindowVM.OnRectangleCreation.Execute((MousePressedCoords, e.GetPosition(MainCanvas)));
         }

         if(e.LeftButton == MouseButtonState.Released)
         {
            Mode = ViewMode.Default;
         }
      }

      private void MainCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
         //if (mainWindowVM.Mode == ViewModelMode.Creation &&
         //   Mode == ViewMode.Creation)
         //{
         //   Mode = ViewMode.Default;
         //}
      }
   }
}
