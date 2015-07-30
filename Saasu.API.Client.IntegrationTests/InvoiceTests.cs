using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web.Script.Serialization;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ola.RestClient.Dto;
using Ola.RestClient.Proxies;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Models.Invoices;
using System.IO;
using Saasu.API.Core.Models.Attachments;
using Saasu.API.Core.Framework;
using InvoiceProxy = Saasu.API.Client.Proxies.InvoiceProxy;
using NUnit.Framework;
using Saasu.API.Client.Proxies;

namespace Saasu.API.Client.IntegrationTests
{
    [TestFixture]
    public class InvoiceTests
    {
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

        private static int _BillingContactId;
        private static int _ShippingContactId;
        private static int _IncomeAccountId;
        private static int _IncomeAccountId2;
        private static int _ExpenseAccountId;
        private static int _ExpenseAccountId2;
        private static int _BankAccountId;
        private static int _AssetAccountId;
        private static int _InventorySaleItemId;
        private static int _InventorySaleItemId2;
        private static int _InventoryPurchaseItemId;
        private static int _InventoryPurchaseItemId2;

        private static int _invoice1Id;
        private static int _invoice2Id;
        private static int _invoice3Id;

        private const string AutoNumber = "<auto number>";
        private const string ItemLayoutForbiddenMessage = " Check the response returned as this may be because your current subscription does not allow item layouts.";
        private const string MultiCurrencyForbiddenMessage = " Check the response returned as this may be because your current subscription does not allow multi currency or because it is turned off.";

        public InvoiceTests()
        {
            //note - don't change the order of these calls, they are dependent.
            CreateTestContacts();
            CreateTestAccounts();
            CreateTestInventoryItems();
            CreatetestInvoices();
        }

        [Test]
        public void ShouldGetInvoicesForKnownFile()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new InvoicesProxy(accessToken);
            var response = proxy.GetInvoices();

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsNotNull(response.DataObject.Invoices);
            Assert.IsNotNull(response.DataObject._links);
            Assert.IsTrue(response.DataObject.Invoices.Count > 0);
            Assert.IsTrue(response.DataObject._links.Count > 0);
        }

