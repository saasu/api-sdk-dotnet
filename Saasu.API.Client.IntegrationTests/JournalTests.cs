using Xunit;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Models.Contacts;
using Saasu.API.Core.Models.Journals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Saasu.API.Client.IntegrationTests
{
    public class JournalTests
    {
        private int _assetAccountId;
        private int _expenseAccountId;

        private int _testJournalTranId1; //Transaction Date today.
        private int _testJournalTranId2; //Transaction Date 5 days ago.
        private int _testJournalTranId3; //Transaction Date 10 days ago.

        private int _contactId1;
        private int _contactId2;

        public JournalTests()
        {
            //note - don't change the order of these calls, they are dependent.
            GetTestContacts();
            GetTestAccounts();
            CreateTestJournals();
        }

        [Fact]
        public void GetJournalsAll()
        {
            AssertJournalsProxy();
        }

        [Fact]
        public void GetJournalsOnePageOneRecord()
        {
            var response = AssertJournalsProxy(pageNumber: 1, pageSize: 1);
            Assert.Single(response.DataObject.Journals);
        }

        [Fact]
        public void GetJournalsFilterOnDates()
        {
            var result = AssertJournalsProxy(fromDate: DateTime.Now.AddDays(-4), toDate: DateTime.Now.AddDays(1));

            //verify test journal with tran date 5 days in the passed doesn't get picked up.
            var journalsFrom5DaysAgo = result.DataObject.Journals.Where(j => j.TransactionId == _testJournalTranId2).ToList();
           
            Assert.Empty(journalsFrom5DaysAgo);
        }

        [Fact]
        public void GetJournalsFilterOnModifiedDates()
        {
            var lastModifiedFromDate = DateTime.Now.AddDays(-1);
            var lastModififedToDate = DateTime.Now;

            var result = AssertJournalsProxy(lastModifiedFromDate: lastModifiedFromDate, lastModifiedToDate: lastModififedToDate);
            //make sure at least the 2 test journals are picked up.
            Assert.True(result.DataObject.Journals.Count > 1);

            var outofRangeJournals = result.DataObject.Journals.Where(j => j.LastModifiedDateUtc < lastModifiedFromDate).ToList();

            Assert.Empty(outofRangeJournals);
        }

        [Fact]
        public void GetJournalsFilterOnContactId()
        {
            var result = AssertJournalsProxy(contactId: _contactId1);
            var journalsWithWrongContactId = result.DataObject.Journals.Where(j => j.JournalContactId == _contactId2).ToList();
            Assert.Empty(journalsWithWrongContactId);

            //make sure test transaction 2 is not picked up as it has a different contact.
            journalsWithWrongContactId = result.DataObject.Journals.Where(j => j.TransactionId == _testJournalTranId2).ToList();
            Assert.Empty(journalsWithWrongContactId);
        }

        [Fact]
        public void GetSingleJournalWithJournalId()
        {
            var response = new JournalProxy().GetJournal(_testJournalTranId1);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.NotNull(response.DataObject._links);
            Assert.True(response.DataObject._links.Count > 0);
        }

        [Fact]
        public void GetJournalsPageSize()
        {
            var proxy = new JournalsProxy();
            var response = proxy.GetJournals(pageNumber: 1, pageSize: 2);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.Equal(response.DataObject.Journals.Count, 2);
        }

        [Fact]
        public void GetJournalsSecondPage()
        {
            var proxy = new JournalsProxy();
            var response = proxy.GetJournals(pageSize: 2);

            var idsFromPage1 = response.DataObject.Journals.Select(i => i.TransactionId).ToList();

            response = proxy.GetJournals(pageNumber: 2);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);

            response.DataObject.Journals.ForEach(i => Assert.DoesNotContain(i.TransactionId, idsFromPage1));
        }

        public static ProxyResponse<JournalTransactionSummaryResponse> AssertJournalsProxy(int? pageNumber = null, int? pageSize = null, DateTime? fromDate = null, DateTime? toDate = null,
            DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null, int? contactId = null, string tags = null, string tagFilterType = null)
        {
            var response = new JournalsProxy().GetJournals(pageNumber, pageSize, fromDate, toDate, lastModifiedFromDate, lastModifiedToDate, contactId, tags, tagFilterType);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull, "Journal response not successfull");
            Assert.NotNull(response.DataObject);
            Assert.NotNull(response.DataObject.Journals);
            Assert.NotNull(response.DataObject._links);
            Assert.True(response.DataObject._links.Count > 0, "No hypermedia links in response");
            Assert.True(response.DataObject.Journals.Count > 0, "No journals in response");

            return response;
        }

        private JournalDetail GetJournalTransaction(DateTime? date = null, string summary = null, string currency = null, string reference = null, int? contactId = null, List<string> tags = null, decimal? fxRate = null, bool autoPopulateFx = false, bool useDefaults = false)
        {

            if (useDefaults)
            {
                return new JournalDetail
                {
                    TransactionDate = date ?? DateTime.Now,
                    Summary = summary ?? "test Summary",
                    Currency = currency ?? "AUD",
                    Reference = reference ?? "123",
                    JournalContactId = contactId ?? _contactId1,
                    Tags = tags ?? null,
                    FxRate = fxRate ?? 1M,
                    AutoPopulateFxRate = autoPopulateFx
                };
            }

            return new JournalDetail
            {
                TransactionDate = date,
                Summary = summary,
                Currency = currency,
                Reference = reference,
                JournalContactId = contactId,
                Tags = tags,
                FxRate = fxRate,
                AutoPopulateFxRate = autoPopulateFx
            };
        }

        private void GetTestContacts()
        {
            var contactsProxy = new ContactsProxy();
            var contactProxy = new ContactProxy();

            var response = contactsProxy.GetContacts(givenName: "Test1", familyName: "APIJournal");

            if (response.DataObject == null || response.DataObject.Contacts == null || response.DataObject.Contacts.Count == 0)
            {
                var contact = new Contact
                {
                    GivenName = "Test1",
                    FamilyName = "APIJournal"
                };

               var insertResult = contactProxy.InsertContact(contact);
                _contactId1 = insertResult.DataObject.InsertedContactId;
            }
            else
            {
                _contactId1 = response.DataObject.Contacts[0].Id.Value;
            }

            response = contactsProxy.GetContacts(givenName: "Test2", familyName: "APIJournal");

            if (response.DataObject == null || response.DataObject.Contacts == null || response.DataObject.Contacts.Count == 0)
            {
                var contact = new Contact
                {
                    GivenName = "Test2",
                    FamilyName = "APIJournal"
                };

                var insertResult = contactProxy.InsertContact(contact);
                _contactId2 = insertResult.DataObject.InsertedContactId;
            }
            else
            {
                _contactId2 = response.DataObject.Contacts[0].Id.Value;
            }
        }

        private void GetTestAccounts()
        {
            var assetAccountResponse = new AccountsProxy().GetAccounts(1, 25, true, false, "Asset", null, null);
            Assert.True(assetAccountResponse.DataObject.Accounts.Count > 0, "You have to have at least one Asset account to run these tests.");
            _assetAccountId = (int)assetAccountResponse.DataObject.Accounts[0].Id;

            var expenseAccountResponse = new AccountsProxy().GetAccounts(1, 25, true, false, "Expense", null, null);
            Assert.True(expenseAccountResponse.DataObject.Accounts.Count > 0, "You have to have at least one Expense account to run these tests.");
            _expenseAccountId = (int)expenseAccountResponse.DataObject.Accounts[0].Id;
        }

        private void CreateTestJournals()
        {
            var tran1 = new JournalDetail
            {
                TransactionDate = DateTime.UtcNow,
                Items = new List<JournalItem>
                {
                    new JournalItem
                    {
                        Type = "Debit",
                        AccountId = _assetAccountId,
                        TaxCode = "G1",
                        Amount = 10.00M
                    },
                     new JournalItem
                    {
                        Type = "Credit",
                        AccountId = _expenseAccountId,
                        TaxCode = "G1",
                        Amount = 10.00M
                    }
                },
                Summary = "test journal 1",
                Currency = "AUD",
                Reference = "123",
                JournalContactId = _contactId1,
                Tags = new List<string> { "test tag 1, test tag 2" }
            };

            var tran2 = new JournalDetail
            {
                TransactionDate = DateTime.UtcNow.AddDays(-5),
                Items = new List<JournalItem>
                {
                    new JournalItem
                    {
                        Type = "Debit",
                        AccountId = _assetAccountId,
                        TaxCode = "G1,G2",
                        Amount = 20.00M
                    },
                     new JournalItem
                    {
                        Type = "Credit",
                        AccountId = _expenseAccountId,
                        TaxCode = "G1",
                        Amount = 20.00M
                    }
                },
                Summary = "test journal 2",
                Currency = "AUD",
                Reference = "456",
                JournalContactId = _contactId2,
                Tags = new List<string> { "test tag 1, test tag 2" }
            };

            var tran3 = new JournalDetail
            {
                TransactionDate = DateTime.UtcNow.AddDays(-10),
                Items = new List<JournalItem>
                {
                    new JournalItem
                    {
                        Type = "Debit",
                        AccountId = _assetAccountId,
                        TaxCode = "G1,G2",
                        Amount = 20.00M
                    },
                     new JournalItem
                    {
                        Type = "Credit",
                        AccountId = _expenseAccountId,
                        TaxCode = "G1",
                        Amount = 20.00M
                    }
                },
                Summary = "test journal 3",
                Currency = "AUD",
                Reference = "456",
                JournalContactId = _contactId2,
                Tags = new List<string> { "test tag 1, test tag 2" }
            };

            _testJournalTranId1 = new JournalProxy().InsertJournal(tran1).DataObject.InsertedEntityId;
            _testJournalTranId2 = new JournalProxy().InsertJournal(tran2).DataObject.InsertedEntityId;
            _testJournalTranId3 = new JournalProxy().InsertJournal(tran2).DataObject.InsertedEntityId;
        }
    }
}
