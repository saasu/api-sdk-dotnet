using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.FileIdentity;
using System.Net.Http;
using System.Text;

namespace Saasu.API.Client.Proxies
{
    public class FileIdentitiesProxy : BaseProxy
    {
        public FileIdentitiesProxy()
            : base()
        {
        }

        public FileIdentitiesProxy(string bearerToken) : base(bearerToken) { }

        public FileIdentitiesProxy(string baseUri, string wsAccessKey, int fileUid)
            : base(baseUri, wsAccessKey, fileUid)
        {
        }

        public override string RequestPrefix
        {
            get { return ResourceNames.FileIdentities; }
        }

        public ProxyResponse<FileIdentitySummaryResponse> GetFileIdentities(int? pageNumber = null, int? pageSize = null)
        {

            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();

            bool inclPageNumber;
            bool inclPageSize;

            base.GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

            var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
            return base.GetResponse<FileIdentitySummaryResponse>(uri);
        }
    }
}
