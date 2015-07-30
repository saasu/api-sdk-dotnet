using System;

namespace Saasu.API.Core.Models.Accounts
{
	public class InsertAccountResult : UpdateAccountResult
	{
        /// <summary>
        /// The Id/key of the newly created/inserted account.
        /// </summary>
		public int InsertedEntityId { get; set; }

		public override string ModelKeyValue()
		{
			return InsertedEntityId.ToString();
		}
	}
}
