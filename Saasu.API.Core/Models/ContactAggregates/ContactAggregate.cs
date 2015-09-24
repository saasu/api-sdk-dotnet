using System;

namespace Saasu.API.Core.Models.ContactAggregates
{
    /// <summary>
    /// Represents a contact aggregate resource, including the contact's Company and Contact Manager
    /// </summary>
    public class ContactAggregate : BaseModel
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
        /// The Organisation or Company that employs the Contact.
        /// </summary>
        public Company Company { get; set; }
        /// <summary>
        /// Contact's position or role.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PositionTitle { get; set; }
        /// <summary>
        /// Primary contact number for the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PrimaryPhone { get; set; }
        /// <summary>
        /// The contact's mobile phone number.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string MobilePhone { get; set; }
        /// <summary>
        /// The contacts fax number.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Fax { get; set; }
        /// <summary>
        /// The contact's email address.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string EmailAddress { get; set; }
        /// <summary>
        /// Is used as an Account or Contact reference for this person if they are a supplier or customer. This is your reference or their reference depending on how you prefer to use this field.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ContactId { get; set; }
        /// <summary>
        /// This is another Contact record in Saasu and is used to represent the Account Manager, Salesperson or similar for this Contact record.
        /// </summary>
        public ContactManager ContactManager { get; set; }
        /// <summary>
        /// Indicates if the contact is a partner.
        /// </summary>
        public bool IsPartner { get; set; }
        /// <summary>
        /// Indicates if the contact is a customer.
        /// </summary>
        public bool IsCustomer { get; set; }
        /// <summary>
        /// Indicates if the contact is a supplier.
        /// </summary>
        public bool IsSupplier { get; set; }
        /// <summary>
        /// Indicates if the contact is a contractor. This is important if you need to use the taxable payment reporting feature.
        /// </summary>
        public bool IsContractor { get; set; }
        /// <summary>
        /// A.K.A. "Mailing Address".
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public Address PostalAddress { get; set; }
        public override string ModelKeyValue()
        {
            return Id.ToString();
        }
    }
}
