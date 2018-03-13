using System;
using System.Collections.Generic;

namespace Saasu.API.Core.Models.Activities
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivitySummary : BaseModel
    {
        /// <summary>
        /// The Id/key of the activity. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// A data field used to check concurreny when performing updates. This data is returned only and used for concurrency checking when performing an update/PUT.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }

        public string ActivityType { get; set; }
        public bool Done { get; set; }
        public DateTime Due { get; set; }
        public string Title { get; set; }

        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerEmail { get; set; }
        public string AttachedToType { get; set; }
        public int? AttachedToId { get; set; }

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
            return Id == null ? string.Empty : Id.ToString();
        }
    }
}
