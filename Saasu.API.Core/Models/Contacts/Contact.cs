using System;
using System.Collections.Generic;
using System.Linq;

namespace Saasu.API.Core.Models.Contacts
{
    /// <summary>
    /// Represents a contact resource
    /// </summary>
    public class Contact : BaseModel
    {
        /// <summary>
        /// Contact's Id in Saasu system.
        /// </summary>
        public Nullable<int> Id { get; set; }
        /// <summary>
        /// UTC date/time that contact was created in Saasu system.
        /// </summary>
        public Nullable<System.DateTime> CreatedDateUtc { get; set; }
        /// <summary>
        /// UTC date/time that contact was last modified in Saasu system.
        /// </summary>
        public Nullable<System.DateTime> LastModifiedDateUtc { get; set; }
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
        /// Indicates whether the contact is active. Default: true.
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// Id in Saasu of the Organisation or Company that employs the Contact.
        /// </summary>
        public Nullable<int> CompanyId { get; set; }

        // Get company/organisation through Company API
        /*
        public string OrganisationName { get; set; }
        /// <summary>
        /// The ABN of the organisation that this contact belongs to (if any)
        /// </summary>
        public string OrganisationAbn { get; set; }
        /// <summary>
        /// The email of the organisation that this contact belongs to (if any)
        /// </summary>
        public string CompanyEmail { get; set; }
        /// <summary>
        /// The website of the organisation that this contact belongs to (if any)
        /// </summary>
        public string OrganisationWebsite { get; set; }
        /// <summary>
        /// The trading name of the organisation that this contact belongs to (if any)
        /// </summary>
        public string TradingName { get; set; }
        /// <summary>
        /// The long description of the organisation that this contact belongs to (if any)
        /// </summary>
        public string LongDescription { get; set; }
         * */
        /// <summary>
        /// Contact's position or role.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PositionTitle { get; set; }
        /// <summary>
        /// Url of a website owned by the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string WebsiteUrl { get; set; }
        /// <summary>
        /// Primary contact number for the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PrimaryPhone { get; set; }
        /// <summary>
        /// Home contact number for the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string HomePhone { get; set; }
        /// <summary>
        /// The contacts alternate or other phone number.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string OtherPhone { get; set; }
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
        public Nullable<int> ContactManagerId { get; set; }
        /// <summary>
        /// Direct deposit details for the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public DirectDepositDetails DirectDepositDetails { get; set; }
        /// <summary>
        /// Cheque details for the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public ChequeDetails ChequeDetails { get; set; }
        /// <summary>
        /// Alternate or other street number and name
        /// </summary>
        //public string OtherStreet { get; set; }
        /// <summary>
        /// Alternate or other city
        /// </summary>
        //public string OtherCity { get; set; }
        /// <summary>
        /// Alternate or other street postcode
        /// </summary>
        //public string OtherPostcode { get; set; }
        /// <summary>
        /// Alternate or other state
        /// </summary>
        //public string OtherState { get; set; }
        /// <summary>
        /// Alternate or other Id of the country
        /// </summary>
        //public Nullable<int> OtherCountryId { get; set; }

        /// <summary>
        /// Can be used to manage extra, customer specific information.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string CustomField1 { get; set; }
        /// <summary>
        /// A second field that can be used to manage extra, customer specific information.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string CustomField2 { get; set; }
        /// <summary>
        /// Twitter handle/id for this contact. This information is for your reference and is not used in Saasu at present.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string TwitterId { get; set; }
        /// <summary>
        /// Skype name for this contact. This information is for your reference and is not used in Saasu at present.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string SkypeId { get; set; }
        /// <summary>
        /// LinkedIn profile name for this contact. This information is for your reference and is not used in Saasu at present.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LinkedInProfile { get; set; }
        /// <summary>
        ///  Determines whether statements will be automatically sent to this contact if they have any outstanding receivables.
        /// </summary>
        public Nullable<bool> AutoSendStatement { get; set; }
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
        /// Indicates the list of tags associated with this contact.
        /// </summary>
        public List<string> Tags { get; set; }

        /*
         public Nullable<byte> SaleTradingTermsType { get; set; }
        /// <summary>
        /// The sale trading terms interval value (if any)
        /// </summary>
         public Nullable<int> SaleTradingTermsInterval { get; set; }       
        /// <summary>
        /// The sale trading terms inerval value type. Available values are: Unspecified = null/0, Day = 1, Week = 2, Month = 3, CashOnDelivery = 4, Year = 5
        /// </summary>
         public Nullable<byte> SaleTradingTermsIntervalType { get; set; }
        /// <summary>
        /// The type of purchase trading terms (if any). Available values are: Unspecified= null/0, Due In = 1, Due in End of month plus # days = 2, Cash on delivery = 3
        /// </summary>
         public Nullable<byte> PurchaseTradingTermsType { get; set; }
        /// <summary>
        /// The purchase trading terms interval value (if any)
        /// </summary>
         public Nullable<int> PurchaseTradingTermsInterval { get; set; }
        /// <summary>
        /// The purchase trading terms inerval value type. Available values are: Unspecified = null/0, Day = 1, Week = 2, Month = 3, CashOnDelivery = 4, Year = 5
        /// </summary>
         public Nullable<byte> PurchaseTradingTermsIntervalType { get; set; }
         */
        /// <summary>
        /// Default discount to be applied when creating a sale for this particular contact.
        /// </summary>
        public Nullable<decimal> DefaultSaleDiscount { get; set; }
        /// <summary>
        /// Default discount to be applied when creating a purchase for this particular contact.
        /// </summary>
        public Nullable<decimal> DefaultPurchaseDiscount { get; set; }
        /// <summary>
        /// The user id of the last person to modify this contact record.
        /// </summary>
        public Nullable<int> LastModifiedByUserId { get; set; }

        /// <summary>
        /// Bpay details for the contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public BpayDetails BpayDetails { get; set; }
        /// <summary>
        /// A.K.A. "Mailing Address".
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public Address PostalAddress { get; set; }
        /// <summary>
        /// E.g. "Shipping Address".
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public Address OtherAddress { get; set; }
        /// <summary>
        /// Used for setting the due date/expiry date when creating sales invoices, orders and quotes for contacts.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public TradingTerms SaleTradingTerms { get; set; }
        /// <summary>
        /// Used for setting the due date/expiry date when creating purchase invoices, orders and quotes for contacts.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public TradingTerms PurchaseTradingTerms { get; set; }

        public override string ModelKeyValue()
        {
            return Id.ToString();
        }


    }

    /// <summary>
    /// A list of contact records.
    /// </summary>
    public class ContactResponse : BaseModel, IApiResponseCollection
    {
        public ContactResponse()
        {
            Contacts = new List<Contact>();
        }
        /// <summary>
        /// A list of contact records.
        /// </summary>
        public List<Contact> Contacts { get; set; }

        public IEnumerable<BaseModel> ListCollection()
        {
            return Contacts.AsEnumerable<BaseModel>();
        }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
