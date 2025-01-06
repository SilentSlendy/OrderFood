using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using POS.Helpers;
using POS.Interfaces;
using POS.Models;
using POS.Services.DAO;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace POS.ViewModels
{
    /// <summary>
    /// View model for Payment
    /// </summary>
    public class PaymentViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// DAO for invoice
        /// </summary>
        private IInvoiceDao _invoiceDao = new PostgresInvoiceDao();
        /// <summary>
        /// DAO for invoice detail
        /// </summary>
        private IInvoiceDetailDao _invoiceDetailDao = new PostgresInvoiceDetailDao();
        /// <summary>
        /// DAO for discount
        /// </summary>
        private IDiscountDao _discountDao = new PostgresDiscountDao();
        /// <summary>
        /// DAO for customer
        /// </summary>
        private ICustomerDao _customerDao = new PostgresCustomerDao();
        /// <summary>
        /// View model for settings
        /// </summary>
        private SettingsViewModel _settingsViewModel = new SettingsViewModel();
        private OrderDetailViewModel _orderDetailViewModel = new OrderDetailViewModel();
        /// <summary>
        /// Mã hóa Feistel
        /// </summary>
        private FeistelCipher _feistelCipher = new FeistelCipher(8, "winui3_discount_key");
        /// <summary>
        /// HttpClient for MoMo API
        /// </summary>
        private static readonly HttpClient client = new HttpClient();
        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public ObservableCollection<string> PaymentMethods { get; set; }
        /// <summary>
        /// Danh sách sản phẩm trong hóa đơn
        /// </summary>
        public ObservableCollection<Order> Items { get; set; }

        /// <summary>
        /// Phương thức thanh toán được chọn
        /// </summary>
        public string SelectedPaymentMethod { get; set; }
        /// <summary>
        /// Tổng tiền cần thanh toán
        /// </summary>
        public int TotalCost { get; set; }
        /// <summary>
        /// Tổng tiền cần thanh toán sau khi đã tính thuế và giảm giá
        /// </summary>
        public int TotalPayable { get; set; }
        /// <summary>
        /// ID của hóa đơn
        /// </summary>
        public int InvoiceId { get; set; } = -1;
        /// <summary>
        /// ID của khách hàng
        /// </summary>
        public int CustomerID { get; set; } = -1;
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime PaymentDate { get; set; }
        /// <summary>
        /// access key cho momo
        /// </summary>
        public string accessKey { get; set; }
        /// <summary>
        /// secret key cho momo
        /// </summary>
        public string secretKey { get; set; }
        /// <summary>
        /// Link webhook
        /// </summary>
        public string ipnUrl { get; set; }

        /// <summary>
        /// Tổng tiền cần thanh toán sau khi đã tính thuế và giảm giá.
        /// </summary>
        private double _vat;
        /// <summary>
        /// Tổng tiền cần thanh toán
        /// </summary>
        private int _receivedAmount;
        /// <summary>
        /// Số tiền thừa hoặc thiếu sau khi đã thanh toán.
        /// </summary>
        private int _change;
        /// <summary>
        /// Mã giảm giá
        /// </summary>
        private string _discountCode;
        /// <summary>
        /// Giá trị giảm giá
        /// </summary>
        private int _discountValue = 0;
        /// <summary>
        /// Trạng thái mã giảm giá
        /// </summary>
        private string _discountStatus = "";

        /// <summary>
        /// Constructor
        /// </summary>
        public PaymentViewModel()
        {
            // Khởi tạo các thuộc tính
            PaymentMethods = new ObservableCollection<string>
                {
                    "Tiền mặt",
                    "Momo"
                };
            SelectedPaymentMethod = "Tiền mặt";

            PaymentDate = DateTime.Now; // Ngày thanh toán hiện tại

        }

        /// <summary>
        /// Gán danh sách sản phẩm và tổng tiền cần thanh toán
        /// </summary>
        /// <param name="items"></param>
        /// <param name="total"></param>
        /// <param name="flag"></param>
        public void SetItems(int invoiceID,ObservableCollection<Order> items, double total, int customerId, int flag)
        {
            Items = new ObservableCollection<Order>(items);
            OnPropertyChanged(nameof(Items));
            TotalCost = (int)total;
            OnPropertyChanged(nameof(TotalCost));
            CalculateTotalPayment();
            OnPropertyChanged(nameof(TotalPayable));
            InvoiceId = invoiceID;
            CustomerID = customerId;
        }

        /// <summary>
        /// Thuế 
        /// </summary>
        public double VAT
        {
            get => _vat;
            set
            {
                if (_vat != value)
                {
                    _vat = value;
                    OnPropertyChanged(nameof(VAT));
                    CalculateTotalPayment();
                }
            }
        }

        /// <summary>
        /// Tính tổng tiền cần thanh toán sau khi đã tính thuế và giảm giá.
        /// </summary>
        public int ReceivedAmount
        {
            get => _receivedAmount;
            set
            {
                if (_receivedAmount != value)
                {
                    _receivedAmount = value;
                    OnPropertyChanged(nameof(ReceivedAmount));
                    CalculateChange();
                }
            }
        }

        /// <summary>
        /// Tính số tiền thừa hoặc thiếu sau khi đã thanh toán.
        /// </summary>
        public int Change
        {
            get => _change;
            private set
            {
                if (_change != value)
                {
                    _change = value;
                    OnPropertyChanged(nameof(Change));
                }
            }
        }

        /// <summary>
        /// Mã giảm giá
        /// </summary>
        public string DiscountCode
        {
            get => _discountCode;
            set
            {
                if (_discountCode != value)
                {
                    _discountCode = value;
                    OnPropertyChanged(nameof(DiscountCode));
                    // Gọi hàm CheckDiscountCode khi DiscountCode thay đổi
                    CheckDiscountCode(_discountCode);
                }
            }
        }

        /// <summary>
        /// Giá trị giảm giá
        /// </summary>
        public int DiscountValue
        {
            get => _discountValue;
            set
            {
                if (_discountValue != value)
                {
                    _discountValue = value;
                    OnPropertyChanged(nameof(DiscountValue));
                    // Tính toán lại TotalPayable khi DiscountValue thay đổi
                    CalculateTotalPayment();
                }
            }
        }

        /// <summary>
        /// Trạng thái mã giảm giá
        /// </summary>
        public string DiscountStatus
        {
            get => _discountStatus;
            set
            {
                if (_discountStatus != value)
                {
                    _discountStatus = value;
                    OnPropertyChanged(nameof(DiscountStatus));
                }
            }
        }

        /// <summary>
        /// Tính tổng tiền cần thanh toán sau khi đã tính thuế và giảm giá.
        /// </summary>
        public void CalculateTotalPayment()
        {
            TotalPayable = (int)(TotalCost + (TotalCost * (VAT / 100)) - DiscountValue);

            if (TotalPayable < 0)
            {
                TotalPayable = 0;
            }
        }

        /// <summary>
        /// Tính số tiền thừa hoặc thiếu sau khi đã thanh toán.
        /// </summary>
        public void CalculateChange()
        {
            if (ReceivedAmount < TotalPayable)
            {
                Change = 0;
                return;
            }
            Change = ReceivedAmount - TotalPayable;
        }

        /// <summary>
        /// Lưu hóa đơn vào cơ sở dữ liệu.
        /// </summary>
        /// <returns></returns>
        public int SaveToDB()
        {
            Invoice invoice = new Invoice()
            {
                TotalAmount = TotalPayable,
                Tax = VAT,
                InvoiceDate = PaymentDate,
                PaymentMethod = SelectedPaymentMethod,
                Discount = DiscountValue,
                InvoiceID = InvoiceId,
                CustomerID = this.CustomerID
            };
            int newInvoiceId;
            if (InvoiceId == -1)
            {
                newInvoiceId = _invoiceDao.InsertInvoice(invoice);
            }
            else
            {
                _invoiceDao.RemoveInvoiceById(InvoiceId);
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
        /// Cập nhật hóa đơn vào cơ sở dữ liệu.
        /// </summary>
        public void UpdateDB()
        {
            Invoice invoice = _invoiceDao.GetInvoiceById(InvoiceId);
            // Update invoice
            invoice.TotalAmount = TotalPayable;
            invoice.Tax = VAT;
            invoice.Discount = DiscountValue;
            invoice.InvoiceDate = PaymentDate;
            invoice.PaymentMethod = SelectedPaymentMethod;
            invoice.Note = "NULL"; // cần cập nhật thêm
            CustomerID = this.CustomerID;
            _invoiceDao.UpdateInvoice(invoice);

            foreach (var item in Items)
            {
                InvoiceDetail invoiceDetail = new InvoiceDetail()
                {
                    InvoiceID = InvoiceId,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _invoiceDetailDao.InsertInvoiceDetail(invoiceDetail);
            }
        }

        /// <summary>
        /// Reset dữ liệu của hóa đơn.
        /// </summary>
        public void ResetData()
        {
            Items.Clear();
            OnPropertyChanged(nameof(Items));
            TotalCost = 0;
            OnPropertyChanged(nameof(TotalCost));
            TotalPayable = 0;
            OnPropertyChanged(nameof(TotalPayable));
            ReceivedAmount = 0;
            OnPropertyChanged(nameof(ReceivedAmount));
            Change = 0;
            OnPropertyChanged(nameof(Change));
            DiscountValue = 0;
            OnPropertyChanged(nameof(DiscountValue));
            DiscountCode = "";
            OnPropertyChanged(nameof(DiscountCode));
            DiscountStatus = "";
            OnPropertyChanged(nameof(DiscountStatus));
        }

        /// <summary>
        /// Kiểm tra mã giảm giá.
        /// </summary>
        /// <param name="discountCode"></param>
        /// <returns></returns>

        public bool CheckDiscountCode(string discountCode)
        {
            List<int> discountValues = new List<int> { 10000, 20000, 50000, 100000, 200000, 500000 };
            try
            {
                int discountValue = _feistelCipher.Decrypt(discountCode);

                if (discountValues.Contains(discountValue))
                {
                    DiscountValue = discountValue;
                    DiscountStatus = "Đã áp dụng mã giảm giá.";
                    CalculateTotalPayment();
                    OnPropertyChanged(nameof(TotalPayable));
                    return true;
                }
                else
                {
                    DiscountValue = 0;
                    DiscountStatus = "Mã giảm giá không hợp lệ.";
                    CalculateTotalPayment();
                    return false;
                }
            }
            catch (Exception ex)
            {
                DiscountValue = 0;
                CalculateTotalPayment();
                System.Diagnostics.Debug.WriteLine($"Error decrypting code: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa mã giảm giá đã sử dụng.
        /// </summary>
        public void DeleteUsedDiscountCode()
        {
            _discountDao.RemoveDiscountByCode(DiscountCode);
        }

        /// <summary>
        /// Tạo chữ ký cho MoMo API
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static String getSignature(String text, String key)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Gửi yêu cầu thanh toán đến MoMo API
        /// </summary>
        /// <returns></returns>
        public async Task<string> RequestMoMoPayment()
        {
            // Generate UUID for requestId and orderId
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();

            QuickPayResquest request = new QuickPayResquest();
            request.orderInfo = "Thanh_toan_hoa_don_POS_HCMUS";
            request.partnerCode = "MOMO";
            request.redirectUrl = "";
            request.ipnUrl = "https://webhook.site/4cb43743-df24-494e-839d-c6cc184d872c";
            request.amount = (long)TotalPayable;
            request.orderId = myuuidAsString;
            request.requestId = myuuidAsString;
            request.extraData = "";
            request.partnerName = "MOMO";
            request.storeId = "POS HCMUS";
            request.orderGroupId = "";
            request.autoCapture = true;
            request.lang = "vi"; // en or vi
            request.requestType = "captureWallet"; // captureWallet or captureMoMo

            var rawSignature = "accessKey=" + accessKey +
                               "&amount=" + request.amount +
                               "&extraData=" + request.extraData +
                               "&ipnUrl=" + request.ipnUrl +
                               "&orderId=" + request.orderId +
                               "&orderInfo=" + request.orderInfo +
                               "&partnerCode=" + request.partnerCode +
                               "&redirectUrl=" + request.redirectUrl +
                               "&requestId=" + request.requestId +
                               "&requestType=" + request.requestType;
            request.signature = getSignature(rawSignature, secretKey);

            // Call MoMo API
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", httpContent);

            // Read response
            var contents = await quickPayResponse.Content.ReadAsStringAsync();
            JObject jMessage = JObject.Parse(contents);
            //System.Diagnostics.Debug.WriteLine("Response: " + jMessage);
            //System.Diagnostics.Debug.WriteLine("payUrl: " + jMessage["payUrl"].ToString());
            
            // Return the payUrl
            return jMessage["payUrl"].ToString();
        }

        /// <summary>
        /// Gán tên khách hàng từ ID.
        /// </summary>
        public void GetCustomerName()
        {
            CustomerName = _customerDao.GetCustomerNameById(CustomerID);
        }

        /// <summary>
        /// Load các thiết lập cục bộ.
        /// </summary>
        public void GetValuesInLocalSettings()
        {
            VAT = _settingsViewModel.VAT;
            OnPropertyChanged(nameof(VAT));
            accessKey = _settingsViewModel.AccessKey;
            secretKey = _settingsViewModel.SecretKey;
            ipnUrl = _settingsViewModel.IpnUrl;
        }

        /// <summary>
        /// Sự kiện khi thuộc tính của ViewModel thay đổi.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Phương thức gọi sự kiện khi thuộc tính thay đổi.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
