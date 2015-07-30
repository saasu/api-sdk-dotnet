using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Company;

namespace Saasu.API.Client.IntegrationTests
{
    [TestFixture]
    public class CompanyTests
    {
        [Test]
        public void ShouldRetrieveTaxCodeListForFile()
        {
            var proxy = new CompaniesProxy();
            var response = proxy.GetCompanies(null, null, null, null, null);
            Assert.IsNotNull(response, "No response returned for companies request.");
            Assert.IsTrue(response.IsSuccessfull, "Request for companies was not processed successfully.");
            Assert.IsTrue(response.DataObject.Companies.Count > 0, "No companies were retrieved for file.");
            Assert.IsTrue(response.DataObject.Companies.Count >= 5, "Expected at least 5 companies to be returned.");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ShouldRetrieveCompanyById()
        {
            var proxy = new CompaniesProxy();
            var listResponse = proxy.GetCompanies(null, null, null, null, null);

            Assert.IsTrue(listResponse.DataObject.Companies.Count > 0, "No companies were retrieved for file.");

            var company = listResponse.DataObject.Companies[0];
            var companyProxy = new CompanyProxy();
            var byIdResponse = companyProxy.GetCompany(company.Id.Value);
            Assert.IsNotNull(byIdResponse, "No response returned for company by Id request.");
            Assert.IsTrue(byIdResponse.IsSuccessfull, "Request for company by Id was not processed successfully.");
            Assert.AreEqual(HttpStatusCode.OK, byIdResponse.StatusCode);

            var companyById = byIdResponse.DataObject;
            Assert.IsNotNull(companyById);
            Assert.AreEqual(company.Id, companyById.Id, "Incorrect Company Id.");
            Assert.AreEqual(company.Abn, companyById.Abn, "Incorrect Abn");
            Assert.AreEqual(company.CompanyEmail, companyById.CompanyEmail, "Incorrect company email");
            Assert.AreEqual(company.CreatedDateUtc, companyById.CreatedDateUtc, "Incorrect created date");
            Assert.AreEqual(company.LastModifiedByUserId, companyById.LastModifiedByUserId, "Incorrect last modified by user id");
            Assert.AreEqual(company.LastModifiedDateUtc, companyById.LastModifiedDateUtc, "Incorrect last modified date");
            Assert.AreEqual(company.LastUpdatedId, companyById.LastUpdatedId, "Incorrect last updated id");
            Assert.AreEqual(company.LongDescription, companyById.LongDescription, "Incorrect long description");
            Assert.AreEqual(company.Name, companyById.Name, "Incorrect name");
            Assert.AreEqual(company.TradingName, companyById.TradingName, "Incorrect trading name");
            Assert.AreEqual(company.Website, companyById.Website, "Incorrect website");
        }

        [Test]
        public void ShouldRetrieveCompanyByNameFilter()
        {
            var proxy = new CompaniesProxy();
            var listResponse = proxy.GetCompanies(null, null, null, null, null);
            Assert.IsNotNull(listResponse);
            Assert.IsNotNull(listResponse.DataObject);
            Assert.IsTrue(listResponse.DataObject.Companies.Count > 0);

            // Get a subset of a companies name to use as the filter value
            var companyToQuery = listResponse.DataObject.Companies.FirstOrDefault(n => n.Name.Length > 5);
            Assert.IsNotNull(companyToQuery, "Did not find a company with name greater than 5 chars");
            var nameToFilterOn = companyToQuery.Name;
            var filterValue = nameToFilterOn.Substring(0, nameToFilterOn.Length - 2);

            var filterResponse = proxy.GetCompanies(filterValue, null, null, null, null);
            Assert.IsNotNull(filterResponse);
            Assert.IsNotNull(filterResponse.DataObject);
            Assert.IsTrue(filterResponse.DataObject.Companies.Count >= 1, "Did not find at least one company with the matching filter value");

            var originalCompany = filterResponse.DataObject.Companies.FirstOrDefault(c => c.Id == companyToQuery.Id);
            Assert.IsNotNull(originalCompany, "Did not find original company in results that used the companies name to do the query");

        }

        [Test]
        public void ShouldRetrieveCompanyByLastModifiedFilter()
        {
            var proxy = new CompaniesProxy();
            var listResponse = proxy.GetCompanies(null, null, null, null, null);
            Assert.IsNotNull(listResponse);
            Assert.IsNotNull(listResponse.DataObject);
            Assert.IsTrue(listResponse.DataObject.Companies.Count > 0);

            // Get a subset of a companies name to use as the filter value
            var companyToQuery = listResponse.DataObject.Companies.First();

            var filterResponse = proxy.GetCompanies(null, companyToQuery.LastModifiedDateUtc, companyToQuery.LastModifiedDateUtc, null, null);
            Assert.IsNotNull(filterResponse);
            Assert.IsNotNull(filterResponse.DataObject);
            Assert.IsTrue(filterResponse.DataObject.Companies.Count >= 1, "Did not find at least one company with the matching filter value");

            var originalCompany = filterResponse.DataObject.Companies.FirstOrDefault(c => c.Id == companyToQuery.Id);
            Assert.IsNotNull(originalCompany, "Did not find original company in results that used the companies last modified date to do the query");

        }

        [Test]
        public void ShouldBeAbleToPageCompanies()
        {
            var proxy = new CompaniesProxy();
            var listResponse = proxy.GetCompanies(null, null, null, null, null);
            Assert.IsNotNull(listResponse);
            Assert.IsNotNull(listResponse.DataObject);
            Assert.IsTrue(listResponse.DataObject.Companies.Count >= 5); // In setup we add at least 5 companies

            listResponse = proxy.GetCompanies(null, null, null, 1, 3);
            Assert.IsTrue(listResponse.DataObject.Companies.Count == 3);

            listResponse = proxy.GetCompanies(null, null, null, 2, 3);
            Assert.IsTrue(listResponse.DataObject.Companies.Count >= 2);
        }

        [Test]
        public void ShouldInsertCompany()
        {
            var proxy = new CompanyProxy();
            
            var company = GetTestCompany();
            var insertResponse = proxy.InsertCompany(company);
            
            Assert.IsTrue(insertResponse.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.OK, insertResponse.StatusCode);
            Assert.Greater(insertResponse.DataObject.InsertedCompanyId, 0);

            var insertedCompany = proxy.GetCompany(insertResponse.DataObject.InsertedCompanyId).DataObject;

            Assert.IsNotNullOrEmpty(insertedCompany.Abn, "Company ABN not saved");
            Assert.AreEqual(company.Abn, insertedCompany.Abn, "Incorrect ABN");
            Assert.IsNotNullOrEmpty(insertedCompany.CompanyEmail, "Company email not saved");
            Assert.AreEqual(company.CompanyEmail, insertedCompany.CompanyEmail, "Incorrect Company email");
            Assert.IsNotNullOrEmpty(insertedCompany.LongDescription, "Company description not saved");
            Assert.AreEqual(company.LongDescription, insertedCompany.LongDescription, "Incorrect company description");
            Assert.IsNotNullOrEmpty(insertedCompany.TradingName, "Company trading name not saved");
            Assert.AreEqual(company.TradingName, insertedCompany.TradingName, "Incorrect company trading name");
            Assert.IsNotNullOrEmpty(insertedCompany.Website, "Company website not saved");
            Assert.AreEqual(company.Website, insertedCompany.Website, "Incorrect company website");
            Assert.IsNotNullOrEmpty(insertedCompany.LastUpdatedId, "Company LastUpdatedId not returned");
            Assert.IsNotNull(insertedCompany.CreatedDateUtc, "Company CreatedDateUtc not returned");
            Assert.IsNotNull(insertedCompany.LastModifiedDateUtc, "Company LastModifiedDateUtc not returned");
            Assert.IsNotNull(insertedCompany.LastModifiedByUserId, "Company LastModifiedByUserId not returned");
        }

        [Test]
        public void ShouldUpdateCompany()
        {
            var proxy = new CompanyProxy();
            
            var company = GetTestCompany();
            var insertResponse = proxy.InsertCompany(company);

            Assert.IsTrue(insertResponse.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.OK, insertResponse.StatusCode);
            Assert.Greater(insertResponse.DataObject.InsertedCompanyId, 0);

            var insertedCompany = proxy.GetCompany(insertResponse.DataObject.InsertedCompanyId).DataObject;
            
            insertedCompany.LongDescription = "Modified company";
            insertedCompany.Abn = "123456789";
            insertedCompany.CompanyEmail = "updated@bigboss@somecompany.com";
            insertedCompany.TradingName = "Updated Trading Name";
            insertedCompany.Name = "Updated Name";
            insertedCompany.Website = "http://updated.com";

            var updateResponse = proxy.UpdateCompany(insertedCompany, insertResponse.DataObject.InsertedCompanyId);
            Assert.IsTrue(updateResponse.IsSuccessfull);
            Assert.AreEqual(insertResponse.DataObject.InsertedCompanyId, updateResponse.DataObject.UpdatedCompanyId);

            var updatedCompany = proxy.GetCompany(updateResponse.DataObject.UpdatedCompanyId).DataObject;

            Assert.AreEqual(updatedCompany.Abn, insertedCompany.Abn, "Incorrect ABN");
            Assert.AreEqual(updatedCompany.CompanyEmail, insertedCompany.CompanyEmail, "Incorrect Company email");
            Assert.AreEqual(updatedCompany.LongDescription, insertedCompany.LongDescription, "Incorrect company description");
            Assert.AreEqual(updatedCompany.TradingName, insertedCompany.TradingName, "Incorrect company trading name");
            Assert.AreEqual(updatedCompany.Website, insertedCompany.Website, "Incorrect company website");
            Assert.IsNotNullOrEmpty(updatedCompany.LastUpdatedId, "Company LastUpdatedId not returned");
            Assert.IsNotNull(updatedCompany.CreatedDateUtc, "Company CreatedDateUtc not returned");
            Assert.IsNotNull(updatedCompany.LastModifiedDateUtc, "Company LastModifiedDateUtc not returned");
            Assert.IsNotNull(updatedCompany.LastModifiedByUserId, "Company LastModifiedByUserId not returned");

        }

        [Test]
        public void ShouldDeleteCompany()
        {
            var proxy = new CompanyProxy();

            var company = GetTestCompany();
            var insertResponse = proxy.InsertCompany(company);

            Assert.IsTrue(insertResponse.IsSuccessfull);
            Assert.Greater(insertResponse.DataObject.InsertedCompanyId, 0);

            var deleteResponse = proxy.DeleteCompany(insertResponse.DataObject.InsertedCompanyId);

            Assert.IsTrue(deleteResponse.IsSuccessfull);

            var deletedCompany = proxy.GetCompany(insertResponse.DataObject.InsertedCompanyId);
            Assert.IsNull(deletedCompany.DataObject);
            Assert.AreEqual(HttpStatusCode.NotFound, deletedCompany.StatusCode);


        }

        [TestFixtureSetUp]
        public void SetupTestData()
        {
            var proxy = new CompaniesProxy();
            ContactTests.AddBradPartner(proxy.WsAccessKey, proxy.FileId);
            ContactTests.AddCarlCustomer(proxy.WsAccessKey, proxy.FileId);
            ContactTests.AddJennySupplier(proxy.WsAccessKey, proxy.FileId);
            ContactTests.AddKathyContractor(proxy.WsAccessKey, proxy.FileId);
            ContactTests.AddBradPartner(proxy.WsAccessKey, proxy.FileId);
        }

        private CompanyDetail GetTestCompany()
        {
            var company = new CompanyDetail
            {
                Abn = "53004085616",
                Name = "MakeMoney Inc2." + System.Guid.NewGuid(),
                Website = "https://makemoney.org",
                LongDescription = "A test company",
                TradingName = "MakeMoney Inc",
                CompanyEmail = "bigboss@somecompany.com",
            };

            return company;
        }
    }
}
