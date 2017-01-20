using Saasu.API.Core.Models.Attachments;
using System.Collections.Generic;

namespace Saasu.API.Core.Models.Activities
{
    /// <summary>
    /// Response moedel for gettting individual activty record.
    /// </summary> 
    public class ActivityDetail : ActivitySummary
    {
        /// <summary>
        /// Details of activity. 
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// List of attachments associated with this invoice. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public List<FileAttachmentInfo> Attachments { get; set; }
    }
}
