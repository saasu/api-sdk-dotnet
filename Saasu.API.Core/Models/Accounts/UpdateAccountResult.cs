using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Accounts
{
	public class UpdateAccountResult : BaseModel
	{
        /// <summary>
        /// The unique Id for this version of the record which is required to be passed in when next updating this resource.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }
        /// <summary>
        /// The date and time this resource was last modified in UTC.
        /// </summary>
		public DateTime UtcLastModified { get; set; }

		public override string ModelKeyValue()
		{
			return string.Empty;
		}
	}
}
