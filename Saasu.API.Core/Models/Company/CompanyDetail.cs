using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Company
{
    /// <summary>
    /// Represents the details of a company.
    /// </summary>
    public class CompanyDetail : BaseModel
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
        /// Url of the company website.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Website { get; set; }

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
        /// Url of the company logo. Note: This field is not supported and will be removed in a future version.
        /// </summary>
        [Obsolete("This field is not supported and will be removed in a future version")]
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LogoUrl { get; set; }
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
        /// <summary>
        /// Date and time that the company data was last modified in UTC.
        /// </summary>
        public System.DateTime LastModifiedDateUtc { get; set; }
        /// <summary>
        /// Date and time that the company was created in UTC.
        /// </summary>
        public System.DateTime CreatedDateUtc { get; set; }
        /// <summary>
        /// The user id that last modified the company data.
        /// </summary>
        public Nullable<int> LastModifiedByUserId { get; set; }

        public override string ModelKeyValue()
        {
            return Id.GetValueOrDefault().ToString();
        }
    }

    /// <summary>
    /// A list of company records.
    /// </summary>
    public class CompanyListResponse : BaseModel, IApiResponseCollection
    {
        public CompanyListResponse()
        {
            Companies = new List<CompanyDetail>();
        }
        /// <summary>
        /// A list of company records.
        /// </summary>
        public List<CompanyDetail> Companies { get; set; }

        public IEnumerable<BaseModel> ListCollection()
        {
            return Companies.AsEnumerable<BaseModel>();
        }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }

}
