﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POS.Models
{
    /// <summary>
    /// Employee model class
    /// </summary>
    public class Employee: INotifyPropertyChanged
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool Status { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
