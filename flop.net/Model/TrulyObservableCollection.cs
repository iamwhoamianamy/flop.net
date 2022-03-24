﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flop.net.Model
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
               if (item != null) 
                  ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
            }
         }
         if (e.OldItems != null)
         {
            foreach (Object item in e.OldItems)
            {
               if (item != null)
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
}
