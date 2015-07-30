using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Framework;
using Saasu.API.Core;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models;
using Saasu.API.Core.Models.Contacts;
using Saasu.API.Core.Models.TaxCode;
using Saasu.API.Core.Models.Company;

namespace Saasu.API.Client.Proxies
{
    public class CompanyProxy : BaseProxy
    {
        public CompanyProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;

		}

        public CompanyProxy(string bearerToken) : base(bearerToken) { }

        public CompanyProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
		}

        public override string RequestPrefix
        {
            get { return ResourceNames.Company; }
        }

        public ProxyResponse<CompanyDetail> GetCompany(int companyId)
        {
            OperationMethod = HttpMethod.Get;
            var uri = base.GetRequestUri(companyId.ToString());
            return base.GetResponse<CompanyDetail>(uri);
        }

        public ProxyResponse<InsertCompanyResult> InsertCompany(CompanyDetail company)
        {
            OperationMethod = HttpMethod.Post;
            var uri = base.GetRequestUri(null);
            return base.GetResponse<CompanyDetail, InsertCompanyResult>(uri, company);
        }

        public ProxyResponse<UpdateCompanyResult> UpdateCompany(CompanyDetail company, int companyId)
        {
            OperationMethod = HttpMethod.Put;
            var uri = base.GetRequestUri(companyId.ToString());
            return base.GetResponse<CompanyDetail, UpdateCompanyResult>(uri, company);
        }

        public ProxyResponse<BaseResponseModel> DeleteCompany(int companyId)
        {
            OperationMethod = HttpMethod.Delete;
            var uri = base.GetRequestUri(companyId.ToString());
            return base.GetResponse<BaseResponseModel>(uri);
        }
    }
}
