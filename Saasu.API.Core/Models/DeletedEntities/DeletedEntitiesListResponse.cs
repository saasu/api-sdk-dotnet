using System.Collections.Generic;

namespace Saasu.API.Core.Models.DeletedEntities
{
    /// <summary>
    /// Response model for Deleted Entities List. 
    /// </summary>
    public class DeletedEntitiesListResponse : BaseModel
    {
        /// <summary>
        /// Response object for deleted entities search
        /// </summary>
        public DeletedEntitiesListResponse()
        {
            Entities = new List<DeletedEntityDetail>();
        }
        /// <summary>
        /// List of deleted entities.
        /// </summary>
		public List<DeletedEntityDetail> Entities { get; set; }

        /// <summary>
        /// Key Identifier for the model
        /// </summary>
        public override string ModelKeyValue()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns model's list of deleted entities. Needed for hypermedia.
        /// </summary>
        public IEnumerable<BaseModel> ListCollection()
        {
            return Entities;
        }
    }
}
