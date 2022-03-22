using flop.net.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace flop.net.Model
{
   public class Layer : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;
      private TrulyObservableCollection<Figure> figures;
      public TrulyObservableCollection<Figure> Figures
      {
         get => this.figures;
         set
         {
            this.figures = value;
            OnPropertyChanged();
         }
      }

      public Layer()
      {
         this.figures = new TrulyObservableCollection<Figure>();

         figures.CollectionChanged += Figures_CollectionChanged;
      }

      private void Figures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         OnPropertyChanged();
      }

      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
