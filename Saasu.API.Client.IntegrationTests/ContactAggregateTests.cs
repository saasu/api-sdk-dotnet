using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saasu.API.Client.IntegrationTests.Helpers;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Models;
using Saasu.API.Core.Models.Company;
using Saasu.API.Core.Models.ContactAggregates;
using Saasu.API.Core.Models.Contacts;
using System;
using System.Net;

namespace Saasu.API.Client.IntegrationTests
{
    [TestClass]
    public class ContactAggregateTests
    {
        private ContactHelper _contactHelper;

        public ContactAggregateTests()
        {
            _contactHelper = new ContactHelper();
        }

        [TestMethod]
        public void ShouldAddNewContactWithNewCompanyAndContactManager()
        {
            var contactAggregateProxy = new ContactAggregateProxy();
            var contactProxy = new ContactProxy();
            var companyProxy = new CompanyProxy();
            var contactAggregate = GetNewContactAggregate();
            var Response = contactAggregateProxy.InsertContactAggregate(contactAggregate);

            Assert.IsTrue(Response.IsSuccessfull, "Contact aggregate insert failed.");
            Assert.IsNotNull(Response.DataObject);
            Assert.IsTrue(Response.DataObject.InsertedContactId > 0, "Invalid InsertedContactId returned from InsertContactAggregate");
            Assert.IsTrue(Response.DataObject.LastUpdatedId.Length > 0, "Invalid LastUpdatedId returned");
            Assert.IsTrue(Response.DataObject.LastModified > DateTime.UtcNow.AddMinutes(-5), "Invalid LastModified returned");

            var contactResponse = contactProxy.GetContact(Response.DataObject.InsertedContactId);
            Assert.IsTrue(contactResponse.IsSuccessfull, "Contact not found");
            Assert.IsNotNull(contactResponse.DataObject);
            Assert.IsNotNull(contactResponse.DataObject.CompanyId);
            Assert.IsNotNull(contactResponse.DataObject.ContactManagerId);

            var contactManagerResponse = contactProxy.GetContact(contactResponse.DataObject.ContactManagerId.Value);
            Assert.IsTrue(contactManagerResponse.IsSuccessfull, "Contact manager not found");
            Assert.AreEqual(contactAggregate.ContactManager.FamilyName, contactManagerResponse.DataObject.FamilyName);

            var companyResponse = companyProxy.GetCompany(contactResponse.DataObject.CompanyId.Value);
            Assert.IsTrue(companyResponse.IsSuccessfull, "Company not found");
            Assert.AreEqual(contactAggregate.Company.Name, companyResponse.DataObject.Name);
        }

