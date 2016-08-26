using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Items;

namespace Saasu.API.Client.Proxies
{
    public class ItemsProxy : BaseProxy
    {
        public ItemsProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;

		}

        public ItemsProxy(string bearerToken) : base(bearerToken) { }

        public ItemsProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
			ContentType = RequestContentType.ApplicationXml;
		}

        public override string RequestPrefix
        {
            get { return ResourceNames.Items; }
        }

        public ProxyResponse<ItemSummaryResponse> GetItems(string itemType, string searchMethod, 
            string searchText, int pageNumber, int pageSize, DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null)
        {
            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();

            if (!string.IsNullOrEmpty(itemType))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterItemType, itemType);
            }
            if (!string.IsNullOrEmpty(searchMethod) && !string.IsNullOrEmpty(searchText))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterSearchMethod, searchMethod);
                AppendQueryArg(queryArgs, ApiConstants.FilterSearchText, searchText);
            }
            if (lastModifiedFromDate.HasValue && lastModifiedToDate.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterLastModifiedFromDate, lastModifiedFromDate.Value.ToString("o"));
                AppendQueryArg(queryArgs, ApiConstants.FilterLastModifiedToDate, lastModifiedToDate.Value.ToString("o"));
            }

            bool inclPageNumber;
            bool inclPageSize;

            base.GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

            var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
            return base.GetResponse<ItemSummaryResponse>(uri);
        }
    }
}
