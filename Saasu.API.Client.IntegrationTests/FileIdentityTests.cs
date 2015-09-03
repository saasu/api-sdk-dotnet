using NUnit.Framework;
using Saasu.API.Client.Proxies;
using System.Linq;

namespace Saasu.API.Client.IntegrationTests
{
    [TestFixture]
    public class FileIdentityTests
    {
        [Test]
        public void ShouldReturnFileIdentityDetails()
        {
            var fileIdentityProxy = new FileIdentityProxy();
            var testFileId = TestConfig.TestFileId;
            if (testFileId == 0)
                return;

            var fileIdentityGetResult = fileIdentityProxy.GetFileIdentity(TestConfig.TestFileId);
            Assert.IsTrue(fileIdentityGetResult.IsSuccessfull, "File Identity GET request failed.");
            Assert.IsNotNull(fileIdentityGetResult.DataObject, "File Identity details were not retrieved successfully.");
            Assert.IsNotNull(fileIdentityGetResult.DataObject.Name, "At least File Identity Name should have been retrieved successfully.");
        }

        [Test]
        public void ShouldReturnListOfFileIdentities()
        {
            var fileIdentitiesProxy = new FileIdentitiesProxy();

            var pageNumber = 1;
            var pageSize = 10;
            var fileIdentities = fileIdentitiesProxy.GetFileIdentities(pageNumber, pageSize);
            Assert.IsTrue(fileIdentities.IsSuccessfull, "File Identities GET request failed");
            Assert.IsNotNull(fileIdentities.DataObject, "File Identities were not retrieved successfully.");
            Assert.IsTrue(fileIdentities.DataObject.FileIdentities.Count > 0, "At least one File Identity should have been retrieved");
            Assert.IsNotNull(fileIdentities.DataObject.FileIdentities.First().Name, "File Identity should have at least a Name");
            Assert.IsNotNull(fileIdentities.DataObject.FileIdentities.First().CurrencyCode, "File Identity should have a Currency Code");

        }
    }
}
