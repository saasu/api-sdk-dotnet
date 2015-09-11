using System;

namespace Saasu.API.Core.Models.Search
{
    /// <summary>
    /// Transaction that matches the search criteria. 
    /// This resource contains only fields that are indexed for a Transaction, not all the fields that would be found on the Invoice resource.
    /// </summary>
    public class TransactionSearchResponse : BaseModel
    {
        /// <summary>
        /// The type of entity transacted.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// The Unique Id/key for the Transaction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The transaction date.
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Date that payment is due for a Transaction. Applies to Sale and Purchase transactions.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Summary/description for a Transaction.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Summary { get; set; }

        /// <summary>
        /// Identifier for the Contact associated with the Transaction.
        /// </summary>
        public int ContactId { get; set; }

        /// <summary>
        /// Full name of the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ContactName { get; set; }

        /// <summary>
        /// Is used as an Account or Contact reference for this person if they are a supplier or customer. This is your reference or their reference depending on how you prefer to use this field.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string CustomerContactId { get; set; }

        /// <summary>
        /// Email address for the contact associated with the Transaction.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ContactEmail { get; set; }

        /// <summary>
        /// The Organisation or Company that employs the Contact.
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// Trading name of the Organisation or Company that employs the Contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string TradingName { get; set; }

        /// <summary>
        /// The transaction type of the invoice. Supported types are: 'S' = sale, 'P' = purchase, 'PE' = payroll, 'J' = journal.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Type { get; set; }

        /// <summary>
        /// Total tax amount of an Invoice transaction.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string TotalTaxAmount { get; set; }

        /// <summary>
        /// The currrency code of the amounts. Eg. AUD.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Currency { get; set; }

        /// <summary>
        /// List of tags associated with this Transaction.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Tags { get; set; }

        /// <summary>
        /// User supplied notes about the Transaction.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Notes { get; set; }

        /// <summary>
        /// Textual notes set by the user.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ExternalNotes { get; set; }

        /// <summary>
        /// Customer's reference for the Transaction. Supported on Journal and Payroll transactions.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Reference { get; set; }

        /// <summary>
        /// Invoice number for a Sale or Purchase transaction.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// The Purchase Order Number on a Sale or Purchase transaction.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PurchaseOrderNumber { get; set; }

        /// <summary>
        /// Indicates whether a Sale or Purchase transaction is fully paid.
        /// </summary>
        public bool Paid { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
