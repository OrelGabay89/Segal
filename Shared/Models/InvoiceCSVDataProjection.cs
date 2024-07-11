using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class InvoiceCSVDataProjection
    {
        public string Invoice_ID { get; set; }
        public string ConfirmationNumber { get; set; }
        public string Last9Digits { get; set; }
    }

}
