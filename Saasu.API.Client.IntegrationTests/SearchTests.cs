using NUnit.Framework;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Items;
using Saasu.API.Core.Models.Search;
using System.Linq;
using System.Threading;

namespace Saasu.API.Client.IntegrationTests
{
    [TestFixture]
    [Ignore]
    public class SearchTests
    {
        private InvoiceHelper _invoiceHelper;
        private readonly ItemHelper _itemHelper;

        public SearchTests()
        {
            _invoiceHelper = new InvoiceHelper();
            _invoiceHelper.CreateTestData();
            Thread.Sleep(3000); // Need to wait for entities to be indexed
            _itemHelper = new ItemHelper();
        }

        [Test]
        public void ShouldReturnTransactionForScopedSearch()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.Transactions, 1, 25);
            Assert.IsNotNull(results, "No search results returned.");
            Assert.IsTrue(results.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.IsTrue(results.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.AreEqual(0, results.DataObject.TotalContactsFound, "Should not return contacts for search scoped to Contacts");
            Assert.AreEqual(0, results.DataObject.TotalInventoryItemsFound, "Should not return items for search scoped to Contacts");
        }

        [Test]
        public void ShouldReturnContactsForScopedSearch()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.Contacts, 1, 25);
            Assert.IsNotNull(results, "No search results returned.");
            Assert.IsTrue(results.DataObject.Contacts.Count > 0, "No contacts returned.");
            Assert.IsTrue(results.DataObject.TotalContactsFound > 0, "transaction count is 0.");
            Assert.IsTrue(results.DataObject.TotalTransactionsFound == 0, "Should not return transactions for search scoped to Contacts");
            Assert.IsTrue(results.DataObject.TotalInventoryItemsFound == 0, "Should not return items for search scoped to Contacts");
        }

        [Test]
        public void ShouldReturnInventoryItemsForScopedSearch()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.InventoryItems, 1, 25);
            Assert.IsNotNull(results, "No search results returned.");
            Assert.IsNotNull(results.DataObject, "No search result object returned.");
            Assert.IsTrue(results.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.IsTrue(results.DataObject.TotalInventoryItemsFound > 0, "Inventory item count is 0.");
            Assert.IsTrue(results.DataObject.TotalTransactionsFound == 0, "Should not return transactions for search scoped to Contacts");
            Assert.IsTrue(results.DataObject.TotalContactsFound == 0, "Should not return contacts for search scoped to Contacts");
        }

        [Test]
        public void ShouldReturnContactsTransactionsInventoryItemsForScopedAllSearch()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.All, 1, 25);
            Assert.IsNotNull(results, "No search results returned.");
            Assert.IsNotNull(results.DataObject, "No search result object returned.");
            Assert.IsTrue(results.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.IsTrue(results.DataObject.Contacts.Count > 0, "No contacts returned.");
            Assert.IsTrue(results.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.IsTrue(results.DataObject.TotalInventoryItemsFound > 0, "Inventory item count is 0.");
            Assert.IsTrue(results.DataObject.TotalContactsFound > 0, "transaction count is 0.");
            Assert.IsTrue(results.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
        }

        [Test]
        public void ShouldReturnNotResults()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("qwerty", SearchScope.All, 1, 25);
            Assert.IsNotNull(results, "No search results returned.");
            Assert.IsNotNull(results.DataObject, "No search result object returned.");
            Assert.AreEqual(0, results.DataObject.InventoryItems.Count);
            Assert.AreEqual(0, results.DataObject.Contacts.Count);
            Assert.AreEqual(0, results.DataObject.InventoryItems.Count);
            Assert.AreEqual(0, results.DataObject.TotalInventoryItemsFound);
            Assert.AreEqual(0, results.DataObject.TotalContactsFound);
            Assert.AreEqual(0, results.DataObject.TotalTransactionsFound);
        }

        [Test]
        public void IndexedTransactionShouldMatchEntityData()
        {
            var searchProxy = new SearchProxy();
            var invoice = new InvoiceProxy().GetInvoice(_invoiceHelper.InvoiceId1.Value).DataObject;
            Assert.IsNotNull(invoice.BillingContactId, "Contact not found");
            var contact = new ContactProxy().GetContact(invoice.BillingContactId.Value).DataObject;

            var searchResults = searchProxy.Search(_invoiceHelper.InvoiceId1Summary, SearchScope.Transactions, 1, 100);
            Assert.IsNotNull(searchResults.DataObject);
            Assert.IsTrue(searchResults.DataObject.Transactions.Count > 0);
            var indexedTransaction = searchResults.DataObject.Transactions.First(x => x.Id == invoice.TransactionId);

            Assert.AreEqual(invoice.DueDate, indexedTransaction.DueDate);
            Assert.AreEqual(invoice.BillingContactOrganisationName, indexedTransaction.Company);
            Assert.AreEqual(contact.EmailAddress, ReplaceEm(indexedTransaction.ContactEmail));
            Assert.AreEqual(invoice.InvoiceNumber, indexedTransaction.InvoiceNumber);
            Assert.AreEqual(invoice.PurchaseOrderNumber, indexedTransaction.PurchaseOrderNumber);
            Assert.AreEqual(invoice.NotesExternal, indexedTransaction.ExternalNotes);
            Assert.AreEqual(invoice.NotesInternal, indexedTransaction.Notes);
            Assert.AreEqual(invoice.TransactionType, indexedTransaction.Type);            
            Assert.AreEqual(invoice.InvoiceType, indexedTransaction.InvoiceType);
        }

        [Test]
        public void IndexedContactShouldMatchEntityData()
        {
            var searchProxy = new SearchProxy();
            var contactHelper = new ContactHelper();

            var contactResponse = contactHelper.AddContact();
            Thread.Sleep(5000);
            var contact = new ContactProxy().GetContact(contactResponse.InsertedContactId).DataObject;

            var searchResults = searchProxy.Search(contact.EmailAddress, SearchScope.Contacts, 1, 10);
            Assert.IsNotNull(searchResults.DataObject);
            Assert.IsTrue(searchResults.DataObject.Contacts.Count > 0);

            var indexedContact = searchResults.DataObject.Contacts.First(x => ReplaceEm(x.ContactEmail) == contact.EmailAddress);
            Assert.IsNotNull(indexedContact);
            Assert.AreEqual(contact.GivenName + " " + contact.FamilyName, ReplaceEm(indexedContact.Name));
            Assert.AreEqual(contact.MobilePhone, indexedContact.MobilePhone);
            Assert.AreEqual(contact.PrimaryPhone, indexedContact.MainPhone);
            Assert.AreEqual(contact.Id, indexedContact.Id);
            Assert.AreEqual("Contact", indexedContact.EntityType);
            Assert.AreEqual(contact.CompanyId ?? 0, indexedContact.CompanyId);
            Assert.AreEqual(contact.TwitterId, indexedContact.TwitterId);
        }

        [Test]
        public void IndexedInventoryItemShouldMatchEntityData()
        {
            var searchProxy = new SearchProxy();

            var inventoryItem = InsertAndGetInventoryItem();
            Thread.Sleep(5000);
            var searchResults = searchProxy.Search(inventoryItem.Code, SearchScope.InventoryItems, 1, 10);
            Assert.IsNotNull(searchResults.DataObject);
            Assert.IsTrue(searchResults.DataObject.InventoryItems.Count > 0);

            var indexedInventoryItem = searchResults.DataObject.InventoryItems.First(x => ReplaceEm(x.Code) == inventoryItem.Code);
            Assert.IsNotNull(indexedInventoryItem);
            Assert.AreEqual(inventoryItem.BuyingPrice, indexedInventoryItem.BuyingPrice);
            Assert.AreEqual(inventoryItem.Description, ReplaceEm(indexedInventoryItem.Description));
            Assert.AreEqual(inventoryItem.Type, indexedInventoryItem.Type);
            Assert.AreEqual("Item", indexedInventoryItem.EntityType);
            Assert.AreEqual(inventoryItem.Id, indexedInventoryItem.Id);
            Assert.AreEqual(inventoryItem.SellingPrice, indexedInventoryItem.SellingPrice);
            Assert.AreEqual(inventoryItem.StockOnHand.HasValue ? inventoryItem.StockOnHand : 0M, indexedInventoryItem.StockOnHand);
            Assert.AreEqual(inventoryItem.PrimarySupplierItemCode, indexedInventoryItem.SupplierItemCode);
        }

        [Test]
        public void ShouldReturnSalesOnlyWhenTransactionEntityTypeSpecified()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.Transactions, 1, 25, "transactions.sale");
            Assert.IsNotNull(results, "No search results returned.");
            Assert.IsNotNull(results.DataObject, "No search result object returned.");
            Assert.IsTrue(results.DataObject.Transactions.TrueForAll(x => x.Type == "S"));
        }

        [Test]
        public void ShouldReturnPurchasesOnlyWhenTransactionEntityTypeSpecified()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.Transactions, 1, 25, "transactions.purchase");
            Assert.IsNotNull(results, "No search results returned.");
            Assert.IsNotNull(results.DataObject, "No search result object returned.");
            Assert.IsTrue(results.DataObject.Transactions.TrueForAll(x => x.Type == "P"));
        }

        private ItemDetail InsertAndGetInventoryItem()
        {
            var itemProxy = new ItemProxy();
            var inventoryItem = _itemHelper.GetTestInventoryItem();
            var itemResponse = itemProxy.InsertItem(inventoryItem);
            var item = itemProxy.GetItem(itemResponse.DataObject.InsertedItemId);
            Assert.IsNotNull(item.DataObject, "Failed to insert and retrieve inventory item");
            Thread.Sleep(2000);
            return item.DataObject;
        }

        private static string ReplaceEm(string someString)
        {
            return someString.Replace("<em>", "").Replace("</em>", "");
        }

    }
}
