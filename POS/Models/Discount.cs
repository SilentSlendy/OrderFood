using System.ComponentModel;

namespace POS.Models
{
    /// <summary>
    /// Discount model
    /// </summary>
    public class Discount : INotifyPropertyChanged
    {
        public int DiscountId { get; set; }
        public string DiscountCode { get; set; }
        public int DiscountValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
