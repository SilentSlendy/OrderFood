using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    /// <summary>
    /// InvoiceDetailWithProductInfo class
    /// </summary>
    public class InvoiceDetailWithProductInfo: INotifyPropertyChanged
    {
        public InvoiceDetail InvoiceDetailProperty { get; set; }
        public string ProductName
        { get; set; }

        public string CategoryName
        { get; set; }

        public string Description
        { get; set; }
        public Product ProductInfo { get; set;}
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
