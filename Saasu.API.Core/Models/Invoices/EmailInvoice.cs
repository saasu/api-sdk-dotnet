using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Invoices
{
    public class EmailInvoice : BaseModel
    {
        /// <summary>
        /// The email address to send the invoice to.
        /// </summary>
        public string EmailTo { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
