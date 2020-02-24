using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Saasu.API.Client.Proxies;

namespace Saasu.API.Client.IntegrationTests
{
    public class TaxCodeTests
    {
        [Fact]
        public void ShouldRetrieveTaxCodeListForFile()
        {
            var taxCodeProxy = new TaxCodesProxy();
            var taxCodesForFileResponse = taxCodeProxy.GetTaxCodes(null, null, null);
            Assert.NotNull(taxCodesForFileResponse);
            Assert.True(taxCodesForFileResponse.IsSuccessfull, "Request for tax codes was not processed successfully.");            
            Assert.True(taxCodesForFileResponse.DataObject.TaxCodes.Count > 0, "No tax codes were retrieved for file.");
        }

        [Fact]
        public void ShouldOnlyReturnActiveTaxCodesForFile()
        {
            var taxCodeProxy = new TaxCodesProxy();
            var taxCodesForFileResponse = taxCodeProxy.GetTaxCodes(true, null, null);
            Assert.NotNull(taxCodesForFileResponse);
            Assert.True(taxCodesForFileResponse.IsSuccessfull, "Request for tax codes was not processed successfully.");
            Assert.True(taxCodesForFileResponse.DataObject.TaxCodes.Count > 0, "No tax codes were retrieved for file.");
            Assert.True(taxCodesForFileResponse.DataObject.TaxCodes.Find(tc => !tc.IsActive) == null, "Only active tax codes should have been returned.");
        }

        [Fact]
        public void ShouldOnlyReturnInactiveTaxCodesForFile()
        {
            var taxCodeProxy = new TaxCodesProxy();
            var taxCodesForFileResponse = taxCodeProxy.GetTaxCodes(false, null, null);
            Assert.NotNull(taxCodesForFileResponse);
            Assert.True(taxCodesForFileResponse.IsSuccessfull, "Request for tax codes was not processed successfully.");
            Assert.True(taxCodesForFileResponse.DataObject.TaxCodes.Count > 0, "No tax codes were retrieved for file.");
            Assert.True(taxCodesForFileResponse.DataObject.TaxCodes.Find(tc => tc.IsActive) == null, "Only inactive tax codes should have been returned.");
        }

        [Fact]
        public void ShouldRetrieveTaxCodeById()
        {
            var taxCodesProxy = new TaxCodesProxy();
            var taxCodesForFileResponse = taxCodesProxy.GetTaxCodes(null, null, null);
            Assert.True(taxCodesForFileResponse.DataObject.TaxCodes.Count > 0, "No tax codes were retrieved for file.");

            var taxCodeToRetrieveById = taxCodesForFileResponse.DataObject.TaxCodes[0];
            var taxCodeProxy = new TaxCodeProxy();
            var taxCodeByIdResponse = taxCodeProxy.GetTaxCode(taxCodeToRetrieveById.Id);
            Assert.NotNull(taxCodeByIdResponse);
            Assert.True(taxCodeByIdResponse.IsSuccessfull, "Request for tax code by Id was not processed successfully.");

            var taxCodeById = taxCodeByIdResponse.DataObject; 
            Assert.NotNull(taxCodeById);
            Assert.Equal(taxCodeToRetrieveById.Id, taxCodeById.Id);
            Assert.Equal(taxCodeToRetrieveById.Code, taxCodeById.Code);
            Assert.Equal(taxCodeToRetrieveById.Name, taxCodeById.Name);
            Assert.Equal(taxCodeToRetrieveById.PostingAccountId.GetValueOrDefault(), taxCodeById.PostingAccountId.GetValueOrDefault());
            Assert.Equal(taxCodeToRetrieveById.IsSale, taxCodeById.IsSale);
            Assert.Equal(taxCodeToRetrieveById.IsPurchase, taxCodeById.IsPurchase);
            Assert.Equal(taxCodeToRetrieveById.IsPayroll, taxCodeById.IsPayroll);
            Assert.Equal(taxCodeToRetrieveById.IsActive, taxCodeById.IsActive);
            Assert.Equal(taxCodeToRetrieveById.Rate, taxCodeById.Rate);
            Assert.Equal(taxCodeToRetrieveById.IsShared, taxCodeById.IsShared);            
        }
    }
}
