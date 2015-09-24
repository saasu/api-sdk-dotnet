using System;

namespace Saasu.API.Core.Models.ContactAggregates
{
    public class InsertContactAggregateResult : BaseModel
    {
        /// <summary>
        /// The id of the newly created/inserted contact.
        /// </summary>
		public int InsertedContactId { get; set; }
        /// <summary>
        /// The unique id associated with this update. This value is required to be passed in on subsequent updates to prevent data loss/corruption.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }
        /// <summary>
        /// The date and time this resource was modified in UTC.
        /// </summary>
		public DateTime LastModified { get; set; }

        public override string ModelKeyValue()
        {
            return InsertedContactId.ToString();
        }
    }
}
