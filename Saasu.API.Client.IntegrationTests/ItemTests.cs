using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Models.Accounts;
using Saasu.API.Core.Models.Contacts;
using Saasu.API.Core.Models.Items;

namespace Saasu.API.Client.IntegrationTests
{
    public class ItemTests
    {
        

        public ItemTests()
        {
            _itemHelper = new ItemHelper();
        }

        [Fact]
        public void GetInventoryAndComboItems()
        {
            var itemsProxy = new ItemsProxy();
            var inventoryAndComboItemResponse = itemsProxy.GetItems(null, null, null, 0, 100);

            Assert.True(inventoryAndComboItemResponse.IsSuccessfull, "Unsuccessful request.");
            var inventoryAndComboItemResult = inventoryAndComboItemResponse.DataObject;

            Assert.NotNull(inventoryAndComboItemResult);
            if (inventoryAndComboItemResult.Items.Count > 0)
            {
                Assert.True(inventoryAndComboItemResult.Items.Find(i => i.Type == "I") != null, "There were no inventory items found.");
                Assert.True(inventoryAndComboItemResult.Items.Find(c => c.Type == "C") != null, "There were no combo items found.");
            }

            var item = inventoryAndComboItemResult.Items.First();
            Assert.True(item.LastModifiedDateUtc.HasValue);
            Assert.NotEqual(DateTime.MinValue, item.LastModifiedDateUtc.Value);
            Assert.True(item.CreatedDateUtc.HasValue);
            Assert.NotEqual(DateTime.MinValue, item.CreatedDateUtc.Value);

        }

        [Fact]
        public void GetOnlyInventoryItems()
        {
            var itemsProxy = new ItemsProxy();
            var inventoryAndComboItemResponse = itemsProxy.GetItems("I", null, null, 0, 25);

            Assert.True(inventoryAndComboItemResponse.IsSuccessfull, "Unsuccessful request.");
            var inventoryAndComboItemResult = inventoryAndComboItemResponse.DataObject;

            Assert.NotNull(inventoryAndComboItemResult);
            if (inventoryAndComboItemResult.Items.Count > 0)
            {
                Assert.True(inventoryAndComboItemResult.Items.Find(i => i.Type == "C") == null, "Combo items should not have been returned.");
            }
        }

        [Fact]
        public void GetOnlyComboItems()
        {
            var itemsProxy = new ItemsProxy();
            var inventoryAndComboItemResponse = itemsProxy.GetItems("C", null, null, 0, 25);

            Assert.True(inventoryAndComboItemResponse.IsSuccessfull, "Unsuccessful request.");
            var inventoryAndComboItemResult = inventoryAndComboItemResponse.DataObject;

            Assert.NotNull(inventoryAndComboItemResult);
            if (inventoryAndComboItemResult.Items.Count > 0)
            {
                Assert.True(inventoryAndComboItemResult.Items.Find(i => i.Type == "I") == null, "Inventory items should not have been returned.");
            }
        }

        /// <summary>
        /// Replace the ID with an existing inventory item ID.
        /// </summary>
        [Fact]
        public void GetInventoryItemById()
        {
            var itemsProxy = new ItemsProxy();
            var inventoryAndComboItemResponse = itemsProxy.GetItems("I", null, null, 0, 25);
            Assert.True(inventoryAndComboItemResponse.IsSuccessfull && inventoryAndComboItemResponse.DataObject.Items.Count > 0, "Cannot continue test, request for items failed or returned no items");

            var existingInventoryItemId = inventoryAndComboItemResponse.DataObject.Items.First().Id;

            var itemProxy = new ItemProxy();
            var inventoryItemResponse = itemProxy.GetItem(existingInventoryItemId.Value);

            Assert.True(inventoryItemResponse.IsSuccessfull, "Unsuccessful request.");
            var inventoryItem = inventoryItemResponse.DataObject;

            Assert.NotNull(inventoryItem);
            Assert.True(inventoryItem.Type == "I", "Invalid item type.");

            Assert.True(inventoryItem.LastModifiedDateUtc.HasValue);
            Assert.NotEqual(DateTime.MinValue, inventoryItem.LastModifiedDateUtc.Value);
            Assert.True(inventoryItem.CreatedDateUtc.HasValue);
            Assert.NotEqual(DateTime.MinValue, inventoryItem.CreatedDateUtc.Value);
        }

