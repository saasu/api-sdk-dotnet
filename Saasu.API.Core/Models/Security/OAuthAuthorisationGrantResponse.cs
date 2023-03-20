using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saasu.API.Core.Models.Security
{
    /// <summary>
    /// OAuth password credentials grant request.
    /// </summary>
    public class OAuthPasswordCredentialsGrantRequest
    {
        /// <summary>
        /// The OAuth grant type. For password credential grant, this must be 'password'.
        /// </summary>
        public string grant_type { get; set; }
        /// <summary>
        /// The username. For example, someone@emailhost.com
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// The password of the user.
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// The scope requested. For example, 'full'.
        /// </summary>
        public string scope { get; set; }
        /// <summary>
        /// The 2FA code / one-time password (OTP) that needs to be supplied if 2FA is enabled for user account.
        /// This code is sent to user's registered mobile number when you initiate login.
        /// </summary>
        public string verification_code { get; set; }
    }

    /// <summary>
    /// OAuth authorisation grant response.
    /// </summary>
    public class OAuthAuthorisationGrantResponse
	{
        /// <summary>
        /// Indicates if the request was successfull.
        /// </summary>
		public bool IsSuccessfull { get; set; }
        /// <summary>
        /// OAuth error details if an error was encountered.
        /// </summary>
		public OAuthGrantRequestError ErrorDetails { get; set; }
        /// <summary>
        /// The access and refresh token details on a successfull authentication request.
        /// </summary>
		public OAuthAccessTokenGrant AccessGrant { get; set; }
	}

    /// <summary>
    /// OAuth access token refresh request.
    /// </summary>
    public class OAuthRefreshAccessTokenRequest
    {
        /// <summary>
        /// The grant type when requesting a token refresh. This must be a value of 'refresh_token'.
        /// </summary>
        public string grant_type { get; set; }
        /// <summary>
        /// The refresh token previously issued in a successfull authentication request.
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// The scope of the request. Not applicable to refresh tokens in this scenario.
        /// </summary>
        public string scope { get; set; }
    }

    /// <summary>
    /// OAuth password credentials grant response.
    /// </summary>
    public class OAuthAccessTokenGrant
	{
        /// <summary>
        /// The access token when authentication is successfull. This is typically a short lived token and expires in a relatively short time.
        /// </summary>
		public string access_token { get; set; }
        /// <summary>
        /// The type of token returned. Currently, only 'Bearer' is supported.
        /// </summary>
		public string token_type { get; set; }
        /// <summary>
        /// The time in seconds when the access token expires. Another access token can be requested using the refresh token.
        /// </summary>
		public int expires_in { get; set; }
        /// <summary>
        /// The refresh token issued as part of a successfull authentication request. This token is typically long lived, and may not expire at all.
        /// </summary>
		public string refresh_token { get; set; }
        /// <summary>
        /// The scope of access granted. Currently, this is the scope access descriptor (eg. "full") and a list of space separated id's that 
        /// represent each file  that the authenticated user has access to in the form "full fileid:1 fileid:1234 fileid:567"
        /// </summary>
		public string scope { get; set; }
    }

    /// <summary>
    /// OAuth access grant error.
    /// </summary>
    public class OAuthGrantRequestError
	{
        /// <summary>
        /// A concise error message
        /// </summary>
		public string error { get; set; }
        /// <summary>
        /// A longer more detailed description of the error. Not always populated.
        /// </summary>
		public string error_description { get; set; }
	}
}
