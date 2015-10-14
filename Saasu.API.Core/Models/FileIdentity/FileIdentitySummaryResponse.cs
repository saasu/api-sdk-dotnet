using System.Collections.Generic;

namespace Saasu.API.Core.Models.FileIdentity
{
    /// <summary>
    /// A list of file identity summaries.
    /// </summary>
	public class FileIdentitySummaryResponse : BaseModel, IApiResponseCollection
    {
        public FileIdentitySummaryResponse()
        {
            FileIdentities = new List<FileIdentitySummary>();
        }

        /// <summary>
        /// A list of file identity summaries.
        /// </summary>
		public List<FileIdentitySummary> FileIdentities { get; set; }

        public IEnumerable<BaseModel> ListCollection()
        {
            return FileIdentities;
        }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