        /// <summary>
        /// Replace the ID with an exising combo item ID. 
        /// </summary>
        [Fact]
        public void GetComboItemById()
        {

            var itemsProxy = new ItemsProxy();
            var inventoryAndComboItemResponse = itemsProxy.GetItems("C", null, null, 0, 25);
            Assert.True(inventoryAndComboItemResponse.IsSuccessfull && inventoryAndComboItemResponse.DataObject.Items.Count > 0, "Cannot continue test, request for items failed or returned no items");

            var existingComboItemId = inventoryAndComboItemResponse.DataObject.Items.First().Id;

            var itemProxy = new ItemProxy();
            var comboItemResponse = itemProxy.GetItem(existingComboItemId.Value);

            Assert.True(comboItemResponse.IsSuccessfull, "Unsuccessful request.");
            var comboItem = comboItemResponse.DataObject;

            Assert.NotNull(comboItem);
            Assert.True(comboItem.Type == "C", "Invalid item type.");

            Assert.True(comboItem.LastModifiedDateUtc.HasValue);
            Assert.NotEqual(DateTime.MinValue, comboItem.LastModifiedDateUtc.Value);
            Assert.True(comboItem.CreatedDateUtc.HasValue);
            Assert.NotEqual(DateTime.MinValue, comboItem.CreatedDateUtc.Value);

        }

        [Fact]
        public void ShouldInsertInventoryItem()
        {
            var inventoryItem = _itemHelper.GetTestInventoryItem();
            var proxy = new ItemProxy();

            var response = proxy.InsertItem(inventoryItem);

            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.True(response.DataObject.InsertedItemId > 0);
            Assert.True(response.DataObject.UtcLastModified >= DateTime.Today.AddMinutes(-10).ToUniversalTime());
        }

        [Fact]
        public void ShouldInsertComboItem()
        {
            var comboItem = _itemHelper.GetTestComboItem();
            var proxy = new ItemProxy();

            var response = proxy.InsertItem(comboItem);

            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.True(response.DataObject.InsertedItemId > 0);
            Assert.True(response.DataObject.UtcLastModified >= DateTime.Today.AddMinutes(-10).ToUniversalTime());
        }

        [Fact]
        public void ShouldInsertWhenUsingOauthAuthentication()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var comboItem = _itemHelper.GetTestComboItem();
            var proxy = new ItemProxy(accessToken);

            var response = proxy.InsertItem(comboItem);

            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.True(response.DataObject.InsertedItemId > 0);
            Assert.True(response.DataObject.UtcLastModified >= DateTime.Today.AddMinutes(-10).ToUniversalTime());

        }


