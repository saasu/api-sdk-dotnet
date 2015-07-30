using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models
{
    /// <summary>
    /// Used for setting the due date/expiry date when creating sales invoices, orders and quotes for contacts.
    /// </summary>
    public class TradingTerms
    {
        /// <summary>
        /// The trading terms type. 1 = Due In, 2 = EOM+(End Of Month + number of Days), 3 = COD(Cash On Delivery).
        /// </summary>
        public byte? TradingTermsType { get; set; }
        /// <summary>
        /// Use with Due In and EOM+ types. Reflects the number of days/weeks/months.
        /// </summary>
        public int? TradingTermsInterval { get; set; }
        /// <summary>
        /// The interval type. 1 = Days, 2 = Weeks, 3 = Months.
        /// </summary>
        public byte? TradingTermsIntervalType { get; set; }
    }
}
