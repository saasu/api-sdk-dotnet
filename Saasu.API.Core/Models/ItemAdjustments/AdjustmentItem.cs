using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.ItemAdjustments
{
    public class AdjustmentItem
    {
        /// <summary>
        /// The quantity. Maximum of 3 decimals.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// The inventory item for this adjustment line item.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// The account for this adjustment line item.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The unit price(excluding tax) for this adjustment line item.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// The total price(excluding tax) for this adjustment line item.
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}
