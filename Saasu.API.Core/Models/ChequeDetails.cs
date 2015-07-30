using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models
{
    /// <summary>
    /// Cheque details.
    /// </summary>
    public class ChequeDetails
    {
        /// <summary>
        /// Accept cheque as a payment method.
        /// </summary>
        public Nullable<bool> AcceptCheque { get; set; }
        /// <summary>
        /// The named payee that will receive the money.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string ChequePayableTo { get; set; }
    }
}
