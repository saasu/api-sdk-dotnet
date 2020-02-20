using Xunit;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Models.Attachments;
using Saasu.API.Core.Models.Invoices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Saasu.API.Core.Globals;
using InvoiceProxy = Saasu.API.Client.Proxies.InvoiceProxy;

namespace Saasu.API.Client.IntegrationTests
{
    public class InvoiceTests
    {
        private readonly InvoiceHelper _invHelper;

        private static int? _testInvoiceId;

        private static int? TestInvoiceId
        {
            get
            {
                if (_testInvoiceId == 0)
                {
                    return GetTestInvoiceId();
                }
                else
                {
                    return _testInvoiceId;
                }
            }
        }


        private const string AutoNumber = "<auto number>";
        private const string ItemLayoutForbiddenMessage = " Check the response returned as this may be because your current subscription does not allow item layouts.";
        private const string MultiCurrencyForbiddenMessage = " Check the response returned as this may be because your current subscription does not allow multi currency or because it is turned off.";

        public InvoiceTests()
        {
            _invHelper = new InvoiceHelper();
            _invHelper.CreateTestData();
        }

        [Fact]
        public void ShouldGetInvoicesForKnownFile()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new InvoicesProxy(accessToken);
            var response = proxy.GetInvoices();

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.NotNull(response.DataObject.Invoices);
            Assert.NotNull(response.DataObject._links);
            Assert.True(response.DataObject.Invoices.Count > 0);
            Assert.True(response.DataObject._links.Count > 0);
        }

