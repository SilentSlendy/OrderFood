using System;

namespace POS.Models
{
    /// <summary>
    /// Represents an inventory report.
    /// </summary>
    public class InventoryReport
    {
        public int ReportID { get; set; }
        public DateTime ReportDate { get; set; }
        public int IngredientID { get; set; }
        public int RemainingQuantity { get; set; }
    }
}
