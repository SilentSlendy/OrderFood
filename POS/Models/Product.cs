
﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POS.Models
{
    /// <summary>
    /// Product Model
    /// </summary>
    public class Product: INotifyPropertyChanged
    {
        private string name;
        public int ProductID { get; set; }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        public string Category { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public bool Status { get; set; } = true;

        /// <summary>
        /// Gán giá trị từ một Product khác
        /// </summary>
        /// <param name="source"></param>
        public void AssignFrom(Product source)
        {
            this.ProductID = source.ProductID;
            this.Name = source.Name;
            this.Category = source.Category;
            this.Price = source.Price;
            this.Description = source.Description;
            this.ImagePath = source.ImagePath;
            this.Status = source.Status;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
