using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Company
{
    public class InsertCompanyResult : BaseModel
    {
        /// <summary>
        /// The id of the newly created/inserted company.
        /// </summary>
        public int InsertedCompanyId { get; set; }

        /// <summary>
        /// The unique id associated with this insert. This value is required to be passed in on subsequent updates to prevent data loss/corruption.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }

        /// <summary>
        /// The date and time this resource was modified in UTC.
        /// </summary>
        public DateTime LastModified { get; set; }

        public override string ModelKeyValue()
        {
            return InsertedCompanyId.ToString(CultureInfo.InvariantCulture);
        }
    }
}
