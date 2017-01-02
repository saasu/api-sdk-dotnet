using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.ItemAdjustments
{
    /// <summary>
    /// A list of item adjustments
    /// </summary>
    public class ItemAdjustmentListResponse : BaseModel, IApiResponseCollection
    {
		/// <summary>
		/// List of item adjustments.
		/// </summary>
		public List<AdjustmentSummary> ItemAdjustments { get; set; }

        public ItemAdjustmentListResponse()
        {
            ItemAdjustments = new List<AdjustmentSummary>();
        }

        public IEnumerable<BaseModel> ListCollection()
        {
            return ItemAdjustments;
        }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }

}
