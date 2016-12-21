using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Journals;
using System;
using System.Net.Http;
using System.Text;

namespace Saasu.API.Client.Proxies
{
    public class JournalsProxy : BaseProxy
    {
        public JournalsProxy()
            : base()
        {
            ContentType = RequestContentType.ApplicationJson;
        }

        public JournalsProxy(string bearerToken) : base(bearerToken) { }

        public JournalsProxy(string baseUri, string wsAccessKey, int fileUid)
            : base(baseUri, wsAccessKey, fileUid)
        {
            ContentType = RequestContentType.ApplicationXml;
        }

        public override string RequestPrefix
        {
            get { return ResourceNames.Journals; }
        }

        public ProxyResponse<JournalTransactionSummaryResponse> GetJournals(int? pageNumber = null, int? pageSize = null, DateTime? fromDate = null, DateTime? toDate = null,
            DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null, int? contactId = null, string tags = null, string tagFilterType = null)
        {
            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();

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
            if (contactId != null && contactId > 0)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterJournalContactId, contactId.ToString());
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
            return base.GetResponse<JournalTransactionSummaryResponse>(uri);
        }
    }
}