        [TestMethod]
        public void ShouldAddNewContactAndUpdateCompanyAndContactManager()
        {
            var contactAggregateProxy = new ContactAggregateProxy();
            var contactProxy = new ContactProxy();
            var companyProxy = new CompanyProxy();

            var companyDetail = GetCompany();
            var companyResponse = companyProxy.InsertCompany(companyDetail);

            var contactManager = GetContact();

            var contactManagerResponse = contactProxy.InsertContact(contactManager);
            var contactAggregate = GetNewContactAggregate();
            contactAggregate.Company.Id = companyResponse.DataObject.InsertedCompanyId;
            contactAggregate.Company.LastUpdatedId = companyResponse.DataObject.LastUpdatedId;
            contactAggregate.ContactManager.Id = contactManagerResponse.DataObject.InsertedContactId;
            contactAggregate.ContactManager.LastUpdatedId = contactManagerResponse.DataObject.LastUpdatedId;

            var contactAggregateResponse = contactAggregateProxy.InsertContactAggregate(contactAggregate);
            Assert.IsTrue(contactAggregateResponse.IsSuccessfull, "Contact aggregate with new contact and updated company and contact manager failed");
            Assert.IsNotNull(contactAggregateResponse.DataObject);

            var dbContactAggregate =
                contactAggregateProxy.GetContactAggregate(contactAggregateResponse.DataObject.InsertedContactId);
            Assert.IsTrue(dbContactAggregate.IsSuccessfull);
            Assert.IsNotNull(dbContactAggregate.DataObject);
            Assert.IsNotNull(dbContactAggregate.DataObject.ContactManager.Id);
            Assert.IsNotNull(dbContactAggregate.DataObject.Company.Id);
            Assert.AreEqual(companyResponse.DataObject.InsertedCompanyId, dbContactAggregate.DataObject.Company.Id.Value, "Existing company not attached to contact aggregate");
            Assert.AreEqual(contactManagerResponse.DataObject.InsertedContactId, dbContactAggregate.DataObject.ContactManager.Id.Value, "Existing contact (manager) not attached to contact aggregate");

            var dbContactManager = contactProxy.GetContact(dbContactAggregate.DataObject.ContactManager.Id.Value);
            Assert.IsTrue(dbContactManager.IsSuccessfull);
            Assert.IsNotNull(dbContactManager.DataObject);

            AssertContactManager(dbContactAggregate.DataObject.ContactManager, dbContactManager.DataObject);
            Assert.AreEqual(dbContactAggregate.DataObject.ContactManager.LastUpdatedId,
                dbContactManager.DataObject.LastUpdatedId, "LastUpdatedId not updated for contact manager");

            var dbCompany = companyProxy.GetCompany(dbContactAggregate.DataObject.Company.Id.Value);
            Assert.IsTrue(dbCompany.IsSuccessfull);
            Assert.IsNotNull(dbCompany.DataObject);

            AssertCompany(dbContactAggregate.DataObject.Company, dbCompany.DataObject);
            Assert.AreEqual(dbContactAggregate.DataObject.Company.LastUpdatedId, dbCompany.DataObject.LastUpdatedId,
                "LastUpdatedId not updated for company");
        }

        [TestMethod]
        public void ShouldUpdateContactAndAddNewCompanyAndContactManager()
        {
            var contactProxy = new ContactProxy();
            var contactAggregateProxy = new ContactAggregateProxy();
            var contact = GetContact();
            var contactResponse = contactProxy.InsertContact(contact);
            Assert.IsTrue(contactResponse.IsSuccessfull);

            var contactAggregate = GetNewContactAggregate();

            var contactAggregateResponse = contactAggregateProxy.UpdateContactAggregate(contactAggregate, contactResponse.DataObject.InsertedContactId);
            Assert.IsTrue(contactAggregateResponse.IsSuccessfull, "Failed to update existing contact and add new company and contact manager");
            Assert.IsNotNull(contactAggregateResponse.DataObject);
            Assert.AreEqual(contactResponse.DataObject.InsertedContactId, contactAggregateResponse.DataObject.UpdatedContactId);

            var updatedContact = contactProxy.GetContact(contactResponse.DataObject.InsertedContactId);
            AssertUpdatedContact(contactAggregate, updatedContact);

            var insertedAggregate =
                contactAggregateProxy.GetContactAggregate(contactResponse.DataObject.InsertedContactId);
            Assert.IsTrue(insertedAggregate.IsSuccessfull);
            Assert.IsNotNull(insertedAggregate.DataObject);
            Assert.IsNotNull(insertedAggregate.DataObject.Company, "New company not associated with existing contact");
            Assert.IsNotNull(insertedAggregate.DataObject.ContactManager, "New contact manager not associated with existing contact");
            Assert.AreEqual(contactAggregate.Company.Name, insertedAggregate.DataObject.Company.Name);
            Assert.AreEqual(contactAggregate.ContactManager.GivenName, insertedAggregate.DataObject.ContactManager.GivenName);
        }



