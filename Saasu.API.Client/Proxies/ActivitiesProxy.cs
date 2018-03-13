using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Activities;
using System;
using System.Net.Http;
using System.Text;

namespace Saasu.API.Client.Proxies
{
    public class ActivitiesProxy : BaseProxy
    {
        public ActivitiesProxy()
            : base()
        {
            ContentType = RequestContentType.ApplicationJson;

        }

        public ActivitiesProxy(string baseUri, string wsAccessKey, int fileUid)
            : base(baseUri, wsAccessKey, fileUid)
        {
            ContentType = RequestContentType.ApplicationXml;
        }

        public ActivitiesProxy(string bearerToken) : base(bearerToken) { }

        public override string RequestPrefix
        {
            get { return ResourceNames.Activities; }
        }

        public ProxyResponse<ActivitySummaryReponse> GetActivities(int? pageNumber = null, int? pageSize = null, string activityType = null, string activityStatus = null, string ownerEmail = null, string searchText = null, string attachedToType = null, int? attachedToId = null,
                        DateTime? fromDate = null, DateTime? toDate = null, DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null, string tags = null, string tagFilterType = null)

        {
            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(activityType))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterActivityType, activityType);
            }

            if (!string.IsNullOrWhiteSpace(activityStatus))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterActivityStatus, activityStatus);
            }

            if (!string.IsNullOrWhiteSpace(ownerEmail))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterActivityOwnerEmail, ownerEmail);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterSearchText, searchText);
            }

            if (!string.IsNullOrWhiteSpace(attachedToType))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterActivityAttachedToType, attachedToType);
            }

            if (attachedToId.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterAttachedToId, attachedToId.Value.ToString());
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterFromDate, fromDate.Value.ToString("u"));
                AppendQueryArg(queryArgs, ApiConstants.FilterToDate, toDate.Value.ToString("u"));
            }
            if (lastModifiedFromDate.HasValue && lastModifiedToDate.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterLastModifiedFromDate, lastModifiedFromDate.Value.ToString("u"));
                AppendQueryArg(queryArgs, ApiConstants.FilterLastModifiedToDate, lastModifiedToDate.Value.ToString("u"));
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
            return base.GetResponse<ActivitySummaryReponse>(uri);
        }
    }
}
