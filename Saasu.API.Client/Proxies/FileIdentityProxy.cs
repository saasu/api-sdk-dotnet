using System.Globalization;
using System.Net.Http;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.FileIdentity;

namespace Saasu.API.Client.Proxies
{
    public class FileIdentityProxy : BaseProxy
    {
        public FileIdentityProxy() : base()
        {
            ContentType = RequestContentType.ApplicationJson;
        }

        public FileIdentityProxy(string bearerToken) : base(bearerToken){}

        public FileIdentityProxy(string baseUri, string wsAccessKey, int fileId)
            : base(baseUri, wsAccessKey, fileId)
        {
            
        }

        public override string RequestPrefix
        {
            get { return ResourceNames.FileIdentity; }
        }

        public ProxyResponse<FileIdentityDetail> GetFileIdentity(int fileId)
        {
            OperationMethod = HttpMethod.Get;
            var uri = base.GetRequestUri(string.Empty);
            return base.GetResponse<FileIdentityDetail>(uri);
        }
    }
}
