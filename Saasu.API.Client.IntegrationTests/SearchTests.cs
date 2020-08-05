using System;
using System.Globalization;
using Xunit;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Items;
using Saasu.API.Core.Models.Search;
using System.Linq;
using System.Threading;
using Saasu.API.Client.IntegrationTests.Helpers;
using Saasu.API.Core.Models;

namespace Saasu.API.Client.IntegrationTests
{
    public class SearchTests
    {
        private SearchHelper _searchHelper;


        public SearchTests()
        {
            _searchHelper = new SearchHelper();
        }

        [SkipByConfigurationFact]
        public void ShouldReturnSaleTransactionForScopedSearchBySummary()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetSaleInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var results1 = searchProxy.Search(invoiceDetail.Summary, SearchScope.Transactions, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);
            
        }
        
        [SkipByConfigurationFact]
        public void ShouldReturnSaleTransactionForMatchAllSearch()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetSaleInvoiceFaker(null, DateTime.Now);
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var results1 = searchProxy.Search("*:*", SearchScope.Transactions, 1, 100);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);
            
        }

        [SkipByConfigurationFact]
        public void ShouldReturnSaleTransactionForScopedSearchByContact()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetSaleInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var results1 = searchProxy.Search($"{invoiceDetail.BillingContactFirstName} {invoiceDetail.BillingContactLastName}", SearchScope.Transactions, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);
        }

        [SkipByConfigurationFact]
        public void ShouldReturnSaleTransactionForScopedSearchByInvoiceNumber()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetSaleInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var results1 = searchProxy.Search(response.DataObject.GeneratedInvoiceNumber, SearchScope.Transactions, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);

        }

        [SkipByConfigurationFact]
        public void ShouldReturnSaleTransactionForScopedSearchByTotalAmount()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetSaleInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var results1 = searchProxy.Search(invoiceDetail.TotalAmount.Value.ToString(CultureInfo.InvariantCulture), SearchScope.Transactions, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);

        }

        [SkipByConfigurationFact]
        public void ShouldReturnSaleTransactionForScopedSearchByCompany()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetSaleInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var companyProxy = new CompanyProxy();
            var proxy = new ContactProxy();
            var contact = proxy.GetContact(invoiceDetail.BillingContactId.Value);
            var company = companyProxy.GetCompany(contact.DataObject.CompanyId.Value);

            var results1 = searchProxy.Search(company.DataObject.Name, SearchScope.Transactions, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);
        }

        [SkipByConfigurationFact]
        public void ShouldReturnPurchaseTransactionForScopedSearchBySummary()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetPurchaseInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var results1 = searchProxy.Search(invoiceDetail.Summary, SearchScope.Transactions, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);
        }

        [SkipByConfigurationFact]
        public void ShouldReturnPurchaseTransactionForScopedSearchByContact()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetPurchaseInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var results1 = searchProxy.Search($"{invoiceDetail.BillingContactFirstName} {invoiceDetail.BillingContactLastName}", SearchScope.Transactions, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);
        }


        [SkipByConfigurationFact]
        public void ShouldReturnPurchaseTransactionForScopedSearchByInvoiceNumber()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetPurchaseInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var results1 = searchProxy.Search(response.DataObject.GeneratedInvoiceNumber, SearchScope.Transactions, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);
        }



        [SkipByConfigurationFact]
        public void ShouldReturnPurchaseTransactionForScopedSearchByTotalAmount()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetPurchaseInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var results1 = searchProxy.Search(invoiceDetail.TotalAmount.Value.ToString(CultureInfo.InvariantCulture), SearchScope.Transactions, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);

        }


        [SkipByConfigurationFact]
        public void ShouldReturnPurchaseTransactionForScopedSearchByCompany()
        {
            var searchProxy = new SearchProxy();
            var faker = _searchHelper.GetPurchaseInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var companyProxy = new CompanyProxy();
            var proxy = new ContactProxy();
            var contact = proxy.GetContact(invoiceDetail.BillingContactId.Value);
            var company = companyProxy.GetCompany(contact.DataObject.CompanyId.Value);

            var results1 = searchProxy.Search(company.DataObject.Name, SearchScope.Transactions, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Transactions.Count > 0, "No transactions returned.");
            Assert.True(results1.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
            Assert.Equal(0, results1.DataObject.TotalContactsFound);
            Assert.Equal(0, results1.DataObject.TotalInventoryItemsFound);
            Assert.Contains(results1.DataObject.Transactions, t => t.Id == response.DataObject.InsertedEntityId);

        }

        [SkipByConfigurationFact]
        public void ShouldReturnContactsForScopedSearch()
        {
            var createdContactInfo = _searchHelper.CreateContact();
            Thread.Sleep(5000); // Need to wait for entities to be indexed
            
            var searchProxy = new SearchProxy();
            var results1 = searchProxy.Search($"{createdContactInfo.ContactDetail.GivenName} {createdContactInfo.ContactDetail.FamilyName}", SearchScope.Contacts, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Contacts.Count > 0, "No contacts returned.");
            Assert.True(results1.DataObject.TotalContactsFound > 0, "transaction count is 0.");
            Assert.True(results1.DataObject.TotalTransactionsFound == 0, "Should not return transactions for search scoped to Contacts");
            Assert.True(results1.DataObject.TotalInventoryItemsFound == 0, "Should not return items for search scoped to Contacts");
            Assert.Contains(results1.DataObject.Contacts, c => c.Id == createdContactInfo.ContactId);
            
            
            var results2 = searchProxy.Search(createdContactInfo.ContactDetail.EmailAddress, SearchScope.Contacts, 1, 25);
            Assert.NotNull(results2);
            Assert.True(results2.DataObject.Contacts.Count > 0, "No contacts returned.");
            Assert.True(results2.DataObject.TotalContactsFound > 0, "transaction count is 0.");
            Assert.Contains(results2.DataObject.Contacts, c => c.Id == createdContactInfo.ContactId);
            
            var companyProxy = new CompanyProxy();
            var companyResponse = companyProxy.GetCompany(createdContactInfo.ContactDetail.CompanyId.Value);
            
            var results3 = searchProxy.Search(companyResponse.DataObject.Name, SearchScope.Contacts, 1, 25);
            Assert.NotNull(results3);
            Assert.True(results3.DataObject.Contacts.Count > 0, "No contacts returned.");
            Assert.True(results3.DataObject.TotalContactsFound > 0, "transaction count is 0.");
            Assert.Contains(results3.DataObject.Contacts, c => c.Id == createdContactInfo.ContactId);

        }
        
        [SkipByConfigurationFact]
        public void ShouldReturnContactsMatchAllSearch()
        {
            var createdContactInfo = _searchHelper.CreateContact();
            Thread.Sleep(5000); // Need to wait for entities to be indexed
            
            var searchProxy = new SearchProxy();
            var results1 = searchProxy.Search("*:*", SearchScope.Contacts, 1, 25);
            Assert.NotNull(results1);
            Assert.True(results1.DataObject.Contacts.Count > 0, "No contacts returned.");
            Assert.True(results1.DataObject.TotalContactsFound > 0, "transaction count is 0.");
            Assert.True(results1.DataObject.TotalTransactionsFound == 0, "Should not return transactions for search scoped to Contacts");
            Assert.True(results1.DataObject.TotalInventoryItemsFound == 0, "Should not return items for search scoped to Contacts");
        }

        [SkipByConfigurationFact]
        public void ShouldReturnInventoryItemsForScopedSearch()
        {
            var itemInfo = _searchHelper.CreatSaleInventoryItem();
            Thread.Sleep(5000);
            
            var searchProxy = new SearchProxy();
            var result1 = searchProxy.Search(itemInfo.ItemDetail.Description, SearchScope.InventoryItems, 1, 25);
            Assert.NotNull(result1);
            Assert.NotNull(result1.DataObject);
            Assert.True(result1.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(result1.DataObject.TotalInventoryItemsFound > 0, "Inventory item count is 0.");
            Assert.True(result1.DataObject.TotalTransactionsFound == 0, "Should not return transactions for search scoped to Contacts");
            Assert.True(result1.DataObject.TotalContactsFound == 0, "Should not return contacts for search scoped to Contacts");
            Assert.Contains(result1.DataObject.InventoryItems, i => i.Id == itemInfo.ItemId);
            
            var result2 = searchProxy.Search(itemInfo.ItemDetail.Code, SearchScope.InventoryItems, 1, 25);
            Assert.NotNull(result2);
            Assert.NotNull(result2.DataObject);
            Assert.True(result2.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(result2.DataObject.TotalInventoryItemsFound > 0, "Inventory item count is 0.");
            Assert.Contains(result2.DataObject.InventoryItems, i => i.Id == itemInfo.ItemId);
            
            var result3 = searchProxy.Search(itemInfo.ItemDetail.Code, SearchScope.InventoryItems, 1, 25);
            Assert.NotNull(result3);
            Assert.NotNull(result3.DataObject);
            Assert.True(result3.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(result3.DataObject.TotalInventoryItemsFound > 0, "Inventory item count is 0.");
            Assert.Contains(result3.DataObject.InventoryItems, i => i.Id == itemInfo.ItemId);
        }

        [SkipByConfigurationFact]
        public void ShouldReturnInventoryItemsForMatchAllSearch()
        {
            var itemInfo = _searchHelper.CreatSaleInventoryItem();
            Thread.Sleep(5000);
            
            var searchProxy = new SearchProxy();
            var result1 = searchProxy.Search("*:*", SearchScope.InventoryItems, 1, 25);
            Assert.NotNull(result1);
            Assert.NotNull(result1.DataObject);
            Assert.True(result1.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(result1.DataObject.TotalInventoryItemsFound > 0, "Inventory item count is 0.");
            Assert.True(result1.DataObject.TotalTransactionsFound == 0, "Should not return transactions for search scoped to Contacts");
            Assert.True(result1.DataObject.TotalContactsFound == 0, "Should not return contacts for search scoped to Contacts");
        }
        
        [SkipByConfigurationFact]
        public void ShouldReturnContactsTransactionsInventoryItemsForScopedAllSearch()
        {
            var faker = _searchHelper.GetSaleInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);

            _searchHelper.CreatSaleInventoryItem(invoiceDetail.Summary);
            _searchHelper.CreateContact(companyName: invoiceDetail.Summary);
            Thread.Sleep(5000); // Need to wait for entities to be indexed


            var searchProxy = new SearchProxy();
            var results = searchProxy.Search(invoiceDetail.Summary, SearchScope.All, 1, 25);
            Assert.NotNull(results);
            Assert.NotNull(results.DataObject);
            Assert.True(results.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(results.DataObject.Contacts.Count > 0, "No contacts returned.");
            Assert.True(results.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(results.DataObject.TotalInventoryItemsFound > 0, "Inventory item count is 0.");
            Assert.True(results.DataObject.TotalContactsFound > 0, "transaction count is 0.");
            Assert.True(results.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
        }
        
        [SkipByConfigurationFact]
        public void ShouldReturnContactsTransactionsInventoryItemsForMatchAll()
        {
            var faker = _searchHelper.GetSaleInvoiceFaker();
            var invoiceDetail = faker.Generate();
            var response = new InvoiceProxy().InsertInvoice(invoiceDetail);
            Assert.True(response != null && response.IsSuccessfull);

            _searchHelper.CreatSaleInventoryItem(invoiceDetail.Summary);
            _searchHelper.CreateContact(companyName: invoiceDetail.Summary);
            Thread.Sleep(5000); // Need to wait for entities to be indexed


            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("*:*", SearchScope.All, 1, 25);
            Assert.NotNull(results);
            Assert.NotNull(results.DataObject);
            Assert.True(results.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(results.DataObject.Contacts.Count > 0, "No contacts returned.");
            Assert.True(results.DataObject.InventoryItems.Count > 0, "No inventory items returned.");
            Assert.True(results.DataObject.TotalInventoryItemsFound > 0, "Inventory item count is 0.");
            Assert.True(results.DataObject.TotalContactsFound > 0, "transaction count is 0.");
            Assert.True(results.DataObject.TotalTransactionsFound > 0, "transaction count is 0.");
        }

        [SkipByConfigurationFact]
        public void ShouldReturnNotResults()
        {
            var searchProxy = new SearchProxy();
            var results = searchProxy.Search("zzzzzzzzzzzzzzzzzzzzzz", SearchScope.All, 1, 25);
            Assert.NotNull(results);
            Assert.NotNull(results.DataObject);
            Assert.Equal(0, results.DataObject.InventoryItems.Count);
            Assert.Equal(0, results.DataObject.Contacts.Count);
            Assert.Equal(0, results.DataObject.InventoryItems.Count);
            Assert.Equal(0, results.DataObject.TotalInventoryItemsFound);
            Assert.Equal(0, results.DataObject.TotalContactsFound);
            Assert.Equal(0, results.DataObject.TotalTransactionsFound);
        }

//        [SkipByConfigurationFact]
//        public void IndexedTransactionShouldMatchEntityData()
//        {
//            var searchProxy = new SearchProxy();
//            var invoice = new InvoiceProxy().GetInvoice(_invoiceHelper.InvoiceId1.Value).DataObject;
//            Assert.NotNull(invoice.BillingContactId);
//            var contact = new ContactProxy().GetContact(invoice.BillingContactId.Value).DataObject;
//
//            var searchResults = searchProxy.Search(_invoiceHelper.InvoiceId1Summary, SearchScope.Transactions, 1, 100);
//            Assert.NotNull(searchResults.DataObject);
//            Assert.True(searchResults.DataObject.Transactions.Count > 0);
//            var indexedTransaction = searchResults.DataObject.Transactions.First(x => x.Id == invoice.TransactionId);
//
//            Assert.Equal(invoice.DueDate, indexedTransaction.DueDate);
//            Assert.Equal(invoice.BillingContactOrganisationName, indexedTransaction.Company);
//            Assert.Equal(contact.EmailAddress, ReplaceEm(indexedTransaction.ContactEmail));
//            Assert.Equal(invoice.InvoiceNumber, indexedTransaction.InvoiceNumber);
//            Assert.Equal(invoice.PurchaseOrderNumber, indexedTransaction.PurchaseOrderNumber);
//            Assert.Equal(invoice.NotesExternal, indexedTransaction.ExternalNotes);
//            Assert.Equal(invoice.NotesInternal, indexedTransaction.Notes);
//            Assert.Equal(invoice.TransactionType, indexedTransaction.Type);            
//            Assert.Equal(invoice.InvoiceType, indexedTransaction.InvoiceType);
//        }

        [SkipByConfigurationFact]
        public void IndexedContactShouldMatchEntityData()
        {
            var contactInfo = _searchHelper.CreateContact();
            Thread.Sleep(5000); // Need to wait for entities to be indexed

            var searchProxy = new SearchProxy();

            var contact = new ContactProxy().GetContact(contactInfo.ContactId).DataObject;

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

        [SkipByConfigurationFact]
        public void IndexedInventoryItemShouldMatchEntityData()
        {
            var searchProxy = new SearchProxy();

            var itemInfo = _searchHelper.CreatSaleInventoryItem();
            Thread.Sleep(5000);

            var searchResults = searchProxy.Search(itemInfo.ItemDetail.Code, SearchScope.InventoryItems, 1, 10);
            Assert.NotNull(searchResults.DataObject);
            Assert.True(searchResults.DataObject.InventoryItems.Count > 0);

            var indexedInventoryItem = searchResults.DataObject.InventoryItems.First(x => ReplaceEm(x.Code) == itemInfo.ItemDetail.Code);
            Assert.NotNull(indexedInventoryItem);
            Assert.Equal(itemInfo.ItemDetail.BuyingPrice, indexedInventoryItem.BuyingPrice);
            Assert.Equal(itemInfo.ItemDetail.Description, ReplaceEm(indexedInventoryItem.Description));
            Assert.Equal(itemInfo.ItemDetail.Type, indexedInventoryItem.Type);
            Assert.Equal("Item", indexedInventoryItem.EntityType);
            Assert.Equal(itemInfo.ItemId, indexedInventoryItem.Id);
            Assert.Equal(itemInfo.ItemDetail.SellingPrice, indexedInventoryItem.SellingPrice);
            Assert.Equal(itemInfo.ItemDetail.StockOnHand.HasValue ? itemInfo.ItemDetail.StockOnHand : 0M, indexedInventoryItem.StockOnHand);
            Assert.Equal(itemInfo.ItemDetail.PrimarySupplierItemCode, indexedInventoryItem.SupplierItemCode);
        }

        [SkipByConfigurationFact]
        public void ShouldReturnSalesOnlyWhenTransactionEntityTypeSpecified()
        {
            var invoiceProxy = new InvoiceProxy();
            var saleInfo = _searchHelper.GetSaleInvoiceFaker().Generate();
            var saleResponse = invoiceProxy.InsertInvoice(saleInfo);
            Assert.True(saleResponse != null && saleResponse.IsSuccessfull);

            var purchaseInfo = _searchHelper.GetPurchaseInvoiceFaker(saleInfo.Summary).Generate();
            var purchaseResponse = invoiceProxy.InsertInvoice(purchaseInfo);
            Assert.True(purchaseResponse != null && purchaseResponse.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed


            var searchProxy = new SearchProxy();
            var results = searchProxy.Search(saleInfo.Summary, SearchScope.Transactions, 1, 25, "transactions.sale");
            Assert.NotNull(results);
            Assert.NotNull(results.DataObject);
            Assert.True(results.DataObject.Transactions.TrueForAll(x => x.Type == "S"));
        }

        [SkipByConfigurationFact]
        public void ShouldReturnPurchasesOnlyWhenTransactionEntityTypeSpecified()
        {
            var invoiceProxy = new InvoiceProxy();
            var saleInfo = _searchHelper.GetSaleInvoiceFaker().Generate();
            var saleResponse = invoiceProxy.InsertInvoice(saleInfo);
            Assert.True(saleResponse != null && saleResponse.IsSuccessfull);

            var purchaseInfo = _searchHelper.GetPurchaseInvoiceFaker(saleInfo.Summary).Generate();
            var purchaseResponse = invoiceProxy.InsertInvoice(purchaseInfo);
            Assert.True(purchaseResponse != null && purchaseResponse.IsSuccessfull);
            Thread.Sleep(5000); // Need to wait for entities to be indexed


            var searchProxy = new SearchProxy();
            var results = searchProxy.Search(purchaseInfo.Summary, SearchScope.Transactions, 1, 25, "transactions.purchase");
            Assert.NotNull(results);
            Assert.NotNull(results.DataObject);
            Assert.True(results.DataObject.Transactions.TrueForAll(x => x.Type == "P"));
        }


        private static string ReplaceEm(string someString)
        {
            return someString.Replace("<em>", "").Replace("</em>", "");
        }

    }
}
