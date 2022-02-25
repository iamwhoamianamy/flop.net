﻿using flop.net.Annotations;
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
        public MainWindow()
        {
            InitializeComponent();

            MainWindowVM = new MainWindowVM();

            DataContext = MainWindowVM;

            Graphic = new Graphic(MainCanvas);

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
            foreach (var x in MainWindowVM.Rectangles)
            {
                Graphic.DrawPolygon(x.Points);
            }
        }
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DrawAll();
        }
    }
}
