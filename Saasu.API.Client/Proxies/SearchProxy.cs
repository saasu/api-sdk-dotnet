using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Framework;
using Saasu.API.Core;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Items;
using Saasu.API.Core.Models.Payments;
using Saasu.API.Core.Models.Search;

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

        public ProxyResponse<SearchResponse> Search(string keywords, SearchScope scope, int pageNumber, int pageSize)
        {
            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();

            AppendQueryArg(queryArgs, ApiConstants.FilterKeywords, keywords);
            AppendQueryArg(queryArgs, ApiConstants.FilterSearchScope, scope.ToString("G"));

            bool inclPageNumber;
            bool inclPageSize;

            base.GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

            var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
            return base.GetResponse<SearchResponse>(uri);
        }
    }
}
