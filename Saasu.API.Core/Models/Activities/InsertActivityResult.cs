using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Activities
{
    /// <summary>
    /// 
    /// </summary>
    public class InsertActivityResult : BaseInsertResultModel
    {
        /// <summary>
        /// The Id/key of the newly created/inserted activity.
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
