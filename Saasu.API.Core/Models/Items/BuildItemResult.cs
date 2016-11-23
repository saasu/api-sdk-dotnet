using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Items
{
    public class BuildItemResult : BaseResponseModel
    {
        /// <summary>
        /// The id of the built item.
        /// </summary>
        public int ItemId { get; set; }

        public override string ModelKeyValue()
        {
            return ItemId.ToString();
        }

    }
}
