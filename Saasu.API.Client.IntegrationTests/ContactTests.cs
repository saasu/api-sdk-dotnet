using Xunit;

using Saasu.API.Client.Proxies;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Models;
using Saasu.API.Core.Models.Contacts;
using System;
using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using Saasu.API.Core.Models.Company;

namespace Saasu.API.Client.IntegrationTests
{
    public class ContactTests
    {
        public ContactTests()
        {
            GetOrCreateContactCustomer();
            GetOrCreateContractorContact();
            GetOrCreatePartnerContact();
            GetOrCreateSupplierContact();
        }


        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [Fact]
        public void GetContactsShouldReturnContactsUsingOAuth()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new ContactsProxy(accessToken);
            AssertContactProxy(proxy);
        }

        [Fact]
        public void GetContactsShouldReturnContactsUsingWsAccessKey()
        {
            var proxy = new ContactsProxy();
            AssertContactProxy(proxy);
        }

        [Fact]
        public void GetContactsFilterOnPersonNameAndOrganisationName()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy, givenName: "Carl", familyName: "O'Neil", organisationName: "O'Neil Capital");

            Assert.True(response.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");
        }

        [Fact]
        public void GetContactsAll()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy);
        }

        [Fact]
        public void GetContactsOnePageOneRecord()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy, pageNumber: 1, pageSize: 1);

            Assert.Equal(1, response.DataObject.Contacts.Count);
        }

        [Fact]
        public void GetContactsSecondPage()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy, pageNumber: 1, pageSize: 2);

            var idsFromPage1 = response.DataObject.Contacts.Select(c => c.Id).ToList();

            response = AssertContactProxy(proxy, pageNumber: 2, pageSize: 2);

            response.DataObject.Contacts.ForEach(c => Assert.False(idsFromPage1.Contains(c.Id), "Record(s) from page 1 were returned"));
        }

        [Fact]
        public void GetContactsFilterOnModifiedDates()
        {
            var proxy = new ContactsProxy();

            //use a year ago and tomorrow as date filters to make sure the test contact Carl O'Brien is picked up.
            AssertContactProxy(proxy, lastModifiedFromDate: DateTime.Now.AddYears(-1), lastModifiedToDate: DateTime.Now.AddDays(1));
        }

        [Fact]
        public void GetContactsFilterActiveCustomer()
        {
            var proxy = new ContactsProxy();

            AssertContactProxy(proxy, isActive: true, isCustomer: true);
        }

        [Fact]
        public void GetContactsFilterActiveSupplier()
        {
            var proxy = new ContactsProxy();

            AssertContactProxy(proxy, isActive: true, isSupplier: true);
        }

        [Fact]
        public void GetContactsFilterActivePartner()
        {
            var proxy = new ContactsProxy();

            AssertContactProxy(proxy, isActive: true, isPartner: true);
        }

        [Fact]
        public void GetContactsFilterActiveContractor()
        {
            var proxy = new ContactsProxy();

            AssertContactProxy(proxy, isActive: true, isContractor: true);
        }

        [Fact]
        public void GetContactsFilterOnEmail()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy, email: "carl@oneilcapital.com");

            Assert.True(response.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");
        }

        [Fact]
        public void GetContactsFilterOnContactId()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy, contactId: "GLD879");

            Assert.True(response.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");
        }

        [Fact]
        public void GetSingleContactWithContactId()
        {
            var contactsProxy = new ContactsProxy();

            //Find Test contact to obtain their Id.
            var contactsResponse = AssertContactProxy(contactsProxy, givenName: "carl", familyName: "o'neil", contactId: "GLD879");

            Assert.True(contactsResponse.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");

            var proxy = new ContactProxy();

            var id = contactsResponse.DataObject.Contacts[0].Id;
            Assert.NotNull(id);
            var response = proxy.GetContact(id.Value);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.NotNull(response.DataObject._links);
            Assert.True(response.DataObject._links.Count > 0);
            Assert.True(response.DataObject.Id == id);
        }

        [Fact]
        public void ModifyingContactsIsBooleanFieldsShouldNotAffectOtherIsBooleanFields()
        {
            var contactsProxy = new ContactsProxy();

            //Find Test contact to obtain their Id.
            var contactsResponse = AssertContactProxy(contactsProxy, givenName: "carl", familyName: "o'neil", contactId: "GLD879");

            Assert.True(contactsResponse.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");

            var proxy = new ContactProxy();

            var testId = contactsResponse.DataObject.Contacts[0].Id;
            var response = proxy.GetContact(testId.Value);
            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);

            // Now Modify the IsCustomer, IsContractor, IsSupplier, Is... fields and ensure setting one does not clear the other
            var testContact = response.DataObject;
            testContact.IsContractor = true;
            testContact.IsCustomer = true;
            testContact.IsPartner = true;
            testContact.IsSupplier = true;
            var updateResponse = proxy.UpdateContact(testContact, testContact.Id.Value);
            Assert.NotNull(updateResponse);
            Assert.True(updateResponse.IsSuccessfull);

            var updatedResponse1 = proxy.GetContact(testContact.Id.Value);
            Assert.True(updatedResponse1.IsSuccessfull);
            Assert.Equal(testContact.IsContractor, updatedResponse1.DataObject.IsContractor);
            Assert.Equal(testContact.IsCustomer, updatedResponse1.DataObject.IsCustomer);
            Assert.Equal(testContact.IsPartner, updatedResponse1.DataObject.IsPartner);
            Assert.Equal(testContact.IsSupplier, updatedResponse1.DataObject.IsSupplier);

            testContact.LastUpdatedId = updateResponse.DataObject.LastUpdatedId;
            testContact.IsSupplier = false;
            testContact.IsPartner = false;
            var updateResponse2 = proxy.UpdateContact(testContact, testContact.Id.Value);
            Assert.NotNull(updateResponse2);
            Assert.True(updateResponse2.IsSuccessfull);

            var updatedResponse2 = proxy.GetContact(testContact.Id.Value);
            Assert.True(updatedResponse2.IsSuccessfull);
            Assert.Equal(testContact.IsContractor, updatedResponse2.DataObject.IsContractor);
            Assert.Equal(testContact.IsCustomer, updatedResponse2.DataObject.IsCustomer);
            Assert.Equal(testContact.IsPartner, updatedResponse2.DataObject.IsPartner);
            Assert.Equal(testContact.IsSupplier, updatedResponse2.DataObject.IsSupplier);
        }

        public static ProxyResponse<ContactResponse> AssertContactProxy(ContactsProxy proxy, int? pageNumber = null, int? pageSize = null, DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null,
            string givenName = null, string familyName = null, string organisationName = null,
            bool? isActive = null, bool? isCustomer = null, bool? isSupplier = null, bool? isContractor = null, bool? isPartner = null,
            string tags = null, string tagSelection = null, string email = null, string contactId = null, string companyName = null, int? companyId = null)
        {
            var response = proxy.GetContacts(pageNumber, pageSize, lastModifiedFromDate, lastModifiedToDate,
                givenName, familyName, organisationName,
                isActive, isCustomer, isSupplier, isContractor, isPartner,
                tags, tagSelection, email, contactId, companyName, companyId);

            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.NotNull(response.DataObject.Contacts);
            Assert.NotNull(response.DataObject._links);
            Assert.True(response.DataObject._links.Count > 0);
            Assert.True(response.DataObject.Contacts.Count > 0);

            return response;
        }

        [Fact]
        public void InsertCompleteContact()
        {
            var contact = GetCompleteContact();

            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);

            Assert.True(response.IsSuccessfull);
            Assert.True(response.DataObject.InsertedContactId > 0);

            System.Threading.Thread.Sleep(1000); // Tags are saved through messagequeue, give time to be processed
            var insertedContact = proxy.GetContact(response.DataObject.InsertedContactId);
            Assert.NotNull(insertedContact.DataObject);

            Assert.Equal(contact.DirectDepositDetails.AcceptDirectDeposit, insertedContact.DataObject.DirectDepositDetails.AcceptDirectDeposit);
            Assert.Equal(contact.ChequeDetails.AcceptCheque, insertedContact.DataObject.ChequeDetails.AcceptCheque);
            Assert.Equal(contact.AutoSendStatement.HasValue, insertedContact.DataObject.AutoSendStatement.HasValue);
            Assert.Equal(contact.BpayDetails.BillerCode, insertedContact.DataObject.BpayDetails.BillerCode);
            Assert.Equal(contact.BpayDetails.CRN, insertedContact.DataObject.BpayDetails.CRN);
            Assert.Equal(contact.ChequeDetails.ChequePayableTo, insertedContact.DataObject.ChequeDetails.ChequePayableTo);
            Assert.Equal(contact.ContactId, insertedContact.DataObject.ContactId);
            Assert.Equal(contact.ContactManagerId, insertedContact.DataObject.ContactManagerId);
            Assert.Equal(contact.CustomField1, insertedContact.DataObject.CustomField1);
            Assert.Equal(contact.CustomField2, insertedContact.DataObject.CustomField2);
            Assert.Equal(contact.DefaultPurchaseDiscount, insertedContact.DataObject.DefaultPurchaseDiscount);
            Assert.Equal(contact.DefaultSaleDiscount, insertedContact.DataObject.DefaultSaleDiscount);
            Assert.Equal(contact.DirectDepositDetails.AccountBSB, insertedContact.DataObject.DirectDepositDetails.AccountBSB);
            Assert.Equal(contact.DirectDepositDetails.AccountName, insertedContact.DataObject.DirectDepositDetails.AccountName);
            Assert.Equal(contact.DirectDepositDetails.AccountNumber, insertedContact.DataObject.DirectDepositDetails.AccountNumber);
            Assert.Equal(contact.EmailAddress, insertedContact.DataObject.EmailAddress);
            Assert.Equal(contact.FamilyName, insertedContact.DataObject.FamilyName);
            Assert.Equal(contact.Fax, insertedContact.DataObject.Fax);
            Assert.Equal(contact.GivenName, insertedContact.DataObject.GivenName);
            Assert.Equal(contact.HomePhone, insertedContact.DataObject.HomePhone);
            Assert.Equal(contact.IsActive, insertedContact.DataObject.IsActive);
            Assert.Equal(contact.IsContractor, insertedContact.DataObject.IsContractor);
            Assert.Equal(contact.IsCustomer, insertedContact.DataObject.IsCustomer);
            Assert.Equal(contact.IsPartner, insertedContact.DataObject.IsPartner);
            Assert.Equal(contact.IsSupplier, insertedContact.DataObject.IsSupplier);
            Assert.Equal(contact.LinkedInProfile, insertedContact.DataObject.LinkedInProfile);
            Assert.Equal(contact.MiddleInitials, insertedContact.DataObject.MiddleInitials);
            Assert.Equal(contact.MobilePhone, insertedContact.DataObject.MobilePhone);
            Assert.Equal(contact.CompanyId, insertedContact.DataObject.CompanyId);
            Assert.Equal(contact.PositionTitle, insertedContact.DataObject.PositionTitle);
            Assert.NotNull(contact.OtherAddress);
            Assert.Equal(contact.OtherAddress.City, insertedContact.DataObject.OtherAddress.City);
            Assert.Equal(contact.OtherAddress.Country, insertedContact.DataObject.OtherAddress.Country);
            Assert.Equal(contact.OtherAddress.Postcode, insertedContact.DataObject.OtherAddress.Postcode);
            Assert.Equal(contact.OtherAddress.State, insertedContact.DataObject.OtherAddress.State);
            Assert.Equal(contact.OtherAddress.Street, insertedContact.DataObject.OtherAddress.Street);
            Assert.Equal(contact.PostalAddress.City, insertedContact.DataObject.PostalAddress.City);
            Assert.Equal(contact.PostalAddress.Country, insertedContact.DataObject.PostalAddress.Country);
            Assert.Equal(contact.PostalAddress.Postcode, insertedContact.DataObject.PostalAddress.Postcode);
            Assert.Equal(contact.PostalAddress.State, insertedContact.DataObject.PostalAddress.State);
            Assert.Equal(contact.PostalAddress.Street, insertedContact.DataObject.PostalAddress.Street);
            Assert.Equal(contact.PrimaryPhone, insertedContact.DataObject.PrimaryPhone);
            Assert.NotNull(insertedContact.DataObject.PurchaseTradingTerms);
            Assert.Equal(contact.PurchaseTradingTerms.TradingTermsInterval, insertedContact.DataObject.PurchaseTradingTerms.TradingTermsInterval);
            Assert.Equal(contact.PurchaseTradingTerms.TradingTermsIntervalType, insertedContact.DataObject.PurchaseTradingTerms.TradingTermsIntervalType);
            Assert.Equal(contact.PurchaseTradingTerms.TradingTermsType, insertedContact.DataObject.PurchaseTradingTerms.TradingTermsType);
            Assert.NotNull(insertedContact.DataObject.SaleTradingTerms);
            Assert.Equal(contact.SaleTradingTerms.TradingTermsInterval, insertedContact.DataObject.SaleTradingTerms.TradingTermsInterval);
            Assert.Equal(contact.SaleTradingTerms.TradingTermsIntervalType, insertedContact.DataObject.SaleTradingTerms.TradingTermsIntervalType);
            Assert.Equal(contact.SaleTradingTerms.TradingTermsType, insertedContact.DataObject.SaleTradingTerms.TradingTermsType);
            Assert.Equal(contact.Salutation, insertedContact.DataObject.Salutation);
            Assert.Equal(contact.SkypeId, insertedContact.DataObject.SkypeId);
            Assert.Equal(contact.TwitterId, insertedContact.DataObject.TwitterId);
            Assert.Equal(contact.WebsiteUrl, insertedContact.DataObject.WebsiteUrl);
        }

        [Fact]
        public void InsertContactWithoutAddressDoesNotFail()
        {
            var contact = new Contact()
            {
                GivenName = "John",
                FamilyName = "Cook",
                EmailAddress = "john@cook.com",
                HomePhone = "0211114444"
            };

            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);

            Assert.True(response.IsSuccessfull);
            Assert.True(response.DataObject.InsertedContactId > 0);

            System.Threading.Thread.Sleep(1000); // Tags are saved through messagequeue, give time to be processed
            var insertedContact = proxy.GetContact(response.DataObject.InsertedContactId);
            Assert.NotNull(insertedContact.DataObject);
            Assert.Equal(contact.EmailAddress, insertedContact.DataObject.EmailAddress);
        }

        [Fact]
        public void InsertingContactWithoutSpecifyingIsActiveShouldReturnActiveContact()
        {
            var contact = GetCompleteContact();
            contact.IsActive = null;

            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);

            Assert.True(response.IsSuccessfull);
            Assert.True(response.DataObject.InsertedContactId > 0);

            System.Threading.Thread.Sleep(1000); // Tags are saved through messagequeue, give time to be processed
            var insertedContact = proxy.GetContact(response.DataObject.InsertedContactId);
            Assert.NotNull(insertedContact.DataObject);
            Assert.NotNull(insertedContact.DataObject.IsActive);
            Assert.True(insertedContact.DataObject.IsActive.Value);
        }

        [Fact]
        public void InsertingContactWithIsActiveFalseShouldReturnInActiveContact()
        {
            var contact = GetCompleteContact();
            contact.IsActive = false;

            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);

            Assert.True(response.IsSuccessfull);
            Assert.True(response.DataObject.InsertedContactId > 0);

            System.Threading.Thread.Sleep(1000); // Tags are saved through messagequeue, give time to be processed
            var insertedContact = proxy.GetContact(response.DataObject.InsertedContactId);
            Assert.NotNull(insertedContact.DataObject);
            Assert.NotNull(insertedContact.DataObject.IsActive);
            Assert.False(insertedContact.DataObject.IsActive.Value);
        }

        [Fact]
        public void InsertingContactWithIsActiveTrueShouldReturnActiveContact()
        {
            var contact = GetCompleteContact();
            contact.IsActive = true;

            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);

            Assert.True(response.IsSuccessfull);
            Assert.True(response.DataObject.InsertedContactId > 0);

            System.Threading.Thread.Sleep(1000); // Tags are saved through messagequeue, give time to be processed
            var insertedContact = proxy.GetContact(response.DataObject.InsertedContactId);
            Assert.NotNull(insertedContact.DataObject);
            Assert.NotNull(insertedContact.DataObject.IsActive);
            Assert.True(insertedContact.DataObject.IsActive.Value);
        }

        [Fact]
        public void UpdateCompleteContact()
        {
            var contact = GetMinimalContact();

            var proxy = new ContactProxy();
            var insertResponse = proxy.InsertContact(contact);

            Assert.True(insertResponse.IsSuccessfull);
            Assert.True(insertResponse.DataObject.InsertedContactId > 0);

            var insertedContact = proxy.GetContact(insertResponse.DataObject.InsertedContactId);
            insertedContact.DataObject.GivenName = "Jack";
            insertedContact.DataObject.PostalAddress.City = "Potts Point";

            var tags = new List<string>();
            tags.Add("Banana");
            tags.Add("Pear");
            insertedContact.DataObject.Tags = tags;

            var updateResponse = proxy.UpdateContact(insertedContact.DataObject, insertResponse.DataObject.InsertedContactId);

            Assert.True(updateResponse.IsSuccessfull);
            //System.Threading.Thread.Sleep(1000);
            var updatedContact = proxy.GetContact(updateResponse.DataObject.UpdatedContactId);

            Assert.Equal(insertedContact.DataObject.GivenName, updatedContact.DataObject.GivenName);
            Assert.NotNull(updatedContact.DataObject.PostalAddress);
            Assert.Equal(insertedContact.DataObject.PostalAddress.City, updatedContact.DataObject.PostalAddress.City);
        }

        [Fact]
        public void DeleteContact()
        {
            var contact = GetMinimalContact();

            var proxy = new ContactProxy();
            var result = proxy.InsertContact(contact);

            Assert.True(result.IsSuccessfull);
            Assert.True(result.DataObject.InsertedContactId > 0);

            var deleteResult = proxy.DeleteContact(result.DataObject.InsertedContactId);
            Assert.True(deleteResult.IsSuccessfull);

            var deletedContact = proxy.GetContact(result.DataObject.InsertedContactId);
            Assert.Null(deletedContact.DataObject);
            Assert.Equal(HttpStatusCode.NotFound, deletedContact.StatusCode);

        }

        #region Test data


        public static int GetOrCreateContactCustomer(string firstName, string lastName, string organisationName, string email)
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: firstName, familyName: lastName, isCustomer: true, organisationName: organisationName);
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
                Tags = new List<string>() { $"{firstName}Tag1", $"{lastName}Tag2" },
                EmailAddress = email,
                CompanyId = cResult.DataObject.InsertedCompanyId
            };

            var result = proxy.InsertContact(contact);
            Assert.True(result.IsSuccessfull, "Failed to add customer contact test data.");
            return result.DataObject.InsertedContactId;

        }


        /// <summary>
        /// Add fictious customer contact. Currently have to use old Rest client API to insert record as this functionality not available in Web API yet.
        /// </summary>
        public static int GetOrCreateContactCustomer()
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: "carl", familyName: "o'neil", isCustomer: true, organisationName: "o'neil capital");
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
                Tags = new List<string>(){"carlTag1","carlTag2"},
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
        public static int GetOrCreateSupplierContact()
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: "jenny", familyName: "o'neil", isCustomer: true, organisationName: "O'Neil Supplier");
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
        public static int GetOrCreatePartnerContact()
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: "brad", familyName: "o'neil", isCustomer: true, organisationName: "O'Neil Partner");
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
        public static int GetOrCreateContractorContact()
        {
            var proxy = new ContactProxy();
            var searchProxy = new ContactsProxy();
            var companyProxy = new CompanyProxy();

            var response = searchProxy.GetContacts(givenName: "kathy", familyName: "o'neil", isCustomer: true, organisationName: "O'Neil Contractor");
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

        #endregion

        #region "Test Data"

        private Contact GetCompleteContact()
        {
            var tags = new List<string>();
            tags.Add("Test");
            tags.Add("Gloin");

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
                EmailAddress = "john@cook.com",
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

        private Contact GetMinimalContact()
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
        #endregion

    }
}
