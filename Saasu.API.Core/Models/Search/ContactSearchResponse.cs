using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Search
{
    /// <summary>
    /// Contact that matches the search criteria. 
    /// This resource contains only fields that are indexed for a contact, not all the fields that would be found on the Contact resource.
    /// </summary>
    public class ContactSearchResponse : BaseModel
    {
        /// <summary>
        /// The type of entity.
        /// </summary>
        public string EntityType { get; set; } 

        /// <summary>
        /// The Unique Id/key for the Contact.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Full name of the Contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Name { get; set; }

        /// <summary>
        /// Id in Saasu of the Organisation or Company that employs the Contact.
        /// </summary>
        public int CompanyId { get; set; }
        /// <summary>
        /// The name of the Organisation or Company that the Contact belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Company { get; set; }
        /// <summary>
        /// The trading name of the Organisation or Company that the Contact belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string TradingName { get; set; }

        /// <summary>
        /// Is used as an Account or Contact reference for this person if they are a supplier or customer. This is your reference or their reference depending on how you prefer to use this field.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string CustomerContactId { get; set; }

        /// <summary>
        /// Email address for the Contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ContactEmail { get; set; }

        /// <summary>
        /// Main phone number for the Contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string MainPhone { get; set; }

        /// <summary>
        /// Secondary phone number for the Contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string SecondaryPhone { get; set; }

        /// <summary>
        /// Mobile phone number for the Contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string MobilePhone { get; set; }

        /// <summary>
        /// Twitter Id for the Contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string TwitterId { get; set; }

        /// <summary>
        /// Represents the Account Manager, Salesperson or similar for this Contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Manager { get; set; }

        /// <summary>
        /// List of tags associated with this Contact.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Tags { get; set; }

        /// <summary>
        /// Indicates whether this Contact is an employee.
        /// </summary>
        public bool IsEmployee { get; set; }

        /// <summary>
        /// Indicates whether this Contact is active. Default: true.
        /// </summary>
        public bool IsActive { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
