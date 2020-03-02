using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models;
using Saasu.API.Core.Models.Company;
using Saasu.API.Core.Models.Contacts;
using Xunit;

namespace Saasu.API.Client.IntegrationTests
{
    public class ContactHelper
    {


        public ContactHelper(bool initData)
        {
            if (initData)
            {
                GetOrCreateContactCustomer();
                GetOrCreateContractorContact();
                GetOrCreatePartnerContact();
                GetOrCreateSupplierContact();
            }
        }

        public int GetOrCreateContact(string firstName, string lastName, string email)
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: firstName, familyName: lastName);
            if (response.IsSuccessfull && response.DataObject.Contacts.Any())
            {
                return response.DataObject.Contacts[0].Id.GetValueOrDefault();
            }


            var contact = new Contact()
            {
                GivenName = firstName,
                FamilyName = lastName,
                EmailAddress = email,
            };

            var result = proxy.InsertContact(contact);
            Assert.True(result.IsSuccessfull, "Failed to add customer contact test data.");
            return result.DataObject.InsertedContactId;

        }

        public InsertContactResult AddContact()
        {
            var contact = GetCompleteContact();
            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);
            return response.DataObject;
        }

        public Contact GetCompleteContact()
        {
            var tags = new List<string>();
            tags.Add("Test");
            tags.Add("Gloin");

            var emailAddress = Guid.NewGuid() + "@cook.com";

            return new Contact()
            {
                DirectDepositDetails = new DirectDepositDetails
                {
                    AcceptDirectDeposit = true, AccountBSB = "321-100", AccountName = "DD Account Name",
                    AccountNumber = "1234567899"
                },
                ChequeDetails = new ChequeDetails {AcceptCheque = true, ChequePayableTo = "me"},
                AutoSendStatement = false,
                BpayDetails = new BpayDetails {BillerCode = "45678", CRN = "98755"},
                CustomField1 = "CF1",
                CustomField2 = "CF2",
                DefaultPurchaseDiscount = 10M,
                DefaultSaleDiscount = 11M,
                EmailAddress = emailAddress,
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

        public Contact GetMinimalContact()
        {
            var tags = new List<string>();
            tags.Add("Apple");
            tags.Add("Pear");

            return new Contact()
            {
                GivenName = "James",
                FamilyName = "Cook",
                EmailAddress = "james@cook.com",
                HomePhone = "0213114444",
                PostalAddress = new Address()
                {
                    City = "Sydney",
                    Country = "Australia",
                    Postcode = "2000",
                    State = "NSW",
                    Street = "1st Street"
                },
                OtherAddress = new Address()
                {
                    City = "Brisbane",
                    Country = "Australia",
                    Postcode = "3000",
                    State = "QLD",
                    Street = "Other Street"
                },
                Tags = tags
            };
        }

        /// <summary>
        /// Add fictious customer contact. Currently have to use old Rest client API to insert record as this functionality not available in Web API yet.
        /// </summary>
        public  int GetOrCreateContactCustomer()
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: "carl", familyName: "o'neil", isCustomer: true,
                organisationName: "o'neil capital");
            if (response.IsSuccessfull && response.DataObject.Contacts.Any())
            {
                return response.DataObject.Contacts[0].Id.GetValueOrDefault();
            }

            var cResult = companyProxy.InsertCompany(new CompanyDetail()
            {
                Name = "O'Neil Capital"
            });

            Assert.True(cResult.IsSuccessfull, "Failed to create organization for customer contact test data.");

            var contact = new Contact()
            {
                Salutation = "Mr.",
                GivenName = "Carl",
                FamilyName = "O'Neil",
                ContactId = "GLD879",
                CustomField1 = "O'NeilC",
                IsCustomer = true,
                Tags = new List<string>() {"carlTag1", "carlTag2"},
                EmailAddress = "carl@oneilcapital.com",
                CompanyId = cResult.DataObject.InsertedCompanyId
            };

            var result = proxy.InsertContact(contact);
            Assert.True(result.IsSuccessfull, "Failed to add customer contact test data.");
            return result.DataObject.InsertedContactId;

        }

        /// <summary>
        /// Add fictious supplier contact. Currently have to use old Rest client API to insert record as this functionality not available in Web API yet.
        /// </summary>
        public  int GetOrCreateSupplierContact()
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: "jenny", familyName: "o'neil", isCustomer: true,
                organisationName: "O'Neil Supplier");
            if (response.IsSuccessfull && response.DataObject.Contacts.Any())
            {
                return response.DataObject.Contacts[0].Id.GetValueOrDefault();
            }

            var cResult = companyProxy.InsertCompany(new CompanyDetail()
            {
                Name = "O'Neil Supplier"
            });

            Assert.True(cResult.IsSuccessfull, "Failed to create organization for supplier contact test data.");

            var contact = new Contact()
            {
                Salutation = "Ms.",
                GivenName = "Jenny",
                FamilyName = "O'Neil",
                ContactId = "GLD880",
                CustomField1 = "O'NeilJ",
                IsSupplier = true,
                CompanyId = cResult.DataObject.InsertedCompanyId
            };
            var result = proxy.InsertContact(contact);
            Assert.True(result.IsSuccessfull, "Failed to add supplier contact test data.");
            return result.DataObject.InsertedContactId;
        }

        /// <summary>
        /// Add fictious partner contact. Currently have to use old Rest client API to insert record as this functionality not available in Web API yet.
        /// </summary>
        public  int GetOrCreatePartnerContact()
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: "brad", familyName: "o'neil", isCustomer: true,
                organisationName: "O'Neil Partner");
            if (response.IsSuccessfull && response.DataObject.Contacts.Any())
            {
                return response.DataObject.Contacts[0].Id.GetValueOrDefault();
            }

            var cResult = companyProxy.InsertCompany(new CompanyDetail()
            {
                Name = "O'Neil Partner"
            });

            Assert.True(cResult.IsSuccessfull, "Failed to create organization for partner contact test data.");

            var contact = new Contact()
            {
                Salutation = "Mr.",
                GivenName = "Brad",
                FamilyName = "O'Neil",
                ContactId = "GLD881",
                CustomField1 = "O'NeilB",
                IsPartner = true,
                CompanyId = cResult.DataObject.InsertedCompanyId
            };
            var result = proxy.InsertContact(contact);
            Assert.True(result.IsSuccessfull, "Failed to add partner contact test data.");
            return result.DataObject.InsertedContactId;
        }

        /// <summary>
        /// Add fictious contractor contact. Currently have to use old Rest client API to insert record as this functionality not available in Web API yet.
        /// </summary>
        public  int GetOrCreateContractorContact()
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: "kathy", familyName: "o'neil", isCustomer: true,
                organisationName: "O'Neil Contractor");
            if (response.IsSuccessfull && response.DataObject.Contacts.Any())
            {
                return response.DataObject.Contacts[0].Id.GetValueOrDefault();
            }

            var cResult = companyProxy.InsertCompany(new CompanyDetail()
            {
                Name = "O'Neil Contractor"
            });

            Assert.True(cResult.IsSuccessfull, "Failed to create organization for supplier contact test data.");

            var contact = new Contact()
            {
                Salutation = "Ms.",
                GivenName = "Kathy",
                FamilyName = "O'Neil",
                ContactId = "GLD882",
                CustomField1 = "O'NeilK",
                CompanyId = cResult.DataObject.InsertedCompanyId,
                IsContractor = true
            };
            var result = proxy.InsertContact(contact);
            Assert.True(result.IsSuccessfull, "Failed to add contractor contact test data.");
            return result.DataObject.InsertedContactId;
        }

        public  int GetOrCreateContactCustomer(string firstName, string lastName, string organisationName,
            string email)
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: firstName, familyName: lastName, isCustomer: true,
                organisationName: organisationName);
            if (response.IsSuccessfull && response.DataObject.Contacts.Any())
            {
                return response.DataObject.Contacts[0].Id.GetValueOrDefault();
            }

            var cResult = companyProxy.InsertCompany(new CompanyDetail()
            {
                Name = organisationName
            });

            Assert.True(cResult.IsSuccessfull, "Failed to create organization for customer contact test data.");

            var contact = new Contact()
            {
                GivenName = firstName,
                FamilyName = lastName,
                //ContactId = "GLD879",
                IsCustomer = true,
                Tags = new List<string>() {$"{firstName}Tag1", $"{lastName}Tag2"},
                EmailAddress = email,
                CompanyId = cResult.DataObject.InsertedCompanyId
            };

            var result = proxy.InsertContact(contact);
            Assert.True(result.IsSuccessfull, "Failed to add customer contact test data.");
            return result.DataObject.InsertedContactId;


        }
    }
}
