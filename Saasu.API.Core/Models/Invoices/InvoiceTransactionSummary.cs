using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Invoices
{
    /// <summary>
    /// Details of an invoice.
    /// </summary>
    public class InvoiceTransactionSummary : BaseModel
	{
		private List<string> _tagList;

        /// <summary>
        /// The Id/key of the invoice/transaction. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
		public int? TransactionId { get; set; }
        /// <summary>
        /// A data field used to check concurreny when performing updates. This data is returned only and used for concurrency checking when performing an update/PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }
        /// <summary>
        /// The currrency code of the amounts. Eg. AUD.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Currency { get; set; }
        /// <summary>
        /// Invoice number. Use a value of <![CDATA[&lt;Auto Number&gt;]]> to automatically generate an invoice number.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// Available types are: "Pre-Quote Opportunity", "Quote" , "Purchase Order", "Sale Order", "Tax Invoice", "Adjustment Note", "RCT Invoice", "Money In (Income)", "Money Out (Expense)". 
        /// </summary>
        [Required]
        public string InvoiceType { get; set; }
        /// <summary>
        /// The transaction type of the invoice. Supported types are: 'S' = sale, 'P' = purchase, 'SP' = sale payment, 'PP' = purchase payment.
        /// </summary>
        [Required]
        public string TransactionType { get; set; }
        /// <summary>
        /// The Layout of the invoice. I = Item Sale, S = Service Sale.
        /// </summary>
        [Required]
        public string Layout { get; set; }
        /// <summary>
        /// Invoice summary.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Summary { get; set; }
        /// <summary>
        /// Total amount of the invoice.
        /// </summary>
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// Total tax amount of the invoice.
        /// </summary>
		public decimal? TotalTaxAmount { get; set; }
        /// <summary>
        /// Indicates if tax is included in the amounts. On an invoice insert or update, if this element is not included or specified,
        /// then false is assumed.
        /// </summary>
		public bool IsTaxInc { get; set; }
        /// <summary>
        /// Total amount paid.
        /// </summary>
		public decimal? AmountPaid { get; set; }
        /// <summary>
        /// Total amount owed.
        /// </summary>
		public decimal? AmountOwed { get; set; }
        /// <summary>
        /// FXRate (Foreign exchange rate) applied to this invoice.
        /// </summary>
		public decimal? FxRate { get; set; }
        /// <summary>
        /// Determines whether the FxRate is automatically populated using the Fx feed.
        /// </summary>
        public bool AutoPopulateFxRate { get; set; }
        /// <summary>
        /// Indicates whether the invoice requires follow up.
        /// </summary>
		public bool? RequiresFollowUp { get; set; }
        /// <summary>
        /// Indicates whether the invoice was sent to the contact.
        /// </summary>
		public bool? SentToContact { get; set; }
        /// <summary>
        /// Date of this transaction.
        /// </summary>
        [Required]
		public DateTime? TransactionDate { get; set; }
        /// <summary>
        /// The Id/key of the billing contact.
        /// </summary>
		public int? BillingContactId { get; set; }
        /// <summary>
        /// Billing contact first name. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string BillingContactFirstName { get; set; }
        /// <summary>
        /// Billing contact last name. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string BillingContactLastName { get; set; }
        /// <summary>
        /// Billing contact organisation name. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string BillingContactOrganisationName { get; set; }
        /// <summary>
        /// The Id/key of the shipping contact.
        /// </summary>
        public int? ShippingContactId { get; set; }
        /// <summary>
        /// Shipping contact first name. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ShippingContactFirstName { get; set; }
        /// <summary>
        /// Shipping contact last name. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ShippingContactLastName { get; set; }
        /// <summary>
        /// Shipping contact organisation name. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ShippingContactOrganisationName { get; set; }
        /// <summary>
        /// The date and time this resource was created in UTC. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
		public DateTime? CreatedDateUtc { get; set; }
        /// <summary>
        /// The date and time this resource was last modified in UTC. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public DateTime? LastModifiedDateUtc { get; set; }
        /// <summary>
        /// The payment status of this invoice. Supported types are: 'A' = all, 'P' = paid, 'U' = unpaid. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PaymentStatus { get; set; }
        /// <summary>
        /// The date that this invoice is due.
        /// </summary>
		public DateTime? DueDate { get; set; }
        /// <summary>
        /// Not required for inserting/adding an invoice as it is determined by the "InvoiceType'. 
        /// Indicator specifying: Q = Quote, O = Order, I = Invoice.
        /// Note: This field is deprecated (is readonly) and is inferred from InvoiceType. It is here for backwards compatibility only.
        /// This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string InvoiceStatus { get; set; }
        /// <summary>
        /// The purchase order number.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PurchaseOrderNumber { get; set; }
        /// <summary>
        /// The number of payments applied (if any). This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
		public short? PaymentCount { get; set; }

        /// <summary>
        /// List of tags associated with this resource.
        /// </summary>
		public List<string> Tags { get; set; }

        public override string ModelKeyValue()
        {
            return TransactionId.HasValue ? TransactionId.Value.ToString() : string.Empty;
        }
    }

	public class InvoiceTransactionSummaryResponse : BaseModel, IApiResponseCollection
	{
		public InvoiceTransactionSummaryResponse()
		{
			Invoices = new List<InvoiceTransactionSummary>();
		}
        /// <summary>
        /// List of invoices.
        /// </summary>
		public List<Core.Models.Invoices.InvoiceTransactionSummary> Invoices { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }

        public IEnumerable<BaseModel> ListCollection()
        {
            return Invoices;
        }
    }
}
