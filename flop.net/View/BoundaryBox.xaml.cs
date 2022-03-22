using System;
using System.Collections.Generic;
using System.Linq;
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

namespace flop.net.View
{
   /// <summary>
   /// Логика взаимодействия для BoundingBox.xaml
   /// </summary>
   public partial class BoundaryBox : UserControl
   {
      public BoundaryBox()
      {
         InitializeComponent();
      }
      #region dp Shape BoundaryBoxContainer
      public Shape BoundaryBoxContainer
      {
         get { return (Shape)GetValue(BoundaryBoxContainerProperty); }
         set { SetValue(BoundaryBoxContainerProperty, value); }
      }

      public static readonly DependencyProperty BoundaryBoxContainerProperty =
          DependencyProperty.Register(
              "BoundaryBoxContainer", typeof(Shape), typeof(BoundaryBox));
      #endregion
   }
}