        [TestMethod]
        public void ShouldUpdateContactAndUpdateCompanyAndContactManager()
        {
            var contactProxy = new ContactProxy();
            var companyProxy = new CompanyProxy();
            var contactAggregateProxy = new ContactAggregateProxy();
            var contact = GetContact();
            var contactResponse = contactProxy.InsertContact(contact);
            Assert.IsTrue(contactResponse.IsSuccessfull);
            var contactManager = GetContact();
            var contactManagerResponse = contactProxy.InsertContact(contactManager);
            Assert.IsTrue(contactManagerResponse.IsSuccessfull);
            var company = GetCompany();
            var companyResponse = companyProxy.InsertCompany(company);
            Assert.IsTrue(companyResponse.IsSuccessfull);

            var contactAggregate = GetNewContactAggregate();
            contactAggregate.Id = contactResponse.DataObject.InsertedContactId;
            contactAggregate.ContactManager.Id = contactManagerResponse.DataObject.InsertedContactId;
            contactAggregate.ContactManager.LastUpdatedId = contactManagerResponse.DataObject.LastUpdatedId;
            contactAggregate.Company.Id = companyResponse.DataObject.InsertedCompanyId;
            contactAggregate.Company.LastUpdatedId = companyResponse.DataObject.LastUpdatedId;

            var contactAggregateUpdateResponse = contactAggregateProxy.UpdateContactAggregate(contactAggregate,
                contactResponse.DataObject.InsertedContactId);
            Assert.IsTrue(contactAggregateUpdateResponse.IsSuccessfull, "Updating contact and company and contact manager in aggregate failed");
            Assert.IsNotNull(contactAggregateUpdateResponse.DataObject);

            var updatedContactAggregateResponse =
                contactAggregateProxy.GetContactAggregate(contactResponse.DataObject.InsertedContactId);
            Assert.IsTrue(updatedContactAggregateResponse.IsSuccessfull, "Updating contact and company and contact manager in aggregate failed");
            Assert.IsNotNull(updatedContactAggregateResponse.DataObject);
            var updatedAggregate = updatedContactAggregateResponse.DataObject;
            Assert.IsNotNull(updatedAggregate.Company, "Company not associated with updated aggregate");
            Assert.IsNotNull(updatedAggregate.ContactManager, "Contact manager not associated with updated aggregate");
            Assert.IsNotNull(updatedAggregate.Company.Id);
            Assert.IsNotNull(updatedAggregate.ContactManager.Id);

            var updatedContact = contactProxy.GetContact(contactResponse.DataObject.InsertedContactId);
            AssertUpdatedContact(contactAggregate, updatedContact);

            var updatedCompany = companyProxy.GetCompany(updatedAggregate.Company.Id.Value);
            Assert.IsTrue(updatedCompany.IsSuccessfull);
            Assert.IsNotNull(updatedCompany.DataObject);

            AssertCompany(contactAggregate.Company, updatedCompany.DataObject);

            var updatedContactManager = contactProxy.GetContact(updatedAggregate.ContactManager.Id.Value);
            Assert.IsTrue(updatedCompany.IsSuccessfull);
            Assert.IsNotNull(updatedCompany.DataObject);
            AssertContactManager(contactAggregate.ContactManager, updatedContactManager.DataObject);


        }

        [TestMethod]
        public void ShouldFailAggregateInsertIfCompanyLastUpdatedIdIncorrect()
        {
            var companyProxy = new CompanyProxy();
            var contactAggregateProxy = new ContactAggregateProxy();
            var contactAggregate = GetNewContactAggregate();
            var companyDetail = GetCompany();
            var companyResponse = companyProxy.InsertCompany(companyDetail);

            contactAggregate.Company.Id = companyResponse.DataObject.InsertedCompanyId;
            contactAggregate.Company.LastUpdatedId = companyResponse.DataObject.LastUpdatedId.ToLower();

            var aggregateResponse = contactAggregateProxy.InsertContactAggregate(contactAggregate);
            Assert.IsFalse(aggregateResponse.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, aggregateResponse.StatusCode);
            Assert.AreEqual("\"Record to be updated has changed since last read.\"", aggregateResponse.RawResponse);
        }

        [TestMethod]
        public void ShouldFailAggregateInsertIfContactManagerLastUpdatedIdIncorrect()
        {
            var contactProxy = new ContactProxy();
            var contactAggregateProxy = new ContactAggregateProxy();
            var contactAggregate = GetNewContactAggregate();
            var contact = GetContact();
            var contactResponse = contactProxy.InsertContact(contact);

            contactAggregate.ContactManager.Id = contactResponse.DataObject.InsertedContactId;
            contactAggregate.ContactManager.LastUpdatedId = contactResponse.DataObject.LastUpdatedId.ToLower();

            var aggregateResponse = contactAggregateProxy.InsertContactAggregate(contactAggregate);
            Assert.IsFalse(aggregateResponse.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, aggregateResponse.StatusCode);
            Assert.AreEqual("\"Record to be updated has changed since last read.\"", aggregateResponse.RawResponse);
        }

