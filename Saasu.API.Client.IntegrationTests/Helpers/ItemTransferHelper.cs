using Saasu.API.Core.Models.ItemTransfers;
using System;
using System.Collections.Generic;

namespace Saasu.API.Client.IntegrationTests.Helpers
{
    public class ItemTransferHelper
    {
        public TransferDetail GetTransferDetail(List<TransferItem> items, DateTime? date = null)
        {
            return new TransferDetail()
            {
                Items = items,
                Date = date ?? DateTime.Now.AddDays(-1),
                Notes = "API Integration Test Note.",
                Summary = "Transfer summary.",
                Tags = new List<string>() { "Test", "API" }
            };
        }


        public TransferItem GetTransferItem(int itemId, decimal? quantity, int accountId, decimal? unitPrice, decimal? totalPrice)
        {
            return new TransferItem()
            {
                Quantity = quantity ?? 2,
                UnitCost = unitPrice ?? 3.50M,
                InventoryItemId = itemId,
                LineTotal = totalPrice ?? 7
            };
        }
    }
}
