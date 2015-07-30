using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saasu.API.Core;

namespace Saasu.API.Core.Models.Attachments
{
    /// <summary>
    /// Summary information about an attachment. This is returned when requesting a list of attachments that are
    /// linked to a specific resource.
    /// </summary>
	public class FileAttachmentInfo : FileAttachmentBaseInfo
	{
        /// <summary>
        /// The Id of the attachment.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The size in bytes of the attachment.
        /// </summary>
		public long Size { get; set; }

        public override string ModelKeyValue()
        {
            return Id.ToString();
        }
    }

    public abstract class FileAttachmentBaseInfo : BaseModel
    {
        /// <summary>
        /// Name of the attachment.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Name { get; set; }
        /// <summary>
        /// Description of the attachment.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Description { get; set; }
        /// <summary>
        /// The Id of the item/entity that this attachment is associated with or attached to.
        /// </summary>
        public int ItemIdAttachedTo { get; set; }
    }

    public class FileAttachmentListResponse : BaseModel, IApiResponseCollection
    {
        public FileAttachmentListResponse()
        {
            Attachments = new List<FileAttachmentInfo>();
        }
        /// <summary>
        /// List of attachments.
        /// </summary>
        public List<FileAttachmentInfo> Attachments { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }

        public IEnumerable<BaseModel> ListCollection()
        {
            return Attachments;
        }
    }
}
