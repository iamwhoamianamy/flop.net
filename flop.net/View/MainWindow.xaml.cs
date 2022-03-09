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
        public MainWindow()
        {
         double eps = 0.001;
         var rectangle = Model.PolygonBuilder.CreateRectangle(new Point(0.0, 0.0), new Point(11.0, 5.0));
         //var t1 =  rectangle.IsIn(new Point(0.0, 1.0));
         //var t2 = rectangle.IsIn(new Point(1.0, 1.0));
         //var t3 = rectangle.IsIn(new Point(1.5, 1.5));
         //var t4 = rectangle.IsIn(new Point(0.0, 0.0));
         //var t5 = rectangle.IsIn(new Point(6.7, 7.8));
         //var t6 = rectangle.IsIn(new Point(-6.7, 7.8));
         //var t7 = rectangle.IsIn(new Point(-1.5, -1.5));
         //var t8 = rectangle.IsIn(new Point(-0.0, 0.0));
         //var t9 = rectangle.IsIn(new Point(11.0, 0.0));

         var triangle = Model.PolygonBuilder.CreateTriangle(new Point(0.0, 0.0), new Point(11.0, 5.0));
         var s1 = rectangle.IsIn(new Point(0.0, 0.0), eps);
         var s2 = rectangle.IsIn(new Point(11.0, 0.0), eps);
         var s3 = rectangle.IsIn(new Point(5.5, 5.0), eps);
         var s4 = rectangle.IsIn(new Point(5.0, 0.0), eps);
         var s5 = rectangle.IsIn(new Point(4.3, 43.0 / 11), eps);
         var s6 = rectangle.IsIn(new Point(7.7, 3.0), eps);
         var s7 = rectangle.IsIn(new Point(2.0, 4.5), eps);
         var s8 = rectangle.IsIn(new Point(11.0, 5.0), eps);
         var s9 = rectangle.IsIn(new Point(5.0, -3.0), eps);
         var s10 = rectangle.IsIn(new Point(-3.3, -2.0), eps);


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
                Graphic.DrawPolygon(figure.Geometric.Points, figure.DrawingParameters);
            }
        }
    }
}
