using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Accounts;
using Saasu.API.Core;

namespace Saasu.API.Client.Proxies
{
	public class AccountProxy : BaseProxy
	{
			public AccountProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;

		}

        public AccountProxy(string bearerToken) : base(bearerToken) { }

		public AccountProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
		}

		public override string RequestPrefix
		{
			get { return ResourceNames.Account; }
		}

        public ProxyResponse<AccountDetail> GetAccount(int accountId)
        {
            OperationMethod = HttpMethod.Get;
			var uri = base.GetRequestUri(accountId.ToString());
            return base.GetResponse<AccountDetail>(uri);
        }

        public ProxyResponse<InsertAccountResult> InsertAccount(AccountDetail accountDetail)
        {
            OperationMethod = HttpMethod.Post;
            var uri = base.GetRequestUri(null);
            return base.GetResponse<AccountDetail, InsertAccountResult>(uri, accountDetail);
        }

		public ProxyResponse<UpdateAccountResult> UpdateAccount(int accountId, AccountDetail accountDetail)
        {
            OperationMethod = HttpMethod.Put;
			var uri = base.GetRequestUri(accountId.ToString());
			return base.GetResponse<AccountDetail, UpdateAccountResult>(uri, accountDetail);
        }

		public ProxyResponse<BaseResponseModel> DeleteAccount(int accountId)
        {            
            OperationMethod = HttpMethod.Delete;
			var uri = base.GetRequestUri(accountId.ToString());
            return base.GetResponse<BaseResponseModel>(uri);
        }        
	}
}
