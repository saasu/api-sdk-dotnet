using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Company;

namespace Saasu.API.Client.IntegrationTests
{
    public class CompanyTests
    {
        [Fact]
        public void ShouldRetrieveTaxCodeListForFile()
        {
            var proxy = new CompaniesProxy();
            var response = proxy.GetCompanies(null, null, null, null, null);
            Assert.NotNull(response);
            Assert.True(response.IsSuccessfull, "Request for companies was not processed successfully.");
            Assert.True(response.DataObject.Companies.Count > 0, "No companies were retrieved for file.");
            Assert.True(response.DataObject.Companies.Count >= 5, "Expected at least 5 companies to be returned.");
            Assert.Equal(response.StatusCode, HttpStatusCode.OK);
        }

        [Fact]
        public void ShouldRetrieveCompanyById()
        {
            var proxy = new CompaniesProxy();
            var listResponse = proxy.GetCompanies(null, null, null, null, null);

            Assert.True(listResponse.DataObject.Companies.Count > 0, "No companies were retrieved for file.");

            var company = listResponse.DataObject.Companies[0];
            var companyProxy = new CompanyProxy();
            var byIdResponse = companyProxy.GetCompany(company.Id.Value);
            Assert.NotNull(byIdResponse);
            Assert.True(byIdResponse.IsSuccessfull, "Request for company by Id was not processed successfully.");
            Assert.Equal(byIdResponse.StatusCode, HttpStatusCode.OK);

            var companyById = byIdResponse.DataObject;
            Assert.NotNull(companyById);
            Assert.Equal(companyById.Id, company.Id);
            Assert.Equal(companyById.Abn, company.Abn);
            Assert.Equal(companyById.CompanyEmail, company.CompanyEmail);
            Assert.Equal(companyById.CreatedDateUtc, company.CreatedDateUtc);
            Assert.Equal(companyById.LastModifiedByUserId, company.LastModifiedByUserId);
            Assert.True(TestHelper.AssertDatetimesEqualWithVariance(company.LastModifiedDateUtc, companyById.LastModifiedDateUtc), "Incorrect last modified date");
            Assert.Equal(companyById.LastUpdatedId, company.LastUpdatedId);
            Assert.Equal(companyById.LongDescription, company.LongDescription);
            Assert.Equal(companyById.Name, company.Name);
            Assert.Equal(companyById.TradingName, company.TradingName);
            Assert.Equal(companyById.Website, company.Website);
        }

        [Fact]
        public void ShouldRetrieveCompanyByNameFilter()
        {
            var proxy = new CompaniesProxy();
            var listResponse = proxy.GetCompanies(null, null, null, null, null);
            Assert.NotNull(listResponse);
            Assert.NotNull(listResponse.DataObject);
            Assert.True(listResponse.DataObject.Companies.Count > 0);

            // Get a subset of a companies name to use as the filter value
            var companyToQuery = listResponse.DataObject.Companies.FirstOrDefault(n => n.Name.Length > 5);
            Assert.NotNull(companyToQuery);
            var nameToFilterOn = companyToQuery.Name;
            var filterValue = nameToFilterOn.Substring(0, nameToFilterOn.Length - 2);

            var filterResponse = proxy.GetCompanies(filterValue, null, null, null, null);
            Assert.NotNull(filterResponse);
            Assert.NotNull(filterResponse.DataObject);
            Assert.True(filterResponse.DataObject.Companies.Count >= 1, "Did not find at least one company with the matching filter value");

            var originalCompany = filterResponse.DataObject.Companies.FirstOrDefault(c => c.Id == companyToQuery.Id);
            Assert.NotNull(originalCompany);

        }

        [Fact]
        public void ShouldRetrieveCompanyByLastModifiedFilter()
        {
            var proxy = new CompaniesProxy();
            var listResponse = proxy.GetCompanies(null, null, null, null, null);
            Assert.NotNull(listResponse);
            Assert.NotNull(listResponse.DataObject);
            Assert.True(listResponse.DataObject.Companies.Count > 0);

            // Get a subset of a companies name to use as the filter value
            var companyToQuery = listResponse.DataObject.Companies.First();

            var filterResponse = proxy.GetCompanies(null, companyToQuery.LastModifiedDateUtc, companyToQuery.LastModifiedDateUtc, null, null);
            Assert.NotNull(filterResponse);
            Assert.NotNull(filterResponse.DataObject);
            Assert.True(filterResponse.DataObject.Companies.Count >= 1, "Did not find at least one company with the matching filter value");

            var originalCompany = filterResponse.DataObject.Companies.FirstOrDefault(c => c.Id == companyToQuery.Id);
            Assert.NotNull(originalCompany);

        }

        [Fact]
        public void ShouldBeAbleToPageCompanies()
        {
            var proxy = new CompaniesProxy();
            var listResponse = proxy.GetCompanies(null, null, null, null, null);
            Assert.NotNull(listResponse);
            Assert.NotNull(listResponse.DataObject);
            Assert.True(listResponse.DataObject.Companies.Count >= 5); // In setup we add at least 5 companies

            listResponse = proxy.GetCompanies(null, null, null, 1, 3);
            Assert.True(listResponse.DataObject.Companies.Count == 3);

            listResponse = proxy.GetCompanies(null, null, null, 2, 3);
            Assert.True(listResponse.DataObject.Companies.Count >= 2);
        }

