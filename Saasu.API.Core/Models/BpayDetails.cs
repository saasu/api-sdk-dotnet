using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models
{
    /// <summary>
    /// Bpay details.
    /// </summary>
    public class BpayDetails
    {
        /// <summary>
        /// Bpay biller code of the contact if using this method.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string BillerCode { get; set; }
        /// <summary>
        /// Bpay CRN if the contact uses Bpay.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string CRN { get; set; }
    }
}
