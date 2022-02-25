using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using flop.net.Annotations;
using flop.net.Model;
using flop.net.View;

namespace flop.net.ViewModel;

public class MainWindowVM : INotifyPropertyChanged
{ 
    public ObservableCollection<Rectangle> Rectangles { get; set; }
    private RelayCommand drawRectangle;
    public RelayCommand DrawRectangle
    {
        get
        {
            return drawRectangle ?? (
                drawRectangle = new RelayCommand(obj =>
                {
                    Point a = new Point(200, 200);
                    Point b = new Point(600, 400);

                    Rectangle rectangle = new Rectangle(a, b);

                    Rectangles.Insert(0, rectangle);
                }));
        }
    }

    public MainWindowVM()
    {
        Rectangles = new ObservableCollection<Rectangle>();

        Point a = new Point(0, 0);
        Point b = new Point(100, 100);

        Rectangle rectangle = new Rectangle(a, b);

        Rectangles.Insert(0, rectangle);
    }
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}