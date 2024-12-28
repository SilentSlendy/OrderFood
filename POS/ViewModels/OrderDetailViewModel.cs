﻿using POS.Interfaces;
using POS.Models;
using POS.Services.DAO;
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
        public event PropertyChangedEventHandler PropertyChanged;
        private IInvoiceDao _invoiceDao = new PostgresInvoiceDao();
        private IInvoiceDetailDao _invoiceDetailDao = new PostgresInvoiceDetailDao();

        public FullObservableCollection<Order> Items { get; set; }
            = new FullObservableCollection<Order>();
        private double _subTotal;
        private double _total;
        private double _tax;

        public int InvoiceID { get; set; } = -1;
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
        public double Tax
        {
            get
            {
                return SubTotal * 0.1;
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
        public int SaveToDatabase(int invoiceID=-1)
        {
            // Save to database
            Invoice invoice = new Invoice()
            {
                TotalAmount = Total,
                Tax = 10.00,
                InvoiceDate = DateTime.Now,
                InvoiceID = invoiceID
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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
