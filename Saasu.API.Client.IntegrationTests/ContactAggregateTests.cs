using Saasu.API.Client.IntegrationTests.Helpers;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Models;
using Saasu.API.Core.Models.Company;
using Saasu.API.Core.Models.ContactAggregates;
using Saasu.API.Core.Models.Contacts;
using System;
using System.Net;
using Xunit;

namespace Saasu.API.Client.IntegrationTests
{

    public class ContactAggregateTests
    {
        private ContactHelper _contactHelper;

        public ContactAggregateTests()
        {
            _contactHelper = new ContactHelper();
        }

        [Fact]
        public void ShouldAddNewContactWithNewCompanyAndContactManager()
        {
            var contactAggregateProxy = new ContactAggregateProxy();
            var contactProxy = new ContactProxy();
            var companyProxy = new CompanyProxy();
            var contactAggregate = GetNewContactAggregate();
            var Response = contactAggregateProxy.InsertContactAggregate(contactAggregate);

            Assert.True(Response.IsSuccessfull, "Contact aggregate insert failed.");
            Assert.NotNull(Response.DataObject);
            Assert.True(Response.DataObject.InsertedContactId > 0, "Invalid InsertedContactId returned from InsertContactAggregate");
            Assert.True(Response.DataObject.LastUpdatedId.Length > 0, "Invalid LastUpdatedId returned");
            Assert.True(Response.DataObject.LastModified > DateTime.UtcNow.AddMinutes(-5), "Invalid LastModified returned");
            Assert.True(Response.DataObject.CompanyId > 0, "Invalid CompanyId returned from InsertContactAggregate when inserting new company");
            Assert.True(Response.DataObject.CompanyLastUpdatedId.Length > 0, "Invalid CompanyLastUpdatedId returned from InsertContactAggregate when inserting new company");
            Assert.True(Response.DataObject.ContactManagerId > 0, "Invalid ContactManagerId returned from InsertContactAggregate when inserting new contact manager");
            Assert.True(Response.DataObject.ContactManagerLastUpdatedId.Length > 0, "Invalid ContactManagerLastUpdatedId returned from InsertContactAggregate when inserting new contact manager");

            var contactResponse = contactProxy.GetContact(Response.DataObject.InsertedContactId);
            Assert.True(contactResponse.IsSuccessfull, "Contact not found");
            Assert.NotNull(contactResponse.DataObject);
            Assert.NotNull(contactResponse.DataObject.CompanyId);
            Assert.NotNull(contactResponse.DataObject.ContactManagerId);

            var contactManagerResponse = contactProxy.GetContact(contactResponse.DataObject.ContactManagerId.Value);
            Assert.True(contactManagerResponse.IsSuccessfull, "Contact manager not found");
            Assert.Equal(contactAggregate.ContactManager.FamilyName, contactManagerResponse.DataObject.FamilyName);

            var companyResponse = companyProxy.GetCompany(contactResponse.DataObject.CompanyId.Value);
            Assert.True(companyResponse.IsSuccessfull, "Company not found");
            Assert.Equal(contactAggregate.Company.Name, companyResponse.DataObject.Name);
        }

        [Fact]
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
            Assert.True(contactAggregateResponse.IsSuccessfull, "Contact aggregate with new contact and updated company and contact manager failed");
            Assert.NotNull(contactAggregateResponse.DataObject);
            Assert.True(contactAggregateResponse.DataObject.CompanyId > 0, "Invalid CompanyId returned from InsertContactAggregate when updating company");
            Assert.True(contactAggregateResponse.DataObject.CompanyLastUpdatedId.Length > 0, "Invalid CompanyLastUpdatedId returned from InsertContactAggregate when updating company");
            Assert.True(contactAggregateResponse.DataObject.ContactManagerId > 0, "Invalid ContactManagerId returned from InsertContactAggregate when updating contact manager");
            Assert.True(contactAggregateResponse.DataObject.ContactManagerLastUpdatedId.Length > 0, "Invalid ContactManagerLastUpdatedId returned from InsertContactAggregate when updating contact manager");

