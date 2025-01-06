using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace POS
{
    /// <summary>
    /// ObservableCollection mở rộng: Thêm sự kiện PropertyChanged cho từng phần tử
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class FullObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Khởi tạo một đối tượng FullObservableCollection
        /// </summary>
        public FullObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        /// <summary>
        /// Khởi tạo một đối tượng FullObservableCollection với một danh sách các phần tử
        /// </summary>
        /// <param name="pItems"></param>
        public FullObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (var item in pItems)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Sự kiện CollectionChanged của ObservableCollection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Sự kiện PropertyChanged của từng phần tử
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            //OnCollectionChanged(args);
        }
    }
}