using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Attachments
{
	public class InsertAttachmentResult : BaseModel
	{
        /// <summary>
        /// The Id/Key of the created/inserted attachment.
        /// </summary>
		public int Id { get; set; }

        public override string ModelKeyValue()
        {
            return Id.ToString();
        }
    }
}
