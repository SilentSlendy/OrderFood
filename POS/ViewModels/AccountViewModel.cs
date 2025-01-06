using Microsoft.UI.Xaml;
using POS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.ViewModels
{
    public class AccountViewModel: INotifyPropertyChanged
    {
        public EmployeeDataForLogin Employee { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public AccountViewModel()
        {
            Employee = (Application.Current as App).CurrentEmployee;
        }

    }
}
