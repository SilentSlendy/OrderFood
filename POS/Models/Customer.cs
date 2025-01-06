using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POS.Models
{
    /// <summary>
    /// Customer class
    /// </summary>
    public class Customer: INotifyPropertyChanged
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CustomerType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
