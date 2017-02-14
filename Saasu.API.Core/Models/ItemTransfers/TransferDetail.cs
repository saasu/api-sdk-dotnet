using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saasu.API.Core.Models.ItemTransfers
{
    /// <summary>
    /// Item transfer transaction record.
    /// </summary>
    public class TransferDetail : TransferSummary
    {
        /// <summary>
        /// The items associated with this transfer transaction.
        /// </summary>
        [Required]
        public List<TransferItem> Items { get; set; }

        /// <summary>
        /// Textual notes set by the user.
        /// </summary>
        public string Notes { get; set; }
    }
}
