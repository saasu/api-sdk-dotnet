using Xunit;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Activities;
using Saasu.API.Core.Models.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Saasu.API.Client.IntegrationTests
{
    public class ActivityTests
    {
        private int _saleInvoiceTranId;
        private int _saleInvoiceTranId2;
        private string _testTag = "testActivitiesTag";
        private int _testActivityId1;
        private int _testActivityId2;
        private int _testActivityToBeDeletedId;
        private int _testActivityToBeUpdated;

        public ActivityTests()
        {
            CreateTestSaleInvoices();
            CreateTestActivities();
        }

        #region Tests

        [Fact]
        public void GetActivities()
        {
            var proxy = new ActivitiesProxy();
            var response = proxy.GetActivities();

            Assert.True(response.IsSuccessfull, "Call to GetActivities was not successful");
            Assert.NotNull(response.DataObject);
            Assert.True(response.DataObject.Activities.Count > 1, "Call to GetActivities returned too few rows");
        }

        [Fact]
        public void GetActivitiesFilterOnType()
        {
            var proxy = new ActivitiesProxy();
            var response = proxy.GetActivities(activityType: "Task");

            Assert.True(response.IsSuccessfull, "Call to GetActivities was not successful");
            Assert.NotNull(response.DataObject);
            Assert.Null(response.DataObject.Activities.FirstOrDefault(a => a.ActivityType != "Task"));
        }

        [Fact]
        public void GetActivitiesFilterOnStatus()
        {
            var proxy = new ActivitiesProxy();
            var response = proxy.GetActivities(activityStatus: "done");

            Assert.True(response.IsSuccessfull, "Call to GetActivitiesFilterOnStatus was not successful");
            Assert.NotNull(response.DataObject);
            Assert.Null(response.DataObject.Activities.FirstOrDefault(a => !a.Done));
        }

        [Fact]
        public void GetActivitiesFilterOwnerEmail()
        {
            var proxy = new ActivitiesProxy();
            var response = proxy.GetActivities(ownerEmail: TestConfig.TestUser);

            Assert.True(response.IsSuccessfull, "Call to GetActivitiesFilterOwnerEmail was not successful");
            Assert.NotNull(response.DataObject);
            Assert.Null(response.DataObject.Activities.FirstOrDefault(a => a.OwnerEmail != TestConfig.TestUser));
        }

        [Fact]
        public void GetActivitiesFilterAttachedToTypeAndAttachedToId()
        {
            var proxy = new ActivitiesProxy();
            var response = proxy.GetActivities(attachedToType: "sale", attachedToId: _saleInvoiceTranId2);

            Assert.True(response.IsSuccessfull, "Call to GetActivitiesFilterAttachedToTypeAndAttachedToId was not successful");
            Assert.NotNull(response.DataObject);
            Assert.Null(response.DataObject.Activities.FirstOrDefault(a => a.AttachedToType.ToLower() != "sale" && a.AttachedToId != _saleInvoiceTranId2));
        }

        [Fact]
        public void GetActivitiesFilterFromAndToDate()
        {
            var proxy = new ActivitiesProxy();
            var response = proxy.GetActivities(fromDate: DateTime.Now.AddDays(-1), toDate: DateTime.Now.AddDays(-1));

            Assert.True(response.IsSuccessfull, "Call to GetActivitiesFilterFromAndToDate was not successful");
            Assert.NotNull(response.DataObject);
            Assert.Null(response.DataObject.Activities.FirstOrDefault(a => a.Due != DateTime.Now.AddDays(-1)));
        }

        [Fact]
        public void GetActivitiesFilterOnTagsRequireAll()
        {
            var proxy = new ActivitiesProxy();
            var response = proxy.GetActivities(tags: "red, blue", tagFilterType: "requireAll");

            Assert.True(response.IsSuccessfull, "Call to GetActivitiesFilterOnTagsRequireAny was not successful");
            Assert.NotNull(response.DataObject);
            Assert.Null(response.DataObject.Activities.FirstOrDefault(a => !a.Tags.Contains("red") || !a.Tags.Contains("blue")));
        }

        [Fact]
        public void GetActivitiesFilterOnTagsRequireAny()
        {
            var proxy = new ActivitiesProxy();
            var response = proxy.GetActivities(tags: "red, blue", tagFilterType: "requireAny");

            Assert.True(response.IsSuccessfull, "Call to GetActivitiesFilterOnTagsRequireAny was not successful");
            Assert.NotNull(response.DataObject);
            Assert.Null(response.DataObject.Activities.FirstOrDefault(a => !a.Tags.Contains("red") && !a.Tags.Contains("blue")));
        }

        [Fact]
        public void GetActivitiesFilterOnTagsExcludeAll()
        {
            var proxy = new ActivitiesProxy();
            var response = proxy.GetActivities(tags: "red, blue", tagFilterType: "excludeAll");

            Assert.True(response.IsSuccessfull, "Call to GetActivitiesFilterOnTagsExcludeAll was not successful");
            Assert.NotNull(response.DataObject);
            Assert.Null(response.DataObject.Activities.FirstOrDefault(a => a.Tags.Contains("red") && a.Tags.Contains("blue")));
        }

        [Fact]
        public void GetActivitiesFilterOnTagsExcludeAny()
        {
            var proxy = new ActivitiesProxy();
            var response = proxy.GetActivities(tags: "red, blue", tagFilterType: "excludeAny");

            Assert.True(response.IsSuccessfull, "Call to GetActivitiesFilterOnTagsExcludeAny was not successful");
            Assert.NotNull(response.DataObject);
            Assert.Null(response.DataObject.Activities.FirstOrDefault(a => a.Tags.Contains("red") || a.Tags.Contains("blue")));
        }

        [Fact]
        public void GetActivity()
        {
            var proxy = new ActivityProxy();
            var response = proxy.GetActivity(_testActivityId1);
            Assert.True(response.IsSuccessfull, "Call to GetActivity was not successful");
            Assert.NotNull(response.DataObject);
            Assert.Equal(_testActivityId1, response.DataObject.Id);
        }

        [Fact]
        public void InsertActivity()
        {
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 5);
            var title = "Test activity " + uniqueId;
            var details = "Details of test activity " + uniqueId;
            var tag = "tag_" + uniqueId;

            var activity = GetActivityDetail(title: title, details: details, tags: tag, ownerEmail: TestConfig.TestUser, done: true, due: DateTime.Now.AddDays(10));
            var proxy = new ActivityProxy();
            var response = proxy.InsertActivity(activity);
            Assert.True(response.IsSuccessfull, "Call to InsertActivity was not successful");
            Assert.NotNull(response.DataObject);
            Assert.True(response.DataObject.InsertedEntityId > 0, "Call to InsertActivity returned 0 entity Id");

            var activityGet = proxy.GetActivity(response.DataObject.InsertedEntityId);
            Assert.True(activityGet.DataObject != null, "No row returned from Get after insert.");

            VerifyDetailsAreSame(activity, activityGet.DataObject);
            Assert.True(activityGet.DataObject.CreatedDateUtc > DateTime.UtcNow.AddHours(-1) && activityGet.DataObject.CreatedDateUtc < DateTime.UtcNow.AddHours(1));
            Assert.True(activityGet.DataObject.LastModifiedDateUtc > DateTime.UtcNow.AddHours(-1) && activityGet.DataObject.LastModifiedDateUtc < DateTime.UtcNow.AddHours(1));
        }

        [Fact]
        public void UpdateActivity()
        {
            var proxy = new ActivityProxy();
            var activity = proxy.GetActivity(_testActivityToBeUpdated);

            var uniqueId = Guid.NewGuid().ToString().Substring(0, 5);
            var title = "Test activity " + uniqueId;
            var details = "Details of test activity " + uniqueId;
            var done = !activity.DataObject.Done;
            var due = activity.DataObject.Due.AddDays(1);
            var ownerEmail = string.IsNullOrWhiteSpace(activity.DataObject.OwnerEmail) ? TestConfig.TestUser : null;
            var attachedToType = string.IsNullOrWhiteSpace(activity.DataObject.AttachedToType) ? null : "Sale";
            int? attachedToId = null;
            if (activity.DataObject.AttachedToId != null && activity.DataObject.AttachedToId > 0)
            {
                attachedToId = _saleInvoiceTranId;
            }

            var updateActivty = new ActivityDetail
            {
                Title = title,
                Details = details,
                Done = done,
                Due = due,
                OwnerEmail = ownerEmail,
                AttachedToType = attachedToType,
                AttachedToId = attachedToId,
                LastUpdatedId = activity.DataObject.LastUpdatedId
            };

            var updateResponse = proxy.UpdateActivity(_testActivityToBeUpdated, updateActivty);

            Assert.True(updateResponse.IsSuccessfull, "Call to UpdateActivity was not successful");
            Assert.True(updateResponse.DataObject!= null, "Call to InsertActivity returned no data");
            Assert.Equal(_testActivityToBeUpdated, updateResponse.DataObject.UpdatedActivityId);

            var activityGet = proxy.GetActivity(_testActivityToBeUpdated);
            Assert.True(activityGet.DataObject != null, "No row returned from Get after update.");

            VerifyDetailsAreSame(updateActivty, activityGet.DataObject);
        }

        [Fact]
        public void DeleteActivity()
        {
            var proxy = new ActivityProxy();
            var response = proxy.DeleteActivity(_testActivityToBeDeletedId);

            Assert.True(response.IsSuccessfull, "Call to DeleteActivity was not successful");
            Assert.True(response.DataObject != null, "Call to DeleteActivity returned no data");

            var verifyDeleted = proxy.GetActivity(_testActivityToBeDeletedId);
            Assert.False(verifyDeleted.IsSuccessfull, "Call to Get deleted activity was successful");
            Assert.Null(verifyDeleted.DataObject);
        }

        private void VerifyDetailsAreSame(ActivityDetail details1, ActivityDetail details2)
        {
            Assert.Equal(details2.Title, details1.Title);
            Assert.Equal(details2.Details, details1.Details);
            Assert.Equal(details2.OwnerEmail, details1.OwnerEmail);
            Assert.Equal(details2.Done, details1.Done);
            Assert.Equal(details2.Due.Date, details1.Due.Date);
        }

        #endregion

        #region Set up data
        private void CreateTestSaleInvoices()
        {
            var accountProxy = new AccountsProxy();
            var accountResponse = accountProxy.GetAccounts(accountType: "Income");
            var incomeAccountId = accountResponse.DataObject.Accounts.Where(a => a.AccountType == "Income").Take(1).SingleOrDefault().Id;

            var invoice = new InvoiceTransactionDetail
            {
                LineItems = new List<InvoiceTransactionLineItem>
                    {
                        new InvoiceTransactionLineItem
                        {
                            Description = "line item 1",
                            AccountId = incomeAccountId,
                            TotalAmount = new decimal(10.00)
                        }
                    },
                Currency = "AUD",
                InvoiceType = "Tax Invoice",
                TransactionType = "S",
                Layout = "S",
                TotalAmount = 10.00M,
                IsTaxInc = true,
                TransactionDate = DateTime.Now.AddDays(-10),
                Tags = new List<string> { _testTag }
            };

            var invoiceProxy = new InvoiceProxy();
            var result = invoiceProxy.InsertInvoice(invoice);
            _saleInvoiceTranId = result.DataObject.InsertedEntityId;

            var invoice2 = new InvoiceTransactionDetail
            {
                LineItems = new List<InvoiceTransactionLineItem>
                    {
                        new InvoiceTransactionLineItem
                        {
                            Description = "line item 1",
                            AccountId = incomeAccountId,
                            TotalAmount = new decimal(10.00)
                        }
                    },
                Currency = "AUD",
                InvoiceType = "Tax Invoice",
                TransactionType = "S",
                Layout = "S",
                TotalAmount = 10.00M,
                IsTaxInc = true,
                TransactionDate = DateTime.Now.AddDays(-10),
                Tags = new List<string> { _testTag }
            };

            result = invoiceProxy.InsertInvoice(invoice2);
            _saleInvoiceTranId2 = result.DataObject.InsertedEntityId;
        }

        private void CreateTestActivities()
        {
            var activity1 = GetActivityDetail(done: true, tags: "blue" );
            var activity2 = GetActivityDetail(ownerEmail: TestConfig.TestUser, tags: "blue, red");
            var activity3 = GetActivityDetail(type: "Task");
            var activity4 = GetActivityDetail(attachedToId: _saleInvoiceTranId2, due: DateTime.Now.AddDays(1), tags: "yellow");
            var activity5 = GetActivityDetail(attachedToId: _saleInvoiceTranId2, due: DateTime.Now.AddDays(1), tags: "orange");

            var activityProxy = new ActivityProxy();

            var response = activityProxy.InsertActivity(activity1);
            _testActivityId1 = response.DataObject.InsertedEntityId;

            response = activityProxy.InsertActivity(activity2);
            _testActivityId2 = response.DataObject.InsertedEntityId;

            response = activityProxy.InsertActivity(activity3);
            _testActivityToBeDeletedId = response.DataObject.InsertedEntityId;

            response = activityProxy.InsertActivity(activity4);

            response = activityProxy.InsertActivity(activity4);
            _testActivityToBeUpdated = response.DataObject.InsertedEntityId;
        }

        private ActivityDetail GetActivityDetail(string title = null, string details = null, string attachedToType = null, int? attachedToId = null, bool done = false,
            DateTime? due = null, string ownerEmail = null, string type = null, string tags = null)
        {
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 5);
            var activity = new ActivityDetail
            {
                Title = title ?? "Test activity " + uniqueId,
                Details = details ?? "Details of test activity " + uniqueId,
                AttachedToId = attachedToId ?? _saleInvoiceTranId,
                AttachedToType = attachedToType ?? "Sale",
                Done = done,
                Due = due ?? DateTime.Now.AddDays(5),
                OwnerEmail = ownerEmail,
                ActivityType = type ?? _testTag,
                Tags = new List<string> { tags }
            };

            return activity;
        }

        #endregion
    }
}
