using System;

namespace POS.Models
{
    /// <summary>
    /// RevenueReport Model
    /// </summary>
    public class RevenueReport
    {
        public int ReportID { get; set; }
        public DateTime ReportDate { get; set; }
        public int Revenue { get; set; }
        public int InvoiceID { get; set; }
    }
}
