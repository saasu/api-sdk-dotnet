using System;

namespace Saasu.API.Core.Models.Invoices
{
	public class UpdateInvoiceResult : BaseModel
	{
        /// <summary>
        /// A unique update Id that is required when updating the record to ensure no consistency conflicts and data loss occur.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }
        /// <summary>
        /// Date and time when this resource was last modified in UTC.
        /// </summary>
		public DateTime UtcLastModified { get; set; }
        /// <summary>
        /// Indicates whether this was sent to the contact.
        /// </summary>
		public bool SentToContact { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
