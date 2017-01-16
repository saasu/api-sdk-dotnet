namespace Saasu.API.Core.Models.Journals
{
    /// <summary>
    /// Journal item details.
    /// </summary>
    public class JournalItem : BaseModel
    {
        /// <summary>
        /// Key Identifier for the model
        /// </summary>
        public override string ModelKeyValue()
        {
            return string.Empty;
        }

        /// <summary>
        /// Credit or Debit
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Account associated with journal item
        /// </summary>
        public int AccountId { get; set; }
        /// <summary>
        /// Tax code if exists for the journal item
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// Amount of journal item
        /// </summary>
        public decimal Amount { get; set; }
    }
}
