using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.DTOs
{
    public class InvoiceToOrderObject
    {
        public List<InvoiceDetailToCartItemObject> InvoiceDetailToCartItemObjects { get;set;}
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; } = -1;
    }
}
