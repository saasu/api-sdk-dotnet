using Xunit;
using Saasu.API.Client.Proxies;
using System.Linq;

namespace Saasu.API.Client.IntegrationTests
{
    public class FileIdentityTests
    {
        [Fact]
        public void ShouldReturnFileIdentityDetails()
        {
            var fileIdentityProxy = new FileIdentityProxy();
            var testFileId = TestConfig.TestFileId;
            if (testFileId == 0)
                return;

            var fileIdentityGetResult = fileIdentityProxy.GetFileIdentity(TestConfig.TestFileId);
            Assert.True(fileIdentityGetResult.IsSuccessfull, "File Identity GET request failed.");
            Assert.NotNull(fileIdentityGetResult.DataObject);
            Assert.NotNull(fileIdentityGetResult.DataObject.Name);
        }

        [Fact]
        public void ShouldReturnListOfFileIdentities()
        {
            var fileIdentitiesProxy = new FileIdentitiesProxy();

            var pageNumber = 1;
            var pageSize = 10;
            var fileIdentities = fileIdentitiesProxy.GetFileIdentities(pageNumber, pageSize);
            Assert.True(fileIdentities.IsSuccessfull, "File Identities GET request failed");
            Assert.NotNull(fileIdentities.DataObject);
            Assert.True(fileIdentities.DataObject.FileIdentities.Count > 0, "At least one File Identity should have been retrieved");
            Assert.NotNull(fileIdentities.DataObject.FileIdentities.First().Name);
            Assert.NotNull(fileIdentities.DataObject.FileIdentities.First().CurrencyCode);

        }
    }
}
