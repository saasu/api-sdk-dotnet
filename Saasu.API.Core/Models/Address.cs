using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models
{
    public class Address
    {
        /// <summary>
        /// Street number and name.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Street { get; set; }
        /// <summary>
        /// City.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string City { get; set; }
        /// <summary>
        /// State.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string State { get; set; }
        /// <summary>
        /// Postcode.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Postcode { get; set; }
        /// <summary>
        /// The country.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Country { get; set; }
    }
}
