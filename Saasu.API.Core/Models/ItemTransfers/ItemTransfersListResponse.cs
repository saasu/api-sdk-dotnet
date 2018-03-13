using System.Collections.Generic;

namespace Saasu.API.Core.Models.ItemTransfers
{
    /// <summary>
    /// Response from GET for filtered list of item transfers.
    /// </summary>
    public class ItemTransfersListResponse : BaseModel, IApiResponseCollection
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ItemTransfersListResponse()
        {
            Transfers = new List<TransferSummary>();
        }
        /// <summary>
        /// List of transfers.
        /// </summary>
        public List<TransferSummary> Transfers { get; set; }

        /// <summary>
        /// ey Identifier for the model
        /// </summary>
        public override string ModelKeyValue()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns model's list of transfers. Needed for hypermedia.
        /// </summary>
        public IEnumerable<BaseModel> ListCollection()
        {
            return Transfers;
        }
    }
}