        [Fact]
        public void ShouldGetOnlyFirstInvoiceForKnownFile()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new InvoicesProxy(accessToken);
            var response = proxy.GetInvoices();

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.NotNull(response.DataObject.Invoices);
            Assert.True(response.DataObject.Invoices.Count > 0);
        }

        [Fact]
        public void ShouldGetOneInvoiceForKnownFile()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();
            var proxy = new InvoiceProxy(accessToken);
            var response = proxy.GetInvoice(Convert.ToInt32(_invHelper.InvoiceId1));

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
        }

        [Fact]
        public void ShouldNotBeAbleToAddAttachmentWithInvalidInvoiceId()
        {
            var attachment = CreateTestAttachment();
            attachment.ItemIdAttachedTo = 0; // invalid invoice id

            var addResponse = new InvoiceProxy().AddAttachment(attachment);

            Assert.NotNull(addResponse);
            Assert.False(addResponse.IsSuccessfull, "Adding an attachment succeeded BUT it should have failed as it had an invalid invoice id of 0");
        }

        [Fact]
        public void ShouldBeAbleToAddSmallAttachmentUsingWsAccessKey()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new InvoicesProxy(accessToken);
            var response = proxy.GetInvoices();

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull, "Getting invoices failed. STatus code: " + ((int)response.StatusCode).ToString());
            Assert.True(response.DataObject.Invoices.Count > 0, "Number of invoices returned was not greater than 0");

            _testInvoiceId = response.DataObject.Invoices[0].TransactionId;

            var attachment = CreateTestAttachment();
            attachment.ItemIdAttachedTo = response.DataObject.Invoices[0].TransactionId.GetValueOrDefault();

            var addResponse = new InvoiceProxy().AddAttachment(attachment);

            Assert.NotNull(addResponse);
            Assert.True(addResponse.IsSuccessfull, "Adding an attachment failed. STatus code: " + ((int)addResponse.StatusCode).ToString());
        }

        [Fact]
        public void ShouldBeAbleToAddLargeAttachmentUsingWsAccessKey()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new InvoicesProxy(accessToken);
            var response = proxy.GetInvoices();

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull, "Getting invoices failed. STatus code: " + ((int)response.StatusCode).ToString());
            Assert.True(response.DataObject.Invoices.Count > 0, "Number of invoices returned was not greater than 0");

            _testInvoiceId = response.DataObject.Invoices[0].TransactionId;

            var attachment = CreateTestAttachment(true);
            attachment.ItemIdAttachedTo = response.DataObject.Invoices[0].TransactionId.GetValueOrDefault();

            var addResponse = new InvoiceProxy().AddAttachment(attachment);

            Assert.NotNull(addResponse);
            Assert.True(addResponse.IsSuccessfull, "Adding an attachment failed. STatus code: " + ((int)addResponse.StatusCode).ToString());
        }

        [Fact]
        public void ShouldBeAbleToGetInfoOnAllAttachmentsUsingWsAccessKey()
        {
            var proxy = new InvoiceProxy();

            var attachment = CreateTestAttachment();

            //Attach invoice Id to attachment and insert attachment.
            attachment.ItemIdAttachedTo = Convert.ToInt32(_invHelper.InvoiceId1);
            var insertResponse = proxy.AddAttachment(attachment);

            var attachmentId = insertResponse.DataObject.Id;

            Assert.True(attachmentId > 0);

            var getResponse = proxy.GetAllAttachmentsInfo(_invHelper.InvoiceId1.GetValueOrDefault());

            Assert.NotNull(getResponse);
            Assert.True(getResponse.IsSuccessfull);
            Assert.NotNull(getResponse.DataObject);
            Assert.NotNull(getResponse.DataObject.Attachments);
            Assert.True(getResponse.DataObject.Attachments.Count > 0);
        }

        [Fact]
        public void ShouldBeAbleToGetAnAllAttachmentUsingOAuth()
        {
            ShouldBeAbleToAddSmallAttachmentUsingWsAccessKey();  // Ensure we add anattachment

            var accessToken = TestHelper.SignInAndGetAccessToken();
            var proxy = new InvoiceProxy(accessToken);
            var response = proxy.GetAllAttachmentsInfo(Convert.ToInt32(TestInvoiceId));

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.NotNull(response.DataObject.Attachments);
            Assert.True(response.DataObject.Attachments.Count > 0);

            var attachmentResponse = proxy.GetAttachment(response.DataObject.Attachments[0].Id);
            Assert.NotNull(attachmentResponse);
            Assert.True(attachmentResponse.IsSuccessfull);
        }

        [Fact]
        public void ShouldBeAbleToDeleteAnAttachment()
        {
            ShouldBeAbleToAddSmallAttachmentUsingWsAccessKey();  // Ensure we add an attachment

            var proxy = new InvoiceProxy();
            var response = proxy.GetAllAttachmentsInfo(Convert.ToInt32(TestInvoiceId));
            Assert.True(response.IsSuccessfull && response.DataObject != null && response.DataObject.Attachments.Count > 0, "Getting all attachments failed. Status Code: " + ((int)response.StatusCode).ToString());

            var attachmentResponse = proxy.DeleteAttachment(response.DataObject.Attachments[0].Id);
            Assert.NotNull(attachmentResponse);
            Assert.True(attachmentResponse.IsSuccessfull, "Adding an attachment was not successfull. Status Code:" + ((int)attachmentResponse.StatusCode).ToString());
        }

        [Fact]
        public void GetInvoicesAll()
        {
            AssertInvoiceProxy();
        }

        [Fact]
        public void GetInvoicesOnePageOneRecord()
        {
            var response = AssertInvoiceProxy(pageNumber: 1, pageSize: 1);
            Assert.Equal(1, response.DataObject.Invoices.Count);
        }

        [Fact]
        public void GetInvoicesFilterOnDates()
        {
            //use a year ago and tomorrow as date filters to make sure the test invoice is picked up.
            AssertInvoiceProxy(fromDate: DateTime.Now.AddYears(-1), toDate: DateTime.Now.AddDays(1));
        }

        [Fact]
        public void GetInvoicesFilterOnModifiedDates()
        {
            //use a year ago and tomorrow as date filters to make sure the test contact Carl O'Brien is picked up.
            AssertInvoiceProxy(lastModifiedFromDate: DateTime.Now.AddYears(-1), lastModifiedToDate: DateTime.Now.AddDays(1));
        }

        [Fact]
        public void GetInvoicesFilterOnInvoiceNumber()
        {
            var invNumber = string.Format("Inv{0}", Guid.NewGuid());

            //Create and insert test invoice.
            GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: invNumber, actuallyInsertAndVerifyResponse: true);

            var response = AssertInvoiceProxy(invoiceNumber: invNumber);

            Assert.Equal(1, response.DataObject.Invoices.Count);
        }

        [Fact]
        public void GetInvoicesFilterOnPurchaseOrderNumber()
        {
            var purchaseOrderNumber = string.Format("Inv{0}", Guid.NewGuid());

            //Create and insert test invoice.
            GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "P", emailContact: false, purchaseOrderNumber: purchaseOrderNumber, actuallyInsertAndVerifyResponse: true);

            var response = AssertInvoiceProxy(purchaseOrderNumber: purchaseOrderNumber);

            Assert.Equal(1, response.DataObject.Invoices.Count);
        }

        [Fact]
        public void GetInvoicesFilterOnPaymentStatus()
        {
            AssertInvoiceProxy(paymentStatus: (int)PaymentStatusType.Unpaid);
        }

        [Fact]
        public void GetInvoicesFilterOnBillingContactId()
        {
            //Get Id of test contact associated with the invoice.
            var contactId = ContactTests.GetOrCreateContactCustomer();
            var contactProxy = new Proxies.ContactProxy();
            var contactResponse = contactProxy.GetContact(contactId);

            var billingContactId = contactResponse.DataObject.Id;

            //Create and insert test invoices for billing contact.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", emailContact: false, billingContactId: billingContactId);
            var invoice2 = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", emailContact: false, billingContactId: billingContactId);

            var proxy = new InvoiceProxy();
            proxy.InsertInvoice(invoice);
            proxy.InsertInvoice(invoice2);

            AssertInvoiceProxy(billingContactId: contactResponse.DataObject.Id);
        }

        [Fact]
        public void GetInvoicesFilterOnInvoiceStatus()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceType: Constants.InvoiceType.SaleOrder, invoiceStatus: InvoiceStatusType.Order.ToQueryParameter());
            new InvoiceProxy().InsertInvoice(invoice);

            AssertInvoiceProxy(invoiceStatus: InvoiceStatusType.Order.ToQueryParameter());
        }

        [Fact]
        public void GetSingleInvoiceWithInvoiceId()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var insertResponse = new InvoiceProxy().InsertInvoice(invoice);

            Assert.True(insertResponse.IsSuccessfull);

            var insertResult = insertResponse.DataObject;

            var tranid = insertResult.InsertedEntityId;

            Assert.True(tranid > 0);

            var response = new InvoiceProxy().GetInvoice(tranid);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.NotNull(response.DataObject._links);
            Assert.True(response.DataObject._links.Count > 0);
        }

        [Fact]
        public void GetInvoicesPageSize()
        {
            var proxy = new InvoicesProxy();
            var response = proxy.GetInvoices(pageNumber: 1, pageSize: 2);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.Equal(response.DataObject.Invoices.Count, 2);
        }

        [Fact]
        public void GetInvoicesSecondPage()
        {
            var proxy = new InvoicesProxy();
            var response = proxy.GetInvoices(pageSize: 2);

            var idsFromPage1 = response.DataObject.Invoices.Select(i => i.TransactionId).ToList();

            response = proxy.GetInvoices(pageNumber: 2);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);

            response.DataObject.Invoices.ForEach(i => Assert.False(idsFromPage1.Contains(i.TransactionId)));
        }


        #region Insert Tests

        /// <summary>
        /// Insert
        ///		- Sale
        ///		- Service layout
        ///		- No Email
        ///		- Defined invoice number
        /// </summary>
        [Fact]
        public void InsertSaleWithServiceItemsNoEmailToContact()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var response = new InvoiceProxy().InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.Null(results.GeneratedInvoiceNumber);
            Assert.False(results.SentToContact);
            Assert.NotEqual(results.InsertedEntityId, 0);
            Assert.NotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.NotEqual(results.LastUpdatedId, null);
            Assert.NotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getResponse = new InvoiceProxy().GetInvoice(results.InsertedEntityId);

            Assert.NotNull(getResponse.DataObject);

            VerifyInvoicesAreEqual(invoice, getResponse.DataObject);
        }

        [Fact]
        public void InsertSaleAndCanUpdateUsingReturnedLastUpdateId()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.Null(results.GeneratedInvoiceNumber);
            Assert.False(results.SentToContact);
            Assert.NotEqual(results.InsertedEntityId, 0);
            Assert.NotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.NotEqual(results.LastUpdatedId, null);
            Assert.NotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getResponse = proxy.GetInvoice(results.InsertedEntityId);
            Assert.NotNull(getResponse.DataObject);

            // Do an Update
            var getInvoice = getResponse.DataObject;
            getInvoice.Summary = "Update 1";
            var updateResponse = proxy.UpdateInvoice(getInvoice.TransactionId.Value, getInvoice);
            Assert.True(updateResponse.IsSuccessfull);

            // Using the LastUpdatedId returned from the previous update, do another update
            getInvoice.Summary = "Update 2";
            getInvoice.LastUpdatedId = updateResponse.DataObject.LastUpdatedId;
            var updateResponse2 = proxy.UpdateInvoice(getInvoice.TransactionId.Value, getInvoice);
            Assert.True(updateResponse2.IsSuccessfull);

        }

        /// <summary>
        /// Insert
        ///		- Sale
        ///		- Item layout
        ///		- Email
        ///		- Auto invoice number
        /// </summary>
        [Fact]
        public void InsertSaleWithItemLayoutEmailToContactAndAutoNumberAndAutoPopulateFxRate()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Item, transactionType: "S", emailContact: true, autoPopulateFxRate: true);

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull, string.Format("Inserting an item layout invoice has failed.{0}", ItemLayoutForbiddenMessage));

            var results = response.DataObject;

            Assert.NotNull(results.GeneratedInvoiceNumber);
            Assert.True(results.SentToContact);
            Assert.NotEqual(results.InsertedEntityId, 0);
            Assert.NotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.NotNull(results.LastUpdatedId);
            Assert.NotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(results.InsertedEntityId);

            Assert.NotNull(getResponse.DataObject);

            VerifyInvoicesAreEqual(invoice, getResponse.DataObject);
        }

        [Fact]
        public void InsertSaleWithAutoPopulateFxRateShouldNotUsePassedInFxRate()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Item, transactionType: "S", autoPopulateFxRate: true, fxRate: 99);

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull, string.Format("Inserting an item layout invoice has failed.{0}", ItemLayoutForbiddenMessage));

            var results = response.DataObject;

            Assert.NotEqual(results.InsertedEntityId, 0);
            Assert.NotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.NotNull(results.LastUpdatedId);
            Assert.NotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(results.InsertedEntityId);

            Assert.NotNull(getResponse.DataObject);
            Assert.NotEqual(99, getResponse.DataObject.FxRate);
        }

        /// <summary>
        /// Insert
        ///		- Purchase
        ///		- Service layout
        ///		- No Email
        ///		- Defined PO number
        /// </summary>
        [Fact]
        public void InsertPurchaseWithServiceItemsNoEmailToContact()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "P", emailContact: false, purchaseOrderNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var response = new InvoiceProxy().InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.Null(results.GeneratedInvoiceNumber);
            Assert.False(results.SentToContact);
            Assert.NotEqual(results.InsertedEntityId, 0);
            Assert.NotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.NotEqual(results.LastUpdatedId, null);
            Assert.NotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getResponse = new InvoiceProxy().GetInvoice(results.InsertedEntityId);

            Assert.NotNull(getResponse.DataObject);

            VerifyInvoicesAreEqual(invoice, getResponse.DataObject);
        }

        /// <summary>
        /// Insert
        ///		- Purchase
        ///		- Item layout
        ///		- Email
        ///		- Auto PO number
        /// </summary>
        [Fact]
        public void InsertPurchaseWithItemLayoutEmailToContactAndAutoNumber()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Item, transactionType: "P", emailContact: true);

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull, string.Format("Inserting an item layout invoice has failed.{0}", ItemLayoutForbiddenMessage));

            var results = response.DataObject;

            Assert.NotNull(results.GeneratedInvoiceNumber);
            Assert.True(results.SentToContact);
            Assert.NotEqual(results.InsertedEntityId, 0);
            Assert.NotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.NotNull(results.LastUpdatedId);
            Assert.NotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(results.InsertedEntityId);

            Assert.NotNull(getResponse.DataObject);

            VerifyInvoicesAreEqual(invoice, getResponse.DataObject);
        }

        [Fact]
        public void InsertDifferentCurrencyForMultiCurrencyFile()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", currency: "USD", fxRate: new decimal(1.50));

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull, string.Format("Inserting a multi currency invoice has failed.{0}", MultiCurrencyForbiddenMessage));

            var results = response.DataObject;

            Assert.NotEqual(results.InsertedEntityId, 0);
            Assert.NotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.NotEqual(results.LastUpdatedId, null);
            Assert.NotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getResponse = new InvoiceProxy().GetInvoice(results.InsertedEntityId);

            Assert.NotNull(getResponse.DataObject);

            VerifyInvoicesAreEqual(invoice, getResponse.DataObject);
            Assert.Equal(invoice.FxRate, getResponse.DataObject.FxRate);
        }

        [Fact]
        public void InsertSaleWithQuickPayment()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            invoice.QuickPayment = new InvoiceQuickPaymentDetail
            {
                DatePaid = DateTime.Now.AddDays(-4),
                DateCleared = DateTime.Now.AddDays(-3),
                BankedToAccountId = _invHelper.BankAccountId,
                Amount = new decimal(10.00),
                Reference = "Test quick payment reference",
                Summary = "Test quick payment summary"
            };

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull);
            Assert.NotEqual(response.DataObject.InsertedEntityId, 0);

            //get invoice.
            var getResponse = proxy.GetInvoice(response.DataObject.InsertedEntityId);

            Assert.NotNull(getResponse.DataObject.PaymentCount);
            Assert.Equal(getResponse.DataObject.PaymentCount, (Int16)1);
            Assert.Equal(getResponse.DataObject.AmountPaid, new decimal (10.00));
            Assert.Equal(getResponse.DataObject.AmountOwed, (getResponse.DataObject.TotalAmount - getResponse.DataObject.AmountPaid));
        }

        [Fact]
        public void InsertPurchaseWithQuickPayment()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "P", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            invoice.QuickPayment = new InvoiceQuickPaymentDetail
            {
                DatePaid = DateTime.Now.AddDays(-4),
                DateCleared = DateTime.Now.AddDays(-3),
                BankedToAccountId = _invHelper.BankAccountId,
                Amount = new decimal(10.00),
                Reference = "Test quick payment reference",
                Summary = "Test quick payment summary"
            };

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull);
            Assert.NotEqual(response.DataObject.InsertedEntityId, 0);

            //get invoice.
            var getResponse = proxy.GetInvoice(response.DataObject.InsertedEntityId);

            Assert.NotNull(getResponse.DataObject.PaymentCount);
            Assert.Equal(getResponse.DataObject.PaymentCount, (Int16)1);
            Assert.Equal(getResponse.DataObject.AmountPaid, new decimal (10.00));
            Assert.Equal(getResponse.DataObject.AmountOwed, (getResponse.DataObject.TotalAmount - getResponse.DataObject.AmountPaid));
        }

        #endregion

        #region Update Tests
        /// <summary>
        /// Insert
        ///		- Sale
        ///		- Service layout
        ///		- No Email sent
        /// </summary>
        [Fact]
        public void UpdateSaleWithServiceLayoutAllFieldsUpdated()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));
            var response = new InvoiceProxy().InsertInvoice(invoice);
            Assert.True(response.IsSuccessfull);

            var insertResult = response.DataObject;
            Assert.NotEqual(insertResult.InsertedEntityId, 0);
            var tranId = insertResult.InsertedEntityId;

            //Get the inserted invoice to pick up any default values which may have been assigned.
            var getInsertedResponse = new InvoiceProxy().GetInvoice(tranId);
            var insertedInvoice = getInsertedResponse.DataObject;

            //Get update invoice. The invoice just returned from GET is passed in to have the updated fields copied to it. This is so it can be compared to the 
            //invoice returned after the update has occurred, to make sure all fields are equal.
            var updateInvoice = GetUpdatedInvoice(tranId, insertResult.LastUpdatedId, false, insertedInvoice);

            //Likewise the change is also made to the original inserted invoice for comparison later.
            updateInvoice.LineItems = insertedInvoice.LineItems = new List<InvoiceTransactionLineItem>
            {
                new InvoiceTransactionLineItem
                {
                    Description = "updated line item",
                    AccountId = _invHelper.IncomeAccountId2,
                    TaxCode = Constants.TaxCode.SaleInputTaxed,
                    TotalAmount = new decimal(100.00),
                    Tags = new List<string> { "update item tag 1", "update item tag 2" }
                }
            };

            var updateResponse = new InvoiceProxy().UpdateInvoice(tranId, updateInvoice);
            var updateResult = updateResponse.DataObject;

            Assert.False(updateResult.SentToContact);
            Assert.True(TestHelper.AssertDatetimesEqualWithVariance(updateResult.UtcLastModified.Date, DateTime.UtcNow.Date));
            Assert.NotNull(updateResult.LastUpdatedId);
            Assert.NotEqual(updateResult.LastUpdatedId.Trim(), string.Empty);

            //Get invoice after update.
            var getResponse = new InvoiceProxy().GetInvoice(tranId);
            Assert.NotNull(getResponse.DataObject);

            //Compare updated with original inserted invoice (which also now contains the updated changes).
            VerifyInvoicesAreEqual(insertedInvoice, getResponse.DataObject);
        }

        /// <summary>
        /// Insert
        ///		- Sale
        ///		- Service layout
        ///		- Email
        /// </summary>
        [Fact]
        public void UpdateSaleWithItemLayoutAllFieldsUpdated()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Item, transactionType: "S", emailContact: true, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));
            var response = new InvoiceProxy().InsertInvoice(invoice);
            Assert.True(response.IsSuccessfull, string.Format("Inserting an item layout invoice has failed.{0}", ItemLayoutForbiddenMessage));

            var insertResult = response.DataObject;
            Assert.NotEqual(insertResult.InsertedEntityId, 0);
            var tranId = insertResult.InsertedEntityId;

            //Get the inserted invoice to pick up any default values which may have been assigned.
            var getInsertedResponse = new InvoiceProxy().GetInvoice(tranId);
            var insertedInvoice = getInsertedResponse.DataObject;

            //Get update invoice. The invoice just returned from GET is passed in to have the updated fields copied to it. This is so it can be compared to the 
            //invoice returned after the update has occurred, to make sure all fields are equal.
            var updateInvoice = GetUpdatedInvoice(tranId, insertResult.LastUpdatedId, true, insertedInvoice);

            //Likewise the change is also made to the original inserted invoice for comparison later.
            updateInvoice.LineItems = insertedInvoice.LineItems = new List<InvoiceTransactionLineItem>
            {
                new InvoiceTransactionLineItem
                        {
                            Description = "updated line item 1",
                            TaxCode = Constants.TaxCode.SaleInputTaxed,
                            Quantity = 20,
                            UnitPrice = new decimal(200.00),
                            PercentageDiscount = new decimal(15.00),
                            InventoryId =  _invHelper.InventorySaleItemId2,
                            Tags = new List<string> {"updated item tag 1", "updated item tag 2"}
							//Attributes = GetItemAttributes()
						},
            };

            var updateResponse = new InvoiceProxy().UpdateInvoice(tranId, updateInvoice);
            var updateResult = updateResponse.DataObject;

            Assert.True(updateResult.SentToContact);
            Assert.True(TestHelper.AssertDatetimesEqualWithVariance(updateResult.UtcLastModified.Date, DateTime.UtcNow.Date));
            Assert.NotNull(updateResult.LastUpdatedId);
            Assert.NotEqual(updateResult.LastUpdatedId.Trim(), string.Empty);

            //Get invoice after update.
            var getResponse = new InvoiceProxy().GetInvoice(tranId);
            Assert.NotNull(getResponse.DataObject);

            //Compare updated with original inserted invoice (which also now contains the updated changes).
            VerifyInvoicesAreEqual(insertedInvoice, getResponse.DataObject);
        }

        /// <summary>
        /// Insert
        ///		- Sale
        ///		- Service layout
        ///		- Email sent with insert, but not update.
        /// </summary>
        [Fact]
        public void UpdatePurchaseWithServiceLayoutAllFieldsUpdated()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "P", emailContact: true, purchaseOrderNumber: string.Format("TestInv{0}", Guid.NewGuid()));
            var response = new InvoiceProxy().InsertInvoice(invoice);
            Assert.True(response.IsSuccessfull);

            var insertResult = response.DataObject;
            Assert.NotEqual(insertResult.InsertedEntityId, 0);
            var tranId = insertResult.InsertedEntityId;

            //Get the inserted invoice to pick up any default values which may have been assigned.
            var getInsertedResponse = new InvoiceProxy().GetInvoice(tranId);
            var insertedInvoice = getInsertedResponse.DataObject;

            //Get update invoice. The invoice just returned from GET is passed in to have the updated fields copied to it. This is so it can be compared to the 
            //invoice returned after the update has occurred, to make sure all fields are equal.
            var updateInvoice = GetUpdatedInvoice(tranId, insertResult.LastUpdatedId, false, insertedInvoice);

            //Likewise the change is also made to the original inserted invoice for comparison later.
            updateInvoice.LineItems = new List<InvoiceTransactionLineItem>
            {
                new InvoiceTransactionLineItem
                {
                    Description = "updated line item",
                    AccountId = _invHelper.IncomeAccountId2,
                    TaxCode = Constants.TaxCode.SaleInputTaxed,
                    TotalAmount = new decimal(100.00),
                    Tags = new List<string> { "update item tag 1", "update item tag 2" }
                }
            };

            var updateResponse = new InvoiceProxy().UpdateInvoice(tranId, updateInvoice);
            var updateResult = updateResponse.DataObject;

            Assert.False(updateResult.SentToContact);
            Assert.True(TestHelper.AssertDatetimesEqualWithVariance(updateResult.UtcLastModified.Date, DateTime.UtcNow.Date));
            Assert.NotNull(updateResult.LastUpdatedId);
            Assert.NotEqual(updateResult.LastUpdatedId.Trim(), string.Empty);

            //Get invoice after update.
            var getResponse = new InvoiceProxy().GetInvoice(tranId);
            Assert.NotNull(getResponse.DataObject);

            //Compare updated with original inserted invoice (which also now contains the updated changes).
            VerifyInvoicesAreEqual(updateInvoice, getResponse.DataObject);
        }

        /// <summary>
        /// Update
        ///		- Purchase
        ///		- Service layout
        ///		- No email insert, but email sent in update.
        /// </summary>
        [Fact]
        public void UpdatePurchaseithItemLayoutAllFieldsUpdated()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Item, transactionType: "P", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));
            var response = new InvoiceProxy().InsertInvoice(invoice);
            Assert.True(response.IsSuccessfull, string.Format("Inserting an item layout invoice has failed.{0}", ItemLayoutForbiddenMessage));

            var insertResult = response.DataObject;
            Assert.NotEqual(insertResult.InsertedEntityId, 0);
            var tranId = insertResult.InsertedEntityId;

            //Get the inserted invoice to pick up any default values which may have been assigned.
            var getInsertedResponse = new InvoiceProxy().GetInvoice(tranId);
            var insertedInvoice = getInsertedResponse.DataObject;

            //Get update invoice. The invoice just returned from GET is passed in to have the updated fields copied to it. This is so it can be compared to the 
            //invoice returned after the update has occurred, to make sure all fields are equal.
            var updateInvoice = GetUpdatedInvoice(tranId, insertResult.LastUpdatedId, true, insertedInvoice);

            updateInvoice.LineItems = new List<InvoiceTransactionLineItem>
            {
                new InvoiceTransactionLineItem
                        {
                            Description = "upadated line item 1",
                            TaxCode = Constants.TaxCode.SaleInputTaxed,
                            Quantity = 20,
                            UnitPrice = new decimal(200.00),
                            PercentageDiscount = new decimal(15.00),
                            InventoryId =  _invHelper.InventoryPurchaseItemId2,
                            Tags = new List<string> {"upadated item tag 1", "updated item tag 2"}
                        }
            };

            var updateResponse = new InvoiceProxy().UpdateInvoice(tranId, updateInvoice);
            var updateResult = updateResponse.DataObject;

            Assert.True(updateResult.SentToContact);
            Assert.True(TestHelper.AssertDatetimesEqualWithVariance(updateResult.UtcLastModified.Date, DateTime.UtcNow.Date));
            Assert.NotNull(updateResult.LastUpdatedId);
            Assert.NotEqual(updateResult.LastUpdatedId.Trim(), string.Empty);

            //Get invoice after update.
            var getResponse = new InvoiceProxy().GetInvoice(tranId);
            Assert.NotNull(getResponse.DataObject);

            //Compare updated with original inserted invoice (which also now contains the updated changes).
            VerifyInvoicesAreEqual(updateInvoice, getResponse.DataObject);

            // The invoice was updated with SendEmailToContact request, so the new response will have set the SentToContact flag whereas
            // the original inserted invoice will not.
            Assert.NotEqual(insertedInvoice.SentToContact, getResponse.DataObject.SentToContact);
        }

        [Fact]
        public void UpdateDifferentCurrencyForMultiCurrencyFile()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S");
            var response = new InvoiceProxy().InsertInvoice(invoice);
            Assert.True(response.IsSuccessfull);

            var insertResult = response.DataObject;
            Assert.NotEqual(insertResult.InsertedEntityId, 0);
            var tranId = insertResult.InsertedEntityId;

            //Get the inserted invoice to pick up any default values which may have been assigned.
            var getInsertedResponse = new InvoiceProxy().GetInvoice(tranId);
            var insertedInvoice = getInsertedResponse.DataObject;

            //Update the currency of the invoice.
            insertedInvoice.Currency = "USD";
            insertedInvoice.FxRate = 1.5M;

            var updateResponse = new InvoiceProxy().UpdateInvoice(tranId, insertedInvoice);
            Assert.True(updateResponse.IsSuccessfull, string.Format("Updating invoice with a different currency has failed.{0}", MultiCurrencyForbiddenMessage));

            var updateResult = updateResponse.DataObject;
            Assert.False(updateResult.SentToContact);
            Assert.True(TestHelper.AssertDatetimesEqualWithVariance(updateResult.UtcLastModified.Date, DateTime.UtcNow.Date));
            Assert.NotNull(updateResult.LastUpdatedId);
            Assert.NotEqual(updateResult.LastUpdatedId.Trim(), string.Empty);

            //Get invoice after update.
            var getUpdatedResponse = new InvoiceProxy().GetInvoice(tranId);
            Assert.NotNull(getUpdatedResponse.DataObject);

            //Compare updated with original inserted invoice (which also now contains the updated changes).
            VerifyInvoicesAreEqual(insertedInvoice, getUpdatedResponse.DataObject);
            Assert.Equal(insertedInvoice.FxRate, getUpdatedResponse.DataObject.FxRate);
        }
        #endregion

        #region Delete Tests

        [Fact]
        public void DeleteSaleInvoiceWithServiceLayout()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", emailContact: true, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.NotEqual(results.InsertedEntityId, 0);
            var tranId = results.InsertedEntityId;

            var invProxy = new InvoiceProxy();

            var deleteResponse = invProxy.DeleteInvoice(tranId);

            Assert.True(deleteResponse.IsSuccessfull);
            //get invoice, verify it has been deleted.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(tranId);

            Assert.Null(getResponse.DataObject);
        }

        [Fact]
        public void DeleteSaleInvoiceWithItemLayout()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Item, transactionType: "S", emailContact: true, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.NotEqual(results.InsertedEntityId, 0);
            var tranId = results.InsertedEntityId;

            var invProxy = new InvoiceProxy();

            var deleteResponse = invProxy.DeleteInvoice(tranId);

            Assert.True(deleteResponse.IsSuccessfull);
            //get invoice, verify it has been deleted.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(tranId);

            Assert.Null(getResponse.DataObject);
        }

        /// <summary>
        /// Defined purchase otder number
        /// </summary>
        [Fact]
        public void DeletePurchaseInvoiceWithServiceLayout()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "P", emailContact: true, purchaseOrderNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.NotEqual(results.InsertedEntityId, 0);
            var tranId = results.InsertedEntityId;

            var invProxy = new InvoiceProxy();

            var deleteResponse = invProxy.DeleteInvoice(tranId);

            Assert.True(deleteResponse.IsSuccessfull);
            //get invoice, verify it has been deleted.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(tranId);

            Assert.Null(getResponse.DataObject);
        }

        /// <summary>
        /// Auto numbered purchase otder number
        /// </summary>
        [Fact]
        public void DeletePurchaseInvoiceWithItemLayout()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Item, transactionType: "S", emailContact: true);

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.True(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.NotEqual(results.InsertedEntityId, 0);
            var tranId = results.InsertedEntityId;

            var invProxy = new InvoiceProxy();

            var deleteResponse = invProxy.DeleteInvoice(tranId);

            Assert.True(deleteResponse.IsSuccessfull);
            //get invoice, verify it has been deleted.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(tranId);

            Assert.Null(getResponse.DataObject);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void InvalidCurrencyInsert()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", currency: "TEST");

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.False(response.IsSuccessfull);
            Assert.NotNull(response.RawResponse);
            Assert.True(response.RawResponse.Contains("Please include a valid currency"));
            Assert.NotNull(response.ReasonCode);
            Assert.Equal(response.ReasonCode.Trim().ToLower(), "bad request");
        }

        [Fact]
        public void EmailInvoiceToBillingContact()
        {
            
            var billingContactId = ContactTests.GetOrCreateContactCustomer();

            var invoice = GetTestInsertInvoice(invoiceLayout: Constants.InvoiceLayout.Service, transactionType: "S", billingContactId: billingContactId, emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));
            var insertResponse = new InvoiceProxy().InsertInvoice(invoice);

            Assert.True(insertResponse.IsSuccessfull);

            var insertResult = insertResponse.DataObject;

            var tranid = insertResult.InsertedEntityId;

            Assert.True(tranid > 0);

            var response = new InvoiceProxy().EmailInvoice(tranid);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.Equal("Invoice has been emailed.", response.DataObject.StatusMessage);
            Assert.NotNull(response.DataObject._links);
            Assert.True(response.DataObject._links.Count > 0);
        }


        #endregion

        #region Validation

        private void VerifyInvoicesAreEqual(InvoiceTransactionDetail inv1, InvoiceTransactionDetail inv2)
        {
            Assert.Equal(inv1.NotesInternal, inv2.NotesInternal);
            Assert.Equal(inv1.NotesExternal, inv2.NotesExternal);
            Assert.Equal(inv1.TemplateId, inv2.TemplateId);
            Assert.Equal(inv1.Currency, inv2.Currency);
            Assert.Equal(inv1.TransactionType, inv2.TransactionType);
            Assert.Equal(inv1.AutoPopulateFxRate, inv2.AutoPopulateFxRate);

            switch (inv1.TransactionType)
            {
                case "S":
                    {
                        if (inv1.InvoiceNumber == AutoNumber)
                        {
                            Assert.False(inv1.InvoiceNumber == inv2.InvoiceNumber, "InvoiceNumber are equal but expected to be different");
                        }
                        else
                        {
                            Assert.Equal(inv1.InvoiceNumber, inv2.InvoiceNumber);
                        }
                    }
                    break;
                case "P":
                    {
                        if (inv1.PurchaseOrderNumber == AutoNumber)
                        {
                            Assert.False(inv1.PurchaseOrderNumber == inv2.PurchaseOrderNumber, "Purchase Order Number are the same but expected to be different");
                        }
                        else
                        {
                            Assert.Equal(inv1.PurchaseOrderNumber, inv2.PurchaseOrderNumber);
                        }
                    }
                    break;
            }

            // Cannot assert this here as it gets changed base don whether consumer changes the flag, or consumer asks email to be sent in whch
            // case this flag gets set as well
            //Assert.AreEqual(inv1.SentToContact, inv2.SentToContact);

            Assert.Equal(inv1.RequiresFollowUp, inv2.RequiresFollowUp);

            Assert.Equal(inv1.InvoiceType, inv2.InvoiceType);
            Assert.Equal(inv1.Layout, inv2.Layout);
            Assert.Equal(inv1.Summary, inv2.Summary);
            //Assert.AreEqual(inv1.TotalAmountExcludingTax, inv2.TotalAmountExcludingTax);-- leave out as calcs currently come from items
            //Assert.AreEqual(inv1.TotalTaxAmount, inv2.TotalTaxAmount); -- leave out as calcs currently come from items
            //Assert.AreEqual(inv1.TotalAmountInclTax, inv2.TotalAmountInclTax); -- leave out as calcs currently come from items
            var inv1Date = Convert.ToDateTime(inv1.TransactionDate).Date;
            var inv2Date = Convert.ToDateTime(inv2.TransactionDate).Date;
            Assert.Equal(inv1Date.Year, inv2Date.Year);
            Assert.Equal(inv1Date.Month, inv2Date.Month);
            Assert.Equal(inv1Date.Day, inv2Date.Day);
            //Assert.AreEqual(Convert.ToDateTime(inv1.TransactionDate).Date, Convert.ToDateTime(inv2.TransactionDate).Date);
            Assert.Equal(inv1.BillingContactId.GetValueOrDefault(), inv2.BillingContactId.GetValueOrDefault());
            Assert.Equal(inv1.ShippingContactId.GetValueOrDefault(), inv2.ShippingContactId.GetValueOrDefault());

            VerifyInvoiceItemsAreEqual(inv1.Layout, inv1.LineItems, inv2.LineItems);
            VerifyInvoiceTermsAreEqual(inv1.Terms, inv2.Terms);
            VerifyInvoiceAttachmentsAreEqual(inv1.Attachments, inv2.Attachments);
            VerifyInvoiceEmailMessagesAreEqual(inv1.EmailMessage, inv2.EmailMessage);
        }

        private void VerifyInvoiceItemsAreEqual(string layout, List<InvoiceTransactionLineItem> list1, List<InvoiceTransactionLineItem> list2)
        {
            if (list1 == null || list2 == null)
            {
                Assert.True(list1 == null && list2 == null, "One list is NULL and the other is not. Expected both to be NULL");
                return;
            }

            Assert.Equal(list1.Count, list2.Count);

            if (layout == "S")
            {
                foreach (var item in list1)
                {
                    var itemInList2 = list2.SingleOrDefault(i => i.Description == item.Description &&
                        i.AccountId == item.AccountId &&
                        i.TaxCode == item.TaxCode
                        );

                    Assert.NotNull(itemInList2);
                }

                foreach (var item in list2)
                {
                    var itemInList1 = list1.SingleOrDefault(i => i.Description == item.Description &&
                        i.AccountId == item.AccountId &&
                        i.TaxCode == item.TaxCode
                        );

                    Assert.NotNull(itemInList1);
                }
            }
            else
            {
                foreach (var item in list1)
                {
                    var itemInList2 = list2.SingleOrDefault(i => i.Description == item.Description &&
                        i.AccountId == item.AccountId &&
                        i.TaxCode == item.TaxCode &&
                        i.Quantity == item.Quantity &&
                        i.UnitPrice == item.UnitPrice &&
                        i.PercentageDiscount == item.PercentageDiscount &&
                        i.InventoryId == item.InventoryId
                        );


                    Assert.NotNull(itemInList2);
                }

                foreach (var item in list2)
                {
                    var itemInList1 = list1.SingleOrDefault(i => i.Description == item.Description &&
                            i.AccountId == item.AccountId &&
                        i.TaxCode == item.TaxCode &&
                        i.Quantity == item.Quantity &&
                        i.UnitPrice == item.UnitPrice &&
                        i.PercentageDiscount == item.PercentageDiscount &&
                        i.InventoryId == item.InventoryId
                        );

                    Assert.NotNull(itemInList1);
                }
            }
        }

        private void VerifyInvoiceEmailMessagesAreEqual(Saasu.API.Core.Models.Email msg1, Saasu.API.Core.Models.Email msg2)
        {
            if (msg1 == null || msg2 == null)
            {
                Assert.Null(msg1);
                Assert.Null(msg2);
                return;
            }

            Assert.Equal(msg1.To, msg2.To);
            Assert.Equal(msg1.From, msg2.From);
            Assert.Equal(msg1.Cc, msg2.Cc);
            Assert.Equal(msg1.Bcc, msg2.Bcc);
            Assert.Equal(msg1.Subject, msg2.Subject);
            Assert.Equal(msg1.Body, msg2.Body);
        }

        private void VerifyInvoiceAttachmentsAreEqual(List<FileAttachmentInfo> list1, List<FileAttachmentInfo> list2)
        {
            if (list1 == null || list2 == null || list1.Count == 0 || list2.Count == 0)
            {
                //this is because an insert may have sent attachments as null but a GET will return them as empy list.
                Assert.True(list1 == null || list1.Count == 0, "Expected no items but list1 had some");
                Assert.True(list2 == null || list2.Count == 0, "Expected no items but list2 had some");
                return;
            }

            foreach (var detail in list1)
            {
                var detailInList2 = list2.SingleOrDefault(d => d.Id == detail.Id && d.Name == detail.Name && d.Description == detail.Description);
                Assert.NotNull(detailInList2);
            }

            foreach (var detail in list2)
            {
                var detailInList1 = list1.SingleOrDefault(d => d.Id == detail.Id && d.Name == detail.Name && d.Description == detail.Description);
                Assert.NotNull(detailInList1);
            }
        }

        private void VerifyInvoiceTermsAreEqual(InvoiceTradingTerms terms1, InvoiceTradingTerms terms2)
        {
            if (terms1 == null || terms2 == null)
            {
                Assert.Null(terms1);
                Assert.Null(terms2);
                return;
            }

            Assert.Equal(terms1.Type, terms2.Type);
            Assert.Equal(terms1.Interval, terms2.Interval);
            Assert.Equal(terms1.IntervalType, terms2.IntervalType);
        }

        #endregion

        public static ProxyResponse<InvoiceTransactionSummaryResponse> AssertInvoiceProxy(int? pageNumber = null, int? pageSize = null, DateTime? fromDate = null, DateTime? toDate = null,
            DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null, string invoiceNumber = null, string purchaseOrderNumber = null, string transactionType = null, int? paymentStatus = null,
            int? billingContactId = null, string invoiceStatus = null, string tags = null, string tagFilterType = null)
        {
            var response = new InvoicesProxy().GetInvoices(pageNumber, pageSize, fromDate, toDate, lastModifiedFromDate, lastModifiedToDate, invoiceNumber, purchaseOrderNumber, transactionType, paymentStatus,
                billingContactId, invoiceStatus, tags, tagFilterType);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull, "Invoice response not successfull");
            Assert.NotNull(response.DataObject);
            Assert.NotNull(response.DataObject.Invoices);
            Assert.NotNull(response.DataObject._links);
            Assert.True(response.DataObject._links.Count > 0, "No hypermedia links in response");
            Assert.True(response.DataObject.Invoices.Count > 0, "No invoices in response");

            return response;
        }

        #region Test data

        private InvoiceTransactionDetail GetTestInsertInvoice(string invoiceLayout, List<InvoiceTransactionLineItem> lineItems = null, string notesInternal = null,
            string notesExternal = null, InvoiceTradingTerms terms = null, List<FileAttachmentInfo> attachments = null, int? templateId = null,
            bool emailContact = false, Saasu.API.Core.Models.Email emailMessage = null, string currency = null, string invoiceNumber = null, string purchaseOrderNumber = null,
            string invoiceType = null, string transactionType = null, string summary = null, decimal? totalAmountInclTax = null, bool requiresFollowUp = false, DateTime? transactionDate = null,
            int? billingContactId = null, int? shippingContactId = null, List<string> tags = null, decimal? fxRate = null, string invoiceStatus = null,
            bool actuallyInsertAndVerifyResponse = false, bool autoPopulateFxRate = false)
        {
            var tranType = transactionType ?? "S";

            var invDetail = new InvoiceTransactionDetail
            {
                LineItems = lineItems ?? GetInsertItems(invoiceLayout, tranType),
                NotesInternal = notesInternal ?? "Test internal note",
                NotesExternal = notesExternal ?? "Test external note",
                Terms = terms ?? GetTradingTerms(),
                Attachments = attachments ?? GetAttachments(),
                TemplateId = templateId ?? GetTemplateUid(),
                SendEmailToContact = emailContact,
                EmailMessage = emailMessage ?? GetEmailMessage(),
                Currency = currency ?? "AUD",
                InvoiceNumber = invoiceNumber ?? AutoNumber,
                PurchaseOrderNumber = purchaseOrderNumber ?? AutoNumber,
                InvoiceType = invoiceType ?? "Tax Invoice",
                TransactionType = tranType,
                Layout = invoiceLayout,
                Summary = summary ?? "Summary InsertInvoiceWithServiceItemsNoEmailToContact",
                TotalAmount = totalAmountInclTax ?? new decimal(20.00),
                IsTaxInc = true,
                RequiresFollowUp = requiresFollowUp,
                TransactionDate = transactionDate ?? DateTime.Now.AddDays(-10),
                BillingContactId = billingContactId ?? _invHelper.BillingContactId,
                ShippingContactId = shippingContactId ?? _invHelper.ShippingContactId,
                FxRate = fxRate,
                AutoPopulateFxRate = autoPopulateFxRate,
                InvoiceStatus = invoiceStatus,
                Tags = tags ?? new List<string> { "invoice header tag 1", "invoice header tag 2" }
            };

            if (actuallyInsertAndVerifyResponse)
            {
                var response = new InvoiceProxy().InsertInvoice(invDetail);

                Assert.NotNull(response);
                Assert.True(response.IsSuccessfull, "Inserting an invoice was not successfull. Status code: " + ((int)response.StatusCode).ToString());
                Assert.NotNull(response.RawResponse);

                var serialized = response.DataObject;

                Assert.True(serialized.InsertedEntityId > 0, "Invoice insert did not return an InsertedEntityId > 0");

                invDetail.TransactionId = serialized.InsertedEntityId;
            }

            return invDetail;
        }

        private InvoiceTransactionDetail GetUpdatedInvoice(int tranId, string lastUpdatedId, bool emailToContact = false, InvoiceTransactionDetail invoiceToCopyFrom = null)
        {
            Assert.True(tranId > 0, "A valid transactionId mus be passed into the GetUpdatedSaleInvoice method");

            var returnInvoice = new InvoiceTransactionDetail();

            if (invoiceToCopyFrom != null)
            {
                returnInvoice = invoiceToCopyFrom;
                returnInvoice.InvoiceNumber = string.Format("Inv{0}", Guid.NewGuid());
                returnInvoice.PurchaseOrderNumber = string.Format("PO{0}", Guid.NewGuid());
                returnInvoice.NotesInternal = "Test internal update note";
                returnInvoice.NotesExternal = "Test external update note";
                returnInvoice.Summary = "Summary Update";
                returnInvoice.Tags = new List<string> { "updated header tag 1", "updated header tag 2" };
                returnInvoice.Terms = new InvoiceTradingTerms
                {
                    Type = 2,
                    Interval = 5,
                    IntervalType = 1
                };
                returnInvoice.Attachments = null;
                returnInvoice.RequiresFollowUp = true;
                returnInvoice.SendEmailToContact = emailToContact;

                return returnInvoice;
            }

            return new InvoiceTransactionDetail
            {
                TransactionId = tranId,
                LastUpdatedId = lastUpdatedId,
                NotesInternal = "Test internal update note",
                NotesExternal = "Test external update note",
                Terms = new InvoiceTradingTerms
                {
                    Type = 2,
                    Interval = 5,
                    IntervalType = 1
                },
                Attachments = null,
                InvoiceNumber = string.Format("Inv{0}", Guid.NewGuid()),
                PurchaseOrderNumber = string.Format("PO{0}", Guid.NewGuid()),
                Summary = "Summary Update",
                RequiresFollowUp = true,
                BillingContactId = _invHelper.ShippingContactId, //swap the contacts
                ShippingContactId = _invHelper.BillingContactId,
                Tags = new List<string> { "updated header tag 1", "updated header tag 2" },
                SendEmailToContact = emailToContact
            };
        }

        private List<InvoiceTransactionLineItem> GetInsertItems(string layout, string tranType)
        {
            switch (layout)
            {
                case "S":
                    return new List<InvoiceTransactionLineItem>
                    {
                        new InvoiceTransactionLineItem
                        {
                            Description = "line item 1",
                            AccountId = tranType == "S" ? _invHelper.IncomeAccountId : _invHelper.ExpenseAccountId,
                            TaxCode = Constants.TaxCode.SaleInclGst,
                            TotalAmount = new decimal(10.00),
                            Tags = new List<string> {"item tag 1", "item tag 2"}
                        },
                        new InvoiceTransactionLineItem
                        {
                            Description = "line item 2",
                            AccountId = tranType == "S" ? _invHelper.IncomeAccountId : _invHelper.ExpenseAccountId,
                            TaxCode = Constants.TaxCode.SaleInputTaxed,
                            TotalAmount = new decimal(20.00),
                            Tags = new List<string> {"item tag 3", "item tag 4"}
                        }

                    };
                case "I":
                    return new List<InvoiceTransactionLineItem>
                    {
                        new InvoiceTransactionLineItem
                        {
                            Description = "line item 1",
                            TaxCode = Constants.TaxCode.SaleInclGst,
                            Quantity = 2,
                            UnitPrice = new decimal(15.00),
                            PercentageDiscount = new decimal(3.00),
                            InventoryId =  tranType == "S" ? _invHelper.InventorySaleItemId : _invHelper.InventoryPurchaseItemId,
                            Tags = new List<string> {"item tag 1", "item tag 2"}
                        },
                        new InvoiceTransactionLineItem
                        {
                            Description = "line item 2",
                            TaxCode = Constants.TaxCode.SaleInputTaxed,
                            Quantity = 3,
                            UnitPrice = new decimal(25.00),
                            PercentageDiscount = new decimal(0.00),
                            InventoryId = tranType == "S" ? _invHelper.InventorySaleItemId : _invHelper.InventoryPurchaseItemId,
                            Tags = new List<string> {"item tag 3", "item tag 4"}
                        }

                    };
                default:
                    return null;
            }
        }

        private InvoiceTradingTerms GetTradingTerms()
        {
            return new InvoiceTradingTerms
            {
                Type = 1,
                Interval = 3,
                IntervalType = 1
            };
        }
        private List<FileAttachmentInfo> GetAttachments()
        {
            return null;
        }
        private int? GetTemplateUid()
        {
            return null;
        }
        private Saasu.API.Core.Models.Email GetEmailMessage()
        {
            return null;
        }









        private static int? GetTestInvoiceId()
        {
            var response = new InvoicesProxy().GetInvoices(invoiceNumber: "TestInv1");
            return response.DataObject.Invoices[0].TransactionId;
        }

        private static FileAttachment CreateTestAttachment(bool createLargeAttachment = false)
        {
            // Create an attachment
            var attachmentName = string.Format("TestAttachment-{0}.txt", Guid.NewGuid().ToString());
            string attachmentData = null;
            if (createLargeAttachment)
            {
                var builder = new StringBuilder();
                builder.AppendFormat("This is a test attachment written at {0} {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
                // Ensure we have an attchment of around 100k

                for (var cnt = 0; cnt < 20240; cnt++)
                {
                    builder.Append("This is Some Data man!");
                }
                attachmentData = builder.ToString();
            }
            else
            {
                attachmentData = string.Format("This is a test attachment written at {0} {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
            }
            File.WriteAllText(attachmentName, attachmentData);

            var byteData = File.ReadAllBytes(attachmentName);
            FileAttachment attachment = new FileAttachment();
            attachment.Name = attachmentName;
            attachment.Description = "Test Only";
            attachment.AttachmentData = byteData;
            attachment.AllowExistingAttachmentToBeOverwritten = true;
            attachment.ItemIdAttachedTo = Convert.ToInt32(TestInvoiceId);
            return attachment;
        }

        #endregion
    }
}

