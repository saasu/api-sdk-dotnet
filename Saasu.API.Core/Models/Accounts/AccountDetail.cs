using System;

namespace Saasu.API.Core.Models.Accounts
{
    /// <summary>
    /// Represents an account.
    /// </summary>
	public class AccountDetail : BaseModel
	{
		/// <summary>
		/// The unique Id for the account.
		/// </summary>
		public int? Id { get; set; }
		/// <summary>
		/// The account name.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable=true)]
		public string Name { get; set; }
        /// <summary>
        /// The level of account - either "Header" or "Detail". Default is Detail.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string AccountLevel { get; set; }
		/// <summary>
		/// The type of account. E.g. "Income".
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable=true)]
		public string AccountType { get; set; }
		/// <summary>
		/// Whether the account is active or not.
		/// </summary>
		public bool? IsActive { get; set; }
		/// <summary>
		/// Whether the account is a default one that comes with the subscription or not.
		/// </summary>
		public bool? IsBuiltIn { get; set; }
		/// <summary>
		/// Unique identifier to ensure concurrency issue avoidance when performing updates.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
		public string LastUpdatedId { get; set; }
		/// <summary>
		/// The default tax code used for the account.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string DefaultTaxCode { get; set; }
		/// <summary>
		/// A unique code to identify the account in general ledger.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LedgerCode { get; set; }
		/// <summary>
		/// The currency used for the account.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Currency { get; set; }
        /// <summary>
        /// The Header account for this detail account.
        /// </summary>
        public int? HeaderAccountId { get; set; }
		/// <summary>
		/// The base currency account for foreign exchange.
		/// </summary>
		public int? ExchangeAccountId { get; set; }
		/// <summary>
		/// Whether the account is a bank account or not.
		/// </summary>
		public bool? IsBankAccount { get; set; }
		/// <summary>
		/// The date and time the account was created.
		/// </summary>
		public DateTime? CreatedDateUtc { get; set; }
		/// <summary>
		/// The date and time and the account was last modified.
		/// </summary>
		public DateTime? LastModifiedDateUtc { get; set; }
		/// <summary>
		/// Whether to include the account in Forecaster report and data forecasting.
		/// </summary>
		public bool? IncludeInForecaster { get; set; }
		//Bank account properties
		/// <summary>
		/// BSB of the bank.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string BSB { get; set; }
		/// <summary>
		/// The bank account number.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Number { get; set; }
		/// <summary>
		/// The name of the bank account.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string BankAccountName { get; set; }
		/// <summary>
		/// Enables/Disables the ability to upload an ABA file to the account.
		/// </summary>
		public bool? BankFileCreationEnabled { get; set; }
		/// <summary>
		/// The Bank code.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string BankCode { get; set; }
		/// <summary>
		/// The Bank client user Id.
		/// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string UserNumber { get; set; }
		/// <summary>
		/// The merchant fee account Id.
		/// </summary>
		public int? MerchantFeeAccountId { get; set; }
		/// <summary>
		///  Indicates whether to include pending transactions.
		/// </summary>
		public bool? IncludePendingTransactions { get; set; }

		public override string ModelKeyValue()
		{
			return Id.ToString();
		}
	}
}