        [TestMethod]
        public void ShouldNotUpdateCompanyWhenContactLastUpdatedIdIncorrect()
        {
            var contactProxy = new ContactProxy();
            var companyProxy = new CompanyProxy();
            var contactAggregateProxy = new ContactAggregateProxy();
            var contactAggregate = GetNewContactAggregate();
            var companyDetail = GetCompany();
            var companyResponse = companyProxy.InsertCompany(companyDetail);

            var contact = GetContact();
            var contactResponse = contactProxy.InsertContact(contact);

            contactAggregate.Id = contactResponse.DataObject.InsertedContactId;
            contactAggregate.LastUpdatedId = contactResponse.DataObject.LastUpdatedId.ToLower();
            contactAggregate.Company.Id = companyResponse.DataObject.InsertedCompanyId;
            contactAggregate.Company.LastUpdatedId = companyResponse.DataObject.LastUpdatedId;

            var aggregateResponse = contactAggregateProxy.UpdateContactAggregate(contactAggregate, contactResponse.DataObject.InsertedContactId);
            Assert.IsFalse(aggregateResponse.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, aggregateResponse.StatusCode);
            Assert.AreEqual("\"Record to be updated has changed since last read.\"", aggregateResponse.RawResponse);

            var updatedCompany = companyProxy.GetCompany(companyResponse.DataObject.InsertedCompanyId);
            Assert.AreEqual(companyResponse.DataObject.LastUpdatedId, updatedCompany.DataObject.LastUpdatedId);
        }

        [TestMethod]
        public void ShouldNotUpdateContactManagerWhenContactLastUpdatedIdIncorrect()
        {
            var contactProxy = new ContactProxy();
            var contactAggregateProxy = new ContactAggregateProxy();
            var contactAggregate = GetNewContactAggregate();
            var contactManager = GetContact();
            var contactManagerResponse = contactProxy.InsertContact(contactManager);

            var contact = GetContact();
            var contactResponse = contactProxy.InsertContact(contact);

            contactAggregate.Id = contactResponse.DataObject.InsertedContactId;
            contactAggregate.LastUpdatedId = contactResponse.DataObject.LastUpdatedId.ToLower();
            contactAggregate.ContactManager.Id = contactManagerResponse.DataObject.InsertedContactId;
            contactAggregate.Company.LastUpdatedId = contactManagerResponse.DataObject.LastUpdatedId;

            var aggregateResponse = contactAggregateProxy.UpdateContactAggregate(contactAggregate, contactResponse.DataObject.InsertedContactId);
            Assert.IsFalse(aggregateResponse.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, aggregateResponse.StatusCode);
            Assert.AreEqual("\"Record to be updated has changed since last read.\"", aggregateResponse.RawResponse);

            var updatedContact = contactProxy.GetContact(contactManagerResponse.DataObject.InsertedContactId);
            Assert.AreEqual(contactManagerResponse.DataObject.LastUpdatedId, updatedContact.DataObject.LastUpdatedId);
        }

