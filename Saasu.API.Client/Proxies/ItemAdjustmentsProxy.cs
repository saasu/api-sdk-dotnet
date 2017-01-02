using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.ItemAdjustments;

namespace Saasu.API.Client.Proxies
{
	public class ItemAdjustmentsProxy : BaseProxy
	{
		public ItemAdjustmentsProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;
		}

		public override string RequestPrefix { get { return ResourceNames.ItemAdjustments; } }

		public ProxyResponse<ItemAdjustmentListResponse> GetItemAdjustments(int? pageNumber = null, int? pageSize = null, DateTime? fromDate = null, DateTime? toDate = null,
			DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null, string tags = null, string tagFilterType = null)
		{
			OperationMethod = HttpMethod.Get;
			var queryArgs = new StringBuilder();

			if (fromDate.HasValue && toDate.HasValue)
			{
				AppendQueryArg(queryArgs, ApiConstants.FilterFromDate, fromDate.Value.ToString("o"));
				AppendQueryArg(queryArgs, ApiConstants.FilterToDate, toDate.Value.ToString("o"));
			}
			if (lastModifiedFromDate.HasValue && lastModifiedToDate.HasValue)
			{
				AppendQueryArg(queryArgs, ApiConstants.FilterLastModifiedFromDate, lastModifiedFromDate.Value.ToString("o"));
				AppendQueryArg(queryArgs, ApiConstants.FilterLastModifiedToDate, lastModifiedToDate.Value.ToString("o"));
			}
			if (!string.IsNullOrWhiteSpace(tags))
			{
				AppendQueryArg(queryArgs, ApiConstants.FilterTags, tags);
			}
			if (!string.IsNullOrWhiteSpace(tagFilterType))
			{
				AppendQueryArg(queryArgs, ApiConstants.FilterTagFilterType, tagFilterType);
			}
			bool inclPageNumber;
			bool inclPageSize;

			base.GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

			var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
			return base.GetResponse<ItemAdjustmentListResponse>(uri);
		}
	}
}
