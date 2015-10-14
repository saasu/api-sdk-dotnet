namespace Saasu.API.Core.Models.FileIdentity
{
    public class FileIdentitySummary : BaseModel
    {
        /// <summary>
        /// The Id/key of this resource.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// File name used by the business in Saasu. 
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Name { get; set; }

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
        /// Name of the subscription.
        /// </summary>
        public string SubscriptionName { get; set; }

        public override string ModelKeyValue()
        {
            return Id.ToString();
        }
    }
}
