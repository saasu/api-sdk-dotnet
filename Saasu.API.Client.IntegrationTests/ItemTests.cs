using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ola.RestClient.Dto;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Models.Accounts;
using Saasu.API.Core.Models.Contacts;
using Saasu.API.Core.Models.Items;

namespace Saasu.API.Client.IntegrationTests
{
    /// <summary>
    /// Following tests assume there's at least one inventory item and one combo item already setup in the target file.
    /// ToDo: Refactor tests after API has been updated to support INSERT, UPDATE and DELETE of Items to setup test data. 
    /// </summary>
    [TestFixture]
    public class ItemTests
    {
        

        public ItemTests()
        {
            _itemHelper = new ItemHelper();
        }

        [Test]
        public void GetInventoryAndComboItems()
        {
            var itemsProxy = new ItemsProxy();
            var inventoryAndComboItemResponse = itemsProxy.GetItems(null, null, null, 0, 100);

            Assert.IsTrue(inventoryAndComboItemResponse.IsSuccessfull, "Unsuccessful request.");
            var inventoryAndComboItemResult = inventoryAndComboItemResponse.DataObject;

            Assert.IsNotNull(inventoryAndComboItemResult, "Could not retrieve inventory and combo items successfully.");
            if (inventoryAndComboItemResult.Items.Count > 0)
            {
                Assert.IsTrue(inventoryAndComboItemResult.Items.Find(i => i.Type == "I") != null, "There were no inventory items found.");
                Assert.IsTrue(inventoryAndComboItemResult.Items.Find(c => c.Type == "C") != null, "There were no combo items found.");
            }

            var item = inventoryAndComboItemResult.Items.First();
            Assert.IsTrue(item.LastModifiedDateUtc.HasValue);
            Assert.AreNotEqual(DateTime.MinValue, item.LastModifiedDateUtc.Value);
            Assert.IsTrue(item.CreatedDateUtc.HasValue);
            Assert.AreNotEqual(DateTime.MinValue, item.CreatedDateUtc.Value);

        }

        [Test]
        public void GetOnlyInventoryItems()
        {
            var itemsProxy = new ItemsProxy();
            var inventoryAndComboItemResponse = itemsProxy.GetItems("I", null, null, 0, 25);

            Assert.IsTrue(inventoryAndComboItemResponse.IsSuccessfull, "Unsuccessful request.");
            var inventoryAndComboItemResult = inventoryAndComboItemResponse.DataObject;

            Assert.IsNotNull(inventoryAndComboItemResult, "Could not retireve inventory items successfully.");
            if (inventoryAndComboItemResult.Items.Count > 0)
            {
                Assert.IsTrue(inventoryAndComboItemResult.Items.Find(i => i.Type == "C") == null, "Combo items should not have been returned.");
            }
        }

        [Test]
        public void GetOnlyComboItems()
        {
            var itemsProxy = new ItemsProxy();
            var inventoryAndComboItemResponse = itemsProxy.GetItems("C", null, null, 0, 25);

            Assert.IsTrue(inventoryAndComboItemResponse.IsSuccessfull, "Unsuccessful request.");
            var inventoryAndComboItemResult = inventoryAndComboItemResponse.DataObject;

            Assert.IsNotNull(inventoryAndComboItemResult, "Could not retireve inventory items successfully.");
            if (inventoryAndComboItemResult.Items.Count > 0)
            {
                Assert.IsTrue(inventoryAndComboItemResult.Items.Find(i => i.Type == "I") == null, "Inventory items should not have been returned.");
            }
        }

        /// <summary>
        /// Replace the ID with an existing inventory item ID.
        /// </summary>
        [Test]
        public void GetInventoryItemById()
        {
            var itemsProxy = new ItemsProxy();
            var inventoryAndComboItemResponse = itemsProxy.GetItems("I", null, null, 0, 25);
            Assert.IsTrue(inventoryAndComboItemResponse.IsSuccessfull && inventoryAndComboItemResponse.DataObject.Items.Count > 0, "Cannot continue test, request for items failed or returned no items");

            var existingInventoryItemId = inventoryAndComboItemResponse.DataObject.Items.First().Id;

            var itemProxy = new ItemProxy();
            var inventoryItemResponse = itemProxy.GetItem(existingInventoryItemId.Value);

            Assert.IsTrue(inventoryItemResponse.IsSuccessfull, "Unsuccessful request.");
            var inventoryItem = inventoryItemResponse.DataObject;

            Assert.IsNotNull(inventoryItem, "No inventory item found.");
            Assert.IsTrue(inventoryItem.Type == "I", "Invalid item type.");

            Assert.IsTrue(inventoryItem.LastModifiedDateUtc.HasValue);
            Assert.AreNotEqual(DateTime.MinValue, inventoryItem.LastModifiedDateUtc.Value);
            Assert.IsTrue(inventoryItem.CreatedDateUtc.HasValue);
            Assert.AreNotEqual(DateTime.MinValue, inventoryItem.CreatedDateUtc.Value);
        }

