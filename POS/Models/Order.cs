using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    /// <summary>
    /// Order class
    /// </summary>
    public class Order : Product, INotifyPropertyChanged
    {
        private int _quantity;
        private int _total;
        private string _note;

        public string Note
        {
            get => _note;
            set
            {
                if (_note != value)
                {
                    _note = value;
                    OnPropertyChanged(nameof(Note));
                }
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public int Total
        {
            get => Quantity * Price;
            set
            {
                if (_total != value)
                {
                    _total = value;
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public Order(Product product,int quanlity, string note)
        {
            ProductID = product.ProductID;
            Name = product.Name;
            Category = product.Category;
            Price = product.Price;
            Description = product.Description;
            ImagePath = product.ImagePath;
            Status = product.Status;
            Quantity = quanlity;
            Note = note;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
