using System.Linq;
using Bogus;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Accounts;
using Saasu.API.Core.Models.Company;
using Saasu.API.Core.Models.Contacts;
using Saasu.API.Core.Models.Invoices;
using Saasu.API.Core.Models.Items;
using Xunit;

namespace Saasu.API.Client.IntegrationTests.Helpers
{
    public class SearchHelper
    {
        public SearchHelper()
        {
            CreateTestAccounts();
        }

        private int _incomeAccountId;
        private int _expenseAccountId;
        private int _bankAccountId;
        private int _assetAccountId;

        private int _billingContactId;
        private int _shippingContactId;

        private void CreateTestAccounts()
        {
            var accountProxy = new AccountProxy();
            if (_incomeAccountId == 0)
            {
                var result1 = accountProxy.InsertAccount(new AccountDetail()
                {
                    AccountType = Constants.AccountType.Income,
                    Name = "Income Account " + " " + System.Guid.NewGuid(),
                    IsBankAccount = false,
                    Currency = "AUD",
                });

                _incomeAccountId = result1.DataObject.InsertedEntityId;
            }

            if (_expenseAccountId == 0)
            {
                var result3 = accountProxy.InsertAccount(new AccountDetail()
                {
                    AccountType = Constants.AccountType.Expense,
                    Name = "Expense Account " + " " + System.Guid.NewGuid(),
                    IsBankAccount = false,
                    Currency = "AUD",
                });

                _expenseAccountId = result3.DataObject.InsertedEntityId;
            }

            if (_assetAccountId == 0)
            {
                var result5 = accountProxy.InsertAccount(new AccountDetail()
                {
                    AccountType = Constants.AccountType.Asset,
                    Name = "Asset Account " + " " + System.Guid.NewGuid(),
                    IsBankAccount = false,
                    Currency = "AUD",
                });

                _assetAccountId = result5.DataObject.InsertedEntityId;
            }

            if (_bankAccountId == 0)
            {
                var acctname = "Bank Account " + " " + System.Guid.NewGuid();

                var result6 = accountProxy.InsertAccount(new AccountDetail()
                {
                    AccountType = Constants.AccountType.Asset,
                    BSB = "111111",
                    Number = "22222222",
                    Name = acctname,
                    BankAccountName = acctname,
                    IsBankAccount = true,
                    Currency = "AUD",
                });

                _bankAccountId = result6.DataObject.InsertedEntityId;
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

        public (ItemDetail ItemDetail, int ItemId) CreatSaleInventoryItem(string description = null)
        {
            var proxy = new ItemProxy();
            var fakerItem = new Faker<ItemDetail>()
                .RuleFor(i => i.AssetAccountId, f => _assetAccountId)
                .RuleFor(i => i.BuyingPrice, f => decimal.Parse(f.Commerce.Price()))
                .RuleFor(i => i.SellingPrice, f => decimal.Parse(f.Commerce.Price()))
                .RuleFor(i => i.CurrentValue, (f, i) => decimal.Parse(f.Commerce.Price(1m, i.BuyingPrice.Value)))
                .RuleFor(i => i.Code, f => f.Commerce.Ean8())
                .RuleFor(i => i.PrimarySupplierItemCode, f => f.Commerce.Ean8())
                .RuleFor(i => i.DefaultReOrderQuantity, f => f.Random.Number(1, 10))
                .RuleFor(i => i.Description, f => description ?? f.Commerce.ProductName())
                .RuleFor(i => i.IsActive, f => true)
                .RuleFor(i => i.IsBought, f =>false)
                .RuleFor(i => i.IsBuyingPriceIncTax, f => false)
                .RuleFor(i => i.IsSold, f => true)
                .RuleFor(i => i.SaleIncomeAccountId, f => _incomeAccountId)
                .RuleFor(i => i.Type, Constants.InvoiceLayout.Item);

            var item = fakerItem.Generate();
            var response = proxy.InsertItem(item);
            Assert.True(response.IsSuccessfull, "Failed to create inventory item test data");
            
            return (item, response.DataObject.InsertedItemId);
        }

        public (CompanyDetail CompanyDetail, int CompanyId) CreateCompany(string companyName = null)
        {
            var fakerCompany = new Faker<CompanyDetail>("en_AU")
                .RuleFor(c => c.Name, (f, c) => companyName ?? f.Company.CompanyName())
                .RuleFor(c => c.TradingName, (f, c) => c.Name + " " + f.Company.CompanySuffix())
                .RuleFor(c => c.CompanyEmail, (f, c) => f.Internet.Email(c.Name))
                .RuleFor(c => c.Website, (f, c) => f.Internet.Url())
                .RuleFor(c => c.Abn, f => f.Random.Long(100000000, 99999999).ToString())
                .RuleFor(c => c.LongDescription, (f, c) => f.Company.CatchPhrase());

            var company = fakerCompany.Generate();
            var companyProxy = new CompanyProxy();
            var response = companyProxy.InsertCompany(company);

            Assert.True(response.IsSuccessfull, "Failed to create organization for customer contact test data.");

            return (company, response.DataObject.InsertedCompanyId);
        }

        public (Contact ContactDetail, int ContactId) CreateContact(int? companyId = null, string companyName = null)
        {
            var contactFake = new Faker<Contact>()
                .RuleFor(c => c.CompanyId, companyId ?? CreateCompany(companyName).CompanyId)
                .RuleFor(c => c.GivenName, (f, c) => f.Name.FirstName())
                .RuleFor(c => c.FamilyName, (f, c) => f.Name.LastName())
                .RuleFor(c => c.EmailAddress, (f, c) => f.Internet.Email(c.GivenName, c.FamilyName));

            var proxy = new ContactProxy();
            var contact = contactFake.Generate();
            var result = proxy.InsertContact(contact);
            Assert.True(result.IsSuccessfull, "Failed to add customer contact test data.");
            return (contact, result.DataObject.InsertedContactId);
        }

        public Faker<InvoiceTransactionDetail> GetSaleInvoiceFaker(string summary = null)
        {
            var billContact = CreateContact();
            var shipContact = CreateContact();

            var saleItemFaker = new Faker<InvoiceTransactionLineItem>()
                .RuleFor(i => i.Tags, f => f.Random.WordsArray(f.Random.Number(1, 3)).ToList())
                .RuleFor(i => i.TotalAmount, f => decimal.Parse(f.Commerce.Price()))
                .RuleFor(i => i.TaxCode, Constants.TaxCode.SaleInclGst)
                .RuleFor(i => i.AccountId, f => _incomeAccountId)
                .RuleFor(i => i.Description, f => f.Commerce.ProductName());


            var invoiceDetailFaker = new Faker<InvoiceTransactionDetail>()
                .RuleFor(i => i.LineItems, f => saleItemFaker.Generate(f.Random.Number(1, 3)))
                .RuleFor(i => i.NotesInternal, f => f.Lorem.Sentence(3))
                .RuleFor(i => i.NotesExternal, f => f.Lorem.Sentence(3))
                .RuleFor(i => i.Terms, GetTradingTerms())
                .RuleFor(i => i.Currency, "AUD")
                //.RuleFor(i => i.PurchaseOrderNumber, Constants.AutoNumber)
                .RuleFor(i => i.InvoiceNumber, Constants.AutoNumber)
                .RuleFor(i => i.InvoiceType, Constants.InvoiceType.TaxInvoice)
                .RuleFor(i => i.TransactionType, Constants.InvoiceLayout.Service)
                .RuleFor(i => i.Layout, Constants.InvoiceLayout.Service)
                .RuleFor(i => i.Summary, f => summary ?? f.Lorem.Sentence(3))
                .RuleFor(i => i.TotalAmount, (f, i) => i.LineItems.Sum(s => s.TotalAmount))
                .RuleFor(i => i.IsTaxInc, true)
                .RuleFor(i => i.TransactionDate, f => f.Date.Recent(3))
                .RuleFor(i => i.BillingContactId, f => billContact.ContactId)
                .RuleFor(i => i.BillingContactFirstName, f => billContact.ContactDetail.GivenName)
                .RuleFor(i => i.BillingContactLastName, f => billContact.ContactDetail.FamilyName)
                .RuleFor(i => i.ShippingContactId, f => shipContact.ContactId)
                .RuleFor(i => i.ShippingContactFirstName, f => shipContact.ContactDetail.GivenName)
                .RuleFor(i => i.ShippingContactLastName, f => shipContact.ContactDetail.FamilyName)
                .RuleFor(i => i.Tags, f => f.Random.WordsArray(f.Random.Number(1, 3)).ToList());

            return invoiceDetailFaker;

        }
        
        public Faker<InvoiceTransactionDetail> GetPurchaseInvoiceFaker(string summary = null)
        {
            var billContact = CreateContact();
            var shipContact = CreateContact();

            var purchaseItemFaker = new Faker<InvoiceTransactionLineItem>()
                .RuleFor(i => i.Tags, f => f.Random.WordsArray(f.Random.Number(1, 3)).ToList())
                .RuleFor(i => i.TotalAmount, f => decimal.Parse(f.Commerce.Price()))
                .RuleFor(i => i.TaxCode, Constants.TaxCode.SaleInclGst)
                .RuleFor(i => i.AccountId, f => _incomeAccountId)
                .RuleFor(i => i.Description, f => f.Commerce.ProductName());


            var invoiceDetailFaker = new Faker<InvoiceTransactionDetail>()
                .RuleFor(i => i.LineItems, f => purchaseItemFaker.Generate(f.Random.Number(1, 3)))
                .RuleFor(i => i.NotesInternal, f => f.Lorem.Sentence(3))
                .RuleFor(i => i.NotesExternal, f => f.Lorem.Sentence(3))
                .RuleFor(i => i.Terms, GetTradingTerms())
                .RuleFor(i => i.Currency, "AUD")
                .RuleFor(i => i.PurchaseOrderNumber, Constants.AutoNumber)
                //.RuleFor(i => i.InvoiceNumber, Constants.AutoNumber)
                .RuleFor(i => i.InvoiceType, Constants.InvoiceType.TaxInvoice)
                .RuleFor(i => i.TransactionType, Constants.InvoiceLayout.Purchase)
                .RuleFor(i => i.Layout, Constants.InvoiceLayout.Service)
                .RuleFor(i => i.Summary, f => summary ?? f.Lorem.Sentence(3))
                .RuleFor(i => i.TotalAmount, (f, i) => i.LineItems.Sum(s => s.TotalAmount))
                .RuleFor(i => i.IsTaxInc, true)
                .RuleFor(i => i.TransactionDate, f => f.Date.Recent(3))
                .RuleFor(i => i.BillingContactId, f => billContact.ContactId)
                .RuleFor(i => i.BillingContactFirstName, f => billContact.ContactDetail.GivenName)
                .RuleFor(i => i.BillingContactLastName, f => billContact.ContactDetail.FamilyName)
                .RuleFor(i => i.ShippingContactId, f => shipContact.ContactId)
                .RuleFor(i => i.ShippingContactFirstName, f => shipContact.ContactDetail.GivenName)
                .RuleFor(i => i.ShippingContactLastName, f => shipContact.ContactDetail.FamilyName)
                .RuleFor(i => i.Tags, f => f.Random.WordsArray(f.Random.Number(1, 3)).ToList());

            return invoiceDetailFaker;

        }
    }
}