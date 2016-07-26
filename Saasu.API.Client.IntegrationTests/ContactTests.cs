using NUnit.Framework;
using Ola.RestClient;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Models;
using Saasu.API.Core.Models.Contacts;
using System;
using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;

namespace Saasu.API.Client.IntegrationTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    //[TestClass]
    [TestFixture]
    public class ContactTests
    {
        public ContactTests()
        {
            //Create test data needed for unit tests.
            VerifyTestContactExistsOrCreate(ContactType.Customer);
            VerifyTestContactExistsOrCreate(ContactType.Supplier);
            VerifyTestContactExistsOrCreate(ContactType.Partner);
            VerifyTestContactExistsOrCreate(ContactType.Contractor);
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

        [Test]
        public void GetContactsShouldReturnContactsUsingOAuth()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new ContactsProxy(accessToken);
            AssertContactProxy(proxy);
        }

        [Test]
        public void GetContactsShouldReturnContactsUsingWsAccessKey()
        {
            var proxy = new ContactsProxy();
            AssertContactProxy(proxy);
        }

        [Test]
        public void GetContactsFilterOnPersonNameAndOrganisationName()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy, givenName: "Carl", familyName: "O'Neil", organisationName: "O'Neil Capital");

            Assert.IsTrue(response.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");
        }

        [Test]
        public void GetContactsAll()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy);
        }

        [Test]
        public void GetContactsOnePageOneRecord()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy, pageNumber: 1, pageSize: 1);

            Assert.AreEqual(1, response.DataObject.Contacts.Count, "Paging of 1 page and 1 record is returning the wrong number of records");
        }

        [Test]
        public void GetContactsSecondPage()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy, pageNumber: 1, pageSize: 2);

            var idsFromPage1 = response.DataObject.Contacts.Select(c => c.Id).ToList();

            response = AssertContactProxy(proxy, pageNumber: 2, pageSize: 2);

            response.DataObject.Contacts.ForEach(c => Assert.IsFalse(idsFromPage1.Contains(c.Id), "Record(s) from page 1 were returned"));
        }

        [Test]
        public void GetContactsFilterOnModifiedDates()
        {
            var proxy = new ContactsProxy();

            //use a year ago and tomorrow as date filters to make sure the test contact Carl O'Brien is picked up.
            AssertContactProxy(proxy, lastModifiedFromDate: DateTime.Now.AddYears(-1), lastModifiedToDate: DateTime.Now.AddDays(1));
        }

        [Test]
        public void GetContactsFilterActiveCustomer()
        {
            var proxy = new ContactsProxy();

            AssertContactProxy(proxy, isActive: true, isCustomer: true);
        }

        [Test]
        public void GetContactsFilterActiveSupplier()
        {
            var proxy = new ContactsProxy();

            AssertContactProxy(proxy, isActive: true, isSupplier: true);
        }

        [Test]
        public void GetContactsFilterActivePartner()
        {
            var proxy = new ContactsProxy();

            AssertContactProxy(proxy, isActive: true, isPartner: true);
        }

        [Test]
        public void GetContactsFilterActiveContractor()
        {
            var proxy = new ContactsProxy();

            AssertContactProxy(proxy, isActive: true, isContractor: true);
        }

        [Test]
        public void GetContactsFilterOnEmail()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy, email: "carl@oneilcapital.com");

            Assert.IsTrue(response.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");
        }

        [Test]
        public void GetContactsFilterOnContactId()
        {
            var proxy = new ContactsProxy();

            var response = AssertContactProxy(proxy, contactId: "GLD879");

            Assert.IsTrue(response.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");
        }

        [Test]
        public void GetSingleContactWithContactId()
        {
            var contactsProxy = new ContactsProxy();

            //Find Test contact to obtain their Id.
            var contactsResponse = AssertContactProxy(contactsProxy, givenName: "carl", familyName: "o'neil", contactId: "GLD879");

            Assert.IsTrue(contactsResponse.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");

            var proxy = new ContactProxy();

            var id = contactsResponse.DataObject.Contacts[0].Id;
            Assert.IsNotNull(id);
            var response = proxy.GetContact(id.Value);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsNotNull(response.DataObject._links);
            Assert.IsTrue(response.DataObject._links.Count > 0);
            Assert.IsTrue(response.DataObject.Id == id);
        }

        [Test]
        public void ModifyingContactsIsBooleanFieldsShouldNotAffectOtherIsBooleanFields()
        {
            var contactsProxy = new ContactsProxy();

            //Find Test contact to obtain their Id.
            var contactsResponse = AssertContactProxy(contactsProxy, givenName: "carl", familyName: "o'neil", contactId: "GLD879");

            Assert.IsTrue(contactsResponse.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");

            var proxy = new ContactProxy();

            var testId = contactsResponse.DataObject.Contacts[0].Id;
            var response = proxy.GetContact(testId.Value);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);

            // Now Modify the IsCustomer, IsContractor, IsSupplier, Is... fields and ensure setting one does not clear the other
            var testContact = response.DataObject;
            testContact.IsContractor = true;
            testContact.IsCustomer = true;
            testContact.IsPartner = true;
            testContact.IsSupplier = true;
            var updateResponse = proxy.UpdateContact(testContact, testContact.Id.Value);
            Assert.IsNotNull(updateResponse);
            Assert.IsTrue(updateResponse.IsSuccessfull);

            var updatedResponse1 = proxy.GetContact(testContact.Id.Value);
            Assert.IsTrue(updatedResponse1.IsSuccessfull);
            Assert.AreEqual(testContact.IsContractor, updatedResponse1.DataObject.IsContractor);
            Assert.AreEqual(testContact.IsCustomer, updatedResponse1.DataObject.IsCustomer);
            Assert.AreEqual(testContact.IsPartner, updatedResponse1.DataObject.IsPartner);
            Assert.AreEqual(testContact.IsSupplier, updatedResponse1.DataObject.IsSupplier);

            testContact.LastUpdatedId = updateResponse.DataObject.LastUpdatedId;
            testContact.IsSupplier = false;
            testContact.IsPartner = false;
            var updateResponse2 = proxy.UpdateContact(testContact, testContact.Id.Value);
            Assert.IsNotNull(updateResponse2);
            Assert.IsTrue(updateResponse2.IsSuccessfull);

            var updatedResponse2 = proxy.GetContact(testContact.Id.Value);
            Assert.IsTrue(updatedResponse2.IsSuccessfull);
            Assert.AreEqual(testContact.IsContractor, updatedResponse2.DataObject.IsContractor);
            Assert.AreEqual(testContact.IsCustomer, updatedResponse2.DataObject.IsCustomer);
            Assert.AreEqual(testContact.IsPartner, updatedResponse2.DataObject.IsPartner);
            Assert.AreEqual(testContact.IsSupplier, updatedResponse2.DataObject.IsSupplier);
        }

        public static ProxyResponse<ContactResponse> AssertContactProxy(ContactsProxy proxy, int? pageNumber = null, int? pageSize = null, DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null,
            string givenName = null, string familyName = null, string organisationName = null,
            bool? isActive = null, bool? isCustomer = null, bool? isSupplier = null, bool? isContractor = null, bool? isPartner = null,
            string tags = null, string tagSelection = null, string email = null, string contactId = null)
        {
            var response = proxy.GetContacts(pageNumber, pageSize, lastModifiedFromDate, lastModifiedToDate,
                givenName, familyName, organisationName,
                isActive, isCustomer, isSupplier, isContractor, isPartner,
                tags, tagSelection, email, contactId);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsNotNull(response.DataObject.Contacts);
            Assert.IsNotNull(response.DataObject._links);
            Assert.IsTrue(response.DataObject._links.Count > 0);
            Assert.IsTrue(response.DataObject.Contacts.Count > 0);

            return response;
        }

        [Test]
        public void InsertCompleteContact()
        {
            var contact = GetCompleteContact();

            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);

            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsTrue(response.DataObject.InsertedContactId > 0);

            System.Threading.Thread.Sleep(1000); // Tags are saved through messagequeue, give time to be processed
            var insertedContact = proxy.GetContact(response.DataObject.InsertedContactId);
            Assert.IsNotNull(insertedContact.DataObject);

            Assert.AreEqual(contact.DirectDepositDetails.AcceptDirectDeposit, insertedContact.DataObject.DirectDepositDetails.AcceptDirectDeposit);
            Assert.AreEqual(contact.ChequeDetails.AcceptCheque, insertedContact.DataObject.ChequeDetails.AcceptCheque);
            Assert.AreEqual(contact.AutoSendStatement.HasValue, insertedContact.DataObject.AutoSendStatement.HasValue);
            Assert.AreEqual(contact.BpayDetails.BillerCode, insertedContact.DataObject.BpayDetails.BillerCode);
            Assert.AreEqual(contact.BpayDetails.CRN, insertedContact.DataObject.BpayDetails.CRN);
            Assert.AreEqual(contact.ChequeDetails.ChequePayableTo, insertedContact.DataObject.ChequeDetails.ChequePayableTo);
            Assert.AreEqual(contact.ContactId, insertedContact.DataObject.ContactId);
            Assert.AreEqual(contact.ContactManagerId, insertedContact.DataObject.ContactManagerId);
            Assert.AreEqual(contact.CustomField1, insertedContact.DataObject.CustomField1);
            Assert.AreEqual(contact.CustomField2, insertedContact.DataObject.CustomField2);
            Assert.AreEqual(contact.DefaultPurchaseDiscount, insertedContact.DataObject.DefaultPurchaseDiscount);
            Assert.AreEqual(contact.DefaultSaleDiscount, insertedContact.DataObject.DefaultSaleDiscount);
            Assert.AreEqual(contact.DirectDepositDetails.AccountBSB, insertedContact.DataObject.DirectDepositDetails.AccountBSB);
            Assert.AreEqual(contact.DirectDepositDetails.AccountName, insertedContact.DataObject.DirectDepositDetails.AccountName);
            Assert.AreEqual(contact.DirectDepositDetails.AccountNumber, insertedContact.DataObject.DirectDepositDetails.AccountNumber);
            Assert.AreEqual(contact.EmailAddress, insertedContact.DataObject.EmailAddress);
            Assert.AreEqual(contact.FamilyName, insertedContact.DataObject.FamilyName);
            Assert.AreEqual(contact.Fax, insertedContact.DataObject.Fax);
            Assert.AreEqual(contact.GivenName, insertedContact.DataObject.GivenName);
            Assert.AreEqual(contact.HomePhone, insertedContact.DataObject.HomePhone);
            Assert.AreEqual(contact.IsActive, insertedContact.DataObject.IsActive);
            Assert.AreEqual(contact.IsContractor, insertedContact.DataObject.IsContractor);
            Assert.AreEqual(contact.IsCustomer, insertedContact.DataObject.IsCustomer);
            Assert.AreEqual(contact.IsPartner, insertedContact.DataObject.IsPartner);
            Assert.AreEqual(contact.IsSupplier, insertedContact.DataObject.IsSupplier);
            Assert.AreEqual(contact.LinkedInProfile, insertedContact.DataObject.LinkedInProfile);
            Assert.AreEqual(contact.MiddleInitials, insertedContact.DataObject.MiddleInitials);
            Assert.AreEqual(contact.MobilePhone, insertedContact.DataObject.MobilePhone);
            Assert.AreEqual(contact.CompanyId, insertedContact.DataObject.CompanyId);
            Assert.AreEqual(contact.PositionTitle, insertedContact.DataObject.PositionTitle);
            Assert.IsNotNull(contact.OtherAddress);
            Assert.AreEqual(contact.OtherAddress.City, insertedContact.DataObject.OtherAddress.City);
            Assert.AreEqual(contact.OtherAddress.Country, insertedContact.DataObject.OtherAddress.Country);
            Assert.AreEqual(contact.OtherAddress.Postcode, insertedContact.DataObject.OtherAddress.Postcode);
            Assert.AreEqual(contact.OtherAddress.State, insertedContact.DataObject.OtherAddress.State);
            Assert.AreEqual(contact.OtherAddress.Street, insertedContact.DataObject.OtherAddress.Street);
            Assert.AreEqual(contact.PostalAddress.City, insertedContact.DataObject.PostalAddress.City);
            Assert.AreEqual(contact.PostalAddress.Country, insertedContact.DataObject.PostalAddress.Country);
            Assert.AreEqual(contact.PostalAddress.Postcode, insertedContact.DataObject.PostalAddress.Postcode);
            Assert.AreEqual(contact.PostalAddress.State, insertedContact.DataObject.PostalAddress.State);
            Assert.AreEqual(contact.PostalAddress.Street, insertedContact.DataObject.PostalAddress.Street);
            Assert.AreEqual(contact.PrimaryPhone, insertedContact.DataObject.PrimaryPhone);
            Assert.IsNotNull(insertedContact.DataObject.PurchaseTradingTerms);
            Assert.AreEqual(contact.PurchaseTradingTerms.TradingTermsInterval, insertedContact.DataObject.PurchaseTradingTerms.TradingTermsInterval);
            Assert.AreEqual(contact.PurchaseTradingTerms.TradingTermsIntervalType, insertedContact.DataObject.PurchaseTradingTerms.TradingTermsIntervalType);
            Assert.AreEqual(contact.PurchaseTradingTerms.TradingTermsType, insertedContact.DataObject.PurchaseTradingTerms.TradingTermsType);
            Assert.IsNotNull(insertedContact.DataObject.SaleTradingTerms);
            Assert.AreEqual(contact.SaleTradingTerms.TradingTermsInterval, insertedContact.DataObject.SaleTradingTerms.TradingTermsInterval);
            Assert.AreEqual(contact.SaleTradingTerms.TradingTermsIntervalType, insertedContact.DataObject.SaleTradingTerms.TradingTermsIntervalType);
            Assert.AreEqual(contact.SaleTradingTerms.TradingTermsType, insertedContact.DataObject.SaleTradingTerms.TradingTermsType);
            Assert.AreEqual(contact.Salutation, insertedContact.DataObject.Salutation);
            Assert.AreEqual(contact.SkypeId, insertedContact.DataObject.SkypeId);
            Assert.AreEqual(contact.TwitterId, insertedContact.DataObject.TwitterId);
            Assert.AreEqual(contact.WebsiteUrl, insertedContact.DataObject.WebsiteUrl);
        }

        [Test]
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

            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsTrue(response.DataObject.InsertedContactId > 0);

            System.Threading.Thread.Sleep(1000); // Tags are saved through messagequeue, give time to be processed
            var insertedContact = proxy.GetContact(response.DataObject.InsertedContactId);
            Assert.IsNotNull(insertedContact.DataObject);
            Assert.AreEqual(contact.EmailAddress, insertedContact.DataObject.EmailAddress);
        }

        [Test]
        public void InsertingContactWithoutSpecifyingIsActiveShouldReturnActiveContact()
        {
            var contact = GetCompleteContact();
            contact.IsActive = null;

            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);

            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsTrue(response.DataObject.InsertedContactId > 0);

            System.Threading.Thread.Sleep(1000); // Tags are saved through messagequeue, give time to be processed
            var insertedContact = proxy.GetContact(response.DataObject.InsertedContactId);
            Assert.IsNotNull(insertedContact.DataObject);
            Assert.IsNotNull(insertedContact.DataObject.IsActive);
            Assert.IsTrue(insertedContact.DataObject.IsActive.Value);
        }

        [Test]
        public void InsertingContactWithIsActiveFalseShouldReturnInActiveContact()
        {
            var contact = GetCompleteContact();
            contact.IsActive = false;

            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);

            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsTrue(response.DataObject.InsertedContactId > 0);

            System.Threading.Thread.Sleep(1000); // Tags are saved through messagequeue, give time to be processed
            var insertedContact = proxy.GetContact(response.DataObject.InsertedContactId);
            Assert.IsNotNull(insertedContact.DataObject);
            Assert.IsNotNull(insertedContact.DataObject.IsActive);
            Assert.IsFalse(insertedContact.DataObject.IsActive.Value);
        }

        [Test]
        public void InsertingContactWithIsActiveTrueShouldReturnActiveContact()
        {
            var contact = GetCompleteContact();
            contact.IsActive = true;

            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);

            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsTrue(response.DataObject.InsertedContactId > 0);

            System.Threading.Thread.Sleep(1000); // Tags are saved through messagequeue, give time to be processed
            var insertedContact = proxy.GetContact(response.DataObject.InsertedContactId);
            Assert.IsNotNull(insertedContact.DataObject);
            Assert.IsNotNull(insertedContact.DataObject.IsActive);
            Assert.IsTrue(insertedContact.DataObject.IsActive.Value);
        }

        [Test]
        public void UpdateCompleteContact()
        {
            var contact = GetMinimalContact();

            var proxy = new ContactProxy();
            var insertResponse = proxy.InsertContact(contact);

            Assert.IsTrue(insertResponse.IsSuccessfull);
            Assert.IsTrue(insertResponse.DataObject.InsertedContactId > 0);

            var insertedContact = proxy.GetContact(insertResponse.DataObject.InsertedContactId);
            insertedContact.DataObject.GivenName = "Jack";
            insertedContact.DataObject.PostalAddress.City = "Potts Point";

            var tags = new List<string>();
            tags.Add("Banana");
            tags.Add("Pear");
            insertedContact.DataObject.Tags = tags;

            var updateResponse = proxy.UpdateContact(insertedContact.DataObject, insertResponse.DataObject.InsertedContactId);

            Assert.IsTrue(updateResponse.IsSuccessfull);
            //System.Threading.Thread.Sleep(1000);
            var updatedContact = proxy.GetContact(updateResponse.DataObject.UpdatedContactId);

            Assert.AreEqual(insertedContact.DataObject.GivenName, updatedContact.DataObject.GivenName);
            Assert.IsNotNull(updatedContact.DataObject.PostalAddress);
            Assert.AreEqual(insertedContact.DataObject.PostalAddress.City, updatedContact.DataObject.PostalAddress.City);
        }

        [Test]
        public void DeleteContact()
        {
            var contact = GetMinimalContact();

            var proxy = new ContactProxy();
            var result = proxy.InsertContact(contact);

            Assert.IsTrue(result.IsSuccessfull);
            Assert.Greater(result.DataObject.InsertedContactId, 0);

            var deleteResult = proxy.DeleteContact(result.DataObject.InsertedContactId);
            Assert.IsTrue(deleteResult.IsSuccessfull);

            var deletedContact = proxy.GetContact(result.DataObject.InsertedContactId);
            Assert.IsNull(deletedContact.DataObject);
            Assert.AreEqual(HttpStatusCode.NotFound, deletedContact.StatusCode);

        }

        #region Test data

        internal static ProxyResponse<ContactResponse> VerifyTestContactExistsOrCreate(ContactType contactType)
        {
            var proxy = new ContactsProxy();
            ProxyResponse<ContactResponse> response = null;

            switch (contactType)
            {
                case Ola.RestClient.ContactType.Customer:
                    {
                        response = SearchCarl(proxy);
                        if (response.DataObject.Contacts.Count == 0)
                        {
                            AddCarlCustomer(proxy.WsAccessKey, proxy.FileId);
                            response = SearchCarl(proxy);
                        }
                        break;
                    }
                case Ola.RestClient.ContactType.Supplier:
                    {
                        response = SearchJenny(proxy);
                        if (response.DataObject.Contacts.Count == 0)
                        {
                            AddJennySupplier(proxy.WsAccessKey, proxy.FileId);
                            response = SearchJenny(proxy);
                        }
                        break;
                    }
                case Ola.RestClient.ContactType.Partner:
                    {
                        response = SearchBrad(proxy);
                        if (response.DataObject.Contacts.Count == 0)
                        {
                            AddBradPartner(proxy.WsAccessKey, proxy.FileId);
                            response = SearchBrad(proxy);
                        }
                        break;
                    }
                case Ola.RestClient.ContactType.Contractor:
                    {
                        response = SearchKathy(proxy);
                        if (response.DataObject.Contacts.Count == 0)
                        {
                            AddKathyContractor(proxy.WsAccessKey, proxy.FileId);
                            response = SearchKathy(proxy);
                        }
                        break;
                    }
            }

            if (response != null)
            {
                Assert.IsTrue(response.DataObject.Contacts.Count >= 1, "Incorrect number of contacts found.");
            }
            else
            {
                Assert.Fail("No contact type specified to be created.");
            }

            return response;
        }

        /// <summary>
        /// Search for fictious customer contact.
        /// </summary>
        private static ProxyResponse<ContactResponse> SearchCarl(ContactsProxy proxy)
        {
            var response = proxy.GetContacts(givenName: "carl", familyName: "o'neil", organisationName: "o'neil capital");
            return response;
        }

        /// <summary>
        /// Search for fictious supplier contact.
        /// </summary>
        private static ProxyResponse<ContactResponse> SearchJenny(ContactsProxy proxy)
        {
            var response = proxy.GetContacts(givenName: "jenny", familyName: "o'neil", organisationName: "o'neil supplier");
            return response;
        }

        /// <summary>
        /// Search for fictious partner contact.
        /// </summary>
        private static ProxyResponse<ContactResponse> SearchBrad(ContactsProxy proxy)
        {
            var response = proxy.GetContacts(givenName: "brad", familyName: "o'neil", organisationName: "o'neil partner");
            return response;
        }

        /// <summary>
        /// Search for fictious contractor contact.
        /// </summary>
        private static ProxyResponse<ContactResponse> SearchKathy(ContactsProxy proxy)
        {
            var response = proxy.GetContacts(givenName: "kathy", familyName: "o'neil", organisationName: "o'neil contractor", isContractor: true);
            return response;
        }

        /// <summary>
        /// Add fictious customer contact. Currently have to use old Rest client API to insert record as this functionality not available in Web API yet.
        /// </summary>
        public static void AddCarlCustomer(string wsAccessKey, int fileuid)
        {
            var proxy = new Ola.RestClient.Proxies.ContactProxy();
            var dto = new Ola.RestClient.Dto.ContactDto()
            {
                Salutation = "Mr.",
                GivenName = "Carl",
                FamilyName = "O'Neil",
                OrganisationName = "O'Neil Capital",
                ContactID = "GLD879",
                CustomField1 = "O'NeilC",
                IsCustomer = true,
                ContactType = (int)ContactType.Customer,
                Tags = "carlTag1,carlTag2",
                Email = "carl@oneilcapital.com"
            };
            proxy.Insert(dto);
            Assert.IsTrue(dto.Uid > 0, "Incorrect uid post save.");
        }

        /// <summary>
        /// Add fictious supplier contact. Currently have to use old Rest client API to insert record as this functionality not available in Web API yet.
        /// </summary>
        public static void AddJennySupplier(string wsAccessKey, int fileuid)
        {
            var proxy = new Ola.RestClient.Proxies.ContactProxy();
            var dto = new Ola.RestClient.Dto.ContactDto()
            {
                Salutation = "Ms.",
                GivenName = "Jenny",
                FamilyName = "O'Neil",
                OrganisationName = "O'Neil Supplier",
                ContactID = "GLD880",
                CustomField1 = "O'NeilJ",
                IsSupplier = true,
                ContactType = (int)ContactType.Supplier
            };
            proxy.Insert(dto);
            Assert.IsTrue(dto.Uid > 0, "Incorrect uid post save.");
        }

        /// <summary>
        /// Add fictious partner contact. Currently have to use old Rest client API to insert record as this functionality not available in Web API yet.
        /// </summary>
        public static void AddBradPartner(string wsAccessKey, int fileuid)
        {
            var proxy = new Ola.RestClient.Proxies.ContactProxy();
            var dto = new Ola.RestClient.Dto.ContactDto()
            {
                Salutation = "Mr.",
                GivenName = "Brad",
                FamilyName = "O'Neil",
                OrganisationName = "O'Neil Partner",
                ContactID = "GLD881",
                CustomField1 = "O'NeilB",
                IsPartner = true,
                ContactType = (int)ContactType.Partner
            };
            proxy.Insert(dto);
            Assert.IsTrue(dto.Uid > 0, "Incorrect uid post save.");
        }

        /// <summary>
        /// Add fictious contractor contact. Currently have to use old Rest client API to insert record as this functionality not available in Web API yet.
        /// </summary>
        public static void AddKathyContractor(string wsAccessKey, int fileuid)
        {
            var proxy = new Ola.RestClient.Proxies.ContactProxy();
            var dto = new Ola.RestClient.Dto.ContactDto()
            {
                Salutation = "Ms.",
                GivenName = "Kathy",
                FamilyName = "O'Neil",
                OrganisationName = "O'Neil Contractor",
                ContactID = "GLD882",
                CustomField1 = "O'NeilK",
                ContactType = (int)Ola.RestClient.ContactType.Contractor
            };
            proxy.Insert(dto);
            Assert.IsTrue(dto.Uid > 0, "Incorrect uid post save.");
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
                BillingSecondaryEmailAddress = "john.second@cook.com",
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