        [Fact]
        public void ShouldFailOnInsertComboItemWithNoBuildItems()
        {
            var comboItem = _itemHelper.GetTestComboItem();
            comboItem.BuildItems = new List<BuildItem>();
            var proxy = new ItemProxy();

            var response = proxy.InsertItem(comboItem);

            Assert.False(response.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(response.RawResponse.Contains("Please select at least one item for the combo item."));
            Assert.Null(response.DataObject);
        }

        [Fact]
        public void ShouldFailOnInsertComboItemWithInvalidBuildItem()
        {
            var comboItem = _itemHelper.GetTestComboItem();
            comboItem.BuildItems.First().Id = 123;
            var proxy = new ItemProxy();

            var response = proxy.InsertItem(comboItem);

            Assert.False(response.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(response.RawResponse.Contains("One or more line items of the Combo Item was not found"));
            Assert.Null(response.DataObject);
        }

        [Fact]
        public void ShouldFailOnInsertSaleInventoryItemWithNoSaleAccount()
        {
            var inventoryItem = _itemHelper.GetTestInventoryItem();
            inventoryItem.SaleIncomeAccountId = null;
            var proxy = new ItemProxy();

            var response = proxy.InsertItem(inventoryItem);

            Assert.False(response.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(response.RawResponse.Contains("The Item is marked as 'Sold' and therefore needs to have an Income Account assigned to it.\r\nParameter name: SaleIncomeAccountUid\r\n"));
            Assert.Null(response.DataObject);
        }

        [Fact]
        public void ShouldFailOnInsertDuplicateInventoryItemCode()
        {
            var inventoryItem = _itemHelper.GetTestInventoryItem();
            var proxy = new ItemProxy();

            var response1 = proxy.InsertItem(inventoryItem);
            var response2 = proxy.InsertItem(inventoryItem);

            Assert.True(response1.IsSuccessfull);
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.NotNull(response1.DataObject);
            Assert.False(response2.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
            Assert.True(response2.RawResponse.Contains("You already have an item with code"));
            Assert.Null(response2.DataObject);
        }

        [Fact]
        public void ShouldUpdateExistingInventoryItem()
        {
            var inventoryItem = _itemHelper.GetTestInventoryItem();
            var proxy = new ItemProxy();

            var insertResponse = proxy.InsertItem(inventoryItem);
            Assert.True(insertResponse.IsSuccessfull);
            Assert.True(insertResponse.DataObject.InsertedItemId > 0);

            var getResponse = proxy.GetItem(insertResponse.DataObject.InsertedItemId);
            var retrievedItem = getResponse.DataObject;

            Assert.True(getResponse.IsSuccessfull);
            Assert.Equal(inventoryItem.Description, retrievedItem.Description);

            retrievedItem.Description = "Updated Item " + Guid.NewGuid();

            var updateResponse = proxy.UpdateItem(retrievedItem, retrievedItem.Id.Value);
            Assert.True(updateResponse.IsSuccessfull);
            Assert.Equal(insertResponse.DataObject.InsertedItemId, updateResponse.DataObject.UpdatedItemId);

            var updatedItem = proxy.GetItem(updateResponse.DataObject.UpdatedItemId);
            Assert.Equal(retrievedItem.Description, updatedItem.DataObject.Description);

        }

        [Fact]
        public void ShouldUpdateExistingComboItemBuildItem()
        {
            var comboItem = _itemHelper.GetTestComboItem();
            var proxy = new ItemProxy();

            var insertResponse = proxy.InsertItem(comboItem);
            Assert.True(insertResponse.IsSuccessfull);
            Assert.True(insertResponse.DataObject.InsertedItemId > 0);

            var getResponse = proxy.GetItem(insertResponse.DataObject.InsertedItemId);
            var retrievedComboItem = getResponse.DataObject;

            Assert.True(getResponse.IsSuccessfull);
            Assert.Equal(comboItem.Description, retrievedComboItem.Description);

            retrievedComboItem.BuildItems.OrderBy(x => x.Code).First().Quantity = 99;

            var updateResponse = proxy.UpdateItem(retrievedComboItem, retrievedComboItem.Id.Value);
            Assert.True(updateResponse.IsSuccessfull);
            Assert.Equal(insertResponse.DataObject.InsertedItemId, updateResponse.DataObject.UpdatedItemId);

            var updatedItem = proxy.GetItem(updateResponse.DataObject.UpdatedItemId);
            Assert.Equal(retrievedComboItem.BuildItems.OrderBy(x => x.Code).First().Quantity, updatedItem.DataObject.BuildItems.OrderBy(x => x.Code).First().Quantity);

        }

        [Fact]
        public void ShouldReplaceExistingComboItemBuildItem()
        {
            var comboItem = _itemHelper.GetTestComboItem();
            var proxy = new ItemProxy();

            var insertResponse = proxy.InsertItem(comboItem);
            Assert.True(insertResponse.IsSuccessfull);
            Assert.True(insertResponse.DataObject.InsertedItemId > 0);

            var getResponse = proxy.GetItem(insertResponse.DataObject.InsertedItemId);
            var retrievedComboItem = getResponse.DataObject;

            var inventoryItem = _itemHelper.GetTestInventoryItem();
            var insertNewItemResponse = proxy.InsertItem(inventoryItem);

            Assert.True(insertNewItemResponse.IsSuccessfull);

            retrievedComboItem.BuildItems.RemoveAt(0);
            retrievedComboItem.BuildItems.Add(new BuildItem
            {
                Id = insertNewItemResponse.DataObject.InsertedItemId,
                Quantity = 444
            });

            var updateResponse = proxy.UpdateItem(retrievedComboItem, retrievedComboItem.Id.Value);
            Assert.True(updateResponse.IsSuccessfull);
            Assert.Equal(insertResponse.DataObject.InsertedItemId, updateResponse.DataObject.UpdatedItemId);

            var updatedItem = proxy.GetItem(updateResponse.DataObject.UpdatedItemId);
            Assert.Equal(2, retrievedComboItem.BuildItems.Count);
            Assert.True(retrievedComboItem.BuildItems.Any(x => x.Id == insertNewItemResponse.DataObject.InsertedItemId));

        }

        [Fact]
        public void ShouldDeleteInventoryItem()
        {
            var proxy = new ItemProxy();
            var item = _itemHelper.GetTestInventoryItem();

            var insertResponse = proxy.InsertItem(item);
            Assert.True(insertResponse.IsSuccessfull);

            var deleteResponse = proxy.DeleteItem(insertResponse.DataObject.InsertedItemId);
            Assert.True(deleteResponse.IsSuccessfull);
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            var getResponse = proxy.GetItem(insertResponse.DataObject.InsertedItemId);
            Assert.False(getResponse.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
        }

        [Fact]
        public void ShouldDeleteComboItem()
        {
            var proxy = new ItemProxy();
            var comboItem = _itemHelper.GetTestComboItem();

            var insertResponse = proxy.InsertItem(comboItem);
            Assert.True(insertResponse.IsSuccessfull);

            var deleteResponse = proxy.DeleteItem(insertResponse.DataObject.InsertedItemId);
            Assert.True(deleteResponse.IsSuccessfull);
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            var getResponse = proxy.GetItem(insertResponse.DataObject.InsertedItemId);
            Assert.False(getResponse.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
        }

        [Fact]
        public void ShouldBuildComboItem()
        {
            var proxy = new ItemProxy();
            var comboItem = _itemHelper.GetTestComboItem(1,1);
            var insertResponse = proxy.InsertItem(comboItem);

            _itemHelper.InventoryAdjustments(comboItem.BuildItems.Select(s => new Tuple<BuildItem, decimal>(s, 10)).ToList());

            var response = proxy.BuildItem(insertResponse.DataObject.InsertedItemId, new BuildComboItem() {Quantity = 1});

            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.Equal("1 combo item(s) have been built.", response.DataObject.StatusMessage);
            Assert.NotNull(response.DataObject._links);
            Assert.True(response.DataObject._links.Count == 2);
        }

        [Fact]
        public void ShouldFailBuildComboItemDueToStockQuantity()
        {
            var proxy = new ItemProxy();
            var comboItem = _itemHelper.GetTestComboItem(1, 1);
            var insertResponse = proxy.InsertItem(comboItem);

            var response = proxy.BuildItem(insertResponse.DataObject.InsertedItemId, new BuildComboItem() { Quantity = 1 });

            Assert.False(response.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(response.RawResponse.StartsWith("Unable to complete the requested operation as it will cause negative stock-on-hand"));
        }

        [Fact]
        public void ShouldFailBuildComboItemDueToNegativeQuantity()
        {
            var proxy = new ItemProxy();
            var comboItem = _itemHelper.GetTestComboItem(1, 1);
            var insertResponse = proxy.InsertItem(comboItem);

            _itemHelper.InventoryAdjustments(comboItem.BuildItems.Select(s => new Tuple<BuildItem, decimal>(s, 10)).ToList());

            var response = proxy.BuildItem(insertResponse.DataObject.InsertedItemId, new BuildComboItem() { Quantity = -1 });

            Assert.False(response.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(response.RawResponse.StartsWith("Unable to complete the requested operation as it will cause negative stock-on-hand"));
        }


        [Fact]
        public void ShouldFailBuildComboItemDueToBadId()
        {
            var proxy = new ItemProxy();

            var response = proxy.BuildItem(-1, new BuildComboItem() { Quantity = 1 });

            Assert.False(response.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        private readonly ItemHelper _itemHelper;
    }
}