        [TestMethod]
        public void ShouldNotModifyExistingContactFieldsNotContainedInAggregateModel()
        {
            var contactProxy = new ContactProxy();
            var contactAggregateProxy = new ContactAggregateProxy();
            var contactAggregate = GetNewContactAggregate();

            var contact = GetCompleteContact();
            var contactResponse = contactProxy.InsertContact(contact);

            contactAggregate.Id = contactResponse.DataObject.InsertedContactId;
            contactAggregate.LastUpdatedId = contactResponse.DataObject.LastUpdatedId;

            var aggregateResponse = contactAggregateProxy.UpdateContactAggregate(contactAggregate, contactResponse.DataObject.InsertedContactId);
            Assert.IsTrue(aggregateResponse.IsSuccessfull);

            var updatedContactResponse = contactProxy.GetContact(contactResponse.DataObject.InsertedContactId);
            Assert.IsTrue(updatedContactResponse.IsSuccessfull);
            Assert.IsNotNull(updatedContactResponse.DataObject);
            var updatedContact = updatedContactResponse.DataObject;
            Assert.AreEqual(contact.AutoSendStatement, updatedContact.AutoSendStatement);
            Assert.AreEqual(contact.BpayDetails.BillerCode, updatedContact.BpayDetails.BillerCode);
            Assert.AreEqual(contact.BpayDetails.CRN, updatedContact.BpayDetails.CRN);
            Assert.AreEqual(contact.ChequeDetails.AcceptCheque, updatedContact.ChequeDetails.AcceptCheque);
            Assert.AreEqual(contact.ChequeDetails.ChequePayableTo, updatedContact.ChequeDetails.ChequePayableTo);
            Assert.AreEqual(contact.CustomField1, updatedContact.CustomField1);
            Assert.AreEqual(contact.CustomField2, updatedContact.CustomField2);
            Assert.AreEqual(contact.DefaultPurchaseDiscount, updatedContact.DefaultPurchaseDiscount);
            Assert.AreEqual(contact.DefaultSaleDiscount, updatedContact.DefaultSaleDiscount);
            Assert.AreEqual(contact.DirectDepositDetails.AcceptDirectDeposit, updatedContact.DirectDepositDetails.AcceptDirectDeposit);
            Assert.AreEqual(contact.DirectDepositDetails.AccountBSB, updatedContact.DirectDepositDetails.AccountBSB);
            Assert.AreEqual(contact.DirectDepositDetails.AccountName, updatedContact.DirectDepositDetails.AccountName);
            Assert.AreEqual(contact.DirectDepositDetails.AccountNumber, updatedContact.DirectDepositDetails.AccountNumber);
            Assert.AreEqual(contact.HomePhone, updatedContact.HomePhone);
            Assert.AreEqual(contact.LinkedInProfile, updatedContact.LinkedInProfile);
            Assert.AreEqual(contact.OtherAddress.City, updatedContact.OtherAddress.City);
            Assert.AreEqual(contact.OtherAddress.Country, updatedContact.OtherAddress.Country);
            Assert.AreEqual(contact.OtherAddress.Postcode, updatedContact.OtherAddress.Postcode);
            Assert.AreEqual(contact.OtherAddress.State, updatedContact.OtherAddress.State);
            Assert.AreEqual(contact.OtherAddress.Street, updatedContact.OtherAddress.Street);
            Assert.AreEqual(contact.OtherPhone, updatedContact.OtherPhone);
            Assert.AreEqual(contact.PurchaseTradingTerms.TradingTermsInterval, updatedContact.PurchaseTradingTerms.TradingTermsInterval);
            Assert.AreEqual(contact.PurchaseTradingTerms.TradingTermsIntervalType, updatedContact.PurchaseTradingTerms.TradingTermsIntervalType);
            Assert.AreEqual(contact.PurchaseTradingTerms.TradingTermsType, updatedContact.PurchaseTradingTerms.TradingTermsType);
            Assert.AreEqual(contact.SaleTradingTerms.TradingTermsInterval, updatedContact.SaleTradingTerms.TradingTermsInterval);
            Assert.AreEqual(contact.SaleTradingTerms.TradingTermsIntervalType, updatedContact.SaleTradingTerms.TradingTermsIntervalType);
            Assert.AreEqual(contact.SaleTradingTerms.TradingTermsType, updatedContact.SaleTradingTerms.TradingTermsType);
            Assert.AreEqual(contact.SkypeId, updatedContact.SkypeId);
            Assert.AreEqual(contact.TwitterId, updatedContact.TwitterId);
            Assert.AreEqual(contact.WebsiteUrl, updatedContact.WebsiteUrl);
        }

