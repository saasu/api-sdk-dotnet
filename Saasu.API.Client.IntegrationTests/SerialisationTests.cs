using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saasu.API.Client.Framework;
using NUnit.Framework;
using Saasu.API.Client.Proxies;

namespace Saasu.API.Client.IntegrationTests
{
    //[TestClass]
    [TestFixture]
    public class SerialisationTests
    {
        [Test]
        public void ShouldBeAbleToReturnApiResultsAsJson()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new ContactsProxy(accessToken);
            proxy.ContentType = RequestContentType.ApplicationJson;
            ContactTests.AssertContactProxy(proxy);

        }

        [Test]
        public void ShouldBeAbleToReturnApiResultsAsXml()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new ContactsProxy(accessToken);
            proxy.ContentType = RequestContentType.ApplicationXml;
            ContactTests.AssertContactProxy(proxy);

        }
    }
}
