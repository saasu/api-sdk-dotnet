namespace Saasu.API.Core.Models.ItemTransfers
{
    /// <summary>
    /// Individual line item on an item transfer transaction.
    /// </summary>
    public class TransferItem : BaseModel
    {
        /// <summary>
		/// The quantity. Maximum of 3 decimals. 
		/// </summary>
		[System.ComponentModel.DataAnnotations.Required]
        public decimal? Quantity { get; set; }

        /// <summary>
        /// The inventory item for this transfer line item.
        /// </summary>
		[System.ComponentModel.DataAnnotations.Required]
        public int InventoryItemId { get; set; }

        /// <summary>
        /// The unit cost(excluding tax) for this transfer line item.
        /// </summary>
		[System.ComponentModel.DataAnnotations.Required]
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// The total price(excluding tax) for this transfer line item.
        /// If not provided or Quantity x UnitCost does not match the provided value it will be re-calculated.
        /// </summary>
        public decimal LineTotal { get; set; }

        /// <summary>
        /// Key Identifier for the model.
        /// </summary>
        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
