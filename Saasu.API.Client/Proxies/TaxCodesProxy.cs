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

namespace Saasu.API.Client.Proxies
{
    public class TaxCodesProxy : BaseProxy
    {
        public TaxCodesProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;

		}

          public TaxCodesProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
			ContentType = RequestContentType.ApplicationXml;
		}

        public override string RequestPrefix
        {
            get { return ResourceNames.TaxCodes; }
        }

        public TaxCodesProxy(string bearerToken) : base(bearerToken) { }

        public ProxyResponse<TaxCodesResponse> GetTaxCodes(bool? isActive, int? pageNumber, int? pageSize)
        {
            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();

            if (isActive != null)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterIsActive, isActive.ToString());
            }

            bool inclPageNumber;
            bool inclPageSize;
            GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

            var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
            
            return base.GetResponse<TaxCodesResponse>(uri);
        }
    }
}
