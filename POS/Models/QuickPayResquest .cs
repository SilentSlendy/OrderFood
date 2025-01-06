using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    /// <summary>
    /// QuickPayResquest Model cho việc thanh toán nhanh Mono
    /// </summary>
    public class QuickPayResquest
    {
        public string orderInfo { get; set; }
        public string partnerCode { get; set; }
        public string redirectUrl { get; set; }
        public string ipnUrl { get; set; }
        public long amount { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public string extraData { get; set; }
        public string partnerName { get; set; }
        public string storeId { get; set; }
        public string paymentCode { get; set; }
        public string orderGroupId { get; set; }
        public bool autoCapture { get; set; }
        public string lang { get; set; }
        public string signature { get; set; }
        public string requestType { get; set; }
    }
}
