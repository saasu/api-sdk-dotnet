using System;
using System.Collections.Generic;

namespace Saasu.API.Core.Models.Payments
{
    public class PaymentTransactionSummary : BaseModel
    {
        /// <summary>
        /// The Id of the transaction.
        /// </summary>
        public int? TransactionId { get; set; }
        /// <summary>
        /// The date the payment was received or made.
        /// </summary>        
        [System.ComponentModel.DataAnnotations.Required]                       
        public System.DateTime TransactionDate { get; set; }
        /// <summary>
        /// Sale Payment(SP) or Purchase Payment(PP)
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]                       
        public string TransactionType { get; set; }
        /// <summary>
        /// Bank Account ID used to receive or pay funds.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]                       
        public int PaymentAccountId { get; set; }
        /// <summary>
        /// Total payment amount received or paid.
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// Fee amount associated with the payment.
        /// </summary>
        public decimal? FeeAmount { get; set; }
        /// <summary>
        /// Summary of the payment.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Summary { get; set; }
        /// <summary>
        /// Payment reference identifier.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Reference { get; set; }
        /// <summary>
        /// The date the payment was cleared.
        /// </summary>
        public Nullable<System.DateTime> ClearedDate { get; set; }
        /// <summary>
        /// Currency code of the payment eg: AUD or USD.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Currency { get; set; }
        /// <summary>
        /// Determines with if FxRate is automatically populated using the Saasu FX Feed.
        /// </summary>
        public bool AutoPopulateFxRate { get; set; }
        /// <summary>
        /// If AutoPopulateFxRate is False then the specified FxRate will be used.
        /// </summary>
        public decimal FxRate { get; set; }
        /// <summary>
        /// The date and time when this payment was created in UTC.
        /// </summary>
        public Nullable<System.DateTime> CreatedDateUtc { get; set; }
        /// <summary>
        /// The date and time when this payment last modified in UTC.
        /// </summary>
        public Nullable<System.DateTime> LastModifiedDateUtc { get; set; }

        /// <summary>
        /// Used for checking concurreny when performing updates.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }
        /// <summary>
        /// Indicates whether this payment requires follow up.
        /// </summary>
        public bool RequiresFollowUp { get;set; }

        public override string ModelKeyValue()
        {
            return TransactionId.HasValue ? TransactionId.Value.ToString() : string.Empty;
        }
    }

    public class PaymentTransaction : PaymentTransactionSummary
    {        
        /// <summary>
        /// Custom user supplied notes about the payment.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Notes { get; set; }

        /// <summary>
        /// Invoices that are paid as part of the payment.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]               
        public List<PaymentItem> PaymentItems { get; set; }      
    }

    public class PaymentTransactionSummaryResponse : BaseModel, IApiResponseCollection
    {
        public PaymentTransactionSummaryResponse()
        {
            PaymentTransactions = new List<PaymentTransactionSummary>();
        }
        
        /// <summary>
        /// A list of payment transactions.
        /// </summary>
        public List<PaymentTransactionSummary> PaymentTransactions { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }

        public IEnumerable<BaseModel> ListCollection()
        {
            return PaymentTransactions;
        }
    }
}
