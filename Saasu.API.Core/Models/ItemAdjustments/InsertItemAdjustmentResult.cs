using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.ItemAdjustments
{

    public class InsertItemAdjustmentResult : BaseInsertResultModel
    {
        /// <summary>
        /// The id of the newly created/inserted item adjustment.
        /// </summary>
        public int InsertedEntityId { get; set; }

        public override string ModelKeyValue()
        {
            return InsertedEntityId.ToString();
        }
    }
}
