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
            get { return graphic; }
            set
            {
                graphic = value;
                OnPropertyChanged();
            }
        }
        private Point mousePos1;
        public Point MousePos1
        {
            get { return mousePos1; }
            set
            {
                mousePos1 = value;
                OnPropertyChanged();
            }
        }
        private Point mousePos2;
        public Point MousePos2
        {
            get { return mousePos2; }
            set
            {
                mousePos2 = value;
                OnPropertyChanged();
            }
        }
        private bool isPressed;
        public bool IsPressed
        {
            get { return isPressed; }
            set
            {
                isPressed = value;
                OnPropertyChanged();
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            MainWindowVM = new MainWindowVM();

            DataContext = MainWindowVM;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void DraggableItemsHost_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isPressed && (bool)Test.IsChecked)
            {
                isPressed = true;
                MousePos1 = e.GetPosition(DraggableItemsHost);
            }
        }

        private void DraggableItemsHost_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isPressed && (bool)Test.IsChecked)
            {
                isPressed = false;
                MousePos2 = e.GetPosition(DraggableItemsHost);
                MainWindowVM.Draw(MousePos1, MousePos2);
                Test.IsChecked = false;
            }
        }

        private void DraggableItemsHost_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPressed && (bool)Test.IsChecked)
            {
                MousePos2 = e.GetPosition(DraggableItemsHost);
                MainWindowVM.Draw(MousePos1, MousePos2);
            }
        }
    }
}
