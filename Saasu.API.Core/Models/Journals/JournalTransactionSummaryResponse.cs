using System.Collections.Generic;

namespace Saasu.API.Core.Models.Journals
{
    /// <summary>
    /// Response from a call to get mode than one journal.
    /// </summary>
    public class JournalTransactionSummaryResponse : BaseModel, IApiResponseCollection
    {
        /// <summary>
        /// Response object for mulitple journal transaction search
        /// </summary>
        public JournalTransactionSummaryResponse()
        {
            Journals = new List<JournalSummary>();
        }
        /// <summary>
        /// List of journals.
        /// </summary>
		public List<JournalSummary> Journals { get; set; }

        /// <summary>
        /// ey Identifier for the model
        /// </summary>
        public override string ModelKeyValue()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns model's list of journals. Needed fro hypermedia.
        /// </summary>
        public IEnumerable<BaseModel> ListCollection()
        {
            return Journals;
        }
    }
}
