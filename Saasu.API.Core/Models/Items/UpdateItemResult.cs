using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Items
{
    public class UpdateItemResult : BaseUpdateResultModel
    {
        /// <summary>
        /// The id of the updated company.
        /// </summary>
        public int UpdatedItemId { get; set; }
        /// <summary>
        /// The unique id associated with this update. This value is required to be passed in on subsequent updates to prevent concurrency errors.
        /// </summary>

        public override string ModelKeyValue()
        {
            return UpdatedItemId.ToString();
        }
    }
}
