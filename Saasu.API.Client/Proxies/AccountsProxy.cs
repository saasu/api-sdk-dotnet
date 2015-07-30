using System.Text;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Models.Accounts;
using System.Net.Http;

namespace Saasu.API.Client.Proxies
{
	public class AccountsProxy : BaseProxy
	{
		public AccountsProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;

		}

		public AccountsProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
			ContentType = RequestContentType.ApplicationXml;
		}

		public AccountsProxy(string bearerToken) : base(bearerToken) { }

		public override string RequestPrefix
		{
			get { return ResourceNames.Accounts; }
		}


		public ProxyResponse<AccountListResponse> GetAccounts(int? pageNumber = null, int? pageSize = null, bool? isActive = null, bool? isBankAccount = null, 
			string accountType = null, bool? includeBuiltIn = null)
		{
			OperationMethod = HttpMethod.Get;
			var queryArgs = new StringBuilder();

			if (isActive.HasValue)
			{
				AppendQueryArg(queryArgs, ApiConstants.FilterIsActive, isActive.Value.ToString());
			}

			if (isBankAccount.HasValue)
			{
				AppendQueryArg(queryArgs, ApiConstants.FilterIsBankAccount, isBankAccount.Value.ToString());
			}

			if (!string.IsNullOrWhiteSpace(accountType))
			{
				AppendQueryArg(queryArgs, ApiConstants.FilterAccountType, accountType);
			}

			if (includeBuiltIn.HasValue)
			{
				AppendQueryArg(queryArgs, ApiConstants.FilterIncludeBuiltIn, includeBuiltIn.Value.ToString());
			}

			bool inclPageNumber;
			bool inclPageSize;

			base.GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

			var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
			return base.GetResponse<AccountListResponse>(uri);
		}
	}
}
