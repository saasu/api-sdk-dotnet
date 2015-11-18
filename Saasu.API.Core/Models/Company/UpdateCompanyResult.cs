using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Company
{
    public class UpdateCompanyResult : BaseModel
    {
        /// <summary>
        /// The id of the updated company.
        /// </summary>
        public int UpdatedCompanyId { get; set; }

        /// <summary>
        /// The unique id associated with this update. This value is required to be passed in on subsequent updates to prevent concurrency errors.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }

        /// <summary>
        /// The date and time this resource was modified in UTC.
        /// </summary>
        public DateTime LastModified { get; set; }

        public override string ModelKeyValue()
        {
            return UpdatedCompanyId.ToString(CultureInfo.InvariantCulture);
        }
    }
}
