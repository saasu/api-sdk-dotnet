//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Saasu.API.Client.IntegrationTests
{
    public static class TestHelper
    {
        public static string SignInAndGetAccessToken()
        {
            var authProxy = new AuthorisationProxy();
            var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
            var authResponse = authProxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword, scope);

            Assert.True(authResponse.IsSuccessfull);
            Assert.True(authResponse.DataObject.IsSuccessfull);

            return authResponse.DataObject.AccessGrant.access_token;
        }

        /// <summary>
        /// This is used to compare dates where a few seconds needs to added to past and future to account for slight differences in value.
        /// </summary>
        public static bool AssertDatetimesEqualWithVariance(DateTime dateToVary, DateTime dateToNotTouch)
        {
            var variance = System.Configuration.ConfigurationManager.AppSettings["TestingDateTimeVariance"];

            Int16 varianceInt = 0;

            Int16.TryParse(variance, out varianceInt);

            if (varianceInt == 0)
            {
                return dateToVary == dateToNotTouch;
            }

            var minDate = dateToVary.AddSeconds(-varianceInt);
            var maxDate = dateToVary.AddSeconds(varianceInt);

            return dateToNotTouch > minDate && dateToNotTouch < maxDate;
        }
    }
}
