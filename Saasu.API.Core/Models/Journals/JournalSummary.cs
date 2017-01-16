using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saasu.API.Core.Models.Journals
{
    /// <summary>
    /// High level details of a journal transaction.
    /// </summary>
    public class JournalSummary : BaseModel
    {
        /// <summary>
        /// The Id/key of the journal/transaction. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public int? TransactionId { get; set; }

        /// <summary>
        /// A data field used to check concurreny when performing updates. This data is returned only and used for concurrency checking when performing an update/PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }

        /// <summary>
        /// Date of this transaction.
        /// </summary>
        [Required]
        public DateTime? TransactionDate { get; set; }

        /// <summary>
        /// Journal summary.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Summary { get; set; }

        /// <summary>
        /// The currrency code of the amounts. Eg. AUD.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Currency { get; set; }


        /// <summary>
        /// FXRate (Foreign exchange rate) applied to this invoice.
        /// </summary>
        public decimal? FxRate { get; set; }

        /// <summary>
        /// Determines whether the FxRate is automatically populated using the Fx feed.
        /// </summary>
        public bool AutoPopulateFxRate { get; set; }

        /// <summary>
        /// Journal reference
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Reference { get; set; }


        /// <summary>
        /// The Id/key of the journal transaction's contact.
        /// </summary>
        public int? JournalContactId { get; set; }

        /// <summary>
        /// Contact first name. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ContactFirstName { get; set; }

        /// <summary>
        /// Contact last name. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ContactLastName { get; set; }

        /// <summary>
        /// Contact organisation name. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ContactOrganisationName { get; set; }


        /// <summary>
        /// Journal transaction requires follow up
        /// </summary>
        public bool? RequiresFollowUp { get; set; }


        /// <summary>
        /// List of tags associated with this resource.
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// The date and time this resource was created in UTC. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public DateTime? CreatedDateUtc { get; set; }
        /// <summary>
        /// The date and time this resource was last modified in UTC. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public DateTime? LastModifiedDateUtc { get; set; }


        /// <summary>
        /// Key Identifier for the model
        /// </summary>
        public override string ModelKeyValue()
        {
            return TransactionId == null ? string.Empty : TransactionId.ToString();
        }
    }
}
