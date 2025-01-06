using System.ComponentModel;

namespace POS.Models
{
    /// <summary>
    /// InvoiceDetail class
    /// </summary>
    public class InvoiceDetail: INotifyPropertyChanged
    {
        public int DetailID { get; set; }
        public int InvoiceID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int TotalAmount { get; set; }
        public string Note { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
