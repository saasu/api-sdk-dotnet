using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.DeletedEntities;
using System;
using System.Net.Http;
using System.Text;

namespace Saasu.API.Client.Proxies
{
    public class DeletedEntitiesProxy : BaseProxy
    {
        public DeletedEntitiesProxy() : base()
		{
            ContentType = RequestContentType.ApplicationJson;
        }

        public DeletedEntitiesProxy(string bearerToken) : base(bearerToken) { }

        public DeletedEntitiesProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
            ContentType = RequestContentType.ApplicationXml;
        }

        public override string RequestPrefix
        {
            get { return ResourceNames.DeletedEntities; }
        }

        public ProxyResponse<DeletedEntitiesListResponse> GetDeletedEntities(int? pageNumber = null, int? pageSize = null, string entityType = null, DateTime? utcDeletedFromDate = null, DateTime? utcDeletedToDate = null)
        {
            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterEntityType, entityType);
            }

            if (utcDeletedFromDate.HasValue && utcDeletedToDate.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterFromDate, utcDeletedFromDate.Value.ToString("u"));
                AppendQueryArg(queryArgs, ApiConstants.FilterToDate, utcDeletedToDate.Value.ToString("u"));
            }

            bool inclPageNumber;
            bool inclPageSize;

            base.GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

            var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
            return base.GetResponse<DeletedEntitiesListResponse>(uri);
        }
    }
}
