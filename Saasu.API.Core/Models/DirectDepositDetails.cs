using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models
{
    /// <summary>
    /// Direct deposit details.
    /// </summary>
    public class DirectDepositDetails
    {
        /// <summary>
        /// Accept "Direct Deposit" as a payment method.
        /// </summary>
        public Nullable<bool> AcceptDirectDeposit { get; set; }
        /// <summary>
        /// The account name to use for direct deposits if the contact accepts direct deposits.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string AccountName { get; set; }
        /// <summary>
        /// The BSB to use for direct deposits if the contact accepts direct deposits.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string AccountBSB { get; set; }
        /// <summary>
        /// The account number to use for direct deposits if the contact accepts direct deposits.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string AccountNumber { get; set; }
    }
}
