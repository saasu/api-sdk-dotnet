using NUnit.Framework;
using Saasu.API.Client.IntegrationTests.Helpers;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Items;
using Saasu.API.Core.Models.ItemTransfers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Saasu.API.Client.IntegrationTests
{
    [TestFixture]
    public class ItemTransferTests
    {
        private readonly ItemHelper _itemHelper;
        private readonly ItemAdjustmentHelper _adjustmentHelper;
        private readonly ItemTransferHelper _transferHelper;

        private ItemDetail _item;
        private int _assetAccountId;
        private int _incomeAccountId;
        private TransferDetail _testTransfer;
        private int _testTransferToDeleteId;

        public ItemTransferTests()
        {
            _itemHelper = new ItemHelper();
            _adjustmentHelper = new ItemAdjustmentHelper();
            _transferHelper = new ItemTransferHelper();

            GetTestAccounts();
            CreateTestItems();
            CreateTestTransfers();
        }

        [Test]
        public void ShouldInsertItemTransfer()
        {
            var transferItem = _transferHelper.GetTransferItem((int)_item.Id, 2, _assetAccountId, (decimal)_item.BuyingPrice, (decimal)(2 * _item.BuyingPrice));
            var transferItem2 = _transferHelper.GetTransferItem((int)_item.Id, -2, _incomeAccountId, (decimal)_item.BuyingPrice, (decimal)(-2 * _item.BuyingPrice));

            var detail = _transferHelper.GetTransferDetail(new List<TransferItem>() { transferItem, transferItem2 });

            var proxy = new ItemTransferProxy();
            var response = proxy.InsertItemTransfer(detail);

            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsTrue(response.DataObject.InsertedEntityId > 0);
            Assert.GreaterOrEqual(response.DataObject.UtcLastModified, DateTime.Today.AddMinutes(-10).ToUniversalTime());
        }

        [Test]
        public void ShouldFailOnInsertWithNoItems()
        {
            var proxy = new ItemTransferProxy();

            var detail = _transferHelper.GetTransferDetail(new List<TransferItem>());
            var response = proxy.InsertItemTransfer(detail);

            Assert.IsFalse(response.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsNull(response.DataObject);
            Assert.IsTrue(response.RawResponse.Contains("Please specify TransferItems for this transaction."));
        }

        [Test]
        public void ShouldGetExistingItemTransfer()
        {
            var proxy = new ItemTransferProxy();

            var response = proxy.GetItemTransfer((int)_testTransfer.Id);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsTrue(response.DataObject.Id == (int)_testTransfer.Id);
        }

        [Test]
        public void ShouldFailGetItemTransferWithBadId()
        {
            var proxy = new ItemTransferProxy();
            var response = proxy.GetItemTransfer((int)_testTransfer.Id - 1);

            Assert.IsFalse(response.IsSuccessfull);
            Assert.IsNull(response.DataObject);
        }

        [Test]
        public void ShouldFailGetItemTransferWrongId()
        {
            var proxy = new ItemTransferProxy();
            var response = proxy.GetItemTransfer(9999999);

            Assert.IsFalse(response.IsSuccessfull);
            Assert.IsNull(response.DataObject);
            Assert.IsTrue(response.RawResponse.Contains("The requested transaction is not found."));
        }

        [Test]
        public void ShouldUpdateExistingTransfer()
        {
            var proxy = new ItemTransferProxy();

            _testTransfer.Summary = "Updated the summary.";
            _testTransfer.Items[0].Quantity = 5;
            _testTransfer.Items[1].Quantity = -5;
            _testTransfer.Notes = "Updated the notes.";
            _testTransfer.Tags = new List<string>() { "Updated" };
            _testTransfer.RequiresFollowUp = true;
            _testTransfer.Date = _testTransfer.Date.AddDays(1);

            var response = proxy.UpdateItemTransfer(_testTransfer, (int)_testTransfer.Id);

            Assert.IsTrue(response.IsSuccessfull);

            var updatedTransfer = proxy.GetItemTransfer((int)_testTransfer.Id);

            Assert.IsTrue(updatedTransfer.DataObject.Items[0].Quantity == 5);
            Assert.IsTrue(updatedTransfer.DataObject.Items[1].Quantity == -5);
            Assert.IsTrue(updatedTransfer.DataObject.Summary.Equals("Updated the summary."));
            Assert.IsTrue(updatedTransfer.DataObject.Notes.Equals("Updated the notes."));
            Assert.IsTrue(updatedTransfer.DataObject.Date == _testTransfer.Date);
            Assert.IsTrue(updatedTransfer.DataObject.RequiresFollowUp.Value);
        }

        [Test]
        public void ShouldDeleteItemTransfer()
        {
            var proxy = new ItemTransferProxy();
            var response = proxy.DeleteItemTransfer(_testTransferToDeleteId);
            Assert.IsTrue(response.IsSuccessfull);
        }

        [Test]
        public void ShouldGetItemTransfers()
        {
            var proxy = new ItemTransfersProxy();
            var response = proxy.GetItemTransfers();

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsNotNull(response.DataObject.Transfers);
            Assert.IsTrue(response.DataObject.Transfers.Count > 0);
        }

        [Test]
        public void ShouldGetItemTransfersFilterOnDate()
        {
            var proxy = new ItemTransferProxy();
            var proxyGet = new ItemTransfersProxy();

            var testDate = new DateTime(2016, 12, 1);

            var transferItem = _transferHelper.GetTransferItem((int)_item.Id, 2, _assetAccountId, (decimal)_item.BuyingPrice, (decimal)(2 * _item.BuyingPrice));
            var transferItem2 = _transferHelper.GetTransferItem((int)_item.Id, -2, _incomeAccountId, (decimal)_item.BuyingPrice, (decimal)(-2 * _item.BuyingPrice));

            var detail = _transferHelper.GetTransferDetail(new List<TransferItem>() { transferItem, transferItem2 }, testDate);

            var insertTransfer = proxy.InsertItemTransfer(detail);

            var response = proxyGet.GetItemTransfers(fromDate: testDate, toDate: testDate);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsNotNull(response.DataObject.Transfers);
            Assert.IsTrue(response.DataObject.Transfers.Count > 0);
            Assert.IsNull(response.DataObject.Transfers.Where(t => t.Date < testDate).SingleOrDefault());
            Assert.IsNull(response.DataObject.Transfers.Where(t => t.Date > testDate).SingleOrDefault());
            Assert.NotNull(response.DataObject.Transfers.Where(t => t.Id == insertTransfer.DataObject.InsertedEntityId));
        }

        private void CreateTestTransfers()
        {
            var transferItem = _transferHelper.GetTransferItem((int)_item.Id, 2, _assetAccountId, (decimal)_item.BuyingPrice, (decimal)(2 * _item.BuyingPrice));
            var transferItem2 = _transferHelper.GetTransferItem((int)_item.Id, -2, _incomeAccountId, (decimal)_item.BuyingPrice, (decimal)(-2 * _item.BuyingPrice));

            var detail = _transferHelper.GetTransferDetail(new List<TransferItem>() { transferItem, transferItem2 });

            var proxy = new ItemTransferProxy();
            var response = proxy.InsertItemTransfer(detail);

            _testTransfer = proxy.GetItemTransfer(response.DataObject.InsertedEntityId).DataObject;

            var transferItem3 = _transferHelper.GetTransferItem((int)_item.Id, 2, _assetAccountId, (decimal)_item.BuyingPrice, (decimal)(2 * _item.BuyingPrice));
            var transferItem4 = _transferHelper.GetTransferItem((int)_item.Id, -2, _incomeAccountId, (decimal)_item.BuyingPrice, (decimal)(-2 * _item.BuyingPrice));
            var detailToDelete = _transferHelper.GetTransferDetail(new List<TransferItem>() { transferItem3, transferItem4 });

            response = proxy.InsertItemTransfer(detailToDelete);

            _testTransferToDeleteId = proxy.InsertItemTransfer(detailToDelete).DataObject.InsertedEntityId;
        }

        private void GetTestAccounts()
        {
            var proxy = new AccountsProxy();
            _assetAccountId = (int)proxy.GetAccounts(accountType: "Asset", isBankAccount: false).DataObject.Accounts[0].Id;
            _incomeAccountId = (int)proxy.GetAccounts(accountType: "Income", isBankAccount: false).DataObject.Accounts[0].Id;
        }

        private void CreateTestItems()
        {
            //Create Item.
            var item = _itemHelper.GetTestInventoryItem();
            var proxy = new ItemProxy();
            var response = new ItemProxy().InsertItem(item);

            _item = proxy.GetItem(response.DataObject.InsertedItemId).DataObject;

            //set SOH for item.
            var adjustment = new Core.Models.ItemAdjustments.AdjustmentDetail
            {
                AdjustmentItems = new List<Core.Models.ItemAdjustments.AdjustmentItem>
                {
                   new Core.Models.ItemAdjustments.AdjustmentItem
                   {
                       ItemId = (int)_item.Id,
                       AccountId = (int)item.AssetAccountId,
                       Quantity = 20,
                       UnitPrice = (decimal)item.BuyingPrice,
                       TotalPrice = (20 * (decimal)item.BuyingPrice)
                   }
                },
                Date = DateTime.Now
            };

            var adjustmentResponse = new ItemAdjustmentProxy().InsertItemAdjustment(adjustment);
        }
    }
}
