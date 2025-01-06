using System;
using System.ComponentModel;

namespace POS.Models
{
    /// <summary>
    /// Warehouse Model
    /// </summary>
    public class Warehouse : INotifyPropertyChanged
    {
        private int warehouseID;
        private string ingredientName;
        private int stockQuantity;
        private string unit;
        private DateTime entryDate;
        private DateTime expirationDate;

        public int WarehouseID
        {
            get => warehouseID;
            set
            {
                warehouseID = value;
                OnPropertyChanged(nameof(WarehouseID));
            }
        }

        public string IngredientName
        {
            get => ingredientName;
            set
            {
                ingredientName = value;
                OnPropertyChanged(nameof(IngredientName));
            }
        }

        public int StockQuantity
        {
            get => stockQuantity;
            set
            {
                stockQuantity = value;
                OnPropertyChanged(nameof(StockQuantity));
            }
        }

        public string Unit
        {
            get => unit;
            set
            {
                unit = value;
                OnPropertyChanged(nameof(Unit));
            }
        }

        public DateTime EntryDate
        {
            get => entryDate;
            set
            {
                entryDate = value;
                OnPropertyChanged(nameof(EntryDate));
            }
        }

        public DateTime ExpirationDate
        {
            get => expirationDate;
            set
            {
                expirationDate = value;
                OnPropertyChanged(nameof(ExpirationDate));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
