using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.TaxCode;
using Saasu.API.Core.Models.Company;

namespace Saasu.API.Client.Proxies
{
    public class CompaniesProxy : BaseProxy
    {
        public CompaniesProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;

		}

          public CompaniesProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
			ContentType = RequestContentType.ApplicationXml;
		}

        public override string RequestPrefix
        {
            get { return ResourceNames.Companies; }
        }

        public CompaniesProxy(string bearerToken) : base(bearerToken) { }

        public ProxyResponse<CompanyListResponse> GetCompanies(string companyName, DateTime? lastModifiedFrom, DateTime? lastModifiedTo, int? pageNumber, int? pageSize)
        {
            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(companyName))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterCompanyName, companyName);
            }

            if (lastModifiedFrom.HasValue && lastModifiedFrom.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterExampleLastModifiedFromDate, lastModifiedFrom.Value.ToString("o"));
                AppendQueryArg(queryArgs, ApiConstants.FilterExampleLastModifiedToDate, lastModifiedTo.Value.ToString("o"));
            }

            bool inclPageNumber;
            bool inclPageSize;
            GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

            var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);

            return base.GetResponse<CompanyListResponse>(uri);
        }
    }
}
