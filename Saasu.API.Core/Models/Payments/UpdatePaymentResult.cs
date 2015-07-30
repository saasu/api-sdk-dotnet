using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Payments
{
    public class UpdatePaymentResult : BaseModel
    {
        /// <summary>
        /// The new unique identifier generated as part of the update. This value is required when submitting subsequent update
        /// to ensure no data loss/overwrites occur.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }
        /// <summary>
        /// The date and time that this payment was last modified in UTC.
        /// </summary>
        public DateTime UtcLastModified { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