            var dbContactAggregate =
                contactAggregateProxy.GetContactAggregate(contactAggregateResponse.DataObject.InsertedContactId);
            Assert.True(dbContactAggregate.IsSuccessfull);
            Assert.NotNull(dbContactAggregate.DataObject);
            Assert.NotNull(dbContactAggregate.DataObject.ContactManager.Id);
            Assert.NotNull(dbContactAggregate.DataObject.Company.Id);
            Assert.True(companyResponse.DataObject.InsertedCompanyId == dbContactAggregate.DataObject.Company.Id.Value, "Existing company not attached to contact aggregate");
            Assert.True(contactManagerResponse.DataObject.InsertedContactId == dbContactAggregate.DataObject.ContactManager.Id.Value, "Existing contact (manager) not attached to contact aggregate");

            var dbContactManager = contactProxy.GetContact(dbContactAggregate.DataObject.ContactManager.Id.Value);
            Assert.True(dbContactManager.IsSuccessfull);
            Assert.NotNull(dbContactManager.DataObject);

            AssertContactManager(dbContactAggregate.DataObject.ContactManager, dbContactManager.DataObject);
            Assert.True(dbContactAggregate.DataObject.ContactManager.LastUpdatedId ==
                dbContactManager.DataObject.LastUpdatedId, "LastUpdatedId not updated for contact manager");

            var dbCompany = companyProxy.GetCompany(dbContactAggregate.DataObject.Company.Id.Value);
            Assert.True(dbCompany.IsSuccessfull);
            Assert.NotNull(dbCompany.DataObject);

            AssertCompany(dbContactAggregate.DataObject.Company, dbCompany.DataObject);
            Assert.True(dbContactAggregate.DataObject.Company.LastUpdatedId == dbCompany.DataObject.LastUpdatedId,
                "LastUpdatedId not updated for company");
        }

        [Fact]
        public void ShouldUpdateContactAndAddNewCompanyAndContactManager()
        {
            var contactProxy = new ContactProxy();
            var contactAggregateProxy = new ContactAggregateProxy();
            var contact = GetContact();
            var contactResponse = contactProxy.InsertContact(contact);
            Assert.True(contactResponse.IsSuccessfull);

            var contactAggregate = GetNewContactAggregate();

            var contactAggregateResponse = contactAggregateProxy.UpdateContactAggregate(contactAggregate, contactResponse.DataObject.InsertedContactId);
            Assert.True(contactAggregateResponse.IsSuccessfull, "Failed to update existing contact and add new company and contact manager");
            Assert.NotNull(contactAggregateResponse.DataObject);
            Assert.Equal(contactResponse.DataObject.InsertedContactId, contactAggregateResponse.DataObject.UpdatedContactId);
            Assert.True(contactAggregateResponse.DataObject.CompanyId > 0, "Invalid CompanyId returned from UpdateContactAggregate when inserting new company");
            Assert.True(contactAggregateResponse.DataObject.CompanyLastUpdatedId.Length > 0, "Invalid CompanyLastUpdatedId returned from UpdateContactAggregate when inserting new company");
            Assert.True(contactAggregateResponse.DataObject.ContactManagerId > 0, "Invalid ContactManagerId returned from UpdateContactAggregate when inserting new contact manager");
            Assert.True(contactAggregateResponse.DataObject.ContactManagerLastUpdatedId.Length > 0, "Invalid ContactManagerLastUpdatedId returned from UpdateContactAggregate when inserting new contact manager");

            var updatedContact = contactProxy.GetContact(contactResponse.DataObject.InsertedContactId);
            AssertUpdatedContact(contactAggregate, updatedContact);

            var insertedAggregate =
                contactAggregateProxy.GetContactAggregate(contactResponse.DataObject.InsertedContactId);
            Assert.True(insertedAggregate.IsSuccessfull);
            Assert.NotNull(insertedAggregate.DataObject);
            Assert.True(insertedAggregate.DataObject.Company != null, "New company not associated with existing contact");
            Assert.True(insertedAggregate.DataObject.ContactManager != null, "New contact manager not associated with existing contact");
            Assert.Equal(contactAggregate.Company.Name, insertedAggregate.DataObject.Company.Name);
            Assert.Equal(contactAggregate.ContactManager.GivenName, insertedAggregate.DataObject.ContactManager.GivenName);
        }



