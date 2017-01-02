using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.ItemAdjustments
{
	public class AdjustmentSummary : BaseModel
	{

		/// <summary>
		/// The unique Id for the item adjustment.
		/// </summary>
		public int? Id { get; set; }

		/// <summary>
		/// The date of the adjustments
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Brief summary for this adjustment.
		/// </summary>
		public string Summary { get; set; }

		/// <summary>
		/// Indicates whether the adjustment requires follow up.
		/// </summary>
		public bool? RequiresFollowUp { get; set; }

		/// <summary>
		/// A data field used to check concurreny when performing updates. This data is returned only and used for concurrency checking when performing an update/PUT.
		/// </summary>
		[System.Xml.Serialization.XmlElement(IsNullable = true)]
		public string LastUpdatedId { get; set; }


		/// <summary>
		/// The date and time this resource was created in UTC. This data is returned only and cannot be added or updated when issuing a POST or PUT.
		/// </summary>
		public DateTime? CreatedDateUtc { get; set; }
		/// <summary>
		/// The date and time this resource was last modified in UTC. This data is returned only and cannot be added or updated when issuing a POST or PUT.
		/// </summary>
		public DateTime? LastModifiedDateUtc { get; set; }


		public override string ModelKeyValue()
		{
			return string.Empty;
		}
	}
}
