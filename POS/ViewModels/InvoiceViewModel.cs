using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Helpers;
using POS.Interfaces;
using POS.Services.DAO;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace POS.ViewModels
{
    /// <summary>
    /// View model for Invoice
    /// </summary>
    public class InvoiceViewModel:INotifyPropertyChanged
    {
        public delegate void OrderMoreDishesEventHandler();
        public event OrderMoreDishesEventHandler OrderMoreDishes;
        private ObservableCollection<WholeInvoice> _invoices;
        public ObservableCollection<WholeInvoice> Invoices
        {
            get => _invoices;

            set
            {
                if (_invoices != value)
                {
                    _invoices = value;
                    OnPropertyChanged();
                }
            }
        }


        private IInvoiceDao _invoiceDao;
        private IInvoiceDetailDao _invoiceDetailDao;
        public string searchText;       
        public int CurrentPage { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 14;
        public int TotalPages { get; set; } = 0;
        public int TotalItems { get; set; } = 0;

        public WholeInvoice SelectedInvoice { get; set; }
        public InvoiceViewModel()
        {
            _invoiceDao = new PostgresInvoiceDao();
            _invoiceDetailDao = new PostgresInvoiceDetailDao();
            Invoices = new ObservableCollection<WholeInvoice>();
            LoadInvoices(1);
        }

        public void DeleteInvoice()
        {
            if (SelectedInvoice == null)
            {
                return;
            }
            _invoiceDao.RemoveInvoiceById(SelectedInvoice.Invoice.InvoiceID);
            LoadInvoices(CurrentPage);
        }
        public void GetAllInvoices()
        {
            var (totalItems, invoices) = _invoiceDao.GetAllInvoices(searchText,

                CurrentPage, ItemsPerPage);
            var temp = new ObservableCollection<WholeInvoice>();
            foreach (var invoice in invoices)
            {
                var (invoiceDetailsNumber, invoiceDetailsWithProductInfo) = _invoiceDetailDao.GetAllInvoiceDetailsWithProductInformation(invoice.InvoiceID);

                var wholeInvoice = new WholeInvoice
                {
                    Invoice = invoice,
                    InvoiceDetailsWithProductInfo = new FullObservableCollection<InvoiceDetailWithProductInfo>(invoiceDetailsWithProductInfo)

                };
                temp.Add(wholeInvoice);
            }
            Invoices = new FullObservableCollection<WholeInvoice>(temp);
            TotalItems = totalItems;
            TotalPages = (TotalItems / ItemsPerPage)
                + ((TotalItems % ItemsPerPage == 0)
                        ? 0 : 1);
        }
        public void GetAllNotPaidInvoices()
        {
            var (totalItems, invoices) = _invoiceDao.GetAllNotPaidInvoices(searchText,

                CurrentPage, ItemsPerPage);
            var temp = new ObservableCollection<WholeInvoice>();
            foreach (var invoice in invoices)
            {
                var (invoiceDetailsNumber, invoiceDetailsWithProductInfo) = _invoiceDetailDao.GetAllInvoiceDetailsWithProductInformation(invoice.InvoiceID);

                var wholeInvoice = new WholeInvoice
                {
                    Invoice = invoice,
                    InvoiceDetailsWithProductInfo = new FullObservableCollection<InvoiceDetailWithProductInfo>(invoiceDetailsWithProductInfo)

                };
                temp.Add(wholeInvoice);
            }
            Invoices = new FullObservableCollection<WholeInvoice>(temp);
            TotalItems = totalItems;
            TotalPages = (TotalItems / ItemsPerPage)
                + ((TotalItems % ItemsPerPage == 0)
                        ? 0 : 1);
        }


        public void LoadInvoices(int page)
        {
            CurrentPage = page;
            GetAllInvoices();
        }

        public void LoadNotPaidInvoices(int page)
        {
            CurrentPage = page;
            GetAllNotPaidInvoices();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
