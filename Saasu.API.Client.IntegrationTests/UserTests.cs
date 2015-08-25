using System.Net;
using NUnit.Framework;
using Saasu.API.Client.Proxies;

namespace Saasu.API.Client.IntegrationTests
{
    [TestFixture]
    public class UserTests
    {
        [Test]
        public void ResetPasswordRequestForEmptyUserStringShouldFail()
        {
            var proxy = new UserProxy();
            var response = proxy.ResetPassword("");
            Assert.IsFalse(response.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void ResetPasswordRequestForExistingUserShouldSucceed()
        {
            var proxy = new UserProxy();
            var response = proxy.ResetPassword(TestConfig.TestUser);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.AreEqual("A password reset email has been sent.", response.DataObject.StatusMessage);
        }

        [Test]
        public void ResetPasswordRequestForNonExistingUserShouldFail()
        {
            var proxy = new UserProxy();
            var response = proxy.ResetPassword("nobody@existmail.com");
            Assert.IsFalse(response.IsSuccessfull);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}