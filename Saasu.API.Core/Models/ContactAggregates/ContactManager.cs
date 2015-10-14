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
        /// A.K.A "Title". Valid values: Mr., Mrs., Ms., Dr., Prof.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Salutation { get; set; }
        /// <summary>
        /// A.K.A "First Name". Name that is unique for an individual in a family. 
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string GivenName { get; set; }
        /// <summary>
        /// A.K.A. "Initial".
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string MiddleInitials { get; set; }
        /// <summary>
        /// A.K.A "Last Name", "Surname".
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
