using Saasu.API.Core.Hypermedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Saasu.API.Core
{
    public class BaseUpdateResultModel : BaseModel
    {
        /// <summary>
        /// The unique identifier generated as part of the update. This value is required when submitting subsequent update
        /// to prevent concurrency errors.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }
        /// <summary>
        /// The date and time this resource was modified in UTC.
        /// </summary>
        public DateTime UtcLastModified { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