        private ContactAggregate GetNewContactAggregate()
        {
            var contactAggregate = new ContactAggregate
            {
                FamilyName = "Last".MakeUnique(),
                GivenName = "First".MakeUnique(),
                EmailAddress = "me@test".MakeUnique() + ".com",
                ContactId = "1234",
                Fax = "0244445555",
                IsContractor = true,
                IsCustomer = true,
                IsPartner = true,
                MiddleInitials = "PK",
                IsSupplier = true,
                MobilePhone = "0412341234",
                PositionTitle = "Super hero",
                PostalAddress = new Address()
                {
                    City = "Sydney",
                    Country = "Australia",
                    Postcode = "2000",
                    State = "NSW",
                    Street = "Elizabeth Street"
                },
                PrimaryPhone = "0278977897",
                Salutation = "Mr.",
                Company = new Company()
                {
                    Name = "MyCo".MakeUnique(),
                    Abn = "12345678",
                    CompanyEmail = "some@myco.com",
                    LongDescription = "My company",
                    TradingName = "MyTrading",
                },
                ContactManager = new ContactManager()
                {
                    FamilyName = "Pan".MakeUnique(),
                    GivenName = "Blossom".MakeUnique(),
                    MiddleInitials = "F",
                    Salutation = "Ms.",
                    PositionTitle = "High flyer",
                }
            };

            return contactAggregate;
        }

        private Contact GetContact()
        {
            var contact = new Contact()
            {
                FamilyName = "Stewart".MakeUnique(),
                IsActive = true,
                GivenName = "Jill".MakeUnique(),
                EmailAddress = "me@test".MakeUnique() + ".com",
                ContactId = "3333",
                Fax = "0211112222",
                IsContractor = false,
                IsCustomer = false,
                IsPartner = false,
                MiddleInitials = "MM",
                IsSupplier = false,
                MobilePhone = "0411112222",
                PositionTitle = "Manager",
                PrimaryPhone = "0222223333",
                Salutation = "Mrs."
            };
            return contact;
        }

        private Contact GetCompleteContact()
        {
            var contact = new Contact()
            {
                FamilyName = "Stewart".MakeUnique(),
                IsActive = true,
                GivenName = "Jill".MakeUnique(),
                EmailAddress = "me@test".MakeUnique() + ".com",
                ContactId = "3333",
                Fax = "0211112222",
                IsContractor = false,
                IsCustomer = false,
                IsPartner = false,
                MiddleInitials = "MM",
                IsSupplier = false,
                MobilePhone = "0411112222",
                PositionTitle = "Manager",
                PrimaryPhone = "0222223333",
                Salutation = "Mrs.",
                AutoSendStatement = true,
                BpayDetails = new BpayDetails { BillerCode = "wer", CRN = "crn1" },
                ChequeDetails = new ChequeDetails { AcceptCheque = true, ChequePayableTo = "someone" },
                CustomField1 = "Custom1",
                CustomField2 = "Custom2",
                DefaultPurchaseDiscount = 10,
                DefaultSaleDiscount = 12,
                DirectDepositDetails = new DirectDepositDetails { AcceptDirectDeposit = true, AccountBSB = "012210", AccountName = "Account name", AccountNumber = "123345" },
                HomePhone = "0234343434",
                OtherAddress = new Address { State = "NSW", Street = "Second street", Postcode = "2100", Country = "Australia", City = "Sydney" },
                PostalAddress = new Address { State = "NSW", Street = "First street", City = "Sydney", Postcode = "2000", Country = "Australia" },
                OtherPhone = "0221212121",
                PurchaseTradingTerms = new TradingTerms { TradingTermsInterval = 1, TradingTermsIntervalType = 2, TradingTermsType = 1 },
                SaleTradingTerms = new TradingTerms { TradingTermsInterval = 2, TradingTermsIntervalType = 2, TradingTermsType = 1 },
                WebsiteUrl = "http://test.com",
                LinkedInProfile = "linked",
                SkypeId = "skype",
                TwitterId = "@twit"
            };
            return contact;
        }