        /// <summary>
        /// Replace the ID with an exising combo item ID. 
        /// </summary>
        [Test]
        public void GetComboItemById()
        {

            var itemsProxy = new ItemsProxy();
            var inventoryAndComboItemResponse = itemsProxy.GetItems("C", null, null, 0, 25);
            Assert.IsTrue(inventoryAndComboItemResponse.IsSuccessfull && inventoryAndComboItemResponse.DataObject.Items.Count > 0, "Cannot continue test, request for items failed or returned no items");

            var existingComboItemId = inventoryAndComboItemResponse.DataObject.Items.First().Id;

            var itemProxy = new ItemProxy();
            var comboItemResponse = itemProxy.GetItem(existingComboItemId.Value);

            Assert.IsTrue(comboItemResponse.IsSuccessfull, "Unsuccessful request.");
            var comboItem = comboItemResponse.DataObject;

            Assert.IsNotNull(comboItem, "No combo item found.");
            Assert.IsTrue(comboItem.Type == "C", "Invalid item type.");

            Assert.IsTrue(comboItem.LastModifiedDateUtc.HasValue);
            Assert.AreNotEqual(DateTime.MinValue, comboItem.LastModifiedDateUtc.Value);
            Assert.IsTrue(comboItem.CreatedDateUtc.HasValue);
            Assert.AreNotEqual(DateTime.MinValue, comboItem.CreatedDateUtc.Value);

        }

        [Test]
        public void ShouldInsertInventoryItem()
        {
            var inventoryItem = _itemHelper.GetTestInventoryItem();
            var proxy = new ItemProxy();

            var response = proxy.InsertItem(inventoryItem);

            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsTrue(response.DataObject.InsertedItemId > 0);
            Assert.GreaterOrEqual(response.DataObject.UtcLastModified, DateTime.Today.AddMinutes(-10).ToUniversalTime());
        }

        [Test]
        public void ShouldInsertComboItem()
        {
            var comboItem = _itemHelper.GetTestComboItem();
            var proxy = new ItemProxy();

            var response = proxy.InsertItem(comboItem);

            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsTrue(response.DataObject.InsertedItemId > 0);
            Assert.GreaterOrEqual(response.DataObject.UtcLastModified, DateTime.Today.AddMinutes(-10).ToUniversalTime());
        }

        [Test]
        public void ShouldInsertWhenUsingOauthAuthentication()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var comboItem = _itemHelper.GetTestComboItem();
            var proxy = new ItemProxy(accessToken);

            var response = proxy.InsertItem(comboItem);

            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsTrue(response.DataObject.InsertedItemId > 0);
            Assert.GreaterOrEqual(response.DataObject.UtcLastModified, DateTime.Today.AddMinutes(-10).ToUniversalTime());

        }


