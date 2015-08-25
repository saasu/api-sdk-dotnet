//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Saasu.API.Client.IntegrationTests
{
    public static class TestHelper
    {
        public static string SignInAndGetAccessToken()
        {
            var authProxy = new AuthorisationProxy();
            var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
            var authResponse = authProxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword, scope);

            Assert.IsTrue(authResponse.IsSuccessfull);
            Assert.IsTrue(authResponse.DataObject.IsSuccessfull);

            return authResponse.DataObject.AccessGrant.access_token;
        }
    }
}
