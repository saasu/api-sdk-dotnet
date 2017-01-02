using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Saasu.API.Client.IntegrationTests.Helpers;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.ItemAdjustments;

namespace Saasu.API.Client.IntegrationTests
{
	[TestFixture]
	public class ItemAdjustmentTests
	{

		private readonly ItemHelper _itemHelper;
		private readonly ItemAdjustmentHelper _adjustmentHelper;

		public ItemAdjustmentTests()
		{
			_itemHelper = new ItemHelper();
			_adjustmentHelper = new ItemAdjustmentHelper();
		}

		[Test]
		public void ShouldInsertItemAdjustment()
		{
			//var itemCode = "test_w1";
			var itemProxy = new ItemProxy();
			var item = _itemHelper.GetTestInventoryItem();
			var result = itemProxy.InsertItem(item);
			var newItem = itemProxy.GetItem(result.DataObject.InsertedItemId);

			var adjustmentItem = _adjustmentHelper.GetAdjustmentItem(newItem.DataObject.Id.Value, 1,
				newItem.DataObject.AssetAccountId.Value, 1, 1);

			var detail = _adjustmentHelper.GetAdjustmentDetail(new List<AdjustmentItem>() {adjustmentItem});
			var adjustmentProxy = new ItemAdjustmentProxy();
			var response = adjustmentProxy.InsertItemAdjustment(detail);

			Assert.IsTrue(response.IsSuccessfull);
			Assert.IsNotNull(response.DataObject);
			Assert.IsTrue(response.DataObject.InsertedEntityId > 0);
			Assert.GreaterOrEqual(response.DataObject.UtcLastModified, DateTime.Today.AddMinutes(-10).ToUniversalTime());
		}

		[Test]
		public void ShouldFailOnInsertWithNoAdjustmentItems()
		{
			var detail = _adjustmentHelper.GetAdjustmentDetail(new List<AdjustmentItem>() );
			var adjustmentProxy = new ItemAdjustmentProxy();
			var response = adjustmentProxy.InsertItemAdjustment(detail);

			Assert.IsFalse(response.IsSuccessfull);
			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.IsNull(response.DataObject);
			Assert.IsTrue(response.RawResponse.Contains("The Inventory Adjustment must contain at least one line item."));
		}

		[Test]
		public void ShouldFailOnInsertInvalidDate()
		{
			var itemProxy = new ItemProxy();
			var item = _itemHelper.GetTestInventoryItem();
			var result = itemProxy.InsertItem(item);
			var newItem = itemProxy.GetItem(result.DataObject.InsertedItemId);

			var adjustmentItem = _adjustmentHelper.GetAdjustmentItem(newItem.DataObject.Id.Value, 1,
				newItem.DataObject.AssetAccountId.Value, 1, 1);

			var detail = _adjustmentHelper.GetAdjustmentDetail(new List<AdjustmentItem>() { adjustmentItem });
			detail.Date = DateTime.MinValue;
			
			var adjustmentProxy = new ItemAdjustmentProxy();
			var response = adjustmentProxy.InsertItemAdjustment(detail);

			Assert.IsFalse(response.IsSuccessfull);
			Assert.IsNull(response.DataObject);
			Assert.IsTrue(response.RawResponse.Contains("Transaction date is not specified or invalid."));
		}

		[Test]
		public void ShouldGetExistingItemAdjustment()
		{
			var itemProxy = new ItemProxy();
			var item = _itemHelper.GetTestInventoryItem();
			var result = itemProxy.InsertItem(item);
			var newItem = itemProxy.GetItem(result.DataObject.InsertedItemId);

			var adjustmentItem = _adjustmentHelper.GetAdjustmentItem(newItem.DataObject.Id.Value, 1,
				newItem.DataObject.AssetAccountId.Value, 1, 1);

			var detail = _adjustmentHelper.GetAdjustmentDetail(new List<AdjustmentItem>() { adjustmentItem });
			var adjustmentProxy = new ItemAdjustmentProxy();
			var insertResponse = adjustmentProxy.InsertItemAdjustment(detail);

			var response = adjustmentProxy.GetItemAdjustment(insertResponse.DataObject.InsertedEntityId);
			Assert.IsTrue(response.IsSuccessfull);
			Assert.IsNotNull(response.DataObject);
			Assert.IsTrue(response.DataObject.Id == insertResponse.DataObject.InsertedEntityId);
		}

		[Test]
		public void ShouldFailGetItemAdjustmentWithBadId()
		{
			var adjustmentProxy = new ItemAdjustmentProxy();
			var response = adjustmentProxy.GetItemAdjustment(-1);

			Assert.IsFalse(response.IsSuccessfull);
			Assert.IsNull(response.DataObject);
		}

		[Test]
		public void ShouldFailGetItemAdjustment()
		{
			var adjustmentProxy = new ItemAdjustmentProxy();
			var response = adjustmentProxy.GetItemAdjustment(99999999);

			Assert.IsFalse(response.IsSuccessfull);
			Assert.IsNull(response.DataObject);
			Assert.IsTrue(response.RawResponse.Contains("The requested transaction is not found."));
		}

