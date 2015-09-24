using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saasu.API.Client.IntegrationTests.Helpers;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models;
using Saasu.API.Core.Models.ContactAggregates;
using System;

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
            var result = contactAggregateProxy.InsertContactAggregate(contactAggregate);

            Assert.IsTrue(result.IsSuccessfull, "Contact aggregate insert failed.");
            Assert.IsNotNull(result.DataObject);
            Assert.IsTrue(result.DataObject.InsertedContactId > 0, "Invalid InsertedContactId returned from InsertContactAggregate");
            Assert.IsTrue(result.DataObject.LastUpdatedId.Length > 0, "Invalid LastUpdatedId returned");
            Assert.IsTrue(result.DataObject.LastModified > DateTime.UtcNow.AddMinutes(-5), "Invalid LastModified returned");

            var contactResponse = contactProxy.GetContact(result.DataObject.InsertedContactId);
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

        }

        [TestMethod]
        public void ShouldUpdateContactAndAddNewCompanyAndContactManager()
        {

        }

        [TestMethod]
        public void ShouldUpdateContactAndUpdateCompanyAndContactManager()
        {

        }

        [TestMethod]
        public void ShouldFailAggregateInsertIfCompanyLastUpdatedIdIncorrect()
        {

        }

        [TestMethod]
        public void ShouldFailAggregateInsertIfContactManagerLastUpdatedIdIncorrect()
        {

        }

        [TestMethod]
        public void ShouldFailCompanyUpdateWhenContactLastUpdatedIdIncorrect()
        {

        }

        [TestMethod]
        public void ShouldFailContactManagerUpdateWhenContactLastUpdatedIdIncorrect()
        {

        }

        [TestMethod]
        public void ShouldNotModifyExistingContactFieldsNotContainedInAggregateModel()
        {

        }

        private ContactAggregate GetNewContactAggregate()
        {
            var contact = _contactHelper.AddContact();
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
    }
}
