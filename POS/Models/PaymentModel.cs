using System.ComponentModel;

namespace POS.Models
{
    /// <summary>
    /// PaymentModel class
    /// </summary>
    public class PaymentModel : INotifyPropertyChanged
    {
        public int TotalBill { get; set; }
        public double VAT { get; set; }
        public int Discount { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