        [Fact]
        public void ShouldUpdateContactAndUpdateCompanyAndContactManager()
        {
            var contactProxy = new ContactProxy();
            var companyProxy = new CompanyProxy();
            var contactAggregateProxy = new ContactAggregateProxy();
            var contact = GetContact();
            var contactResponse = contactProxy.InsertContact(contact);
            Assert.True(contactResponse.IsSuccessfull);
            var contactManager = GetContact();
            var contactManagerResponse = contactProxy.InsertContact(contactManager);
            Assert.True(contactManagerResponse.IsSuccessfull);
            var company = GetCompany();
            var companyResponse = companyProxy.InsertCompany(company);
            Assert.True(companyResponse.IsSuccessfull);

            var contactAggregate = GetNewContactAggregate();
            contactAggregate.Id = contactResponse.DataObject.InsertedContactId;
            contactAggregate.ContactManager.Id = contactManagerResponse.DataObject.InsertedContactId;
            contactAggregate.ContactManager.LastUpdatedId = contactManagerResponse.DataObject.LastUpdatedId;
            contactAggregate.Company.Id = companyResponse.DataObject.InsertedCompanyId;
            contactAggregate.Company.LastUpdatedId = companyResponse.DataObject.LastUpdatedId;

            var contactAggregateUpdateResponse = contactAggregateProxy.UpdateContactAggregate(contactAggregate,
                contactResponse.DataObject.InsertedContactId);
            Assert.True(contactAggregateUpdateResponse.IsSuccessfull, "Updating contact and company and contact manager in aggregate failed");
            Assert.NotNull(contactAggregateUpdateResponse.DataObject);
            Assert.True(contactAggregateUpdateResponse.DataObject.CompanyId > 0, "Invalid CompanyId returned from UpdateContactAggregate when updating company");
            Assert.True(contactAggregateUpdateResponse.DataObject.CompanyLastUpdatedId.Length > 0, "Invalid CompanyLastUpdatedId returned from UpdateContactAggregate when updating company");
            Assert.True(contactAggregateUpdateResponse.DataObject.ContactManagerId > 0, "Invalid ContactManagerId returned from UpdateContactAggregate when updating contact manager");
            Assert.True(contactAggregateUpdateResponse.DataObject.ContactManagerLastUpdatedId.Length > 0, "Invalid ContactManagerLastUpdatedId returned from UpdateContactAggregate when updating contact manager");

            var updatedContactAggregateResponse =
                contactAggregateProxy.GetContactAggregate(contactResponse.DataObject.InsertedContactId);
            Assert.True(updatedContactAggregateResponse.IsSuccessfull, "Updating contact and company and contact manager in aggregate failed");
            Assert.NotNull(updatedContactAggregateResponse.DataObject);
            var updatedAggregate = updatedContactAggregateResponse.DataObject;
            Assert.True(updatedAggregate.Company != null, "Company not associated with updated aggregate");
            Assert.True(updatedAggregate.ContactManager != null, "Contact manager not associated with updated aggregate");
            Assert.NotNull(updatedAggregate.Company.Id);
            Assert.NotNull(updatedAggregate.ContactManager.Id);

            var updatedContact = contactProxy.GetContact(contactResponse.DataObject.InsertedContactId);
            AssertUpdatedContact(contactAggregate, updatedContact);

            var updatedCompany = companyProxy.GetCompany(updatedAggregate.Company.Id.Value);
            Assert.True(updatedCompany.IsSuccessfull);
            Assert.NotNull(updatedCompany.DataObject);

            AssertCompany(contactAggregate.Company, updatedCompany.DataObject);

            var updatedContactManager = contactProxy.GetContact(updatedAggregate.ContactManager.Id.Value);
            Assert.True(updatedCompany.IsSuccessfull);
            Assert.NotNull(updatedCompany.DataObject);
            AssertContactManager(contactAggregate.ContactManager, updatedContactManager.DataObject);


        }

