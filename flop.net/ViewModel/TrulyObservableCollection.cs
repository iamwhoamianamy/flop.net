using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace flop.net.ViewModel
{
   public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
   {
      public TrulyObservableCollection()
      {
         CollectionChanged += FullObservableCollectionCollectionChanged;
      }

      public TrulyObservableCollection(IEnumerable<T> pItems) : this()
      {
         foreach (var item in pItems)
         {
            this.Add(item);
         }
      }

      private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if (e.NewItems != null)
         {
            foreach (Object item in e.NewItems)
            {
               ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
            }
         }
         if (e.OldItems != null)
         {
            foreach (Object item in e.OldItems)
            {
               ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
            }
         }
      }

      private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
         OnCollectionChanged(args);
      }
   }
   //public class TrulyObservableCollection<T>
   //   where T : INotifyPropertyChanged
   //{
   //   private ObservableCollection<T> source;

   //   public void Add(T elem)
   //   {
   //      source.Add(elem);
   //      elem.PropertyChanged += Elem_PropertyChanged;
   //   }
   //   public void Remove(int index)
   //   {
   //      source[index].PropertyChanged -= Elem_PropertyChanged;
   //      source.RemoveAt(index);
   //   }

   //   private void Elem_PropertyChanged(object sender, PropertyChangedEventArgs e)
   //   {

   //   }

   //   public T this[int i]
   //   {
   //      get => source[i];
   //      set => source[i] = value;
   //   }
   //}
}
