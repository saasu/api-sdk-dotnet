using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Invoices
{
	public class InsertInvoiceResult : UpdateInvoiceResult
	{
        /// <summary>
        /// The Id of the inserted invoice.
        /// </summary>
		public int InsertedEntityId { get; set; }
        /// <summary>
        /// The invoice number that was generated for the invoice.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string GeneratedInvoiceNumber { get; set; }
        
        public override string ModelKeyValue()
        {
            return InsertedEntityId.ToString();
        }
    }
}
