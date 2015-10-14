using System.Globalization;

namespace Saasu.API.Core.Models.Accounts
{
    public class UpdateAccountResult : BaseUpdateResultModel
	{
		/// <summary>
		/// The id of the updated account.
		/// </summary>
		public int UpdatedAccountId { get; set; }
	

		public override string ModelKeyValue()
		{
			return UpdatedAccountId.ToString(CultureInfo.InvariantCulture);
		}
	}
}
