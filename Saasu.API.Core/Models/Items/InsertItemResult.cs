using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Items
{
    public class InsertItemResult : BaseInsertResultModel
    {
        /// <summary>
        /// The id of the newly created/inserted inventory item.
        /// </summary>
        public int InsertedItemId { get; set; }
        
        public override string ModelKeyValue()
        {
            return InsertedItemId.ToString(CultureInfo.InvariantCulture);
        }
    }
}
