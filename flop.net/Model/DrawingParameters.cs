using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using flop.net.Annotations;

namespace flop.net.Model
{
   public class DrawingParameters : INotifyPropertyChanged
   {
      private Color fill;
      public Color Fill
      {
         get => fill;
         set
         {
            fill = value;
            OnPropertyChanged();
         }
      }

      private Color stroke;
      public Color Stroke
      {
         get => stroke;
         set
         {
            stroke = value;
            OnPropertyChanged();
         }
      }

      private int strokeThickness;
      public int StrokeThickness
      {
         get => strokeThickness;
         set
         {
            strokeThickness = value;
            OnPropertyChanged();
         }
      }

      private PenLineCap penLineCap;
      public PenLineCap PenLineCap
      {
         get => penLineCap;
         set
         {
            penLineCap = value;
            OnPropertyChanged();
         }
      }

      private List<double> strokeDashArray;
      public List<double> StrokeDashArray
      {
         get => strokeDashArray;
         set
         {
            strokeDashArray = value;
            OnPropertyChanged();
         }
      }

      private double opacity;
      public double Opacity
      {
         get => opacity;
         set
         {
            opacity = value;
            OnPropertyChanged();
         }
      }

      private int zIndex;
      public int ZIndex
      {
         get => zIndex;
         set
         {
            zIndex = value;
            OnPropertyChanged();
         }
      }
      public DrawingParameters()
      {
         this.Fill = Colors.Green;
         this.Stroke = Colors.Black;
         this.StrokeThickness = 1;
         this.StrokeDashArray = new List<double>();
         this.Opacity = 1;
         this.PenLineCap = PenLineCap.Flat;
      }
      public DrawingParameters(DrawingParameters parameters)
      {
         this.Fill = parameters.Fill;
         this.Stroke = parameters.Stroke;
         this.StrokeThickness = parameters.StrokeThickness;
         this.StrokeDashArray = parameters.StrokeDashArray;
         this.Opacity = parameters.Opacity;
         this.ZIndex = parameters.ZIndex;
         this.PenLineCap = parameters.PenLineCap;
      }
      public void Copy(DrawingParameters parameters)
      {
         fill = parameters.Fill;
         stroke = parameters.Stroke;                
         strokeThickness = parameters.StrokeThickness;
         strokeDashArray = parameters.StrokeDashArray;
         opacity = parameters.Opacity;
         zIndex = parameters.ZIndex;
         penLineCap = parameters.PenLineCap;
         OnPropertyChanged("Fill");
         OnPropertyChanged("Stroke");
         OnPropertyChanged("StrokeThickness");
         OnPropertyChanged("StrokeDashArray");
         OnPropertyChanged("Opacity");
         OnPropertyChanged("ZIndex");
         OnPropertyChanged("PenLineCap");
      }
      public bool Compare(DrawingParameters parameters)
      {
         return this.Fill == parameters.Fill
            && this.Stroke == parameters.Stroke
            && this.StrokeThickness == parameters.StrokeThickness
            && this.StrokeDashArray == parameters.StrokeDashArray
            && this.Opacity == parameters.Opacity
            && this.ZIndex == parameters.ZIndex
            && this.PenLineCap == parameters.PenLineCap;   
      }


      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}