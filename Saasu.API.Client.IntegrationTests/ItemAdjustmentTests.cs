using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Saasu.API.Client.IntegrationTests.Helpers;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.ItemAdjustments;

namespace Saasu.API.Client.IntegrationTests
{
	public class ItemAdjustmentTests
	{

		private readonly ItemHelper _itemHelper;
		private readonly ItemAdjustmentHelper _adjustmentHelper;

		public ItemAdjustmentTests()
		{
			_itemHelper = new ItemHelper();
			_adjustmentHelper = new ItemAdjustmentHelper();
		}

		[Fact]
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

			Assert.True(response.IsSuccessfull);
			Assert.NotNull(response.DataObject);
			Assert.True(response.DataObject.InsertedEntityId > 0);
			Assert.True(response.DataObject.UtcLastModified >= DateTime.Today.AddMinutes(-10).ToUniversalTime());
		}

		[Fact]
		public void ShouldFailOnInsertWithNoAdjustmentItems()
		{
			var detail = _adjustmentHelper.GetAdjustmentDetail(new List<AdjustmentItem>() );
			var adjustmentProxy = new ItemAdjustmentProxy();
			var response = adjustmentProxy.InsertItemAdjustment(detail);

			Assert.False(response.IsSuccessfull);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Null(response.DataObject);
			Assert.Contains("Please specify AdjustmentItems for this transaction.", response.RawResponse);
		}

		[Fact]
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
			Assert.True(response.IsSuccessfull);
			Assert.NotNull(response.DataObject);
			Assert.True(response.DataObject.Id == insertResponse.DataObject.InsertedEntityId);
		}

		[Fact]
		public void ShouldFailGetItemAdjustmentWithBadId()
		{
			var adjustmentProxy = new ItemAdjustmentProxy();
			var response = adjustmentProxy.GetItemAdjustment(-1);

			Assert.False(response.IsSuccessfull);
			Assert.Null(response.DataObject);
		}

		[Fact]
		public void ShouldFailGetItemAdjustment()
		{
			var adjustmentProxy = new ItemAdjustmentProxy();
			var response = adjustmentProxy.GetItemAdjustment(99999999);

			Assert.False(response.IsSuccessfull);
			Assert.Null(response.DataObject);
			Assert.True(response.RawResponse.Contains("The requested transaction is not found."));
		}

		[Fact]
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
		
			Assert.True(response.IsSuccessfull);

			var updatedAdjustment = adjustmentProxy.GetItemAdjustment(insertResponse.DataObject.InsertedEntityId).DataObject;

			Assert.True(updatedAdjustment.AdjustmentItems[0].Quantity == 5);
			Assert.True(updatedAdjustment.Summary.Equals("Updated the summary."));
			Assert.True(updatedAdjustment.Notes.Equals("Updated the notes."));
			//Assert.IsTrue(updatedAdjustment.Tags[0] == "Updated");
			Assert.True(updateDate == updatedAdjustment.Date);
			Assert.True(updatedAdjustment.RequiresFollowUp.Value);


		}

		[Fact]
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

			Assert.True(response.IsSuccessfull);
		}

		[Fact]
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
			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull);
			Assert.NotNull(response.DataObject);
			Assert.NotNull(response.DataObject.ItemAdjustments);
			Assert.True(response.DataObject.ItemAdjustments.Count > 0);
			//Assert.IsTrue(response.DataObject.ItemAdjustments.Exists(x => x.Id == insertResponse.DataObject.InsertedEntityId));

		}

		[Fact]
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
			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull);
			Assert.NotNull(response.DataObject);
			Assert.NotNull(response.DataObject.ItemAdjustments);
			Assert.True(response.DataObject.ItemAdjustments.Count > 0);

		}

	}
}
