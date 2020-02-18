using Xunit;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Accounts;
using Saasu.API.Core.Models.Journals;
using Saasu.API.Core.Models.Payments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Saasu.API.Client.IntegrationTests
{
    public class DeletedEntityTests
    {
        private int _DeletedContactId;
        private int _NotDeletedContactId;

        private int _DeletedItemId;
        private int _NotDeletedItemId;

        private int _DeletedSaleTranId;
        private int _NotDeletedSaleTranId;

        private int _DeletedPurchaseTranId;
        private int _NotDeletedPurchaseTranId;

        private int _DeletedSalePaymentTranId;
        private int _NotDeletedSalePaymentTranId;

        private int _DeletedPurchasePaymentTranId;
        private int _NotDeletedPurchasePaymentTranId;

        private int _DeletedJournalTranId;
        private int _NotDeletedJournalTranId;

        private int _assetAccountId;
        private int _liabilityAccountId;

        private string _testIdentifier;

        private List<int> _allDeleted;
        private List<int> _allNotDeleted;

        private const int _entityTypeSale = 4;
        private const int _entityTypePurchase = 7;
        private const int _entityTypeSalePayment = 5;
        private const int _entityTypePurchasePayment = 8;
        private const int _entityTypeContact = 10;
        private const int _entityTypeItem = 20;
        private const int _entityTypeJournal = 26;


        public DeletedEntityTests()
        {
            _testIdentifier = Guid.NewGuid().ToString().Substring(0, 5);

            CreateContacts();
            CreateInvoices();
            CreatePayments();
            CreateItems();
            CreateJournals();

            _allDeleted = new List<int> { _DeletedContactId, _DeletedItemId, _DeletedSaleTranId, _DeletedPurchaseTranId, _DeletedSalePaymentTranId, _DeletedPurchasePaymentTranId, _DeletedJournalTranId };
            _allNotDeleted = new List<int> { _NotDeletedContactId, _NotDeletedItemId, _NotDeletedSaleTranId, _NotDeletedPurchaseTranId, _NotDeletedSalePaymentTranId, _NotDeletedPurchasePaymentTranId, _NotDeletedJournalTranId };
        }

        #region Tests

        [Fact]
        public void GetAllNoFilters()
        {
            var result = new DeletedEntitiesProxy().GetDeletedEntities();
            Assert.True(result.DataObject != null, "Error with response object");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection");

            Assert.Null(result.DataObject.Entities.SingleOrDefault(e => _allNotDeleted.Contains(e.EntityId)));
        }

        [Fact]
        public void FilterOnEntityTypeSale()
        {
            var result = new DeletedEntitiesProxy().GetDeletedEntities(entityType: "Sale");
            Assert.True(result.DataObject != null, "Error with response object");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection");
            Assert.Null(result.DataObject.Entities.SingleOrDefault(e => e.EntityType != "Sale"));
        }

        [Fact]
        public void FilterOnEntityTypePurchase()
        {
            var result = new DeletedEntitiesProxy().GetDeletedEntities(entityType: "Purchase");
            Assert.True(result.DataObject != null, "Error with response object");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection");
            Assert.Null(result.DataObject.Entities.SingleOrDefault(e => e.EntityType != "Purchase"));
        }

        [Fact]
        public void FilterOnEntityTypeSalePayment()
        {
            var result = new DeletedEntitiesProxy().GetDeletedEntities(entityType: "SalePayment");
            Assert.True(result.DataObject != null, "Error with response object");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection");
            Assert.Null(result.DataObject.Entities.SingleOrDefault(e => e.EntityType != "SalePayment"));
        }

        [Fact]
        public void FilterOnEntityTypePurchasePayment()
        {
            var result = new DeletedEntitiesProxy().GetDeletedEntities(entityType: "PurchasePayment");
            Assert.True(result.DataObject != null, "Error with response object");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection");
            Assert.Null(result.DataObject.Entities.SingleOrDefault(e => e.EntityType != "PurchasePayment"));
        }


        [Fact]
        public void FilterOnEntityTypeContact()
        {
            var result = new DeletedEntitiesProxy().GetDeletedEntities(entityType: "Contact");
            Assert.True(result.DataObject != null, "Error with response object");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection");
            Assert.Null(result.DataObject.Entities.SingleOrDefault(e => e.EntityType != "Contact"));
        }

        [Fact]
        public void FilterOnEntityTypeInventoryItem()
        {
            var result = new DeletedEntitiesProxy().GetDeletedEntities(entityType: "Item");
            Assert.True(result.DataObject != null, "Error with response object");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection");
            Assert.Null(result.DataObject.Entities.SingleOrDefault(e => e.EntityType != "Item"));
        }

        [Fact]
        public void FilterOnEntityTypeJournal()
        {
            var result = new DeletedEntitiesProxy().GetDeletedEntities(entityType: "Journal");
            Assert.True(result.DataObject != null, "Error with response object");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection");
            Assert.Null(result.DataObject.Entities.SingleOrDefault(e => e.EntityType != "Journal"));
        }

        [Fact]
        public void FilterOnInvalidEntityTypeJournal()
        {
            var result = new DeletedEntitiesProxy().GetDeletedEntities(entityType: "aaaa");

            Assert.False(result.IsSuccessfull, "Filter on invalid entity type should not have succeded");
            Assert.Null(result.DataObject);
        }

        [Fact]
        public void FilterOnUtcDeletedDateTime()
        {
            var fromDate = DateTime.UtcNow.AddHours(-1);
            var toDate = DateTime.UtcNow;
            var result = new DeletedEntitiesProxy().GetDeletedEntities(utcDeletedFromDate: fromDate, utcDeletedToDate: toDate);
            Assert.True(result.DataObject != null, "Error with response object");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection");
            Assert.Null(result.DataObject.Entities.SingleOrDefault(e => e.Timestamp < fromDate || e.Timestamp > toDate));
        }

        [Fact]
        public void TestPaging()
        {
            var proxy = new DeletedEntitiesProxy();
            var result = proxy.GetDeletedEntities(pageNumber: 1, pageSize: 3);
            Assert.True(result.DataObject != null, "Error with response object");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection");
            Assert.True(result.DataObject.Entities.Count == 3);

            var idsFromResult = new List<int>
            {
                result.DataObject.Entities[0].EntityId,
                result.DataObject.Entities[1].EntityId,
                result.DataObject.Entities[2].EntityId
            };

            result = proxy.GetDeletedEntities(pageNumber: 2, pageSize: 3);
            Assert.True(result.DataObject != null, "Error with response object, second request");
            Assert.True(result.DataObject.Entities != null, "Error with response object entity collection, second request");

            result.DataObject.Entities.ForEach(e => Assert.False(idsFromResult.Contains(e.EntityId), "Entity from first page also contained in second"));
        }



        #endregion



        private void CreateContacts()
        {
            var helper = new ContactHelper();

            //Create contacts.
            _DeletedContactId = helper.AddContact().InsertedContactId;
            _NotDeletedContactId = helper.AddContact().InsertedContactId;

            //Delete only fist contact.
            new ContactProxy().DeleteContact(_DeletedContactId);
        }

        private void CreateInvoices()
        {
            var proxy = new InvoiceProxy();
            var helper = new InvoiceHelper();

            helper.CreateTestData();

            //Create sales.
            _DeletedSaleTranId = (int)helper.CreateASingleInvoice(amount: 20.00M).TransactionId;
            _NotDeletedSaleTranId = (int)helper.CreateASingleInvoice(amount: 20.00M).TransactionId;

            //Delete only first sale.
            proxy.DeleteInvoice(_DeletedSaleTranId);

            //Create purchases.
            _DeletedPurchaseTranId = (int)helper.CreateASingleInvoice("P", 20.00M).TransactionId;
            _NotDeletedPurchaseTranId = (int)helper.CreateASingleInvoice("P", 20.00M).TransactionId;

            //Delete only first purchases.
            proxy.DeleteInvoice(_DeletedPurchaseTranId);
        }

        private void CreatePayments()
        {
            CreateAccounts();

            var proxy = new PaymentProxy();

            var salePayment1 = new PaymentTransaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = "SP",
                Currency = "AUD",
                Summary = string.Format("Test deleted entities_{0}", _testIdentifier),
                PaymentAccountId = _assetAccountId,
                PaymentItems = new List<PaymentItem>
                                    {
                                        new PaymentItem()
                                        {
                                            InvoiceTransactionId =_NotDeletedSaleTranId,
                                            AmountPaid = 10.00M
                                        }
                                    }
            };

            var salePayment2 = new PaymentTransaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = "SP",
                Currency = "AUD",
                Summary = string.Format("Test deleted entities_{0}", _testIdentifier),
                PaymentAccountId = _assetAccountId,
                PaymentItems = new List<PaymentItem>
                                    {
                                        new PaymentItem()
                                        {
                                            InvoiceTransactionId =_NotDeletedSaleTranId,
                                            AmountPaid = 10.00M
                                        }
                                    }
            };

            var purchasePayment1 = new PaymentTransaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = "PP",
                Currency = "AUD",
                Summary = string.Format("Test deleted entities_{0}", _testIdentifier),
                PaymentAccountId = _liabilityAccountId,
                PaymentItems = new List<PaymentItem>
                                    {
                                        new PaymentItem()
                                        {
                                            InvoiceTransactionId = _NotDeletedPurchaseTranId,
                                            AmountPaid = 10.00M
                                        }
                                    }
            };

            var purchasePayment2 = new PaymentTransaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = "PP",
                Currency = "AUD",
                Summary = string.Format("Test deleted entities_{0}", _testIdentifier),
                PaymentAccountId = _liabilityAccountId,
                PaymentItems = new List<PaymentItem>
                                    {
                                        new PaymentItem()
                                        {
                                            InvoiceTransactionId = _NotDeletedPurchaseTranId,
                                            AmountPaid = 10.00M
                                        }
                                    }
            };

            //Create payments.
            _DeletedSalePaymentTranId = proxy.InsertInvoicePayment(salePayment1).DataObject.InsertedEntityId;
            _NotDeletedSalePaymentTranId = proxy.InsertInvoicePayment(salePayment2).DataObject.InsertedEntityId;
            _DeletedPurchasePaymentTranId = proxy.InsertInvoicePayment(purchasePayment1).DataObject.InsertedEntityId;
            _NotDeletedPurchasePaymentTranId = proxy.InsertInvoicePayment(purchasePayment2).DataObject.InsertedEntityId;

            //Delete only first of each.
            proxy.DeleteInvoicePayment(_DeletedSalePaymentTranId);
            proxy.DeleteInvoicePayment(_DeletedPurchasePaymentTranId);
        }

        private void CreateAccounts()
        {
            var incomeAccount = new AccountDetail
            {
                Name = string.Format("TestAccount_{0}", _testIdentifier),
                AccountType = "Asset",
                IsActive = true,
                LedgerCode = "AA",
                Currency = "AUD",
                IsBankAccount = true
            };

            var expenseAccount = new AccountDetail
            {
                Name = string.Format("TestAccount_{0}", _testIdentifier),
                AccountType = "Liability",
                IsActive = true,
                LedgerCode = "AA",
                Currency = "AUD",
                IsBankAccount = true
            };

            //Create accounts.
            _assetAccountId = new AccountProxy().InsertAccount(incomeAccount).DataObject.InsertedEntityId;
            _liabilityAccountId = new AccountProxy().InsertAccount(expenseAccount).DataObject.InsertedEntityId;
        }

        private void CreateJournals()
        {
            var proxy = new JournalProxy();

            var journal1 = new JournalDetail
            {
                TransactionDate = DateTime.Now,
                Summary = string.Format("test Summary_{0}", _testIdentifier),
                Currency = "AUD",
                Items = new List<JournalItem>
                {
                    new JournalItem
                    {
                        AccountId = _assetAccountId,
                        Type = "debit",
                        Amount = 10.00M
                    },
                    new JournalItem
                    {
                        AccountId = _liabilityAccountId,
                        Type = "credit",
                        Amount = 10.00M
                    }

                }
            };

            var journal2 = new JournalDetail
            {
                TransactionDate = DateTime.Now,
                Summary = string.Format("test Summary_{0}", _testIdentifier),
                Currency = "AUD",
                Items = new List<JournalItem>
                {
                    new JournalItem
                    {
                        AccountId = _assetAccountId,
                        Type = "debit",
                        Amount = 10.00M
                    },
                    new JournalItem
                    {
                        AccountId = _liabilityAccountId,
                        Type = "credit",
                        Amount = 10.00M
                    }

                }
            };

            //Create journals.
            _DeletedJournalTranId = proxy.InsertJournal(journal1).DataObject.InsertedEntityId;
            _NotDeletedJournalTranId = proxy.InsertJournal(journal2).DataObject.InsertedEntityId;

            //Delete only first journal.
            proxy.DeleteJournal(_DeletedJournalTranId);
        }

        private void CreateItems()
        {
            var proxy = new ItemProxy();
            var helper = new ItemHelper();

            var inventoryItem1 = helper.GetTestInventoryItem();
            var inventoryItem2 = helper.GetTestInventoryItem();

            //Create items.
            var result = proxy.InsertItem(inventoryItem1);
            _DeletedItemId = result.DataObject.InsertedItemId;

            result = proxy.InsertItem(inventoryItem2);
            _NotDeletedItemId = result.DataObject.InsertedItemId;

            //Delete only first item.
            proxy.DeleteItem(_DeletedItemId);
        }
    }
}
