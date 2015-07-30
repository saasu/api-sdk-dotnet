using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.TaxCode;

namespace Saasu.API.Client.Proxies
{
    public class TaxCodeProxy : BaseProxy
    {
        public TaxCodeProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;

		}

        public TaxCodeProxy(string bearerToken) : base(bearerToken) { }

        public TaxCodeProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
		}

        public override string RequestPrefix
        {
            get { return ResourceNames.TaxCode; }
        }

        public ProxyResponse<TaxCodeDetail> GetTaxCode(int taxCodeId)
        {
            OperationMethod = HttpMethod.Get;
            var uri = base.GetRequestUri(taxCodeId.ToString());
            return base.GetResponse<TaxCodeDetail>(uri);
        }
    }
}
