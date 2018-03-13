using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.ItemAdjustments
{
    public class AdjustmentDetail : AdjustmentSummary
    {

		/// <summary>
		/// List of tags associated with this resource.
		/// </summary>
		public List<string> Tags { get; set; }
		/// <summary>
		/// Notes for this adjustment.
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// A list of item adjustments.
		/// </summary>
		public List<AdjustmentItem> AdjustmentItems { get; set; }



    }
}
