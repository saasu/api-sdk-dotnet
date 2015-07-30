using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Saasu.API.Client.Proxies;

namespace Saasu.API.Client.IntegrationTests
{
    [TestFixture]
    public class TaxCodeTests
    {
        [Test]
        public void ShouldRetrieveTaxCodeListForFile()
        {
            var taxCodeProxy = new TaxCodesProxy();
            var taxCodesForFileResponse = taxCodeProxy.GetTaxCodes(null, null, null);
            Assert.IsNotNull(taxCodesForFileResponse, "No response returned for tax codes request.");
            Assert.IsTrue(taxCodesForFileResponse.IsSuccessfull, "Request for tax codes was not processed successfully.");            
            Assert.IsTrue(taxCodesForFileResponse.DataObject.TaxCodes.Count > 0, "No tax codes were retrieved for file.");
        }

        [Test]
        public void ShouldOnlyReturnActiveTaxCodesForFile()
        {
            var taxCodeProxy = new TaxCodesProxy();
            var taxCodesForFileResponse = taxCodeProxy.GetTaxCodes(true, null, null);
            Assert.IsNotNull(taxCodesForFileResponse, "No response returned for tax codes request.");
            Assert.IsTrue(taxCodesForFileResponse.IsSuccessfull, "Request for tax codes was not processed successfully.");
            Assert.IsTrue(taxCodesForFileResponse.DataObject.TaxCodes.Count > 0, "No tax codes were retrieved for file.");
            Assert.IsTrue(taxCodesForFileResponse.DataObject.TaxCodes.Find(tc => !tc.IsActive) == null, "Only active tax codes should have been returned.");
        }

        [Test]
        public void ShouldOnlyReturnInactiveTaxCodesForFile()
        {
            var taxCodeProxy = new TaxCodesProxy();
            var taxCodesForFileResponse = taxCodeProxy.GetTaxCodes(false, null, null);
            Assert.IsNotNull(taxCodesForFileResponse, "No response returned for tax codes request.");
            Assert.IsTrue(taxCodesForFileResponse.IsSuccessfull, "Request for tax codes was not processed successfully.");
            Assert.IsTrue(taxCodesForFileResponse.DataObject.TaxCodes.Count > 0, "No tax codes were retrieved for file.");
            Assert.IsTrue(taxCodesForFileResponse.DataObject.TaxCodes.Find(tc => tc.IsActive) == null, "Only inactive tax codes should have been returned.");
        }

        [Test]
        public void ShouldRetrieveTaxCodeById()
        {
            var taxCodesProxy = new TaxCodesProxy();
            var taxCodesForFileResponse = taxCodesProxy.GetTaxCodes(null, null, null);
            Assert.IsTrue(taxCodesForFileResponse.DataObject.TaxCodes.Count > 0, "No tax codes were retrieved for file.");

            var taxCodeToRetrieveById = taxCodesForFileResponse.DataObject.TaxCodes[0];
            var taxCodeProxy = new TaxCodeProxy();
            var taxCodeByIdResponse = taxCodeProxy.GetTaxCode(taxCodeToRetrieveById.Id);
            Assert.IsNotNull(taxCodeByIdResponse, "No response returned for tax code by Id request.");
            Assert.IsTrue(taxCodeByIdResponse.IsSuccessfull, "Request for tax code by Id was not processed successfully.");

            var taxCodeById = taxCodeByIdResponse.DataObject; 
            Assert.IsNotNull(taxCodeById);
            Assert.AreEqual(taxCodeToRetrieveById.Id, taxCodeById.Id, "Incorrect Tax Code Id.");
            Assert.AreEqual(taxCodeToRetrieveById.Code, taxCodeById.Code, "Incorrect Tax Code.");
            Assert.AreEqual(taxCodeToRetrieveById.Name, taxCodeById.Name, "Incorrect Tax Code Name.");
            Assert.AreEqual(taxCodeToRetrieveById.PostingAccountId.GetValueOrDefault(), taxCodeById.PostingAccountId.GetValueOrDefault(), "Incorrect Tax Code Id.");
            Assert.AreEqual(taxCodeToRetrieveById.IsSale, taxCodeById.IsSale, "Incorrect Is Sale status.");
            Assert.AreEqual(taxCodeToRetrieveById.IsPurchase, taxCodeById.IsPurchase, "Incorrect Is Purchase status.");
            Assert.AreEqual(taxCodeToRetrieveById.IsPayroll, taxCodeById.IsPayroll, "Incorrect Is Payroll status.");
            Assert.AreEqual(taxCodeToRetrieveById.IsActive, taxCodeById.IsActive, "Incorrect Is Active status.");
            Assert.AreEqual(taxCodeToRetrieveById.Rate, taxCodeById.Rate, "Incorrect Tax Code Rate.");
            Assert.AreEqual(taxCodeToRetrieveById.IsShared, taxCodeById.IsShared, "Incorrect Is Shared status.");            
        }
    }
}
