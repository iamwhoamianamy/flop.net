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

      private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         MousePosition1 = e.GetPosition(MainCanvas);

         if (mainWindowVM.WorkingMode == ViewMode.Creation &&
            WorkingMode == ViewMode.Default)
         {
            //mainWindowVM.BeginRectangleCreation.Execute();
            WorkingMode = ViewMode.Creation;
         }

         if (mainWindowVM.WorkingMode == ViewMode.Moving &&
            WorkingMode == ViewMode.Default)
         {
            //mainWindowVM.BeginActiveFigureMoving.Execute();
            WorkingMode = ViewMode.Moving;

            MousePosition2 = e.GetPosition(MainCanvas);
         }
      }

      private void OnPreviewMouseMove(object sender, MouseEventArgs e)
      {
         if (mainWindowVM.WorkingMode == ViewMode.Creation &&
            WorkingMode == ViewMode.Creation)
         {
            //mainWindowVM.OnRectangleCreation.Execute((MousePressedCoords, e.GetPosition(MainCanvas)));
         }

         if (mainWindowVM.WorkingMode == ViewMode.Moving &&
            WorkingMode == ViewMode.Moving)
         {
            //mainWindowVM.OnActiveFigureMoving.Execute((PreviousMouseCoords, e.GetPosition(MainCanvas)));
            //mainWindowVM.OnActiveFigureMoving.Execute((e.GetPosition(MainCanvas) - MousePosition2));
            MousePosition2 = e.GetPosition(MainCanvas);
         }

         if (e.LeftButton == MouseButtonState.Released)
         {
            WorkingMode = ViewMode.Default;
         }
      }

      private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
      {

      }
   }
}
