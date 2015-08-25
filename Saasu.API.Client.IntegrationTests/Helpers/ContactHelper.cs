using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models;
using Saasu.API.Core.Models.Contacts;

namespace Saasu.API.Client.IntegrationTests
{
    public class ContactHelper
    {

        public InsertContactResult AddContact()
        {
            var contact = GetCompleteContact();
            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);
            return response.DataObject;
        }

        private Contact GetCompleteContact()
        {
            var tags = new List<string>();
            tags.Add("Test");
            tags.Add("Gloin");

            var emailAddress = Guid.NewGuid() + "@cook.com";

            return new Contact()
            {
                DirectDepositDetails = new DirectDepositDetails { AcceptDirectDeposit = true, AccountBSB = "321-100", AccountName = "DD Account Name", AccountNumber = "1234567899" },
                ChequeDetails = new ChequeDetails { AcceptCheque = true, ChequePayableTo = "me" },
                AutoSendStatement = false,
                BpayDetails = new BpayDetails { BillerCode = "45678", CRN = "98755" },
                CustomField1 = "CF1",
                CustomField2 = "CF2",
                DefaultPurchaseDiscount = 10M,
                DefaultSaleDiscount = 11M,
                EmailAddress =  emailAddress,
                FamilyName = "Cook",
                Fax = "0254112244",
                GivenName = "John",
                HomePhone = "0211114444",
                IsActive = true,
                IsCustomer = true,
                IsContractor = false,
                IsSupplier = false,
                PostalAddress = new Address()
                {
                    City = "Sydney",
                    Country = "Australia",
                    Postcode = "2000",
                    State = "NSW",
                    Street = "Postal Street"
                },
                PrimaryPhone = "0277778888",
                OtherPhone = "0233323332",
                OtherAddress = new Address()
                {
                    City = "Brisbane",
                    Country = "Australia",
                    Postcode = "3000",
                    State = "QLD",
                    Street = "Other Street"
                },
                Salutation = "Mr.",
                PositionTitle = "Office Dude",
                SaleTradingTerms = new TradingTerms()
                {
                    TradingTermsInterval = 1,
                    TradingTermsIntervalType = 2,
                    TradingTermsType = 1
                },
                PurchaseTradingTerms = new TradingTerms()
                {
                    TradingTermsIntervalType = 1,
                    TradingTermsInterval = 3,
                    TradingTermsType = 1
                },
                Tags = tags
            };
        }
    }
}
