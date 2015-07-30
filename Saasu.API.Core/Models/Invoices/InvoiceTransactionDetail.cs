using Saasu.API.Core.Models.Attachments;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Invoices
{
    /// <summary>
    /// Details of an invoice
    /// </summary>
    public class InvoiceTransactionDetail : InvoiceTransactionSummary
    {
        /// <summary>
        /// The line items associated with this invoice.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public List<InvoiceTransactionLineItem> LineItems { get; set; }
        // The fields below are only populated when details are retrieved (ie. single tran Id)
        /// <summary>
        /// Textual notes set by the system.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string NotesInternal { get; set; }
        /// <summary>
        /// Textual notes set by the user.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string NotesExternal { get; set; }
        /// <summary>
        /// The trading terms of the invoice.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
		public InvoiceTradingTerms Terms { get; set; }
        /// <summary>
        /// List of attachments associated with this invoice. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
		public List<FileAttachmentInfo> Attachments { get; set; }
        /// <summary>
        /// The Id/key of the template (if any) associated with this invoice.
        /// </summary>
		public int? TemplateId { get; set; }
        /// <summary>
        /// Instruct the system to send an email to the contact as part of the insert/update.
        /// </summary>
		public bool? SendEmailToContact { get; set; }
        /// <summary>
        /// The Email message to send to the contact if instructed. Note: This is only applicable when updating (PUT) or inserting (POST) a transaction and is not returned when making a GET request.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public Email EmailMessage { get; set; }

		/// <summary>
        /// Payment to be applied to this invoice. Quickpayments can only be inserted/added via a HTTP POST request. Updates via HTTP PUT are not support.
        /// Note: This is only applicable when updating (PUT) or inserting (POST) a transaction and is not returned when making a GET request.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public InvoiceQuickPaymentDetail QuickPayment { get; set; }
    }
}
