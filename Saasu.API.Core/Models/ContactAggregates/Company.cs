namespace Saasu.API.Core.Models.ContactAggregates
{
    public class Company : BaseModel
    {
        /// <summary>
        /// The unique key/Id of the company.
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// Name of the company.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Name { get; set; }
        /// <summary>
        /// Company ABN.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Abn { get; set; }
        /// <summary>
        /// Identifier used for concurrency checking. Required for update.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }
        /// <summary>
        /// Description of the company.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LongDescription { get; set; }
        /// <summary>
        /// Company trading name.
        /// </summary>
        /// <remarks></remarks>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string TradingName { get; set; }
        /// <summary>
        /// Company email address.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string CompanyEmail { get; set; }

        public override string ModelKeyValue()
        {
            throw new System.NotImplementedException();
        }
    }
}
