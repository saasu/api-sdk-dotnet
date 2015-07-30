using System.Collections.Generic;
using System.Linq;

namespace Saasu.API.Core.Models.Accounts
{
    /// <summary>
    /// A list of accounts.
    /// </summary>
	public class AccountListResponse : BaseModel, IApiResponseCollection
	{
		public AccountListResponse()
		{
			Accounts = new List<AccountDetail>();
		}
        /// <summary>
        /// </summary>
		public List<AccountDetail> Accounts { get; set; }

		public IEnumerable<BaseModel> ListCollection()
		{
			return Accounts;
		}

		public override string ModelKeyValue()
		{
			return string.Empty;
		}
	}
}
