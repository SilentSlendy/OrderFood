using System;
using System.ComponentModel;

namespace POS.Models
{
    /// <summary>
    /// Invoice model
    /// </summary>
    public class Invoice:INotifyPropertyChanged
    {

        public int InvoiceID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int CustomerID { get; set; } = -1;
        public int EmployeeID { get; set; } = -1;
        public double Discount { get; set; } = 0;
        public double Tax { get; set; } = 10;
        public string Note { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
