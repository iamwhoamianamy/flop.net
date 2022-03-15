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
        Creation,
        Moving,
        Rotating,
        Scaling
    }
    public enum FigureCreation
    {
        Rectangle,
        Triangle,
        Ellipse,
        Polyline,
        Polygon
    }
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
    }
}
