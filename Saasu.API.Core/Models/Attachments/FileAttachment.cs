using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saasu.API.Core;

namespace Saasu.API.Core.Models.Attachments
{
    /// <summary>
    /// This is what is returned when retrieving an attachment.
    /// </summary>
    public class FileAttachment : FileAttachmentBaseInfo
	{
        /// <summary>
        ///  This is an array of bytes and represents the data of the attachment (ie. the attachment itself). 
        ///  You must convert the file you want to attach into a byte array. This is usually done programmatically which our client code does for you. 
        ///  This process is called serialisation and more information on this can be found here http://en.wikipedia.org/wiki/Serialization"
        /// </summary>
		public byte[] AttachmentData { get; set; }
        /// <summary>
        /// A flag that indicates if an attachment of the same name already exists, whether it can be overwritten or not when storing.
        /// </summary>
		public bool AllowExistingAttachmentToBeOverwritten { get; set; }

        public override string ModelKeyValue()
        {
            return base.ItemIdAttachedTo.ToString();
        }
    }
}