        [Test]
        public void ShouldFailOnInsertComboItemWithNoBuildItems()
        {
            var comboItem = _itemHelper.GetTestComboItem();
            comboItem.BuildItems = new List<BuildItem>();
            var proxy = new ItemProxy();

            var response = proxy.InsertItem(comboItem);

            Assert.IsFalse(response.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(response.RawResponse.Contains("Please select at least one item for the combo item."));
            Assert.IsNull(response.DataObject);
        }

        [Test]
        public void ShouldFailOnInsertComboItemWithInvalidBuildItem()
        {
            var comboItem = _itemHelper.GetTestComboItem();
            comboItem.BuildItems.First().Id = 123;
            var proxy = new ItemProxy();

            var response = proxy.InsertItem(comboItem);

            Assert.IsFalse(response.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(response.RawResponse.Contains("One or more line items of the Combo Item was not found"));
            Assert.IsNull(response.DataObject);
        }

        [Test]
        public void ShouldFailOnInsertSaleInventoryItemWithNoSaleAccount()
        {
            var inventoryItem = _itemHelper.GetTestInventoryItem();
            inventoryItem.SaleIncomeAccountId = null;
            var proxy = new ItemProxy();

            var response = proxy.InsertItem(inventoryItem);

            Assert.IsFalse(response.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(response.RawResponse.Contains("The Item is marked as 'Sold' and therefore needs to have an Income Account assigned to it.\r\nParameter name: SaleIncomeAccountUid\r\n"));
            Assert.IsNull(response.DataObject);
        }

        [Test]
        public void ShouldFailOnInsertDuplicateInventoryItemCode()
        {
            var inventoryItem = _itemHelper.GetTestInventoryItem();
            var proxy = new ItemProxy();

            var response1 = proxy.InsertItem(inventoryItem);
            var response2 = proxy.InsertItem(inventoryItem);

            Assert.IsTrue(response1.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);
            Assert.IsNotNull(response1.DataObject);
            Assert.IsFalse(response2.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, response2.StatusCode);
            Assert.IsTrue(response2.RawResponse.Contains("You already have an item with code"));
            Assert.IsNull(response2.DataObject);
        }

        [Test]
        public void ShouldUpdateExistingInventoryItem()
        {
            var inventoryItem = _itemHelper.GetTestInventoryItem();
            var proxy = new ItemProxy();

            var insertResponse = proxy.InsertItem(inventoryItem);
            Assert.IsTrue(insertResponse.IsSuccessfull);
            Assert.IsTrue(insertResponse.DataObject.InsertedItemId > 0);

            var getResponse = proxy.GetItem(insertResponse.DataObject.InsertedItemId);
            var retrievedItem = getResponse.DataObject;

            Assert.IsTrue(getResponse.IsSuccessfull);
            Assert.AreEqual(inventoryItem.Description, retrievedItem.Description);

            retrievedItem.Description = "Updated Item " + Guid.NewGuid();

            var updateResponse = proxy.UpdateItem(retrievedItem, retrievedItem.Id.Value);
            Assert.IsTrue(updateResponse.IsSuccessfull);
            Assert.AreEqual(insertResponse.DataObject.InsertedItemId, updateResponse.DataObject.UpdatedItemId);

            var updatedItem = proxy.GetItem(updateResponse.DataObject.UpdatedItemId);
            Assert.AreEqual(retrievedItem.Description, updatedItem.DataObject.Description);

        }

        [Test]
        public void ShouldUpdateExistingComboItemBuildItem()
        {
            var comboItem = _itemHelper.GetTestComboItem();
            var proxy = new ItemProxy();

            var insertResponse = proxy.InsertItem(comboItem);
            Assert.IsTrue(insertResponse.IsSuccessfull);
            Assert.IsTrue(insertResponse.DataObject.InsertedItemId > 0);

            var getResponse = proxy.GetItem(insertResponse.DataObject.InsertedItemId);
            var retrievedComboItem = getResponse.DataObject;

            Assert.IsTrue(getResponse.IsSuccessfull);
            Assert.AreEqual(comboItem.Description, retrievedComboItem.Description);

            retrievedComboItem.BuildItems.OrderBy(x => x.Code).First().Quantity = 99;

            var updateResponse = proxy.UpdateItem(retrievedComboItem, retrievedComboItem.Id.Value);
            Assert.IsTrue(updateResponse.IsSuccessfull);
            Assert.AreEqual(insertResponse.DataObject.InsertedItemId, updateResponse.DataObject.UpdatedItemId);

            var updatedItem = proxy.GetItem(updateResponse.DataObject.UpdatedItemId);
            Assert.AreEqual(retrievedComboItem.BuildItems.OrderBy(x => x.Code).First().Quantity, updatedItem.DataObject.BuildItems.OrderBy(x => x.Code).First().Quantity);

        }

        [Test]
        public void ShouldReplaceExistingComboItemBuildItem()
        {
            var comboItem = _itemHelper.GetTestComboItem();
            var proxy = new ItemProxy();

            var insertResponse = proxy.InsertItem(comboItem);
            Assert.IsTrue(insertResponse.IsSuccessfull);
            Assert.IsTrue(insertResponse.DataObject.InsertedItemId > 0);

            var getResponse = proxy.GetItem(insertResponse.DataObject.InsertedItemId);
            var retrievedComboItem = getResponse.DataObject;

            var inventoryItem = _itemHelper.GetTestInventoryItem();
            var insertNewItemResponse = proxy.InsertItem(inventoryItem);

            Assert.IsTrue(insertNewItemResponse.IsSuccessfull);

            retrievedComboItem.BuildItems.RemoveAt(0);
            retrievedComboItem.BuildItems.Add(new BuildItem
            {
                Id = insertNewItemResponse.DataObject.InsertedItemId,
                Quantity = 444
            });

            var updateResponse = proxy.UpdateItem(retrievedComboItem, retrievedComboItem.Id.Value);
            Assert.IsTrue(updateResponse.IsSuccessfull);
            Assert.AreEqual(insertResponse.DataObject.InsertedItemId, updateResponse.DataObject.UpdatedItemId);

            var updatedItem = proxy.GetItem(updateResponse.DataObject.UpdatedItemId);
            Assert.AreEqual(2, retrievedComboItem.BuildItems.Count);
            Assert.IsTrue(retrievedComboItem.BuildItems.Any(x => x.Id == insertNewItemResponse.DataObject.InsertedItemId));

        }

        [Test]
        public void ShouldDeleteInventoryItem()
        {
            var proxy = new ItemProxy();
            var item = _itemHelper.GetTestInventoryItem();

            var insertResponse = proxy.InsertItem(item);
            Assert.IsTrue(insertResponse.IsSuccessfull);

            var deleteResponse = proxy.DeleteItem(insertResponse.DataObject.InsertedItemId);
            Assert.IsTrue(deleteResponse.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);

            var getResponse = proxy.GetItem(insertResponse.DataObject.InsertedItemId);
            Assert.IsFalse(getResponse.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, getResponse.StatusCode);
        }

        [Test]
        public void ShouldDeleteComboItem()
        {
            var proxy = new ItemProxy();
            var comboItem = _itemHelper.GetTestComboItem();

            var insertResponse = proxy.InsertItem(comboItem);
            Assert.IsTrue(insertResponse.IsSuccessfull);

            var deleteResponse = proxy.DeleteItem(insertResponse.DataObject.InsertedItemId);
            Assert.IsTrue(deleteResponse.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);

            var getResponse = proxy.GetItem(insertResponse.DataObject.InsertedItemId);
            Assert.IsFalse(getResponse.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, getResponse.StatusCode);
        }

        [Test]
        public void ShouldBuildComboItem()
        {
            var proxy = new ItemProxy();
            var comboItem = _itemHelper.GetTestComboItem(1,1);
            var insertResponse = proxy.InsertItem(comboItem);

            _itemHelper.InventoryAdjustments(comboItem.BuildItems.Select(s => new Tuple<BuildItem, decimal>(s, 10)).ToList());

            var response = proxy.BuildItem(insertResponse.DataObject.InsertedItemId, new BuildComboItem() {Quantity = 1});

            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.AreEqual("1 combo item(s) have been built.", response.DataObject.StatusMessage);
            Assert.IsNotNull(response.DataObject._links);
            Assert.IsTrue(response.DataObject._links.Count == 2);
        }

        [Test]
        public void ShouldFailBuildComboItemDueToStockQuantity()
        {
            var proxy = new ItemProxy();
            var comboItem = _itemHelper.GetTestComboItem(1, 1);
            var insertResponse = proxy.InsertItem(comboItem);

            var response = proxy.BuildItem(insertResponse.DataObject.InsertedItemId, new BuildComboItem() { Quantity = 1 });

            Assert.IsFalse(response.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(response.RawResponse.StartsWith("Unable to complete the requested operation as it will cause negative stock-on-hand"));
        }

        [Test]
        public void ShouldFailBuildComboItemDueToNegativeQuantity()
        {
            var proxy = new ItemProxy();
            var comboItem = _itemHelper.GetTestComboItem(1, 1);
            var insertResponse = proxy.InsertItem(comboItem);

            _itemHelper.InventoryAdjustments(comboItem.BuildItems.Select(s => new Tuple<BuildItem, decimal>(s, 10)).ToList());

            var response = proxy.BuildItem(insertResponse.DataObject.InsertedItemId, new BuildComboItem() { Quantity = -1 });

            Assert.IsFalse(response.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(response.RawResponse.StartsWith("Unable to complete the requested operation as it will cause negative stock-on-hand"));
        }


        [Test]
        public void ShouldFailBuildComboItemDueToBadId()
        {
            var proxy = new ItemProxy();

            var response = proxy.BuildItem(-1, new BuildComboItem() { Quantity = 1 });

            Assert.IsFalse(response.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }


        private readonly ItemHelper _itemHelper;
    }
}
