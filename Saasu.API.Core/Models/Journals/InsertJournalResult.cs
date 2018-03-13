namespace Saasu.API.Core.Models.Journals
{
    /// <summary>
    /// Returned value from inserting a journal transaction.
    /// </summary>
    public class InsertJournalResult : UpdateJournalResult
    {
        /// <summary>
        /// The Id/key of the newly created/inserted journal.
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
