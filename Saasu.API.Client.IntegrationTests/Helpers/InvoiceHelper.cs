using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ola.RestClient.Dto;
using Ola.RestClient.Proxies;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Attachments;
using Saasu.API.Core.Models.Invoices;
using InvoiceProxy = Saasu.API.Client.Proxies.InvoiceProxy;

namespace Saasu.API.Client.IntegrationTests
{
    public class InvoiceHelper
    {
        public InvoiceHelper()
        {

        }

        public void CreateTestData()
        {
            //note - don't change the order of these calls, they are dependent.
            CreateTestContacts();
            CreateTestAccounts();
            CreateTestInventoryItems();
            CreatetestInvoices();
        }

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

        public int? InvoiceId1
        {
            get
            {
                return _invoice1Id;
            }
        }

        public string InvoiceId1Summary
        {
            get
            {
                return _invoice1IdSummary;
            }
        }

        public int? InvoiceId2
        {
            get
            {
                return _invoice2Id;
            }
        }

        public string InvoiceId2Summary
        {
            get
            {
                return _invoice2IdSummary;
            }
        }

        public int? InvoiceId3
        {
            get
            {
                return _invoice3Id;
            }
        }

        public string InvoiceId3Summary
        {
            get
            {
                return _invoice3IdSummary;
            }
        }

        private int _BillingContactId;
        private int _ShippingContactId;
        private int _IncomeAccountId;
        private int _IncomeAccountId2;
        private int _ExpenseAccountId;
        private int _ExpenseAccountId2;
        private int _BankAccountId;
        private int _AssetAccountId;
        private int _InventorySaleItemId;
        private int _InventorySaleItemId2;
        private int _InventoryPurchaseItemId;
        private int _InventoryPurchaseItemId2;

        private int _invoice1Id;
        private int _invoice2Id;
        private int _invoice3Id;
        private string _invoice1IdSummary;
        private string _invoice2IdSummary;
        private string _invoice3IdSummary;

        private const string AutoNumber = "<auto number>";

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
                    FamilyName = "BillingContact",
                    Email = "test@test.com"
                };

                Ola.RestClient.Proxies.CrudProxy proxy = new Ola.RestClient.Proxies.ContactProxy();
                proxy.Insert(dto);
            }
            response = new ContactsProxy().GetContacts(givenName: "TestAPIInvoice", familyName: "BillingContact");

            _BillingContactId = response.DataObject.Contacts[0].Id.Value;


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

            response = new ContactsProxy().GetContacts(givenName: "TestAPIInvoice", familyName: "ShippingContact");
            _ShippingContactId = response.DataObject.Contacts[0].Id.Value;

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
        public void CreatetestInvoices()
        {
            var inv1 = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, actuallyInsertAndVerifyResponse: true);
            Assert.IsNotNull(inv1);
            Assert.Greater(inv1.TransactionId, 0);
            _invoice1Id = Convert.ToInt32(inv1.TransactionId);
            _invoice1IdSummary = inv1.Summary;

            var inv2 = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, actuallyInsertAndVerifyResponse: true);
            Assert.IsNotNull(inv2);
            Assert.Greater(inv2.TransactionId, 0);
            _invoice2Id = Convert.ToInt32(inv2.TransactionId);
            _invoice2IdSummary = inv2.Summary;

            var inv3 = GetTestInsertInvoice(invoiceLayout: InvoiceLayout.Service, actuallyInsertAndVerifyResponse: true);
            Assert.IsNotNull(inv3);
            Assert.Greater(inv3.TransactionId, 0);
            _invoice3Id = Convert.ToInt32(inv3.TransactionId);
            _invoice3IdSummary = inv3.Summary;
        }

        private static int? GetTestInvoiceId()
        {
            var response = new InvoicesProxy().GetInvoices(invoiceNumber: "TestInv1");
            return response.DataObject.Invoices[0].TransactionId;
        }

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
                Summary = summary ?? "Summary " + Guid.NewGuid(),
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
    }
}