		[Test]
		public void ShouldUpdateExistingItemAdjustment()
		{
			var itemProxy = new ItemProxy();
			var item = _itemHelper.GetTestInventoryItem();
			var result = itemProxy.InsertItem(item);
			var newItem = itemProxy.GetItem(result.DataObject.InsertedItemId);

			var adjustmentItem = _adjustmentHelper.GetAdjustmentItem(newItem.DataObject.Id.Value, 1,
				newItem.DataObject.AssetAccountId.Value, 1, 1);

			var detail = _adjustmentHelper.GetAdjustmentDetail(new List<AdjustmentItem>() { adjustmentItem });
			var adjustmentProxy = new ItemAdjustmentProxy();
			var insertResponse = adjustmentProxy.InsertItemAdjustment(detail);


			detail.LastUpdatedId = insertResponse.DataObject.LastUpdatedId;
			detail.Summary = "Updated the summary.";
			detail.AdjustmentItems[0].Quantity = 5;
			detail.Notes = "Updated the notes.";
			detail.Tags = new List<string>() {"Updated"};
			detail.RequiresFollowUp = true;

			var updateDate = DateTime.Now.Date;
			detail.Date = updateDate;
			
			var response = adjustmentProxy.UpdateItemAdjustment(detail, insertResponse.DataObject.InsertedEntityId);
		
			Assert.IsTrue(response.IsSuccessfull);

			var updatedAdjustment = adjustmentProxy.GetItemAdjustment(insertResponse.DataObject.InsertedEntityId).DataObject;

			Assert.IsTrue(updatedAdjustment.AdjustmentItems[0].Quantity == 5);
			Assert.IsTrue(updatedAdjustment.Summary.Equals("Updated the summary."));
			Assert.IsTrue(updatedAdjustment.Notes.Equals("Updated the notes."));
			//Assert.IsTrue(updatedAdjustment.Tags[0] == "Updated");
			Assert.IsTrue(updateDate == updatedAdjustment.Date);
			Assert.IsTrue(updatedAdjustment.RequiresFollowUp.Value);


		}

		[Test]
		public void ShouldDeleteItemAdjustment()
		{
			var itemProxy = new ItemProxy();
			var item = _itemHelper.GetTestInventoryItem();
			var result = itemProxy.InsertItem(item);
			var newItem = itemProxy.GetItem(result.DataObject.InsertedItemId);

			var adjustmentItem = _adjustmentHelper.GetAdjustmentItem(newItem.DataObject.Id.Value, 1,
				newItem.DataObject.AssetAccountId.Value, 1, 1);

			var detail = _adjustmentHelper.GetAdjustmentDetail(new List<AdjustmentItem>() { adjustmentItem });
			var adjustmentProxy = new ItemAdjustmentProxy();
			var insertResponse = adjustmentProxy.InsertItemAdjustment(detail);

			var response = adjustmentProxy.DeleteItemAdjustment(insertResponse.DataObject.InsertedEntityId);

			Assert.IsTrue(response.IsSuccessfull);
		}

		[Test]
		public void ShouldGetItemAdjustments()
		{
			var itemProxy = new ItemProxy();
			var item = _itemHelper.GetTestInventoryItem();
			var result = itemProxy.InsertItem(item);
			var newItem = itemProxy.GetItem(result.DataObject.InsertedItemId);

			var adjustmentItem = _adjustmentHelper.GetAdjustmentItem(newItem.DataObject.Id.Value, 1,
				newItem.DataObject.AssetAccountId.Value, 1, 1);

			var detail = _adjustmentHelper.GetAdjustmentDetail(new List<AdjustmentItem>() { adjustmentItem });
			detail.Date = detail.Date.AddDays(-1);
			var adjustmentProxy = new ItemAdjustmentProxy();
			var insertResponse = adjustmentProxy.InsertItemAdjustment(detail);



			var response = new ItemAdjustmentsProxy().GetItemAdjustments();
			Assert.IsNotNull(response);
			Assert.IsTrue(response.IsSuccessfull);
			Assert.IsNotNull(response.DataObject);
			Assert.IsNotNull(response.DataObject.ItemAdjustments);
			Assert.IsTrue(response.DataObject.ItemAdjustments.Count > 0);
			//Assert.IsTrue(response.DataObject.ItemAdjustments.Exists(x => x.Id == insertResponse.DataObject.InsertedEntityId));

		}

		[Test]
		public void ShouldGetItemAdjustmentByDate()
		{
			var testDate = new DateTime(2016, 5, 8);

			var itemProxy = new ItemProxy();
			var item = _itemHelper.GetTestInventoryItem();
			var result = itemProxy.InsertItem(item);
			var newItem = itemProxy.GetItem(result.DataObject.InsertedItemId);

			var adjustmentItem = _adjustmentHelper.GetAdjustmentItem(newItem.DataObject.Id.Value, 1,
				newItem.DataObject.AssetAccountId.Value, 1, 1);

			var detail = _adjustmentHelper.GetAdjustmentDetail(new List<AdjustmentItem>() { adjustmentItem });
			detail.Date = testDate;
			var adjustmentProxy = new ItemAdjustmentProxy();
			var insertResponse = adjustmentProxy.InsertItemAdjustment(detail);

			var response = new ItemAdjustmentsProxy().GetItemAdjustments(null, null, testDate.AddDays(-1), testDate.AddDays(1));
			Assert.IsNotNull(response);
			Assert.IsTrue(response.IsSuccessfull);
			Assert.IsNotNull(response.DataObject);
			Assert.IsNotNull(response.DataObject.ItemAdjustments);
			Assert.IsTrue(response.DataObject.ItemAdjustments.Count > 0);

		}

	}
}