        [Fact]
        public void ShouldInsertCompany()
        {
            var proxy = new CompanyProxy();
            
            var company = GetTestCompany();
            var insertResponse = proxy.InsertCompany(company);
            
            Assert.True(insertResponse.IsSuccessfull);
            Assert.Equal(insertResponse.StatusCode, HttpStatusCode.OK);
            Assert.True(insertResponse.DataObject.InsertedCompanyId > 0);

            var insertedCompany = proxy.GetCompany(insertResponse.DataObject.InsertedCompanyId).DataObject;

            Assert.True(!string.IsNullOrEmpty(insertedCompany.Abn), "Company ABN not saved");
            Assert.Equal(insertedCompany.Abn, company.Abn);
            Assert.True(!string.IsNullOrEmpty(insertedCompany.CompanyEmail), "Company email not saved");
            Assert.Equal(insertedCompany.CompanyEmail, company.CompanyEmail);
            Assert.True(!string.IsNullOrEmpty(insertedCompany.LongDescription), "Company description not saved");
            Assert.Equal(insertedCompany.LongDescription, company.LongDescription);
            Assert.True(!string.IsNullOrEmpty(insertedCompany.TradingName), "Company trading name not saved");
            Assert.Equal(insertedCompany.TradingName, company.TradingName);
            Assert.True(!string.IsNullOrEmpty(insertedCompany.Website), "Company website not saved");
            Assert.Equal(insertedCompany.Website, company.Website);
            Assert.True(!string.IsNullOrEmpty(insertedCompany.LastUpdatedId), "Company LastUpdatedId not returned");
            Assert.NotNull(insertedCompany.CreatedDateUtc);
            Assert.NotNull(insertedCompany.LastModifiedDateUtc);
            Assert.NotNull(insertedCompany.LastModifiedByUserId);
        }

        [Fact]
        public void ShouldUpdateCompany()
        {
            var proxy = new CompanyProxy();
            
            var company = GetTestCompany();
            var insertResponse = proxy.InsertCompany(company);

            Assert.True(insertResponse.IsSuccessfull);
            Assert.Equal(insertResponse.StatusCode, HttpStatusCode.OK);
            Assert.True(insertResponse.DataObject.InsertedCompanyId > 0);

            var insertedCompany = proxy.GetCompany(insertResponse.DataObject.InsertedCompanyId).DataObject;
            
            insertedCompany.LongDescription = "Modified company";
            insertedCompany.Abn = "123456789";
            insertedCompany.CompanyEmail = "updated@bigboss@somecompany.com";
            insertedCompany.TradingName = "Updated Trading Name";
            insertedCompany.Name = "Updated Name";
            insertedCompany.Website = "http://updated.com";

            var updateResponse = proxy.UpdateCompany(insertedCompany, insertResponse.DataObject.InsertedCompanyId);
            Assert.True(updateResponse.IsSuccessfull);
            Assert.Equal(updateResponse.DataObject.UpdatedCompanyId, insertResponse.DataObject.InsertedCompanyId);

            var updatedCompany = proxy.GetCompany(updateResponse.DataObject.UpdatedCompanyId).DataObject;

            Assert.Equal(insertedCompany.Abn, updatedCompany.Abn);
            Assert.Equal(insertedCompany.CompanyEmail, updatedCompany.CompanyEmail);
            Assert.Equal(insertedCompany.LongDescription, updatedCompany.LongDescription);
            Assert.Equal(insertedCompany.TradingName, updatedCompany.TradingName);
            Assert.Equal(insertedCompany.Website, updatedCompany.Website);
            Assert.True(!string.IsNullOrEmpty(updatedCompany.LastUpdatedId), "Company LastUpdatedId not returned");
            Assert.NotNull(updatedCompany.CreatedDateUtc);
            Assert.NotNull(updatedCompany.LastModifiedDateUtc);
            Assert.NotNull(updatedCompany.LastModifiedByUserId);

        }

        [Fact]
        public void ShouldDeleteCompany()
        {
            var proxy = new CompanyProxy();

            var company = GetTestCompany();
            var insertResponse = proxy.InsertCompany(company);

            Assert.True(insertResponse.IsSuccessfull);
            Assert.True(insertResponse.DataObject.InsertedCompanyId > 0);

            var deleteResponse = proxy.DeleteCompany(insertResponse.DataObject.InsertedCompanyId);

            Assert.True(deleteResponse.IsSuccessfull);

            var deletedCompany = proxy.GetCompany(insertResponse.DataObject.InsertedCompanyId);
            Assert.Null(deletedCompany.DataObject);
            Assert.Equal(HttpStatusCode.NotFound, deletedCompany.StatusCode);


        }

        public  CompanyTests()
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
