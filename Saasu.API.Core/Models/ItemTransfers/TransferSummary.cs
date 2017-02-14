using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saasu.API.Core.Models.ItemTransfers
{
    /// <summary>
    /// Record returned in a colletion when requesting a lit of item transfers.
    /// </summary>
    public class TransferSummary : BaseModel
    {
        /// <summary>
        /// The Id/key of the transfer transaction. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// A data field used to check concurreny when performing updates. This data is returned only and used for concurrency checking when performing an update/PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }

        /// <summary>
        /// Date of this transaction.
        /// </summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// Item transfer summary.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// List of tags associated with this resource.
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Transfer transaction requires follow up.
        /// </summary>
        public bool? RequiresFollowUp { get; set; }

        /// <summary>
        /// The date and time this resource was created in UTC. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public DateTime? CreatedDateUtc { get; set; }
        /// <summary>
        /// The date and time this resource was last modified in UTC. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public DateTime? LastModifiedDateUtc { get; set; }

        /// <summary>
        /// Key Identifier for the model.
        /// </summary>
        public override string ModelKeyValue()
        {
            return Id == null ? string.Empty : Id.ToString();
        }
    }
}
