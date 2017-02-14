namespace Saasu.API.Core.Models.ItemTransfers
{
    /// <summary>
    /// Response from inserting item transfer.
    /// </summary>
    public class InsertItemTransferResult : BaseInsertResultModel
    {
        /// <summary>
        /// The Id/key of the newly created/inserted transfer.
        /// </summary>
        public int InsertedEntityId { get; set; }

        /// <summary>
        /// Key Identifier for the model.
        /// </summary>
        public override string ModelKeyValue()
        {
            return InsertedEntityId.ToString();
        }
    }
}
