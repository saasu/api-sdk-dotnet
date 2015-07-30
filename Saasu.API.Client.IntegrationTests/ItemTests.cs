using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Saasu.API.Client.Proxies;

namespace Saasu.API.Client.IntegrationTests
{
    /// <summary>
    /// Following tests assume there's at least one inventory item and one combo item already setup in the target file.
    /// ToDo: Refactor tests after API has been updated to support INSERT, UPDATE and DELETE of Items to setup test data. 
    /// </summary>
    [TestFixture]
    public class ItemTests 
    {        
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
    }
}
