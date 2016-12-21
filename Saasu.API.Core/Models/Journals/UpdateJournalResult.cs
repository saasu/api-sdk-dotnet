namespace Saasu.API.Core.Models.Journals
{
    /// <summary>
    /// Result for updating existing journal transaction
    /// </summary>
    public class UpdateJournalResult : BaseUpdateResultModel
    {
        /// <summary>
        /// Key Identifier for the model.
        /// </summary>
        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