        [Test]
        public void ShouldGetOnlyFirstInvoiceForKnownFile()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new InvoicesProxy(accessToken);
            var response = proxy.GetInvoices();

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsNotNull(response.DataObject.Invoices);
            Assert.IsTrue(response.DataObject.Invoices.Count > 0);
        }

        [Test]
        public void ShouldGetOneInvoiceForKnownFile()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();
            var proxy = new InvoiceProxy(accessToken);
            var response = proxy.GetInvoice(Convert.ToInt32(_invoice1Id));

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
        }

        [Test]
        public void ShouldNotBeAbleToAddAttachmentWithInvalidInvoiceId()
        {
            var attachment = CreateTestAttachment();
            attachment.ItemIdAttachedTo = 0; // invalid invoice id

            var addResponse = new InvoiceProxy().AddAttachment(attachment);

            Assert.IsNotNull(addResponse, "No response when adding an attachment");
            Assert.IsFalse(addResponse.IsSuccessfull, "Adding an attachment succeeded BUT it should have failed as it had an invalid invoice id of 0");
        }

        [Test]
        public void ShouldBeAbleToAddSmallAttachmentUsingWsAccessKey()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new InvoicesProxy(accessToken);
            var response = proxy.GetInvoices();

            Assert.IsNotNull(response, "No response when getting invoices");
            Assert.IsTrue(response.IsSuccessfull, "Getting invoices failed. STatus code: " + ((int)response.StatusCode).ToString());
            Assert.IsTrue(response.DataObject.Invoices.Count > 0, "Number of invoices returned was not greater than 0");

            _testInvoiceId = response.DataObject.Invoices[0].TransactionId;

            var attachment = CreateTestAttachment();
            attachment.ItemIdAttachedTo = response.DataObject.Invoices[0].TransactionId.GetValueOrDefault();

            var addResponse = new InvoiceProxy().AddAttachment(attachment);

            Assert.IsNotNull(addResponse, "No response when adding an attachment");
            Assert.IsTrue(addResponse.IsSuccessfull, "Adding an attachment failed. STatus code: " + ((int)addResponse.StatusCode).ToString());
        }

        [Test]
        public void ShouldBeAbleToAddLargeAttachmentUsingWsAccessKey()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new InvoicesProxy(accessToken);
            var response = proxy.GetInvoices();

            Assert.IsNotNull(response, "No response when getting invoices");
            Assert.IsTrue(response.IsSuccessfull, "Getting invoices failed. STatus code: " + ((int)response.StatusCode).ToString());
            Assert.IsTrue(response.DataObject.Invoices.Count > 0, "Number of invoices returned was not greater than 0");

            _testInvoiceId = response.DataObject.Invoices[0].TransactionId;

            var attachment = CreateTestAttachment(true);
            attachment.ItemIdAttachedTo = response.DataObject.Invoices[0].TransactionId.GetValueOrDefault();

            var addResponse = new InvoiceProxy().AddAttachment(attachment);

            Assert.IsNotNull(addResponse, "No response when adding an attachment");
            Assert.IsTrue(addResponse.IsSuccessfull, "Adding an attachment failed. STatus code: " + ((int)addResponse.StatusCode).ToString());
        }

        [Test]
        public void ShouldBeAbleToGetInfoOnAllAttachmentsUsingWsAccessKey()
        {
            var proxy = new InvoiceProxy();

            var attachment = CreateTestAttachment();

            //Attach invoice Id to attachment and insert attachment.
            attachment.ItemIdAttachedTo = Convert.ToInt32(_invoice1Id);
            var insertResponse = proxy.AddAttachment(attachment);

            var attachmentId = insertResponse.DataObject.Id;

            Assert.IsTrue(attachmentId > 0);

            var getResponse = proxy.GetAllAttachmentsInfo(_invoice1Id);

            Assert.IsNotNull(getResponse);
            Assert.IsTrue(getResponse.IsSuccessfull);
            Assert.IsNotNull(getResponse.DataObject);
            Assert.IsNotNull(getResponse.DataObject.Attachments);
            Assert.IsTrue(getResponse.DataObject.Attachments.Count > 0);
        }

        [Test]
        public void ShouldBeAbleToGetAnAllAttachmentUsingOAuth()
        {
            ShouldBeAbleToAddSmallAttachmentUsingWsAccessKey();  // Ensure we add anattachment

            var accessToken = TestHelper.SignInAndGetAccessToken();
            var proxy = new InvoiceProxy(accessToken);
            var response = proxy.GetAllAttachmentsInfo(Convert.ToInt32(TestInvoiceId));

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsNotNull(response.DataObject.Attachments);
            Assert.IsTrue(response.DataObject.Attachments.Count > 0);

            var attachmentResponse = proxy.GetAttachment(response.DataObject.Attachments[0].Id);
            Assert.IsNotNull(attachmentResponse);
            Assert.IsTrue(attachmentResponse.IsSuccessfull);
        }

        [Test]
        public void ShouldBeAbleToDeleteAnAttachment()
        {
            ShouldBeAbleToAddSmallAttachmentUsingWsAccessKey();  // Ensure we add an attachment

            var proxy = new InvoiceProxy();
            var response = proxy.GetAllAttachmentsInfo(Convert.ToInt32(TestInvoiceId));
            Assert.IsTrue(response.IsSuccessfull && response.DataObject != null && response.DataObject.Attachments.Count > 0, "Getting all attachments failed. Status Code: " + ((int)response.StatusCode).ToString());

            var attachmentResponse = proxy.DeleteAttachment(response.DataObject.Attachments[0].Id);
            Assert.IsNotNull(attachmentResponse, "No response when deleting an attachment");
            Assert.IsTrue(attachmentResponse.IsSuccessfull, "Adding an attachment was not successfull. Status Code:" + ((int)attachmentResponse.StatusCode).ToString());
        }

        [Test]
        public void GetInvoicesAll()
        {
            AssertInvoiceProxy();
        }

        [Test]
        public void GetInvoicesOnePageOneRecord()
        {
            var response = AssertInvoiceProxy(pageNumber: 1, pageSize: 1);
            Assert.AreEqual(1, response.DataObject.Invoices.Count, "Paging of 1 page and 1 record is returning the wrong number of records");
        }

        [Test]
        public void GetInvoicesFilterOnDates()
        {
            //use a year ago and tomorrow as date filters to make sure the test invoice is picked up.
            AssertInvoiceProxy(fromDate: DateTime.Now.AddYears(-1), toDate: DateTime.Now.AddDays(1));
        }

        [Test]
        public void GetInvoicesFilterOnModifiedDates()
        {
            //use a year ago and tomorrow as date filters to make sure the test contact Carl O'Brien is picked up.
            AssertInvoiceProxy(lastModifiedFromDate: DateTime.Now.AddYears(-1), lastModifiedToDate: DateTime.Now.AddDays(1));
        }

        [Test]
        public void GetInvoicesFilterOnInvoiceNumber()
        {
            var invNumber = string.Format("Inv{0}", Guid.NewGuid());

            //Create and insert test invoice.
            GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: invNumber, actuallyInsertAndVerifyResponse: true);

            var response = AssertInvoiceProxy(invoiceNumber: invNumber);

            Assert.AreEqual(1, response.DataObject.Invoices.Count, "Incorrect number of invoices found.");
        }

        [Test]
        public void GetInvoicesFilterOnPaymentStatus()
        {
            AssertInvoiceProxy(paymentStatus: (int)PaymentStatusType.Unpaid);
        }

        [Test]
        public void GetInvoicesFilterOnBillingContactId()
        {
            //Get Id of test contact associated with the invoice.
            var contactResponse = ContactTests.VerifyTestContactExistsOrCreate(contactType: Ola.RestClient.ContactType.Customer);

            var billingContactId = contactResponse.DataObject.Contacts[0].Id;

            //Create and insert test invoices for billing contact.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", emailContact: false, billingContactId: billingContactId);
            var invoice2 = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", emailContact: false, billingContactId: billingContactId);

            var proxy = new InvoiceProxy();
            proxy.InsertInvoice(invoice);
            proxy.InsertInvoice(invoice2);

            AssertInvoiceProxy(billingContactId: contactResponse.DataObject.Contacts[0].Id);
        }

        [Test]
        public void GetInvoicesFilterOnInvoiceStatus()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceType: InvoiceType.SaleOrder, invoiceStatus: InvoiceStatusType.Order.ToQueryParameter());
            new InvoiceProxy().InsertInvoice(invoice);

            AssertInvoiceProxy(invoiceStatus: InvoiceStatusType.Order.ToQueryParameter());
        }

        [Test]
        public void GetSingleInvoiceWithInvoiceId()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var insertResponse = new InvoiceProxy().InsertInvoice(invoice);

            Assert.IsTrue(insertResponse.IsSuccessfull);

            var insertResult = insertResponse.DataObject;

            var tranid = insertResult.InsertedEntityId;

            Assert.IsTrue(tranid > 0);

            var response = new InvoiceProxy().GetInvoice(tranid);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsNotNull(response.DataObject._links);
            Assert.IsTrue(response.DataObject._links.Count > 0);
        }

        [Test]
        public void GetInvoicesPageSize()
        {
            var proxy = new InvoicesProxy();
            var response = proxy.GetInvoices(pageNumber: 1, pageSize: 2);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.AreEqual(response.DataObject.Invoices.Count, 2);
        }

        [Test]
        public void GetInvoicesSecondPage()
        {
            var proxy = new InvoicesProxy();
            var response = proxy.GetInvoices(pageSize: 2);

            var idsFromPage1 = response.DataObject.Invoices.Select(i => i.TransactionId).ToList();

            response = proxy.GetInvoices(pageNumber: 2);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);

            response.DataObject.Invoices.ForEach(i => Assert.IsFalse(idsFromPage1.Contains(i.TransactionId)));
        }


        #region Insert Tests

        /// <summary>
        /// Insert
        ///		- Sale
        ///		- Service layout
        ///		- No Email
        ///		- Defined invoice number
        /// </summary>
        [Test]
        public void InsertSaleWithServiceItemsNoEmailToContact()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var response = new InvoiceProxy().InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.IsNull(results.GeneratedInvoiceNumber);
            Assert.IsFalse(results.SentToContact);
            Assert.AreNotEqual(results.InsertedEntityId, 0);
            Assert.AreNotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.AreNotEqual(results.LastUpdatedId, null);
            Assert.AreNotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getResponse = new InvoiceProxy().GetInvoice(results.InsertedEntityId);

            Assert.IsNotNull(getResponse.DataObject);

            VerifyInvoicesAreEqual(invoice, getResponse.DataObject);
        }

        [Test]
        public void InsertSaleAndCanUpdateUsingReturnedLastUpdateId()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.IsNull(results.GeneratedInvoiceNumber);
            Assert.IsFalse(results.SentToContact);
            Assert.AreNotEqual(results.InsertedEntityId, 0);
            Assert.AreNotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.AreNotEqual(results.LastUpdatedId, null);
            Assert.AreNotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getResponse = proxy.GetInvoice(results.InsertedEntityId);
            Assert.IsNotNull(getResponse.DataObject);

            // Do an Update
            var getInvoice = getResponse.DataObject;
            getInvoice.Summary = "Update 1";
            var updateResponse = proxy.UpdateInvoice(getInvoice.TransactionId.Value, getInvoice);
            Assert.IsTrue(updateResponse.IsSuccessfull);

            // Using the LastUpdatedId returned from the previous update, do another update
            getInvoice.Summary = "Update 2";
            getInvoice.LastUpdatedId = updateResponse.DataObject.LastUpdatedId;
            var updateResponse2 = proxy.UpdateInvoice(getInvoice.TransactionId.Value, getInvoice);
            Assert.IsTrue(updateResponse2.IsSuccessfull);

        }

        /// <summary>
        /// Insert
        ///		- Sale
        ///		- Item layout
        ///		- Email
        ///		- Auto invoice number
        /// </summary>
        [Test]
        public void InsertSaleWithItemLayoutEmailToContactAndAutoNumberAndAutoPopulateFxRate()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Item, transactionType: "S", emailContact: true, autoPopulateFxRate: true);

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull, string.Format("Inserting an item layout invoice has failed.{0}", ItemLayoutForbiddenMessage));

            var results = response.DataObject;

            Assert.IsNotNull(results.GeneratedInvoiceNumber);
            Assert.IsTrue(results.SentToContact);
            Assert.AreNotEqual(results.InsertedEntityId, 0);
            Assert.AreNotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.IsNotNull(results.LastUpdatedId);
            Assert.AreNotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(results.InsertedEntityId);

            Assert.IsNotNull(getResponse.DataObject);

            VerifyInvoicesAreEqual(invoice, getResponse.DataObject);
        }

        [Test]
        public void InsertSaleWithAutoPopulateFxRateShouldNotUsePassedInFxRate()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Item, transactionType: "S", autoPopulateFxRate: true, fxRate: 99);

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull, string.Format("Inserting an item layout invoice has failed.{0}", ItemLayoutForbiddenMessage));

            var results = response.DataObject;

            Assert.AreNotEqual(results.InsertedEntityId, 0);
            Assert.AreNotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.IsNotNull(results.LastUpdatedId);
            Assert.AreNotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(results.InsertedEntityId);

            Assert.IsNotNull(getResponse.DataObject);
            Assert.AreNotEqual(99, getResponse.DataObject.FxRate);
        }

        /// <summary>
        /// Insert
        ///		- Purchase
        ///		- Service layout
        ///		- No Email
        ///		- Defined PO number
        /// </summary>
        [Test]
        public void InsertPurchaseWithServiceItemsNoEmailToContact()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "P", emailContact: false, purchaseOrderNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var response = new InvoiceProxy().InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.IsNull(results.GeneratedInvoiceNumber);
            Assert.IsFalse(results.SentToContact);
            Assert.AreNotEqual(results.InsertedEntityId, 0);
            Assert.AreNotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.AreNotEqual(results.LastUpdatedId, null);
            Assert.AreNotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getResponse = new InvoiceProxy().GetInvoice(results.InsertedEntityId);

            Assert.IsNotNull(getResponse.DataObject);

            VerifyInvoicesAreEqual(invoice, getResponse.DataObject);
        }

        /// <summary>
        /// Insert
        ///		- Purchase
        ///		- Item layout
        ///		- Email
        ///		- Auto PO number
        /// </summary>
        [Test]
        public void InsertPurchaseWithItemLayoutEmailToContactAndAutoNumber()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Item, transactionType: "P", emailContact: true);

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull, string.Format("Inserting an item layout invoice has failed.{0}", ItemLayoutForbiddenMessage));

            var results = response.DataObject;

            Assert.IsNotNull(results.GeneratedInvoiceNumber);
            Assert.IsTrue(results.SentToContact);
            Assert.AreNotEqual(results.InsertedEntityId, 0);
            Assert.AreNotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.IsNotNull(results.LastUpdatedId);
            Assert.AreNotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(results.InsertedEntityId);

            Assert.IsNotNull(getResponse.DataObject);

            VerifyInvoicesAreEqual(invoice, getResponse.DataObject);
        }

        [Test]
        public void InsertDifferentCurrencyForMultiCurrencyFile()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", currency: "USD", fxRate: new decimal(1.50));

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull, string.Format("Inserting a multi currency invoice has failed.{0}", MultiCurrencyForbiddenMessage));

            var results = response.DataObject;

            Assert.AreNotEqual(results.InsertedEntityId, 0);
            Assert.AreNotEqual(results.UtcLastModified, DateTime.MinValue);
            Assert.AreNotEqual(results.LastUpdatedId, null);
            Assert.AreNotEqual(results.LastUpdatedId.Trim(), string.Empty);

            //get invoice.
            var getResponse = new InvoiceProxy().GetInvoice(results.InsertedEntityId);

            Assert.IsNotNull(getResponse.DataObject);

            VerifyInvoicesAreEqual(invoice, getResponse.DataObject);
            Assert.AreEqual(invoice.FxRate, getResponse.DataObject.FxRate);
        }

        [Test]
        public void InsertSaleWithQuickPayment()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            invoice.QuickPayment = new InvoiceQuickPaymentDetail
            {
                DatePaid = DateTime.Now.AddDays(-4),
                DateCleared = DateTime.Now.AddDays(-3),
                BankedToAccountId = _BankAccountId,
                Amount = new decimal(10.00),
                Reference = "Test quick payment reference",
                Summary = "Test quick payment summary"
            };

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull);
            Assert.AreNotEqual(response.DataObject.InsertedEntityId, 0);

            //get invoice.
            var getResponse = proxy.GetInvoice(response.DataObject.InsertedEntityId);

            Assert.IsNotNull(getResponse.DataObject.PaymentCount);
            Assert.AreEqual(getResponse.DataObject.PaymentCount, (Int16)1);
            Assert.AreEqual(getResponse.DataObject.AmountPaid, new decimal(10.00));
            Assert.AreEqual(getResponse.DataObject.AmountOwed, (getResponse.DataObject.TotalAmount - getResponse.DataObject.AmountPaid));
        }

        [Test]
        public void InsertPurchaseWithQuickPayment()
        {
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "P", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            invoice.QuickPayment = new InvoiceQuickPaymentDetail
            {
                DatePaid = DateTime.Now.AddDays(-4),
                DateCleared = DateTime.Now.AddDays(-3),
                BankedToAccountId = _BankAccountId,
                Amount = new decimal(10.00),
                Reference = "Test quick payment reference",
                Summary = "Test quick payment summary"
            };

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull);
            Assert.AreNotEqual(response.DataObject.InsertedEntityId, 0);

            //get invoice.
            var getResponse = proxy.GetInvoice(response.DataObject.InsertedEntityId);

            Assert.IsNotNull(getResponse.DataObject.PaymentCount);
            Assert.AreEqual(getResponse.DataObject.PaymentCount, (Int16)1);
            Assert.AreEqual(getResponse.DataObject.AmountPaid, new decimal(10.00));
            Assert.AreEqual(getResponse.DataObject.AmountOwed, (getResponse.DataObject.TotalAmount - getResponse.DataObject.AmountPaid));
        }

        #endregion

        #region Update Tests
        /// <summary>
        /// Insert
        ///		- Sale
        ///		- Service layout
        ///		- No Email sent
        /// </summary>
        [Test]
        public void UpdateSaleWithServiceLayoutAllFieldsUpdated()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));
            var response = new InvoiceProxy().InsertInvoice(invoice);
            Assert.IsTrue(response.IsSuccessfull);

            var insertResult = response.DataObject;
            Assert.AreNotEqual(insertResult.InsertedEntityId, 0);
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
					AccountId = _IncomeAccountId2,
					TaxCode = TaxCode.SaleInputTaxed,
					TotalAmount = new decimal(100.00),                    
					Tags = new List<string> { "update item tag 1", "update item tag 2" }
				} 
			};

            var updateResponse = new InvoiceProxy().UpdateInvoice(tranId, updateInvoice);
            var updateResult = updateResponse.DataObject;

            Assert.IsFalse(updateResult.SentToContact);
            Assert.AreEqual(Convert.ToDateTime(updateResult.UtcLastModified).Date, DateTime.UtcNow.Date);
            Assert.IsNotNull(updateResult.LastUpdatedId);
            Assert.AreNotEqual(updateResult.LastUpdatedId.Trim(), string.Empty);

            //Get invoice after update.
            var getResponse = new InvoiceProxy().GetInvoice(tranId);
            Assert.IsNotNull(getResponse.DataObject);

            //Compare updated with original inserted invoice (which also now contains the updated changes).
            VerifyInvoicesAreEqual(insertedInvoice, getResponse.DataObject);
        }

        /// <summary>
        /// Insert
        ///		- Sale
        ///		- Service layout
        ///		- Email
        /// </summary>
        [Test]
        public void UpdateSaleWithItemLayoutAllFieldsUpdated()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Item, transactionType: "S", emailContact: true, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));
            var response = new InvoiceProxy().InsertInvoice(invoice);
            Assert.IsTrue(response.IsSuccessfull, string.Format("Inserting an item layout invoice has failed.{0}", ItemLayoutForbiddenMessage));

            var insertResult = response.DataObject;
            Assert.AreNotEqual(insertResult.InsertedEntityId, 0);
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
							TaxCode = TaxCode.SaleInputTaxed,
							Quantity = 20,
							UnitPrice = new decimal(200.00),
							PercentageDiscount = new decimal(15.00),
							InventoryId =  _InventorySaleItemId2,
							Tags = new List<string> {"updated item tag 1", "updated item tag 2"}
							//Attributes = GetItemAttributes()
						},
			};

            var updateResponse = new InvoiceProxy().UpdateInvoice(tranId, updateInvoice);
            var updateResult = updateResponse.DataObject;

            Assert.IsTrue(updateResult.SentToContact);
            Assert.AreEqual(Convert.ToDateTime(updateResult.UtcLastModified).Date, DateTime.UtcNow.Date);
            Assert.IsNotNull(updateResult.LastUpdatedId);
            Assert.AreNotEqual(updateResult.LastUpdatedId.Trim(), string.Empty);

            //Get invoice after update.
            var getResponse = new InvoiceProxy().GetInvoice(tranId);
            Assert.IsNotNull(getResponse.DataObject);

            //Compare updated with original inserted invoice (which also now contains the updated changes).
            VerifyInvoicesAreEqual(insertedInvoice, getResponse.DataObject);
        }

        /// <summary>
        /// Insert
        ///		- Sale
        ///		- Service layout
        ///		- Email sent with insert, but not update.
        /// </summary>
        [Test]
        public void UpdatePurchaseWithServiceLayoutAllFieldsUpdated()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "P", emailContact: true, purchaseOrderNumber: string.Format("TestInv{0}", Guid.NewGuid()));
            var response = new InvoiceProxy().InsertInvoice(invoice);
            Assert.IsTrue(response.IsSuccessfull);

            var insertResult = response.DataObject;
            Assert.AreNotEqual(insertResult.InsertedEntityId, 0);
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
					AccountId = _IncomeAccountId2,
					TaxCode = TaxCode.SaleInputTaxed,
					TotalAmount = new decimal(100.00),
					Tags = new List<string> { "update item tag 1", "update item tag 2" }
				} 
			};

            var updateResponse = new InvoiceProxy().UpdateInvoice(tranId, updateInvoice);
            var updateResult = updateResponse.DataObject;

            Assert.IsFalse(updateResult.SentToContact);
            Assert.AreEqual(Convert.ToDateTime(updateResult.UtcLastModified).Date, DateTime.UtcNow.Date);
            Assert.IsNotNull(updateResult.LastUpdatedId);
            Assert.AreNotEqual(updateResult.LastUpdatedId.Trim(), string.Empty);

            //Get invoice after update.
            var getResponse = new InvoiceProxy().GetInvoice(tranId);
            Assert.IsNotNull(getResponse.DataObject);

            //Compare updated with original inserted invoice (which also now contains the updated changes).
            VerifyInvoicesAreEqual(updateInvoice, getResponse.DataObject);
        }

        /// <summary>
        /// Update
        ///		- Purchase
        ///		- Service layout
        ///		- No email insert, but email sent in update.
        /// </summary>
        [Test]
        public void UpdatePurchaseithItemLayoutAllFieldsUpdated()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Item, transactionType: "P", emailContact: false, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));
            var response = new InvoiceProxy().InsertInvoice(invoice);
            Assert.IsTrue(response.IsSuccessfull, string.Format("Inserting an item layout invoice has failed.{0}", ItemLayoutForbiddenMessage));

            var insertResult = response.DataObject;
            Assert.AreNotEqual(insertResult.InsertedEntityId, 0);
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
							TaxCode = TaxCode.SaleInputTaxed,
							Quantity = 20,
							UnitPrice = new decimal(200.00),
							PercentageDiscount = new decimal(15.00),
							InventoryId =  _InventoryPurchaseItemId2,
							Tags = new List<string> {"upadated item tag 1", "updated item tag 2"}							
						}
			};

            var updateResponse = new InvoiceProxy().UpdateInvoice(tranId, updateInvoice);
            var updateResult = updateResponse.DataObject;

            Assert.IsTrue(updateResult.SentToContact);
            Assert.AreEqual(Convert.ToDateTime(updateResult.UtcLastModified).Date, DateTime.UtcNow.Date);
            Assert.IsNotNull(updateResult.LastUpdatedId);
            Assert.AreNotEqual(updateResult.LastUpdatedId.Trim(), string.Empty);

            //Get invoice after update.
            var getResponse = new InvoiceProxy().GetInvoice(tranId);
            Assert.IsNotNull(getResponse.DataObject);

            //Compare updated with original inserted invoice (which also now contains the updated changes).
            VerifyInvoicesAreEqual(updateInvoice, getResponse.DataObject);

            // The invoice was updated with SendEmailToContact request, so the new response will have set the SentToContact flag whereas
            // the original inserted invoice will not.
            Assert.AreNotEqual(insertedInvoice.SentToContact, getResponse.DataObject.SentToContact);
        }

        [Test]
        public void UpdateDifferentCurrencyForMultiCurrencyFile()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S");
            var response = new InvoiceProxy().InsertInvoice(invoice);
            Assert.IsTrue(response.IsSuccessfull);

            var insertResult = response.DataObject;
            Assert.AreNotEqual(insertResult.InsertedEntityId, 0);
            var tranId = insertResult.InsertedEntityId;

            //Get the inserted invoice to pick up any default values which may have been assigned.
            var getInsertedResponse = new InvoiceProxy().GetInvoice(tranId);
            var insertedInvoice = getInsertedResponse.DataObject;

            //Update the currency of the invoice.
            insertedInvoice.Currency = "USD";
            insertedInvoice.FxRate = 1.5M;

            var updateResponse = new InvoiceProxy().UpdateInvoice(tranId, insertedInvoice);
            Assert.IsTrue(updateResponse.IsSuccessfull, string.Format("Updating invoice with a different currency has failed.{0}", MultiCurrencyForbiddenMessage));

            var updateResult = updateResponse.DataObject;
            Assert.IsFalse(updateResult.SentToContact);
            Assert.AreEqual(Convert.ToDateTime(updateResult.UtcLastModified).Date, DateTime.UtcNow.Date);
            Assert.IsNotNull(updateResult.LastUpdatedId);
            Assert.AreNotEqual(updateResult.LastUpdatedId.Trim(), string.Empty);

            //Get invoice after update.
            var getUpdatedResponse = new InvoiceProxy().GetInvoice(tranId);
            Assert.IsNotNull(getUpdatedResponse.DataObject);

            //Compare updated with original inserted invoice (which also now contains the updated changes).
            VerifyInvoicesAreEqual(insertedInvoice, getUpdatedResponse.DataObject);
            Assert.AreEqual(insertedInvoice.FxRate, getUpdatedResponse.DataObject.FxRate);
        }
        #endregion

        #region Delete Tests

        [Test]
        public void DeleteSaleInvoiceWithServiceLayout()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", emailContact: true, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.AreNotEqual(results.InsertedEntityId, 0);
            var tranId = results.InsertedEntityId;

            var invProxy = new InvoiceProxy();

            var deleteResponse = invProxy.DeleteInvoice(tranId);

            Assert.IsTrue(deleteResponse.IsSuccessfull);
            //get invoice, verify it has been deleted.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(tranId);

            Assert.IsNull(getResponse.DataObject);
        }

        [Test]
        public void DeleteSaleInvoiceWithItemLayout()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Item, transactionType: "S", emailContact: true, invoiceNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.AreNotEqual(results.InsertedEntityId, 0);
            var tranId = results.InsertedEntityId;

            var invProxy = new InvoiceProxy();

            var deleteResponse = invProxy.DeleteInvoice(tranId);

            Assert.IsTrue(deleteResponse.IsSuccessfull);
            //get invoice, verify it has been deleted.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(tranId);

            Assert.IsNull(getResponse.DataObject);
        }

        /// <summary>
        /// Defined purchase otder number
        /// </summary>
        [Test]
        public void DeletePurchaseInvoiceWithServiceLayout()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "P", emailContact: true, purchaseOrderNumber: string.Format("TestInv{0}", Guid.NewGuid()));

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.AreNotEqual(results.InsertedEntityId, 0);
            var tranId = results.InsertedEntityId;

            var invProxy = new InvoiceProxy();

            var deleteResponse = invProxy.DeleteInvoice(tranId);

            Assert.IsTrue(deleteResponse.IsSuccessfull);
            //get invoice, verify it has been deleted.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(tranId);

            Assert.IsNull(getResponse.DataObject);
        }

        /// <summary>
        /// Auto numbered purchase otder number
        /// </summary>
        [Test]
        public void DeletePurchaseInvoiceWithItemLayout()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Item, transactionType: "S", emailContact: true);

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsTrue(response.IsSuccessfull);

            var results = response.DataObject;

            Assert.AreNotEqual(results.InsertedEntityId, 0);
            var tranId = results.InsertedEntityId;

            var invProxy = new InvoiceProxy();

            var deleteResponse = invProxy.DeleteInvoice(tranId);

            Assert.IsTrue(deleteResponse.IsSuccessfull);
            //get invoice, verify it has been deleted.
            var getProxy = new InvoiceProxy();
            var getResponse = getProxy.GetInvoice(tranId);

            Assert.IsNull(getResponse.DataObject);
        }

        #endregion

        #region Validation Tests

        [Test]
        public void InvalidCurrencyInsert()
        {
            //Insert invoice.
            var invoice = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, transactionType: "S", currency: "TEST");

            var proxy = new InvoiceProxy();
            var response = proxy.InsertInvoice(invoice);

            Assert.IsFalse(response.IsSuccessfull);
            Assert.IsNotNull(response.RawResponse);
            Assert.IsTrue(response.RawResponse.Contains("Please include a valid currency"));
            Assert.IsNotNull(response.ReasonCode);
            Assert.AreEqual(response.ReasonCode.Trim().ToLower(), "bad request");
        }



        #endregion

        #region Validation

        private void VerifyInvoicesAreEqual(InvoiceTransactionDetail inv1, InvoiceTransactionDetail inv2)
        {
            Assert.AreEqual(inv1.NotesInternal, inv2.NotesInternal, "Internal Notes are different");
            Assert.AreEqual(inv1.NotesExternal, inv2.NotesExternal, "External Notes are different");
            Assert.AreEqual(inv1.TemplateId, inv2.TemplateId, "TemplateIds are different");
            Assert.AreEqual(inv1.Currency, inv2.Currency, "Currency is different");
            Assert.AreEqual(inv1.TransactionType, inv2.TransactionType, "TransactionType is different");
            Assert.AreEqual(inv1.AutoPopulateFxRate, inv2.AutoPopulateFxRate, "AutoPopulateFxRate is different");

            switch (inv1.TransactionType)
            {
                case "S":
                    {
                        if (inv1.InvoiceNumber == AutoNumber)
                        {
                            Assert.AreNotEqual(inv1.InvoiceNumber, inv2.InvoiceNumber, "InvoiceNumber are equal but expected to be different");
                        }
                        else
                        {
                            Assert.AreEqual(inv1.InvoiceNumber, inv2.InvoiceNumber, "InvoiceNumber is different");
                        }
                    }
                    break;
                case "P":
                    {
                        if (inv1.PurchaseOrderNumber == AutoNumber)
                        {
                            Assert.AreNotEqual(inv1.PurchaseOrderNumber, inv2.PurchaseOrderNumber, "Purchase Order Number are the same but expected to be different");
                        }
                        else
                        {
                            Assert.AreEqual(inv1.PurchaseOrderNumber, inv2.PurchaseOrderNumber, "Purchase Order number is different");
                        }
                    }
                    break;
            }

            // Cannot assert this here as it gets changed base don whether consumer changes the flag, or consumer asks email to be sent in whch
            // case this flag gets set as well
            //Assert.AreEqual(inv1.SentToContact, inv2.SentToContact);

            Assert.AreEqual(inv1.RequiresFollowUp, inv2.RequiresFollowUp, "RequiresFollowUp is different");

            Assert.AreEqual(inv1.InvoiceType, inv2.InvoiceType, "InvoiceType is different");
            Assert.AreEqual(inv1.Layout, inv2.Layout, "Layout is different");
            Assert.AreEqual(inv1.Summary, inv2.Summary, "Summary is different");
            //Assert.AreEqual(inv1.TotalAmountExcludingTax, inv2.TotalAmountExcludingTax);-- leave out as calcs currently come from items
            //Assert.AreEqual(inv1.TotalTaxAmount, inv2.TotalTaxAmount); -- leave out as calcs currently come from items
            //Assert.AreEqual(inv1.TotalAmountInclTax, inv2.TotalAmountInclTax); -- leave out as calcs currently come from items
            var inv1Date = Convert.ToDateTime(inv1.TransactionDate).Date;
            var inv2Date = Convert.ToDateTime(inv2.TransactionDate).Date;
            Assert.AreEqual(inv1Date.Year, inv2Date.Year, "Year is different");
            Assert.AreEqual(inv1Date.Month, inv2Date.Month, "Month is different");
            Assert.AreEqual(inv1Date.Day, inv2Date.Day, "Day is different");
            //Assert.AreEqual(Convert.ToDateTime(inv1.TransactionDate).Date, Convert.ToDateTime(inv2.TransactionDate).Date);
            Assert.AreEqual(inv1.BillingContactId.GetValueOrDefault(), inv2.BillingContactId.GetValueOrDefault(), "BillingContactId is different");
            Assert.AreEqual(inv1.ShippingContactId.GetValueOrDefault(), inv2.ShippingContactId.GetValueOrDefault(), "ShippingContactId is different");

            VerifyInvoiceItemsAreEqual(inv1.Layout, inv1.LineItems, inv2.LineItems);
            VerifyInvoiceTermsAreEqual(inv1.Terms, inv2.Terms);
            VerifyInvoiceAttachmentsAreEqual(inv1.Attachments, inv2.Attachments);
            VerifyInvoiceEmailMessagesAreEqual(inv1.EmailMessage, inv2.EmailMessage);
        }

        private void VerifyInvoiceItemsAreEqual(string layout, List<InvoiceTransactionLineItem> list1, List<InvoiceTransactionLineItem> list2)
        {
            if (list1 == null || list2 == null)
            {
                Assert.IsTrue(list1 == null && list2 == null, "One list is NULL and the other is not. Expected both to be NULL");
                return;
            }

            Assert.AreEqual(list1.Count, list2.Count, "Number of items are different");

            if (layout == "S")
            {
                foreach (var item in list1)
                {
                    var itemInList2 = list2.SingleOrDefault(i => i.Description == item.Description &&
                        i.AccountId == item.AccountId &&
                        i.TaxCode == item.TaxCode
                        );

                    Assert.IsNotNull(itemInList2, "Service item differs in first list");
                }

                foreach (var item in list2)
                {
                    var itemInList1 = list1.SingleOrDefault(i => i.Description == item.Description &&
                        i.AccountId == item.AccountId &&
                        i.TaxCode == item.TaxCode
                        );

                    Assert.IsNotNull(itemInList1, "Service item differs in second list");
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


                    Assert.IsNotNull(itemInList2, "Line item differs in first list");
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

                    Assert.IsNotNull(itemInList1, "Line item differs in second list");
                }
            }
        }

        private void VerifyInvoiceEmailMessagesAreEqual(Saasu.API.Core.Models.Email msg1, Saasu.API.Core.Models.Email msg2)
        {
            if (msg1 == null || msg2 == null)
            {
                Assert.IsNull(msg1, "Expected first email message to be null");
                Assert.IsNull(msg2, "Expected second email message to be null");
                return;
            }

            Assert.AreEqual(msg1.To, msg2.To, "Email 'to' is not equal");
            Assert.AreEqual(msg1.From, msg2.From, "Email 'from' is not equal");
            Assert.AreEqual(msg1.Cc, msg2.Cc, "Email 'Cc' is not equal");
            Assert.AreEqual(msg1.Bcc, msg2.Bcc, "Email 'Bcc' is not equal");
            Assert.AreEqual(msg1.Subject, msg2.Subject, "Email 'subject' is not equal");
            Assert.AreEqual(msg1.Body, msg2.Body, "Email 'body' is not equal");
        }

        private void VerifyInvoiceAttachmentsAreEqual(List<FileAttachmentInfo> list1, List<FileAttachmentInfo> list2)
        {
            if (list1 == null || list2 == null || list1.Count == 0 || list2.Count == 0)
            {
                //this is because an insert may have sent attachments as null but a GET will return them as empy list.
                Assert.IsTrue(list1 == null || list1.Count == 0, "Expected no items but list1 had some");
                Assert.IsTrue(list2 == null || list2.Count == 0, "Expected no items but list2 had some");
                return;
            }

            foreach (var detail in list1)
            {
                var detailInList2 = list2.SingleOrDefault(d => d.Id == detail.Id && d.Name == detail.Name && d.Description == detail.Description);
                Assert.IsNotNull(detailInList2, "Attachments do not match");
            }

            foreach (var detail in list2)
            {
                var detailInList1 = list1.SingleOrDefault(d => d.Id == detail.Id && d.Name == detail.Name && d.Description == detail.Description);
                Assert.IsNotNull(detailInList1, "Attachments do not match");
            }
        }

        private void VerifyInvoiceTermsAreEqual(InvoiceTradingTerms terms1, InvoiceTradingTerms terms2)
        {
            if (terms1 == null || terms2 == null)
            {
                Assert.IsNull(terms1, "First Terms is not NULL");
                Assert.IsNull(terms2, "Second terms is different");
                return;
            }

            Assert.AreEqual(terms1.Type, terms2.Type, "Terms type is different");
            Assert.AreEqual(terms1.Interval, terms2.Interval, "Terms Interval is different");
            Assert.AreEqual(terms1.IntervalType, terms2.IntervalType, "Terms Interval type is different");
        }

        #endregion

        public static ProxyResponse<InvoiceTransactionSummaryResponse> AssertInvoiceProxy(int? pageNumber = null, int? pageSize = null, DateTime? fromDate = null, DateTime? toDate = null,
            DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null, string invoiceNumber = null, string transactionType = null, int? paymentStatus = null,
            int? billingContactId = null, string invoiceStatus = null, string tags = null, string tagFilterType = null)
        {
            var response = new InvoicesProxy().GetInvoices(pageNumber, pageSize, fromDate, toDate, lastModifiedFromDate, lastModifiedToDate, invoiceNumber, transactionType, paymentStatus,
                billingContactId, invoiceStatus, tags, tagFilterType);

            Assert.IsNotNull(response, "Invoice response is null");
            Assert.IsTrue(response.IsSuccessfull, "Invoice response not successfull");
            Assert.IsNotNull(response.DataObject, "No data in invoice reposnse");
            Assert.IsNotNull(response.DataObject.Invoices, "Empty invoices in response");
            Assert.IsNotNull(response.DataObject._links, "Empty hypermedia links in response");
            Assert.IsTrue(response.DataObject._links.Count > 0, "No hypermedia links in response");
            Assert.IsTrue(response.DataObject.Invoices.Count > 0, "No invoices in response");

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
                    BillingContactId = billingContactId ?? _BillingContactId,
                    ShippingContactId = shippingContactId ?? _ShippingContactId,
                    FxRate = fxRate,
                    AutoPopulateFxRate = autoPopulateFxRate,
                    InvoiceStatus = invoiceStatus,
                    Tags = tags ?? new List<string> { "invoice header tag 1", "invoice header tag 2" }
                };

            if (actuallyInsertAndVerifyResponse)
            {
                var response = new InvoiceProxy().InsertInvoice(invDetail);

                Assert.IsNotNull(response, "Inserting an invoice did not return a response");
                Assert.IsTrue(response.IsSuccessfull, "Inserting an invoice was not successfull. Status code: " + ((int)response.StatusCode).ToString());
                Assert.IsNotNull(response.RawResponse, "No raw response returned as part of inserting an invoice");

                var serialized = response.DataObject;

                Assert.IsTrue(serialized.InsertedEntityId > 0, "Invoice insert did not return an InsertedEntityId > 0");

                invDetail.TransactionId = serialized.InsertedEntityId;
            }

            return invDetail;
        }

        private InvoiceTransactionDetail GetUpdatedInvoice(int tranId, string lastUpdatedId, bool emailToContact = false, InvoiceTransactionDetail invoiceToCopyFrom = null)
        {
            Assert.IsTrue(tranId > 0, "A valid transactionId mus be passed into the GetUpdatedSaleInvoice method");

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
                BillingContactId = _ShippingContactId, //swap the contacts
                ShippingContactId = _BillingContactId,
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
							AccountId = tranType == "S" ? _IncomeAccountId : _ExpenseAccountId,
							TaxCode = TaxCode.SaleInclGst,
							TotalAmount = new decimal(10.00),
							Tags = new List<string> {"item tag 1", "item tag 2"}
						},
						new InvoiceTransactionLineItem
						{
							Description = "line item 2",
							AccountId = tranType == "S" ? _IncomeAccountId : _ExpenseAccountId,
							TaxCode = TaxCode.SaleInputTaxed,
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
							TaxCode = TaxCode.SaleInclGst,
							Quantity = 2,
							UnitPrice = new decimal(15.00),
							PercentageDiscount = new decimal(3.00),
							InventoryId =  tranType == "S" ? _InventorySaleItemId : _InventoryPurchaseItemId,
							Tags = new List<string> {"item tag 1", "item tag 2"}
						},
						new InvoiceTransactionLineItem
						{
							Description = "line item 2",
							TaxCode = TaxCode.SaleInputTaxed,
							Quantity = 3,
							UnitPrice = new decimal(25.00),
							PercentageDiscount = new decimal(0.00),
							InventoryId = tranType == "S" ? _InventorySaleItemId : _InventoryPurchaseItemId,
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

        private void CreateTestContacts()
        {
            //Billing contact.
            var response = new ContactsProxy().GetContacts(givenName: "TestAPIInvoice", familyName: "BillingContact");

            if (response.DataObject.Contacts.Count == 0)
            {
                var dto = new Ola.RestClient.Dto.ContactDto
                    {
                        GivenName = "TestAPIInvoice",
                        FamilyName = "BillingContact"
                    };

                Ola.RestClient.Proxies.CrudProxy proxy = new Ola.RestClient.Proxies.ContactProxy();
                proxy.Insert(dto);
            }
            else
            {
                _BillingContactId = response.DataObject.Contacts[0].Id.Value;
            }

            //Shipping contact.
            response = new ContactsProxy().GetContacts(givenName: "TestAPIInvoice", familyName: "ShippingContact");

            if (response.DataObject.Contacts.Count == 0)
            {
                var dto = new Ola.RestClient.Dto.ContactDto
                {
                    GivenName = "TestAPIInvoice",
                    FamilyName = "ShippingContact"
                };

                Ola.RestClient.Proxies.CrudProxy proxy = new Ola.RestClient.Proxies.ContactProxy();
                proxy.Insert(dto);
            }
            else
            {
                _ShippingContactId = response.DataObject.Contacts[0].Id.Value;
            }
        }

        private void CreateTestAccounts()
        {
            if (_IncomeAccountId == 0)
            {
                var dto = new TransactionCategoryDto
                {
                    Type = AccountType.Income,
                    Name = "Income Account " + " " + System.Guid.NewGuid()
                };

                new Ola.RestClient.Proxies.TransactionCategoryProxy().Insert(dto);
                _IncomeAccountId = dto.Uid;
            }

            if (_IncomeAccountId2 == 0)
            {
                var dto = new TransactionCategoryDto
                {
                    Type = AccountType.Income,
                    Name = "Income Account " + " " + System.Guid.NewGuid()
                };

                new Ola.RestClient.Proxies.TransactionCategoryProxy().Insert(dto);
                _IncomeAccountId2 = dto.Uid;
            }

            if (_ExpenseAccountId == 0)
            {
                var dto = new TransactionCategoryDto
                {
                    Type = AccountType.Expense,
                    Name = "Expense Account " + " " + System.Guid.NewGuid()
                };

                new Ola.RestClient.Proxies.TransactionCategoryProxy().Insert(dto);
                _ExpenseAccountId = dto.Uid;
            }

            if (_ExpenseAccountId2 == 0)
            {
                var dto = new TransactionCategoryDto
                {
                    Type = AccountType.Expense,
                    Name = "Expense Account2 " + " " + System.Guid.NewGuid()
                };

                new Ola.RestClient.Proxies.TransactionCategoryProxy().Insert(dto);
                _ExpenseAccountId2 = dto.Uid;
            }

            if (_AssetAccountId == 0)
            {
                var dto = new TransactionCategoryDto
                {
                    Type = AccountType.Asset,
                    Name = "Asset Account " + " " + System.Guid.NewGuid()
                };

                new Ola.RestClient.Proxies.TransactionCategoryProxy().Insert(dto);
                _AssetAccountId = dto.Uid;
            }

            if (_BankAccountId == 0)
            {
                var acctname = "Bank Account " + " " + System.Guid.NewGuid();

                var dto = new BankAccountDto
                {
                    BSB = "111111",
                    AccountNumber = "22222222",
                    Type = AccountType.Income,
                    Name = acctname,
                    DisplayName = acctname
                };

                new Ola.RestClient.Proxies.BankAccountProxy().Insert(dto);
                _BankAccountId = dto.Uid;
            }
        }

        private void CreateTestInventoryItems()
        {
            var proxy = new InventoryItemProxy();

            //sale items.
            if (_InventorySaleItemId == 0)
            {
                var dto = new InventoryItemDto
                {
                    AssetAccountUid = _AssetAccountId,
                    AverageCost = new decimal(5.00),
                    BuyingPrice = new decimal(4.00),
                    Code = string.Format("Inv{0}", Guid.NewGuid().ToString().Substring(0, 20)),
                    CurrentValue = new decimal(6.00),
                    DefaultReOrderQuantity = 2,
                    Description = "Test inventory sale item",
                    IsActive = true,
                    IsBought = false,
                    IsBuyingPriceIncTax = false,
                    IsSold = true,
                    SaleIncomeAccountUid = _IncomeAccountId
                };

                proxy.Insert(dto);
                _InventorySaleItemId = dto.Uid;
            }

            if (_InventorySaleItemId2 == 0)
            {
                var dto = new InventoryItemDto
                {
                    AssetAccountUid = _AssetAccountId,
                    AverageCost = new decimal(25.00),
                    BuyingPrice = new decimal(40.00),
                    Code = string.Format("Inv{0}", Guid.NewGuid().ToString().Substring(0, 20)),
                    CurrentValue = new decimal(16.00),
                    DefaultReOrderQuantity = 20,
                    Description = "updated Test inventory sale item",
                    IsActive = true,
                    IsBought = false,
                    IsBuyingPriceIncTax = false,
                    IsSold = true,
                    SaleIncomeAccountUid = _IncomeAccountId2
                };

                proxy.Insert(dto);
                _InventorySaleItemId2 = dto.Uid;
            }

            //purchase items.
            if (_InventoryPurchaseItemId == 0)
            {
                var dto = new InventoryItemDto
                {
                    AssetAccountUid = _AssetAccountId,
                    AverageCost = new decimal(5.00),
                    BuyingPrice = new decimal(4.00),
                    Code = string.Format("Inv{0}", Guid.NewGuid().ToString().Substring(0, 20)),
                    CurrentValue = new decimal(6.00),
                    DefaultReOrderQuantity = 2,
                    Description = "Test inventory purchase item",
                    IsActive = true,
                    IsBought = true,
                    IsBuyingPriceIncTax = true,
                    PurchaseExpenseAccountUid = _ExpenseAccountId
                };

                proxy.Insert(dto);
                _InventoryPurchaseItemId = dto.Uid;
            }

            if (_InventoryPurchaseItemId2 == 0)
            {
                var dto = new InventoryItemDto
                {
                    AssetAccountUid = _AssetAccountId,
                    AverageCost = new decimal(50.00),
                    BuyingPrice = new decimal(40.00),
                    Code = string.Format("Inv{0}", Guid.NewGuid().ToString().Substring(0, 20)),
                    CurrentValue = new decimal(60.00),
                    DefaultReOrderQuantity = 20,
                    Description = "Updated test inventory purchase item",
                    IsActive = true,
                    IsBought = true,
                    IsBuyingPriceIncTax = true,
                    PurchaseExpenseAccountUid = _ExpenseAccountId2
                };

                proxy.Insert(dto);
                _InventoryPurchaseItemId2 = dto.Uid;
            }
        }

        //For test cases where we need at least a few invoices to exist.
        private void CreatetestInvoices()
        {
            var inv1 = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, actuallyInsertAndVerifyResponse: true);
            Assert.IsNotNull(inv1);
            Assert.Greater(inv1.TransactionId, 0);
            _invoice1Id = Convert.ToInt32(inv1.TransactionId);

            var inv2 = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, actuallyInsertAndVerifyResponse: true);
            Assert.IsNotNull(inv2);
            Assert.Greater(inv2.TransactionId, 0);
            _invoice2Id = Convert.ToInt32(inv2.TransactionId);

            var inv3 = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, actuallyInsertAndVerifyResponse: true);
            Assert.IsNotNull(inv3);
            Assert.Greater(inv3.TransactionId, 0);
            _invoice3Id = Convert.ToInt32(inv3.TransactionId);
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
                
                for (var cnt=0; cnt < 20240; cnt++ )
                {
                    builder.Append("This is Some Data man!");
                }
                attachmentData = builder.ToString();
            } else
            {
                attachmentData = string.Format("This is a test attachment written at {0} {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
            }
            File.WriteAllText(attachmentName, attachmentData );

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

