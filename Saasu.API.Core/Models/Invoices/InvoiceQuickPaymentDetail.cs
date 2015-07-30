using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Invoices
{
    /// <summary>
    /// Payment to be applied to this invoice.
    /// </summary>
	public class InvoiceQuickPaymentDetail
	{
        /// <summary>
        /// When the payment was made.
        /// </summary>
        [Required]
		public DateTime? DatePaid { get; set; }
        /// <summary>
        /// When the payment was cleared.
        /// </summary>
		public DateTime? DateCleared { get; set; }
        /// <summary>
        /// The bank account uid where the payment was banked to.
        /// </summary>
        [Required]
		public int? BankedToAccountId { get; set; }
        /// <summary>
        /// The payment amount. It must be less than the invoice total. Maximum 2 decimals.
        /// </summary>
        [Required]
		public decimal? Amount { get; set; }
        /// <summary>
        /// Payment reference. It can be used to track cheque #, etc.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Reference { get; set; }
        /// <summary>
        /// Brief summary for this payment. Leave this blank to let the system sets this automatically.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Summary { get; set; }

	}
}
