namespace Saasu.API.Core.Models.FileIdentity
{
    public class FileIdentityDetail : BaseModel
    {
        /// <summary>
        /// File name used by the business in Saasu. 
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Name { get; set; }
        /// <summary>
        /// Legal registered business name.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string FullLegalName { get; set; }
        /// <summary>
        /// 'Trading as' name or alternative brand name.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string TradingNameOrAlternativeBrandName { get; set; }
        /// <summary>
        /// Business identifier issued for the business(eg. ABN in Australia). 
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string BusinessIdentifier { get; set; }
        /// <summary>
        /// Company identifier issued for the business(eg. ACN in Australia).
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string CompanyIdentifier { get; set; }
        /// <summary>
        /// Primary telephone number used by the business.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PrimaryPhone { get; set; }
        /// <summary>
        /// Website used for the business.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Website { get; set; }
        /// <summary>
        /// Primary email of the business.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Email { get; set; }
        /// <summary>
        /// Street number and name of the business.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Street { get; set; }
        /// <summary>
        /// City the business is located in.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string City { get; set; }
        /// <summary>
        /// State the business is located in.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string State { get; set; }
        /// <summary>
        /// Postcode where the business is located.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PostCode { get; set; }
        /// <summary>
        /// The country where the business is located.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Country { get; set; }
        /// <summary>
        /// Tax zone the business is operating from.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Zone { get; set; }

        /// <summary>
        /// Currency code of the payment eg: AUD or USD.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Is the business registered for tax or not(eg: GST, VAT).
        /// </summary>
        public bool IsTaxRegistered { get; set; }        

        /// <summary>
        /// Settings that specify the default behaviour for a file.
        /// </summary>
        public FileSettings FileSettings { get; set; }
        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
