using Xunit;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Items;
using Saasu.API.Core.Models.Search;
using System.Linq;
using System.Threading;

namespace Saasu.API.Client.IntegrationTests
{
    public class SearchTests
    {
        private InvoiceHelper _invoiceHelper;
        private readonly ItemHelper _itemHelper;

        public SearchTests()
        {
            _invoiceHelper = new InvoiceHelper();
            Thread.Sleep(3000); // Need to wait for entities to be indexed
            _itemHelper = new ItemHelper();
        }

        [Fact(Skip = "Solr required.")]
        public void ShouldReturnTransactionForScopedSearch()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.Transactions, 1, 25);
            Assert.NotNull(results);
            Assert.True(results.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results.DataObject.TotalContactsFound);
            Assert.Equal(0, results.DataObject.TotalInventoryItemsFound);
        }

        [Fact(Skip = "Solr required.")]
        public void ShouldReturnContactsForScopedSearch()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.Contacts, 1, 25);
            Assert.NotNull(results);
            Assert.True(results.DataObject.Contacts.Count > 0, "No contacts returned.");
            Assert.True(results.DataObject.TotalContactsFound > 0, "transaction count is 0.");
            Assert.True(results.DataObject.TotalTransactionsFound == 0, "Should not return transactions for search scoped to Contacts");
            Assert.True(results.DataObject.TotalInventoryItemsFound == 0, "Should not return items for search scoped to Contacts");
        }

        [Fact(Skip = "Solr required.")]
        public void ShouldReturnInventoryItemsForScopedSearch()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.InventoryItems, 1, 25);
            Assert.NotNull(results);
            Assert.NotNull(results.DataObject);
            Assert.True(results.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(results.DataObject.TotalInventoryItemsFound > 0, "Inventory item count is 0.");
            Assert.True(results.DataObject.TotalTransactionsFound == 0, "Should not return transactions for search scoped to Contacts");
            Assert.True(results.DataObject.TotalContactsFound == 0, "Should not return contacts for search scoped to Contacts");
        }

        [Fact(Skip = "Solr required.")]
        public void ShouldReturnContactsTransactionsInventoryItemsForScopedAllSearch()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.All, 1, 25);
            Assert.NotNull(results);
            Assert.NotNull(results.DataObject);
            Assert.True(results.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(results.DataObject.Contacts.Count > 0, "No contacts returned.");
            Assert.True(results.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(results.DataObject.TotalInventoryItemsFound > 0, "Inventory item count is 0.");
            Assert.True(results.DataObject.TotalContactsFound > 0, "transaction count is 0.");
            Assert.True(results.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
        }

        [Fact(Skip = "Solr required.")]
        public void ShouldReturnNotResults()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("qwerty", SearchScope.All, 1, 25);
            Assert.NotNull(results);
            Assert.NotNull(results.DataObject);
            Assert.Equal(0, results.DataObject.InventoryItems.Count);
            Assert.Equal(0, results.DataObject.Contacts.Count);
            Assert.Equal(0, results.DataObject.InventoryItems.Count);
            Assert.Equal(0, results.DataObject.TotalInventoryItemsFound);
            Assert.Equal(0, results.DataObject.TotalContactsFound);
            Assert.Equal(0, results.DataObject.TotalTransactionsFound);
        }

        [Fact(Skip = "Solr required.")]
        public void IndexedTransactionShouldMatchEntityData()
        {
            var searchProxy = new SearchProxy();
            var invoice = new InvoiceProxy().GetInvoice(_invoiceHelper.InvoiceId1.Value).DataObject;
            Assert.NotNull(invoice.BillingContactId);
            var contact = new ContactProxy().GetContact(invoice.BillingContactId.Value).DataObject;

            var searchResults = searchProxy.Search(_invoiceHelper.InvoiceId1Summary, SearchScope.Transactions, 1, 100);
            Assert.NotNull(searchResults.DataObject);
            Assert.True(searchResults.DataObject.Transactions.Count > 0);
            var indexedTransaction = searchResults.DataObject.Transactions.First(x => x.Id == invoice.TransactionId);

            Assert.Equal(invoice.DueDate, indexedTransaction.DueDate);
            Assert.Equal(invoice.BillingContactOrganisationName, indexedTransaction.Company);
            Assert.Equal(contact.EmailAddress, ReplaceEm(indexedTransaction.ContactEmail));
            Assert.Equal(invoice.InvoiceNumber, indexedTransaction.InvoiceNumber);
            Assert.Equal(invoice.PurchaseOrderNumber, indexedTransaction.PurchaseOrderNumber);
            Assert.Equal(invoice.NotesExternal, indexedTransaction.ExternalNotes);
            Assert.Equal(invoice.NotesInternal, indexedTransaction.Notes);
            Assert.Equal(invoice.TransactionType, indexedTransaction.Type);            
            Assert.Equal(invoice.InvoiceType, indexedTransaction.InvoiceType);
        }

        [Fact(Skip = "Solr required.")]
        public void IndexedContactShouldMatchEntityData()
        {
            var searchProxy = new SearchProxy();
            var contactHelper = new ContactHelper();

            var contactResponse = contactHelper.AddContact();
            Thread.Sleep(5000);
            var contact = new ContactProxy().GetContact(contactResponse.InsertedContactId).DataObject;

            var searchResults = searchProxy.Search(contact.EmailAddress, SearchScope.Contacts, 1, 10);
            Assert.NotNull(searchResults.DataObject);
            Assert.True(searchResults.DataObject.Contacts.Count > 0);

            var indexedContact = searchResults.DataObject.Contacts.First(x => ReplaceEm(x.ContactEmail) == contact.EmailAddress);
            Assert.NotNull(indexedContact);
            Assert.Equal(contact.GivenName + " " + contact.FamilyName, ReplaceEm(indexedContact.Name));
            Assert.Equal(contact.MobilePhone, indexedContact.MobilePhone);
            Assert.Equal(contact.PrimaryPhone, indexedContact.MainPhone);
            Assert.Equal(contact.Id, indexedContact.Id);
            Assert.Equal("Contact", indexedContact.EntityType);
            Assert.Equal(contact.CompanyId ?? 0, indexedContact.CompanyId);
            Assert.Equal(contact.TwitterId, indexedContact.TwitterId);
        }

        [Fact(Skip = "Solr required.")]
        public void IndexedInventoryItemShouldMatchEntityData()
        {
            var searchProxy = new SearchProxy();

            var inventoryItem = InsertAndGetInventoryItem();
            Thread.Sleep(5000);
            var searchResults = searchProxy.Search(inventoryItem.Code, SearchScope.InventoryItems, 1, 10);
            Assert.NotNull(searchResults.DataObject);
            Assert.True(searchResults.DataObject.InventoryItems.Count > 0);

            var indexedInventoryItem = searchResults.DataObject.InventoryItems.First(x => ReplaceEm(x.Code) == inventoryItem.Code);
            Assert.NotNull(indexedInventoryItem);
            Assert.Equal(inventoryItem.BuyingPrice, indexedInventoryItem.BuyingPrice);
            Assert.Equal(inventoryItem.Description, ReplaceEm(indexedInventoryItem.Description));
            Assert.Equal(inventoryItem.Type, indexedInventoryItem.Type);
            Assert.Equal("Item", indexedInventoryItem.EntityType);
            Assert.Equal(inventoryItem.Id, indexedInventoryItem.Id);
            Assert.Equal(inventoryItem.SellingPrice, indexedInventoryItem.SellingPrice);
            Assert.Equal(inventoryItem.StockOnHand.HasValue ? inventoryItem.StockOnHand : 0M, indexedInventoryItem.StockOnHand);
            Assert.Equal(inventoryItem.PrimarySupplierItemCode, indexedInventoryItem.SupplierItemCode);
        }

        [Fact(Skip = "Solr required.")]
        public void ShouldReturnSalesOnlyWhenTransactionEntityTypeSpecified()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.Transactions, 1, 25, "transactions.sale");
            Assert.NotNull(results);
            Assert.NotNull(results.DataObject);
            Assert.True(results.DataObject.Transactions.TrueForAll(x => x.Type == "S"));
        }

        [Fact(Skip = "Solr required.")]
        public void ShouldReturnPurchasesOnlyWhenTransactionEntityTypeSpecified()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("test", SearchScope.Transactions, 1, 25, "transactions.purchase");
            Assert.NotNull(results);
            Assert.NotNull(results.DataObject);
            Assert.True(results.DataObject.Transactions.TrueForAll(x => x.Type == "P"));
        }

        private ItemDetail InsertAndGetInventoryItem()
        {
            var itemProxy = new ItemProxy();
            var inventoryItem = _itemHelper.GetTestInventoryItem();
            var itemResponse = itemProxy.InsertItem(inventoryItem);
            var item = itemProxy.GetItem(itemResponse.DataObject.InsertedItemId);
            Assert.NotNull(item.DataObject);
            Thread.Sleep(2000);
            return item.DataObject;
        }

        private static string ReplaceEm(string someString)
        {
            return someString.Replace("<em>", "").Replace("</em>", "");
        }

    }
}
