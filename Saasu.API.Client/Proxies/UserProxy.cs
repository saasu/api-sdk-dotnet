using System.Net.Http;
using Saasu.API.Client.Framework;
using Saasu.API.Core;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Users;

namespace Saasu.API.Client.Proxies
{
    public class UserProxy : BaseProxy
    {
        public UserProxy()
        {
            OperationMethod = HttpMethod.Post;
        }

        public override string RequestPrefix
        {
            get { return ResourceNames.User; }
        }

        public ProxyResponse<BaseResponseModel> ResetPassword(string username)
        {
            RequestPostfix = ApiConstants.ResetPasswordUrlPath;
            ContentType = RequestContentType.ApplicationJson;
            AuthenticationMethod = AuthenticationType.Anonymous;
            PagingEnabled = false;
            var uri = GetRequestUri(null);
            return GetResponse<UserCredential, BaseResponseModel>(uri, new UserCredential {Username = username});
        }
    }
}