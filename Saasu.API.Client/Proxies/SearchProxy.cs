using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Search;
using System.Net.Http;
using System.Text;

namespace Saasu.API.Client.Proxies
{
    public class SearchProxy : BaseProxy
    {
        public SearchProxy()
            : base()
        {
            ContentType = RequestContentType.ApplicationJson;
        }

        public SearchProxy(string bearerToken) : base(bearerToken) { }

        public SearchProxy(string baseUri, string wsAccessKey, int fileUid)
            : base(baseUri, wsAccessKey, fileUid)
        {
        }

        public override string RequestPrefix
        {
            get { return ResourceNames.Search; }
        }

        public ProxyResponse<SearchResponse> Search(string keywords, SearchScope scope, int pageNumber, int pageSize, string entityType = "", string includeSearchTermHighlights = "false")
        {
            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();

            AppendQueryArg(queryArgs, ApiConstants.FilterKeywords, keywords);
            AppendQueryArg(queryArgs, ApiConstants.FilterSearchScope, scope.ToString("G"));
            AppendQueryArg(queryArgs, ApiConstants.FilterSearchEntityType, entityType);
            AppendQueryArg(queryArgs, ApiConstants.FilterExampleIncludeSearchTermHighlights, includeSearchTermHighlights);

            bool inclPageNumber;
            bool inclPageSize;

            base.GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

            var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
            return base.GetResponse<SearchResponse>(uri);
        }
    }
}
