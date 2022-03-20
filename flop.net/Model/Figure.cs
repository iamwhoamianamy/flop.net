using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace flop.net.Model
{
   public class Figure : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      private IGeometric geometric;
      public IGeometric Geometric
      {
         get => geometric;
         set
         {
            geometric = value;
            OnPropertyChanged();
         }
      }

      private DrawingParameters drawingParameters;
      public DrawingParameters DrawingParameters
      {
         get => drawingParameters;
         set
         {
            drawingParameters = value;
            OnPropertyChanged();
         }
      }

      public void Move(Vector delta)
      {
         geometric.Move(delta);
         OnPropertyChanged();
      }

      public void Rotate(double angle, Point? rotationCenter = null)
      {
         geometric.Rotate(angle, rotationCenter);
         OnPropertyChanged();
      }

      public void Scale(Point scale, Point? scalePoint = null)
      {
         geometric.Scale(scale, scalePoint);
         OnPropertyChanged();
      }

      public Figure(IGeometric geometric, DrawingParameters drawingParameters)
      {
         Geometric = geometric;
         DrawingParameters = drawingParameters;

         drawingParameters.PropertyChanged += DrawingParameters_PropertyChanged;
      }

      private void DrawingParameters_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         OnPropertyChanged();
      }
   }
}