namespace Saasu.API.Core.Models.ItemTransfers
{
    /// <summary>
    /// Response from update of existing item transfer.
    /// </summary>
    public class UpdateItemTransferResult : BaseUpdateResultModel
    {
        /// <summary>
        /// Updated item transfer id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Key Identifier for the model.
        /// </summary>
        /// <returns></returns>
        public override string ModelKeyValue()
        {
            return Id.ToString();
        }
    }
}