        [Fact]
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
            Assert.False(aggregateResponse.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, aggregateResponse.StatusCode);
            Assert.Equal("\"Record to be updated has changed since last read.\"", aggregateResponse.RawResponse);
        }

        [Fact]
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
            Assert.False(aggregateResponse.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, aggregateResponse.StatusCode);
            Assert.Equal("\"Record to be updated has changed since last read.\"", aggregateResponse.RawResponse);
        }

        [Fact]
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
            Assert.False(aggregateResponse.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, aggregateResponse.StatusCode);
            Assert.Equal("\"Record to be updated has changed since last read.\"", aggregateResponse.RawResponse);

            var updatedCompany = companyProxy.GetCompany(companyResponse.DataObject.InsertedCompanyId);
            Assert.Equal(companyResponse.DataObject.LastUpdatedId, updatedCompany.DataObject.LastUpdatedId);
        }

        [Fact]
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
            Assert.False(aggregateResponse.IsSuccessfull);
            Assert.Equal(HttpStatusCode.BadRequest, aggregateResponse.StatusCode);
            Assert.Equal("\"Record to be updated has changed since last read.\"", aggregateResponse.RawResponse);

            var updatedContact = contactProxy.GetContact(contactManagerResponse.DataObject.InsertedContactId);
            Assert.Equal(contactManagerResponse.DataObject.LastUpdatedId, updatedContact.DataObject.LastUpdatedId);
        }

        [Fact]
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
            Assert.True(aggregateResponse.IsSuccessfull);

            var updatedContactResponse = contactProxy.GetContact(contactResponse.DataObject.InsertedContactId);
            Assert.True(updatedContactResponse.IsSuccessfull);
            Assert.NotNull(updatedContactResponse.DataObject);
            var updatedContact = updatedContactResponse.DataObject;
            Assert.Equal(contact.AutoSendStatement, updatedContact.AutoSendStatement);
            Assert.Equal(contact.BpayDetails.BillerCode, updatedContact.BpayDetails.BillerCode);
            Assert.Equal(contact.BpayDetails.CRN, updatedContact.BpayDetails.CRN);
            Assert.Equal(contact.ChequeDetails.AcceptCheque, updatedContact.ChequeDetails.AcceptCheque);
            Assert.Equal(contact.ChequeDetails.ChequePayableTo, updatedContact.ChequeDetails.ChequePayableTo);
            Assert.Equal(contact.CustomField1, updatedContact.CustomField1);
            Assert.Equal(contact.CustomField2, updatedContact.CustomField2);
            Assert.Equal(contact.DefaultPurchaseDiscount, updatedContact.DefaultPurchaseDiscount);
            Assert.Equal(contact.DefaultSaleDiscount, updatedContact.DefaultSaleDiscount);
            Assert.Equal(contact.DirectDepositDetails.AcceptDirectDeposit, updatedContact.DirectDepositDetails.AcceptDirectDeposit);
            Assert.Equal(contact.DirectDepositDetails.AccountBSB, updatedContact.DirectDepositDetails.AccountBSB);
            Assert.Equal(contact.DirectDepositDetails.AccountName, updatedContact.DirectDepositDetails.AccountName);
            Assert.Equal(contact.DirectDepositDetails.AccountNumber, updatedContact.DirectDepositDetails.AccountNumber);
            Assert.Equal(contact.HomePhone, updatedContact.HomePhone);
            Assert.Equal(contact.LinkedInProfile, updatedContact.LinkedInProfile);
            Assert.Equal(contact.OtherAddress.City, updatedContact.OtherAddress.City);
            Assert.Equal(contact.OtherAddress.Country, updatedContact.OtherAddress.Country);
            Assert.Equal(contact.OtherAddress.Postcode, updatedContact.OtherAddress.Postcode);
            Assert.Equal(contact.OtherAddress.State, updatedContact.OtherAddress.State);
            Assert.Equal(contact.OtherAddress.Street, updatedContact.OtherAddress.Street);
            Assert.Equal(contact.OtherPhone, updatedContact.OtherPhone);
            Assert.Equal(contact.PurchaseTradingTerms.TradingTermsInterval, updatedContact.PurchaseTradingTerms.TradingTermsInterval);
            Assert.Equal(contact.PurchaseTradingTerms.TradingTermsIntervalType, updatedContact.PurchaseTradingTerms.TradingTermsIntervalType);
            Assert.Equal(contact.PurchaseTradingTerms.TradingTermsType, updatedContact.PurchaseTradingTerms.TradingTermsType);
            Assert.Equal(contact.SaleTradingTerms.TradingTermsInterval, updatedContact.SaleTradingTerms.TradingTermsInterval);
            Assert.Equal(contact.SaleTradingTerms.TradingTermsIntervalType, updatedContact.SaleTradingTerms.TradingTermsIntervalType);
            Assert.Equal(contact.SaleTradingTerms.TradingTermsType, updatedContact.SaleTradingTerms.TradingTermsType);
            Assert.Equal(contact.SkypeId, updatedContact.SkypeId);
            Assert.Equal(contact.TwitterId, updatedContact.TwitterId);
            Assert.Equal(contact.WebsiteUrl, updatedContact.WebsiteUrl);
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
                HomePhone = "0245674567",
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
                HomePhone = "0245674567",
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
                HomePhone = "0245674567",
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

            Assert.True(expectedContactManager.FamilyName == actualContact.FamilyName,
                "FamilyName not updated for contact manager");
            Assert.True( expectedContactManager.GivenName == actualContact.GivenName,
                "GivenName not updated for contact manager");
            Assert.True(expectedContactManager.MiddleInitials ==
                actualContact.MiddleInitials, "MiddleInitials not updated for contact manager");
            Assert.True(expectedContactManager.PositionTitle ==
                actualContact.PositionTitle, "PositionTitle not updated for contact manager");
            Assert.True(expectedContactManager.Salutation == actualContact.Salutation,
                "Salutation not updated for contact manager");
        }

        private static void AssertCompany(Company aggregateCompany, CompanyDetail company)
        {

            Assert.True(aggregateCompany.TradingName ==company.TradingName,
                "TradingName not updated for company");
            Assert.True(aggregateCompany.Abn == company.Abn,
                "ABN not updated for contact manager");
            Assert.True(aggregateCompany.CompanyEmail == company.CompanyEmail,
                "CompanyEmail not updated for company");
            Assert.True(aggregateCompany.LongDescription == company.LongDescription,
                "LongDescription not updated for company");
            Assert.True(aggregateCompany.Name == company.Name,
                "Name not updated for company");
        }

        private static void AssertUpdatedContact(ContactAggregate contactAggregate, ProxyResponse<Contact> updatedContact)
        {
            Assert.Equal(contactAggregate.EmailAddress, updatedContact.DataObject.EmailAddress);
            Assert.Equal(contactAggregate.ContactId, updatedContact.DataObject.ContactId);
            Assert.Equal(contactAggregate.FamilyName, updatedContact.DataObject.FamilyName);
            Assert.Equal(contactAggregate.Fax, updatedContact.DataObject.Fax);
            Assert.Equal(contactAggregate.GivenName, updatedContact.DataObject.GivenName);
            Assert.Equal(contactAggregate.IsPartner, updatedContact.DataObject.IsPartner);
            Assert.Equal(contactAggregate.IsContractor, updatedContact.DataObject.IsContractor);
            Assert.Equal(contactAggregate.IsCustomer, updatedContact.DataObject.IsCustomer);
            Assert.Equal(contactAggregate.IsPartner, updatedContact.DataObject.IsPartner);
            Assert.Equal(contactAggregate.IsSupplier, updatedContact.DataObject.IsSupplier);
            Assert.Equal(contactAggregate.MiddleInitials, updatedContact.DataObject.MiddleInitials);
            Assert.Equal(contactAggregate.MobilePhone, updatedContact.DataObject.MobilePhone);
            Assert.Equal(contactAggregate.HomePhone, updatedContact.DataObject.HomePhone);
            Assert.Equal(contactAggregate.PositionTitle, updatedContact.DataObject.PositionTitle);
            Assert.Equal(contactAggregate.PostalAddress.City, updatedContact.DataObject.PostalAddress.City);
            Assert.Equal(contactAggregate.PostalAddress.Country, updatedContact.DataObject.PostalAddress.Country);
            Assert.Equal(contactAggregate.PostalAddress.Postcode, updatedContact.DataObject.PostalAddress.Postcode);
            Assert.Equal(contactAggregate.PostalAddress.State, updatedContact.DataObject.PostalAddress.State);
            Assert.Equal(contactAggregate.PostalAddress.Street, updatedContact.DataObject.PostalAddress.Street);
            Assert.Equal(contactAggregate.PrimaryPhone, updatedContact.DataObject.PrimaryPhone);
            Assert.Equal(contactAggregate.Salutation, updatedContact.DataObject.Salutation);
        }
    }
}