        private CompanyDetail GetCompany()
        {
            var companyDetail = new CompanyDetail
            {
                LongDescription = "Some ACME Company".MakeUnique(),
                TradingName = "Trading".MakeUnique(),
                Abn = "123456718",
                CompanyEmail = "co@email.com",
                Name = "ACME".MakeUnique(),
                CreatedDateUtc = DateTime.UtcNow,
                Website = "https://acme.com"
            };
            return companyDetail;
        }

        private static void AssertContactManager(ContactManager expectedContactManager,
            Contact actualContact)
        {

            Assert.AreEqual(expectedContactManager.FamilyName, actualContact.FamilyName,
                "FamilyName not updated for contact manager");
            Assert.AreEqual(expectedContactManager.GivenName, actualContact.GivenName,
                "GivenName not updated for contact manager");
            Assert.AreEqual(expectedContactManager.MiddleInitials,
                actualContact.MiddleInitials, "MiddleInitials not updated for contact manager");
            Assert.AreEqual(expectedContactManager.PositionTitle,
                actualContact.PositionTitle, "PositionTitle not updated for contact manager");
            Assert.AreEqual(expectedContactManager.Salutation, actualContact.Salutation,
                "Salutation not updated for contact manager");
        }

        private static void AssertCompany(Company aggregateCompany, CompanyDetail company)
        {

            Assert.AreEqual(aggregateCompany.TradingName, company.TradingName,
                "TradingName not updated for company");
            Assert.AreEqual(aggregateCompany.Abn, company.Abn,
                "ABN not updated for contact manager");
            Assert.AreEqual(aggregateCompany.CompanyEmail, company.CompanyEmail,
                "CompanyEmail not updated for company");
            Assert.AreEqual(aggregateCompany.LongDescription, company.LongDescription,
                "LongDescription not updated for company");
            Assert.AreEqual(aggregateCompany.Name, company.Name,
                "Name not updated for company");
        }

        private static void AssertUpdatedContact(ContactAggregate contactAggregate, ProxyResponse<Contact> updatedContact)
        {
            Assert.AreEqual(contactAggregate.EmailAddress, updatedContact.DataObject.EmailAddress);
            Assert.AreEqual(contactAggregate.ContactId, updatedContact.DataObject.ContactId);
            Assert.AreEqual(contactAggregate.FamilyName, updatedContact.DataObject.FamilyName);
            Assert.AreEqual(contactAggregate.Fax, updatedContact.DataObject.Fax);
            Assert.AreEqual(contactAggregate.GivenName, updatedContact.DataObject.GivenName);
            Assert.AreEqual(contactAggregate.IsPartner, updatedContact.DataObject.IsPartner);
            Assert.AreEqual(contactAggregate.IsContractor, updatedContact.DataObject.IsContractor);
            Assert.AreEqual(contactAggregate.IsCustomer, updatedContact.DataObject.IsCustomer);
            Assert.AreEqual(contactAggregate.IsPartner, updatedContact.DataObject.IsPartner);
            Assert.AreEqual(contactAggregate.IsSupplier, updatedContact.DataObject.IsSupplier);
            Assert.AreEqual(contactAggregate.MiddleInitials, updatedContact.DataObject.MiddleInitials);
            Assert.AreEqual(contactAggregate.MobilePhone, updatedContact.DataObject.MobilePhone);
            Assert.AreEqual(contactAggregate.PositionTitle, updatedContact.DataObject.PositionTitle);
            Assert.AreEqual(contactAggregate.PostalAddress.City, updatedContact.DataObject.PostalAddress.City);
            Assert.AreEqual(contactAggregate.PostalAddress.Country, updatedContact.DataObject.PostalAddress.Country);
            Assert.AreEqual(contactAggregate.PostalAddress.Postcode, updatedContact.DataObject.PostalAddress.Postcode);
            Assert.AreEqual(contactAggregate.PostalAddress.State, updatedContact.DataObject.PostalAddress.State);
            Assert.AreEqual(contactAggregate.PostalAddress.Street, updatedContact.DataObject.PostalAddress.Street);
            Assert.AreEqual(contactAggregate.PrimaryPhone, updatedContact.DataObject.PrimaryPhone);
            Assert.AreEqual(contactAggregate.Salutation, updatedContact.DataObject.Salutation);
        }
    }
}
