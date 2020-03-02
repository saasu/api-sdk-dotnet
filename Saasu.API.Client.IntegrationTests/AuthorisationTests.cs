using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saasu.API.Client.Proxies;
using System.Net;
using Saasu.API.Core.Globals;
using Xunit;
using Saasu.API.Core.Framework;
using Saasu.API.Client.Framework;

namespace Saasu.API.Client.IntegrationTests
{
	public class AuthorisationTests
	{
		[Fact]
		public void PasswordCredentialsGrantShouldSucceed()
		{
			var proxy = new AuthorisationProxy();
			var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType= AuthorisationScopeType.Full} }.ToTextValues();
			var response = proxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword,scope);
			Assert.True(response.IsSuccessfull);
			Assert.NotNull(response.DataObject);
			Assert.True(response.DataObject.IsSuccessfull);
			Assert.NotNull(response.DataObject.AccessGrant);
			Assert.NotNull(response.DataObject.AccessGrant.access_token);
			Assert.NotNull(response.DataObject.AccessGrant.refresh_token);
			Assert.NotNull(response.DataObject.AccessGrant.token_type);
			Assert.NotNull(response.DataObject.AccessGrant.scope);
			Assert.True(response.DataObject.AccessGrant.scope.Contains(AuthorisationScopeValue.FileId));

			var returnedScope = response.DataObject.AccessGrant.scope.ToScopeArray();
			Assert.NotNull(returnedScope);
			Assert.True(returnedScope.Length > 0);
			Assert.True(returnedScope.Count(s => s.ScopeType == AuthorisationScopeType.FileId) > 0, "Access response should contain at least 1 valid FileId applicable to user in the scope");

		}


		[Fact(Skip = "This functionality is not implemented yet on the API.")]
        public void AuthorisationCodeGrantTests()
		{
			var proxy = new AuthorisationProxy();
			var response = proxy.AuthorisationCodeGrantRequest("12345", "test_scope", "test_state");
			Assert.True(response.IsSuccessfull);

		}

		[Fact]
		public void ValidBearerTokenShouldPingSuccessfully()
		{
			var proxy = new AuthorisationProxy();
			var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
			var response = proxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword,scope);
			Assert.True(response.IsSuccessfull);
			Assert.True(response.DataObject.IsSuccessfull);
			proxy.BearerToken = response.DataObject.AccessGrant.access_token;
			var pingResult = proxy.AuthorisationPing();
			Assert.NotNull(pingResult);
			Assert.True(pingResult.IsSuccessfull);
		}
		
		[Fact]
		public void InValidBearerTokenShouldNotPingSuccessfully()
		{
			var proxy = new AuthorisationProxy("bogustoken");
			var pingResult = proxy.AuthorisationPing();
			Assert.NotNull(pingResult);
			Assert.False(pingResult.IsSuccessfull);
			//Assert.AreEqual<HttpStatusCode>(HttpStatusCode.Unauthorized, pingResult.StatusCode);
            Assert.Equal(pingResult.StatusCode, HttpStatusCode.Unauthorized);
		}

        [Fact]
        public void OldAccessTokenTokenShouldNotPingSuccessfully()
        {
            // Authenticate and get an access token
            var proxy1 = new AuthorisationProxy();
            var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
            var response = proxy1.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword, scope);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.True(response.DataObject.IsSuccessfull);
            var originalAccessToken = response.DataObject.AccessGrant.access_token;
            
            // Refresh the access token so the old one becomes invalid
            response = proxy1.RefreshAccessToken(response.DataObject.AccessGrant.refresh_token, scope);
            Assert.True(response.IsSuccessfull);
            Assert.NotNull(response.DataObject);
            Assert.True(response.DataObject.IsSuccessfull);

            // Now try and authenicate and access a resource with the invalidated original access token which should fail.
            var proxy2 = new AuthorisationProxy(originalAccessToken);
            var pingResult = proxy2.AuthorisationPing();
            Assert.NotNull(pingResult);
            Assert.False(pingResult.IsSuccessfull);
            //Assert.AreEqual<HttpStatusCode>(HttpStatusCode.Unauthorized, pingResult.StatusCode);
            Assert.Equal(pingResult.StatusCode, HttpStatusCode.Unauthorized);
        }
        
        [Fact]
		public void ShouldBeAbleToRefreshValidAccessToken()
		{
			var proxy = new AuthorisationProxy();
			var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
			var response = proxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword,scope);
			Assert.True(response.IsSuccessfull);
			Assert.True(response.DataObject.IsSuccessfull);
			proxy.BearerToken = response.DataObject.AccessGrant.access_token;
			var pingResult = proxy.AuthorisationPing();
			Assert.NotNull(pingResult);
			Assert.True(pingResult.IsSuccessfull);

			// Now ask for a refresh token
			var proxy2 = new AuthorisationProxy();
			var refreshResponse = proxy2.RefreshAccessToken(response.DataObject.AccessGrant.refresh_token, scope);
			Assert.NotNull(refreshResponse);
			Assert.True(refreshResponse.IsSuccessfull);
            //Assert.AreNotEqual<string>(response.DataObject.AccessGrant.access_token, refreshResponse.DataObject.AccessGrant.access_token);
            //Assert.AreEqual<string>(response.DataObject.AccessGrant.refresh_token, refreshResponse.DataObject.AccessGrant.refresh_token);
            Assert.NotEqual(response.DataObject.AccessGrant.access_token, refreshResponse.DataObject.AccessGrant.access_token);
            Assert.Equal(refreshResponse.DataObject.AccessGrant.refresh_token, response.DataObject.AccessGrant.refresh_token);

			// Now check the access token after refresh works
			var proxy3 = new AuthorisationProxy(refreshResponse.DataObject.AccessGrant.access_token);
			var pingResult2 = proxy3.AuthorisationPing();
			Assert.NotNull(pingResult2);
			Assert.True(pingResult2.IsSuccessfull);

			// And finally ensure the previous access token (before refresh is invalid)

			var proxy4 = new AuthorisationProxy(response.DataObject.AccessGrant.access_token);
			var pingResult3 = proxy4.AuthorisationPing();
			Assert.NotNull(pingResult3);
			Assert.False(pingResult3.IsSuccessfull);
			//Assert.AreEqual<HttpStatusCode>(HttpStatusCode.Unauthorized, pingResult3.StatusCode);
            Assert.Equal(pingResult3.StatusCode, HttpStatusCode.Unauthorized);
		}

        [Fact]
        public void ValidBearerTokenButNoFileIdShouldFailWhenMakingApiCall()
        {
            var proxy = new AuthorisationProxy();
            var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
            var response = proxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword, scope);
            Assert.True(response.IsSuccessfull);
            Assert.True(response.DataObject.IsSuccessfull);
            proxy.BearerToken = response.DataObject.AccessGrant.access_token;

            var contactProxy = new ContactsProxy(response.DataObject.AccessGrant.access_token);
            contactProxy.FileId = 0; // invalid file id so it should fail.
            var contactResponse = contactProxy.GetContacts();

            Assert.NotNull(contactResponse);
            Assert.False(contactResponse.IsSuccessfull, "Expected GET Contacts to fail as used an invalid File Id");
            Assert.Equal(HttpStatusCode.Unauthorized, contactResponse.StatusCode);
        }

        [Fact]
        public void ShouldNotBeAbleToUseRefreshTokenToAccessApi()
        {
            var proxy = new AuthorisationProxy();
            var scope = new AuthorisationScope[] { new AuthorisationScope { ScopeType = AuthorisationScopeType.Full } }.ToTextValues();
            var response = proxy.PasswordCredentialsGrantRequest(TestConfig.TestUser, TestConfig.TestUserPassword, scope);
            Assert.True(response.IsSuccessfull);
            Assert.True(response.DataObject.IsSuccessfull);

            var contactProxy = new ContactsProxy(response.DataObject.AccessGrant.refresh_token);
            var contactResponse = contactProxy.GetContacts();
            Assert.NotNull(contactResponse);
            Assert.False(contactResponse.IsSuccessfull, "Expected GET Contacts to fail used refreshToken instead of access token to access API");
            Assert.Equal(HttpStatusCode.Unauthorized, contactResponse.StatusCode);

            // And now just reverify that we can access via the access token
            contactProxy.BearerToken = response.DataObject.AccessGrant.access_token;
            contactResponse = contactProxy.GetContacts();
            Assert.NotNull(contactResponse);
            Assert.True(contactResponse.IsSuccessfull, "Expected GET Contacts to succeed since we used valid accessToken to access API");
        }

	}
}
