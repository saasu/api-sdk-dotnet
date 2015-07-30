using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models
{
    /// <summary>
    /// Email related content such as body, subject and addressing information
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Who the email is from.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string From { get; set; }
        /// <summary>
        /// Who the email is being sent to.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string To { get; set; }
        /// <summary>
        /// Subject of the email.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Subject { get; set; }
        /// <summary>
        /// The body of the email message.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Body { get; set; }
        /// <summary>
        /// The CC address to send a copy of the email to.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Cc { get; set; }
        /// <summary>
        /// The BCC address (blind carbon copy) o send a copy of the email to.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Bcc { get; set; }
    }
}
