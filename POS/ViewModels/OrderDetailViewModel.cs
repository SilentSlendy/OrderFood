using POS.Interfaces;
using POS.Models;
using POS.Services.DAO;
using POS.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace POS.ViewModels
{
    
    public class OrderDetailViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// View model for settings
        /// </summary>
        private SettingsViewModel _settingsViewModel = new SettingsViewModel();
        public event PropertyChangedEventHandler PropertyChanged;
        private IInvoiceDao _invoiceDao = new PostgresInvoiceDao();
        private ICustomerDao _customerDao = new PostgresCustomerDao();
        private IInvoiceDetailDao _invoiceDetailDao = new PostgresInvoiceDetailDao();
        public List<Customer> AllCustomers { get; set; }
        public FullObservableCollection<Order> Items { get; set; }
            = new FullObservableCollection<Order>();
        private double _subTotal;
        private double _total;
        private double _tax;
        private double VAT;
        public int CustomerID { get; set; } = -1;
        public string CustomerName { get; set; }
        public void getCustomersListForAutoSuggest()
        {
            AllCustomers = _customerDao.GetAllCustomers();
        }
        public void SetCustomerNameByCustomerID(int customerID)
        {
            CustomerID = customerID;
            CustomerName = (CustomerID == -1) ? "" : AllCustomers.Find(c => c.CustomerID == CustomerID).Name;
        }
        public OrderDetailViewModel()
        {
            getCustomersListForAutoSuggest();
            CustomerName = (CustomerID == -1) ? "" : AllCustomers.Find(c => c.CustomerID == CustomerID).Name;
        }
        public int InvoiceID { get; set; } = -1;
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public double SubTotal
        {
            get
            {
                return Items.Sum(item => item.Total);
            }
            set
            {
                if (_subTotal != value)
                {
                    _subTotal = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Total
        {
            get
            {
                return SubTotal + Tax;
            }
            set
            {
                if (_total != value)
                {
                    _total = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// ///////////////////
        /// </summary>

        public double Tax
        {
            get
            {
                return SubTotal * VAT * 0.01; 
            }
            set
            {
                if (_tax != value)
                {
                    _tax = value;
                    OnPropertyChanged();
                }
            }
        }
        public void Add(Product info, int quanlity, string note)
        {
            var foundItem = Items.FirstOrDefault(item => item.Name == info.Name && item.Note==note &&item.Price==info.Price);
            if (foundItem != null)
            {
                foundItem.Quantity += quanlity;
                foundItem.Total = foundItem.Quantity * foundItem.Price;
                foundItem.Note = note;
            }
            else
            {
                Items.Add(new Order(info,quanlity,note));
            }
            SubTotal = Items.Sum(item => item.Total);
            Tax = SubTotal * 0.1;
            Total = SubTotal + Tax;
        }
        public void Remove(Order item)
        {
            Items.Remove(item);
            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(SubTotal));
        }
        public void Clear()
        {
            Items.Clear();
            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(SubTotal));
        }
        public int SaveToDatabase(int invoiceID=-1,int customerID=-1)
        {
            // Save to database
            Invoice invoice = new Invoice()
            {
                TotalAmount = Total,
                Tax = 10.00,
                InvoiceDate = DateTime.Now,
                InvoiceID = invoiceID,
                CustomerID = customerID,
            };
            int newInvoiceId;
            if (invoiceID==-1)
            { 
                newInvoiceId = _invoiceDao.InsertInvoice(invoice); 
            }
            else
            {
                _invoiceDao.RemoveInvoiceById(invoiceID);
                newInvoiceId = _invoiceDao.InsertInvoiceWithId(invoice);
            }
            foreach (var item in Items)
            {
                InvoiceDetail invoiceDetail = new InvoiceDetail()
                {
                    InvoiceID = newInvoiceId,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Note = item.Note
                };
                _invoiceDetailDao.InsertInvoiceDetail(invoiceDetail);
            }
            return newInvoiceId;
        }

        /// <summary>
        /// Cập nhật VAT sau khi setting they đổi
        /// </summary>
        /// <param name="value"></param>
        public void GetVATValue()
        {
            VAT = _settingsViewModel.VAT;

        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
