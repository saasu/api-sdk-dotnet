namespace Saasu.API.Core.Models.DeletedEntities
{
    /// <summary>
    /// Details of a deleted entity.
    /// </summary>
    public class DeletedEntityDetail : BaseModel
    {
        /// <summary>
        /// The Id/key of the deleted entity.
        /// </summary>
        public int EntityTypeId { get; set; }

        /// <summary>
        /// The Id/key of the deleted entity.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Login of user who deleted the entity.
        /// </summary>
        public string DeletedByUser { get; set; }

        /// <summary>
        /// Datetime the entity was deleted.
        /// </summary>
        public System.DateTime Timestamp { get; set; }

        /// <summary>
        /// Key Identifier for the model
        /// </summary>
        public override string ModelKeyValue()
        {
            return EntityId.ToString();
        }
    }
}
