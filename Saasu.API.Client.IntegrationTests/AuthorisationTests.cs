using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saasu.API.Client.Proxies;
using System.Net;
using Saasu.API.Core.Globals;
using NUnit.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Client.Framework;

namespace Saasu.API.Client.IntegrationTests
{
	//[TestClass]
    [TestFixture]
	public class AuthorisationTests
	{
		[Test]
		public void PasswordCredentialsGrantShouldSucceed()
		{
			var proxy = new AuthorisationProxy();
			var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType= AuthorisationScopeType.Full} }.ToTextValues();
			var response = proxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword,scope);
			Assert.IsTrue(response.IsSuccessfull);
			Assert.IsNotNull(response.DataObject);
			Assert.IsTrue(response.DataObject.IsSuccessfull);
			Assert.IsNotNull(response.DataObject.AccessGrant);
			Assert.IsNotNull(response.DataObject.AccessGrant.access_token);
			Assert.IsNotNull(response.DataObject.AccessGrant.refresh_token);
			Assert.IsNotNull(response.DataObject.AccessGrant.token_type);
			Assert.IsNotNull(response.DataObject.AccessGrant.scope);
			Assert.IsTrue(response.DataObject.AccessGrant.scope.Contains(AuthorisationScopeValue.FileId));

			var returnedScope = response.DataObject.AccessGrant.scope.ToScopeArray();
			Assert.IsNotNull(returnedScope);
			Assert.IsTrue(returnedScope.Length > 0);
			Assert.IsTrue(returnedScope.Count(s => s.ScopeType == AuthorisationScopeType.FileId) > 0, "Access response should contain at least 1 valid FileId applicable to user in the scope");

		}


		[Test]
		[Ignore]  // this functionality is not implemented yet on the API 
		public void AuthorisationCodeGrantTests()
		{
			var proxy = new AuthorisationProxy();
			var response = proxy.AuthorisationCodeGrantRequest("12345", "test_scope", "test_state");
			Assert.IsTrue(response.IsSuccessfull);

		}

		[Test]
		public void ValidBearerTokenShouldPingSuccessfully()
		{
			var proxy = new AuthorisationProxy();
			var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
			var response = proxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword,scope);
			Assert.IsTrue(response.IsSuccessfull);
			Assert.IsTrue(response.DataObject.IsSuccessfull);
			proxy.BearerToken = response.DataObject.AccessGrant.access_token;
			var pingResult = proxy.AuthorisationPing();
			Assert.IsNotNull(pingResult);
			Assert.IsTrue(pingResult.IsSuccessfull);
		}
		
		[Test]
		public void InValidBearerTokenShouldNotPingSuccessfully()
		{
			var proxy = new AuthorisationProxy("bogustoken");
			var pingResult = proxy.AuthorisationPing();
			Assert.IsNotNull(pingResult);
			Assert.IsFalse(pingResult.IsSuccessfull);
			//Assert.AreEqual<HttpStatusCode>(HttpStatusCode.Unauthorized, pingResult.StatusCode);
            Assert.AreEqual(HttpStatusCode.Unauthorized, pingResult.StatusCode);
		}

        [Test]
        public void OldAccessTokenTokenShouldNotPingSuccessfully()
        {
            // Authenticate and get an access token
            var proxy1 = new AuthorisationProxy();
            var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
            var response = proxy1.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword, scope);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsTrue(response.DataObject.IsSuccessfull);
            var originalAccessToken = response.DataObject.AccessGrant.access_token;
            
            // Refresh the access token so the old one becomes invalid
            response = proxy1.RefreshAccessToken(response.DataObject.AccessGrant.refresh_token, scope);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsNotNull(response.DataObject);
            Assert.IsTrue(response.DataObject.IsSuccessfull);

            // Now try and authenicate and access a resource with the invalidated original access token which should fail.
            var proxy2 = new AuthorisationProxy(originalAccessToken);
            var pingResult = proxy2.AuthorisationPing();
            Assert.IsNotNull(pingResult);
            Assert.IsFalse(pingResult.IsSuccessfull);
            //Assert.AreEqual<HttpStatusCode>(HttpStatusCode.Unauthorized, pingResult.StatusCode);
            Assert.AreEqual(HttpStatusCode.Unauthorized, pingResult.StatusCode);
        }
        
        [Test]
		public void ShouldBeAbleToRefreshValidAccessToken()
		{
			var proxy = new AuthorisationProxy();
			var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
			var response = proxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword,scope);
			Assert.IsTrue(response.IsSuccessfull);
			Assert.IsTrue(response.DataObject.IsSuccessfull);
			proxy.BearerToken = response.DataObject.AccessGrant.access_token;
			var pingResult = proxy.AuthorisationPing();
			Assert.IsNotNull(pingResult);
			Assert.IsTrue(pingResult.IsSuccessfull);

			// Now ask for a refresh token
			var proxy2 = new AuthorisationProxy();
			var refreshResponse = proxy2.RefreshAccessToken(response.DataObject.AccessGrant.refresh_token, scope);
			Assert.IsNotNull(refreshResponse);
			Assert.IsTrue(refreshResponse.IsSuccessfull);
            //Assert.AreNotEqual<string>(response.DataObject.AccessGrant.access_token, refreshResponse.DataObject.AccessGrant.access_token);
            //Assert.AreEqual<string>(response.DataObject.AccessGrant.refresh_token, refreshResponse.DataObject.AccessGrant.refresh_token);
            Assert.AreNotEqual(response.DataObject.AccessGrant.access_token, refreshResponse.DataObject.AccessGrant.access_token);
            Assert.AreEqual(response.DataObject.AccessGrant.refresh_token, refreshResponse.DataObject.AccessGrant.refresh_token);

			// Now check the access token after refresh works
			var proxy3 = new AuthorisationProxy(refreshResponse.DataObject.AccessGrant.access_token);
			var pingResult2 = proxy3.AuthorisationPing();
			Assert.IsNotNull(pingResult2);
			Assert.IsTrue(pingResult2.IsSuccessfull);

			// And finally ensure the previous access token (before refresh is invalid)

			var proxy4 = new AuthorisationProxy(response.DataObject.AccessGrant.access_token);
			var pingResult3 = proxy4.AuthorisationPing();
			Assert.IsNotNull(pingResult3);
			Assert.IsFalse(pingResult3.IsSuccessfull);
			//Assert.AreEqual<HttpStatusCode>(HttpStatusCode.Unauthorized, pingResult3.StatusCode);
            Assert.AreEqual(HttpStatusCode.Unauthorized, pingResult3.StatusCode);
		}

        [Test]
        public void ValidBearerTokenButNoFileIdShouldFailWhenMakingApiCall()
        {
            var proxy = new AuthorisationProxy();
            var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
            var response = proxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword, scope);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsTrue(response.DataObject.IsSuccessfull);
            proxy.BearerToken = response.DataObject.AccessGrant.access_token;

            var contactProxy = new ContactsProxy(response.DataObject.AccessGrant.access_token);
            contactProxy.FileId = 0; // invalid file id so it should fail.
            var contactResponse = contactProxy.GetContacts();

            Assert.IsNotNull(contactResponse, "No response returned wwhen access contacts resource");
            Assert.IsFalse(contactResponse.IsSuccessfull, "Expected GET Contacts to fail as used an invalid File Id");
            Assert.AreEqual(contactResponse.StatusCode, HttpStatusCode.Unauthorized, "Expected Unauthorized status code as used an invalid FileId");
        }

        [Test]
        public void ShouldNotBeAbleToUseRefreshTokenToAccessApi()
        {
            var proxy = new AuthorisationProxy();
            var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
            var response = proxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword, scope);
            Assert.IsTrue(response.IsSuccessfull);
            Assert.IsTrue(response.DataObject.IsSuccessfull);

            var contactProxy = new ContactsProxy(response.DataObject.AccessGrant.refresh_token);
            var contactResponse = contactProxy.GetContacts();
            Assert.IsNotNull(contactResponse, "No response returned wwhen access contacts resource");
            Assert.IsFalse(contactResponse.IsSuccessfull, "Expected GET Contacts to fail used refreshToken instead of access token to access API");
            Assert.AreEqual(contactResponse.StatusCode, HttpStatusCode.Unauthorized, "Expected Unauthorised Status code as used an invalid access token");

            // And now just reverify that we can access via the access token
            contactProxy.BearerToken = response.DataObject.AccessGrant.access_token;
            contactResponse = contactProxy.GetContacts();
            Assert.IsNotNull(contactResponse, "No response returned wwhen access contacts resource");
            Assert.IsTrue(contactResponse.IsSuccessfull, "Expected GET Contacts to succeed since we used valid accessToken to access API");
        }

	}
}
