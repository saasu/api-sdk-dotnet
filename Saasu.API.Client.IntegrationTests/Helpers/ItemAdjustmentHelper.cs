using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Core.Models.ItemAdjustments;

namespace Saasu.API.Client.IntegrationTests.Helpers
{
	public class ItemAdjustmentHelper
	{
		public AdjustmentDetail GetAdjustmentDetail(List<AdjustmentItem> items)
		{
			return new AdjustmentDetail()
			{
				AdjustmentItems = items,
				Date = DateTime.Now.AddDays(-1),
				Notes = "API Integration Test Note.",
				Summary = "Adjustment summary.",
				Tags = new List<string>() { "Test", "API"}
			};
		}


		public AdjustmentItem GetAdjustmentItem(int itemId, decimal quantity, int accountId, decimal unitPrice, decimal totalPrice)
		{
			return new AdjustmentItem()
			{
				ItemId = itemId,
				Quantity = quantity,
				AccountId = accountId,
				UnitPrice = unitPrice,
				TotalPrice = totalPrice
			};
		}

	}
}
