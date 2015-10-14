using System;

namespace Saasu.API.Core.Models.ContactAggregates
{
    public class ContactManager : BaseModel
    {
        /// <summary>
        /// Contact's Id in Saasu system.
        /// </summary>
        public Nullable<int> Id { get; set; }
        /// <summary>
        /// Identifier used for concurrency checking. Required for update.
        /// </summary>
        public string LastUpdatedId { get; set; }
        /// <summary>
        /// The salutation or title of the contact. Valid values: Mr., Mrs., Ms., Dr., Prof.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Salutation { get; set; }
        /// <summary>
        /// The first or given name of the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string GivenName { get; set; }
        /// <summary>
        /// The middle initials of the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string MiddleInitials { get; set; }
        /// <summary>
        /// The last name or surname of the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string FamilyName { get; set; }
        /// <summary>
        /// Contact's position or role.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PositionTitle { get; set; }
        public override string ModelKeyValue()
        {
            throw new NotImplementedException();
        }
    }
}
