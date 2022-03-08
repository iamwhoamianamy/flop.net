using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace flop.net.View
{
    /// <summary>
    /// Логика взаимодействия для FigureUserControl.xaml
    /// </summary>
    public partial class FigureUserControl : UserControl
    {
        public FigureUserControl()
        {
            InitializeComponent();
            SetBinding(RequestMoveCommandProperty, new Binding("RequestMove"));
            SetBinding(RequestDrawCommandProperty, new Binding("RequestDraw"));
        }
        // стандартное DependencyProperty
        #region dp ICommand RequestMoveCommand
        public ICommand RequestMoveCommand
        {
            get { return (ICommand)GetValue(RequestMoveCommandProperty); }
            set { SetValue(RequestMoveCommandProperty, value); }
        }

        public static readonly DependencyProperty RequestMoveCommandProperty =
            DependencyProperty.Register("RequestMoveCommand", typeof(ICommand),
                                        typeof(FigureUserControl));
        #endregion

        #region dp ICommand RequestDrawCommand
        public ICommand RequestDrawCommand
        {
            get { return (ICommand)GetValue(RequestDrawCommandProperty); }
            set { SetValue(RequestDrawCommandProperty, value); }
        }

        public static readonly DependencyProperty RequestDrawCommandProperty =
            DependencyProperty.Register("RequestDrawCommand", typeof(ICommand),
                                        typeof(FigureUserControl));
        #endregion

        #region dp Shape DraggedImageContainer
        public Shape DraggedImageContainer
        {
            get { return (Shape)GetValue(DraggedImageContainerProperty); }
            set { SetValue(DraggedImageContainerProperty, value); }
        }

        public static readonly DependencyProperty DraggedImageContainerProperty =
            DependencyProperty.Register(
                "DraggedImageContainer", typeof(Shape), typeof(FigureUserControl));
        #endregion
        Vector relativeMousePos; // смещение мыши от левого верхнего угла квадрата
        Canvas container;        // канвас-контейнер

        // по нажатию на левую клавишу начинаем следить за мышью
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {            
            container = FindParent<Canvas>(this);
            relativeMousePos = e.GetPosition(this) - new Point();
            MouseMove += OnDragMove;             
            LostMouseCapture += OnLostCapture;
            Mouse.Capture(this);
            ResizeGrid.Visibility = Visibility.Visible;
        }

        // клавиша отпущена - завершаем процесс
        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            FinishDrag(sender, e);
            Mouse.Capture(null);
        }
        // потеряли фокус (например, юзер переключился в другое окно) - завершаем тоже
        void OnLostCapture(object sender, MouseEventArgs e)
        {
            FinishDrag(sender, e);
            ResizeGrid.Visibility = Visibility.Hidden;
        }

        void OnDragMove(object sender, MouseEventArgs e)
        {
            //UpdatePosition(e);
            UpdateDraggedSquarePosition(e);
        }

        void FinishDrag(object sender, MouseEventArgs e)
        {
            MouseMove -= OnDragMove;
            LostMouseCapture -= OnLostCapture;
            UpdatePosition(e);
            UpdateDraggedSquarePosition(null);
        }

        void UpdateDraggedSquarePosition(MouseEventArgs e)
        {
            var dragImageContainer = DraggedImageContainer;
            if (dragImageContainer == null)
                return;
            var needVisible = e != null;
            var wasVisible = dragImageContainer.Visibility == Visibility.Visible;
            // включаем/выключаем видимость перемещаемой картинки
            dragImageContainer.Visibility = needVisible ? Visibility.Visible : Visibility.Collapsed;
            if (!needVisible) // если мы выключились, нам больше нечего делать
                return;
            if (!wasVisible) // а если мы были выключены и включились,
            {                // нам надо привязать изображение себя
                dragImageContainer.Fill = new VisualBrush(this);
                dragImageContainer.SetBinding( // а также ширину/высоту
                    Shape.WidthProperty,
                    new Binding(nameof(ActualWidth)) { Source = this });
                dragImageContainer.SetBinding(
                    Shape.HeightProperty,
                    new Binding(nameof(ActualHeight)) { Source = this });
                // Binding нужен потому, что наш размер может по идее измениться
            }
            // перемещаем картинку на нужную позицию
            var parent = FindParent<Canvas>(dragImageContainer);
            var position = e.GetPosition(parent) - relativeMousePos;
            Canvas.SetLeft(dragImageContainer, position.X);
            Canvas.SetTop(dragImageContainer, position.Y);
        }

        // требуем у VM обновить позицию через команду
        void UpdatePosition(MouseEventArgs e)
        {
            var point = e.GetPosition(container);
            // не забываем проверку на null
            RequestMoveCommand?.Execute(point - relativeMousePos);
        }

        // это вспомогательная функция, ей место в общей библиотеке
        static private T FindParent<T>(FrameworkElement from) where T : FrameworkElement
        {
            FrameworkElement current = from;
            T t;
            do
            {
                t = current as T;
                current = (FrameworkElement)VisualTreeHelper.GetParent(current);
            }
            while (t == null && current != null);
            return t;
        }

        private void TopEdgeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double y = double.IsNaN(figureUserControl.Height) ?
                figureUserControl.ActualHeight + e.VerticalChange :
                figureUserControl.Height + e.VerticalChange;

            if (y >= 0)
            {
                figureUserControl.Height = y;
            }
        }
    }
}
