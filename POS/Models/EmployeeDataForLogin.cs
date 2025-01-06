using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POS.Models
{
    /// <summary>
    /// Employee model class
    /// </summary>
    public class EmployeeDataForLogin : INotifyPropertyChanged
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool Status { get; set; } = true;
        public byte[] Username { get; set; }
        public byte[] Username_iv { get; set; }
        public byte[] Password { get; set; }
        public byte[] Password_iv { get; set; }
        public string UsernameString { get; set; }
        public string PasswordString { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
