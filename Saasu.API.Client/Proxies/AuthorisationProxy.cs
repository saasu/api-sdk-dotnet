using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Models.Security;

using System.Net;
using System.Net.Http;


#if NETSTANDARD2_0

using Newtonsoft.Json;

#else

using System.Web.Script.Serialization;

#endif


namespace Saasu.API.Client.Proxies
{
	public class AuthorisationProxy : BaseProxy
	{
		public AuthorisationProxy() : base()
		{
			AuthenticationMethod = AuthenticationType.OAuth;
			PagingEnabled = false;
            OperationMethod = System.Net.Http.HttpMethod.Post;
		}

		public AuthorisationProxy(string bearerToken) : base(bearerToken)
		{
			AuthenticationMethod = AuthenticationType.OAuth;
			PagingEnabled = false;
		}

		public override string RequestPrefix
		{
			get { return ResourceNames.Authorisation; }
		}

		/// <summary>
		/// Resource owner password credentials grant
		/// (Section 4.3 of OAuth2 Draft spec)
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public ProxyResponse<OAuthAuthorisationGrantResponse> PasswordCredentialsGrantRequest(string username, string password, string scope = "")
		{
			var result = new OAuthAuthorisationGrantResponse();

			ContentType = RequestContentType.ApplicationJson;
            var postBody = new OAuthPasswordCredentialsGrantRequest { grant_type = "password", password = password, username = username, scope = scope };
			//var uri = base.GetRequestUri(string.Format("?grant_type=password&username={0}&password={1}&scope={2}",username,password,scope));
            var uri = base.GetRequestUri("token");
            var response = GetResponse(uri, postBody);
			if (response.IsSuccessfull)
			{
				try
				{
#if NETSTANDARD2_0
                    var dto = JsonConvert.DeserializeObject<OAuthAccessTokenGrant>(response.RawResponse);
                    result.IsSuccessfull = true;
                    result.AccessGrant = dto;

#else
					JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
					var dto = jsonSerializer.Deserialize<OAuthAccessTokenGrant>(response.RawResponse);
					result.IsSuccessfull = true;
					result.AccessGrant = dto;
#endif
				}
				catch
				{
#if NETSTANDARD2_0
                    var dto = JsonConvert.DeserializeObject<OAuthGrantRequestError>(response.RawResponse);
                    result.IsSuccessfull = false;
                    result.ErrorDetails = dto;
#else
					JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
					var dto = jsonSerializer.Deserialize<OAuthGrantRequestError>(response.RawResponse);
					result.IsSuccessfull = false;
					result.ErrorDetails = dto;
#endif

				}
			}
			else
			{
				result.IsSuccessfull = false;
			}

			var statusCode = result.IsSuccessfull ? HttpStatusCode.OK : HttpStatusCode.Unauthorized;
			return new ProxyResponse<OAuthAuthorisationGrantResponse>(response.RawResponse, result, result.IsSuccessfull, statusCode, string.Empty);
		}

		/// <summary>
		/// Resource owner Authorisation code grant
		/// (Section 4.1 of OAuth2 Draft spec)
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public ProxyResponse<OAuthAuthorisationGrantResponse> AuthorisationCodeGrantRequest(string client_id, string scope = "", string state = "")
		{
            throw new NotImplementedException();
        }

		public ProxyResponse<OAuthAuthorisationGrantResponse> RefreshAccessToken(string refreshToken, string scope)
		{
			var result = new OAuthAuthorisationGrantResponse();
            OperationMethod = HttpMethod.Post;

			ContentType = RequestContentType.ApplicationJson;
            var uri = base.GetRequestUri("refresh");
            var postBody = new OAuthRefreshAccessTokenRequest { grant_type = "refresh_token", refresh_token = refreshToken, scope = scope };
			var response = GetResponse(uri, postBody);

			if (response.IsSuccessfull)
			{
				try
				{
#if NETSTANDARD2_0
                    var dto = JsonConvert.DeserializeObject<OAuthAccessTokenGrant>(response.RawResponse);
                    result.IsSuccessfull = true;
                    result.AccessGrant = dto;
#else
					JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
					var dto = jsonSerializer.Deserialize<OAuthAccessTokenGrant>(response.RawResponse);
					result.IsSuccessfull = true;
					result.AccessGrant = dto;
#endif

                }
				catch
				{
#if NETSTANDARD2_0
                    var dto = JsonConvert.DeserializeObject<OAuthGrantRequestError>(response.RawResponse);
                    result.IsSuccessfull = false;
                    result.ErrorDetails = dto;
#else
					JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
					var dto = jsonSerializer.Deserialize<OAuthGrantRequestError>(response.RawResponse);
					result.IsSuccessfull = false;
					result.ErrorDetails = dto;
#endif

				}
			}
			else
			{
				result.IsSuccessfull = false;
			}

			var statusCode = result.IsSuccessfull ? HttpStatusCode.OK : HttpStatusCode.Unauthorized;
			return new ProxyResponse<OAuthAuthorisationGrantResponse>(response.RawResponse, result, result.IsSuccessfull, statusCode, string.Empty);

			
		}

		public ProxyResponse<string> AuthorisationPing()
		{
			ContentType = RequestContentType.ApplicationJson;
            OperationMethod = HttpMethod.Get;
			var uri = base.GetRequestUri("Ping");
			return base.GetResponse<string>(uri);
		}
	}
}
