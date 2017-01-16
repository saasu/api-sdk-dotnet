using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saasu.API.Core.Models.Journals
{
    /// <summary>
    /// Details of a journal transaction.
    /// </summary>
    public class JournalDetail : JournalSummary
    {
        /// <summary>
        /// The items associated with this journal transaction.
        /// </summary>
        [Required]
        public List<JournalItem> Items { get; set; }

        /// <summary>
        /// Textual notes set by the user.
        /// </summary>
        public string Notes { get; set; }
    }
}
