using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    /// <summary>
    /// WholeInvoice class
    /// </summary>
    public class WholeInvoice: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Invoice Invoice { get; set; }
        public bool IsPaid => Invoice.PaymentMethod != null;
        public ObservableCollection<InvoiceDetailWithProductInfo> InvoiceDetailsWithProductInfo { get; set; }
    }
}
