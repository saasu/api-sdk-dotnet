using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Saasu.API.Client.Proxies;

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
    }
}
