using System;
using System.Linq;
using Saasu.API.Client.IntegrationTests.Helpers;
using Xunit;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Accounts;

namespace Saasu.API.Client.IntegrationTests
{
	public class AccountTests : IClassFixture<AccountHelper>
	{

		private readonly AccountHelper _accountHelper;

		public AccountTests(AccountHelper accountHelper)
		{
			_accountHelper = accountHelper;
		}

		[Fact]
		public void GetAccountsNoFilter()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts();

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.Accounts.Count > 0, "Zero accounts returned");
		}

		[Fact]
		public void GetNonBankAccount()
		{
			var accountsProxy = new AccountProxy();
			var response = accountsProxy.GetAccount(_accountHelper.NonBankAcctId);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.False(Convert.ToBoolean(response.DataObject.IsBankAccount), "Account returned is a bank account");
		}

		[Fact]
		public void GetBankAccount()
		{
			var accountsProxy = new AccountProxy();
			var response = accountsProxy.GetAccount(_accountHelper.BankAcctId);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(Convert.ToBoolean(response.DataObject.IsBankAccount), "Account returned is not a bank account");
		}

		[Fact]
		public void GetAccountsFilterOnIsBankAccount()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(isBankAccount: true);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.Accounts.Count > 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a =>
				Assert.True(Convert.ToBoolean(a.IsBankAccount), "Non bank accounts have been returned"));
		}

		[Fact]
		public void GetAccountsFilterOnIsNotBankAccount()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(isBankAccount: false);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.Accounts.Count > 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a =>
				Assert.False(Convert.ToBoolean(a.IsBankAccount), "Bank accounts have been returned"));
		}

		[Fact]
		public void GetAccountsFilterOnActive()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(isActive: true);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.Accounts.Count > 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a =>
				Assert.True(Convert.ToBoolean(a.IsActive), "inactive accounts have been returned"));
		}

		[Fact]
		public void GetAccountsFilterOnInactive()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(isActive: false);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.Accounts.Count > 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a => Assert.False(Convert.ToBoolean(a.IsActive)));
		}

		[Fact]
		public void GetAccountsFilterOnIncludeBuiltIn()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(includeBuiltIn: true);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.Accounts.Count > 0, "Zero accounts returned");

			var builtInAccounts = response.DataObject.Accounts.Where(a => Convert.ToBoolean(a.IsBuiltIn));
			Assert.True(builtInAccounts.Any());
		}

		[Fact]
		public void GetAccountsFilterOnNotIncludeBuiltIn()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(includeBuiltIn: false);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.Accounts.Count > 0, "Zero accounts returned");

			var builtInAccounts = response.DataObject.Accounts.Where(a => Convert.ToBoolean(a.IsBuiltIn));
			Assert.Empty(builtInAccounts);
		}

		[Fact]
		public void GetAccountsFilterOnAccountType()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(accountType: "Income");

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.Accounts.Count > 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a => Assert.Equal("Income", a.AccountType));
		}

		[Fact]
		public void GetAccountsFilterOnHeaderAccountId()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(headerAccountId: _accountHelper.HeaderAccountId);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Equal(1, response.DataObject.Accounts.Count);
			Assert.Equal(_accountHelper.AccountToAssignToHeaderAccount, response.DataObject.Accounts[0].Id);
		}

		[Fact]
		public void GetAccountsPageSize()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(pageSize: 10);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Equal(10, response.DataObject.Accounts.Count);
		}

		[Fact]
		public void GetAccountsSecondPage()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(pageNumber: 1, pageSize: 10);

			Assert.NotNull(response);
			Assert.Equal(10, response.DataObject.Accounts.Count);

			var acctIdsFirstPage = response.DataObject.Accounts.Select(a => a.Id).ToList();

			response = accountsProxy.GetAccounts(pageNumber: 2, pageSize: 10);

			Assert.NotNull(response);
			Assert.True(response.DataObject.Accounts.Count > 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a =>
				Assert.False(acctIdsFirstPage.Contains(a.Id), "Record from page 1 returned"));
		}

		[Fact]
		public void InsertNonBankAccount()
		{
			//Create and Insert
			var account = _accountHelper.GetTestAccount();

			var accountProxy = new AccountProxy();
			var response = accountProxy.InsertAccount(account);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.InsertedEntityId > 0, "Zero accounts returned");

			//Get account again and verify inserted fields.
			var acct = accountProxy.GetAccount(response.DataObject.InsertedEntityId);

			Assert.Equal(account.Name, acct.DataObject.Name);
			Assert.Equal(account.AccountType, acct.DataObject.AccountType);
			Assert.Equal(account.DefaultTaxCode, acct.DataObject.DefaultTaxCode);
			Assert.Equal(account.LedgerCode, acct.DataObject.LedgerCode);
			Assert.Equal(account.Currency, acct.DataObject.Currency);
			Assert.Equal(account.IsBankAccount, acct.DataObject.IsBankAccount);
			Assert.Equal(false, acct.DataObject.IncludeInForecaster);
			Assert.Null(acct.DataObject.HeaderAccountId);
		}

		[Fact]
		public void InsertBankAccount()
		{
			//Create and Insert
			var account = _accountHelper.GetTestBankAccount();

			var accountProxy = new AccountProxy();
			var response = accountProxy.InsertAccount(account);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.InsertedEntityId > 0, "Zero accounts returned");

			//Get account again and verify inserted fields.
			var acct = accountProxy.GetAccount(response.DataObject.InsertedEntityId);

			Assert.Equal(account.Name, acct.DataObject.Name);
			Assert.Equal(account.AccountType, acct.DataObject.AccountType);
			Assert.Equal(account.DefaultTaxCode, acct.DataObject.DefaultTaxCode);
			Assert.Equal(account.LedgerCode, acct.DataObject.LedgerCode);
			Assert.Equal(account.Currency, acct.DataObject.Currency);
			Assert.Equal(account.IsBankAccount, acct.DataObject.IsBankAccount);
			Assert.Equal(account.IncludeInForecaster, acct.DataObject.IncludeInForecaster);
			Assert.Equal(account.BSB, acct.DataObject.BSB);
			Assert.Equal(account.Number, acct.DataObject.Number);
			Assert.Equal(account.BankAccountName, acct.DataObject.BankAccountName);
			Assert.Equal(account.BankFileCreationEnabled, acct.DataObject.BankFileCreationEnabled);
			Assert.Equal(account.BankCode, acct.DataObject.BankCode);
			Assert.Equal(account.UserNumber, acct.DataObject.UserNumber);
			Assert.Equal(account.MerchantFeeAccountId, acct.DataObject.MerchantFeeAccountId);
			Assert.Equal(account.IncludePendingTransactions, acct.DataObject.IncludePendingTransactions);
		}

		[Fact]
		public void InsertAccountWithHeader()
		{
			//Create and Insert
			var headerAccount = _accountHelper.GetTestHeaderAccount();
			var accountProxy = new AccountProxy();
			var headerInsertResponse = accountProxy.InsertAccount(headerAccount);
			Assert.NotNull(headerInsertResponse);
			Assert.True(headerInsertResponse.IsSuccessfull, "Reponse has not been successful");
			Assert.True(headerInsertResponse.DataObject.InsertedEntityId > 0, "Zero accounts returned");

			var headerAccountId = headerInsertResponse.DataObject.InsertedEntityId;

			var account = _accountHelper.GetTestAccount();
			account.HeaderAccountId = headerAccountId;

			var response = accountProxy.InsertAccount(account);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.InsertedEntityId > 0, "Zero accounts returned");

			//Get account again and verify inserted fields.
			var acct = accountProxy.GetAccount(response.DataObject.InsertedEntityId);

			Assert.Equal(account.Name, acct.DataObject.Name);
			Assert.Equal(account.AccountType, acct.DataObject.AccountType);
			Assert.Equal(account.DefaultTaxCode, acct.DataObject.DefaultTaxCode);
			Assert.Equal(account.LedgerCode, acct.DataObject.LedgerCode);
			Assert.Equal(account.Currency, acct.DataObject.Currency);
			Assert.Equal(account.IsBankAccount, acct.DataObject.IsBankAccount);
			Assert.Equal(false, acct.DataObject.IncludeInForecaster);
			Assert.Equal(headerAccountId, acct.DataObject.HeaderAccountId);
		}

		[Fact]
		public void InsertHeaderAccount()
		{
			//Create and Insert
			var account = _accountHelper.GetTestHeaderAccount();

			var accountProxy = new AccountProxy();
			var response = accountProxy.InsertAccount(account);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.InsertedEntityId > 0, "Zero accounts returned");

			//Get account again and verify inserted fields.
			var acct = accountProxy.GetAccount(response.DataObject.InsertedEntityId);

			Assert.Equal(account.Name, acct.DataObject.Name);
			Assert.Equal("header", acct.DataObject.AccountLevel.ToLower());
			Assert.Equal(account.AccountType, acct.DataObject.AccountType);
			Assert.Null(acct.DataObject.DefaultTaxCode);
			Assert.Equal(account.LedgerCode, acct.DataObject.LedgerCode);
			Assert.False(Convert.ToBoolean(acct.DataObject.IsBankAccount), "Header accounts cannot be bank accounts");
			Assert.False(Convert.ToBoolean(acct.DataObject.IncludeInForecaster),
				"Header accounts cannot be included in forecaster");
		}

		[Fact]
		public void UpdateAccount()
		{
			var accountProxy = new AccountProxy();

			//Get account, change name then update.
			var acct = accountProxy.GetAccount(_accountHelper.AccountToBeUpdated);

			var newName = string.Format("UpdatedAccount_{0}", Guid.NewGuid());

			var updatedAccount = new AccountDetail
			{
				Name = newName,
				AccountType = "Equity",
				IsActive = false,
				IsBankAccount = false,
				LastUpdatedId = acct.DataObject.LastUpdatedId,
				DefaultTaxCode = "G1,G4",
				Currency = "AUD",
				LedgerCode = "BB"
			};

			var response = accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), updatedAccount);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");

			//Get account again and verify change.
			acct = accountProxy.GetAccount(_accountHelper.AccountToBeUpdated);

			Assert.NotNull(acct);
			Assert.Equal(newName, acct.DataObject.Name);
			Assert.Equal("Equity", acct.DataObject.AccountType);
			Assert.Equal(false, acct.DataObject.IsActive);
			Assert.Equal("G1,G4", acct.DataObject.DefaultTaxCode);
			Assert.Equal("AUD", acct.DataObject.Currency);
			Assert.Equal("BB", acct.DataObject.LedgerCode);
			Assert.Equal(false, acct.DataObject.IncludeInForecaster);
		}

		[Fact]
		public void UpdateBankAccount()
		{
			var accountProxy = new AccountProxy();

			//Get account, change name then update.
			var acct = accountProxy.GetAccount(_accountHelper.BankAccountToBeUpdated);

			var newName = string.Format("UpdatedAccount_{0}", Guid.NewGuid());
			var newBankAccountName = string.Format("Update Bank Account_{0}", Guid.NewGuid());

			var updatedAccount = new AccountDetail
			{
				Name = newName,
				AccountType = "Equity",
				IsActive = false,
				IsBankAccount = true,
				LastUpdatedId = acct.DataObject.LastUpdatedId,
				DefaultTaxCode = null,
				Currency = "AUD",
				LedgerCode = "BB",
				IncludeInForecaster = false,
				BSB = "020202",
				Number = "22222222",
				BankAccountName = newBankAccountName,
				BankFileCreationEnabled = true,
				BankCode = "B",
				UserNumber = "333",
				MerchantFeeAccountId = _accountHelper.BankAcctId,
				IncludePendingTransactions = false
			};

			var response = accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), updatedAccount);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");

			//Get account again and verify change.
			acct = accountProxy.GetAccount(_accountHelper.BankAccountToBeUpdated);

			Assert.NotNull(acct);
			Assert.Equal(newName, acct.DataObject.Name);
			Assert.Equal("Equity", acct.DataObject.AccountType);
			Assert.Equal(false, acct.DataObject.IsActive);
			Assert.Null(acct.DataObject.DefaultTaxCode);
			Assert.Equal("AUD", acct.DataObject.Currency);
			Assert.Equal("BB", acct.DataObject.LedgerCode);
			Assert.Equal(false, acct.DataObject.IncludeInForecaster);
			Assert.Equal("020202", acct.DataObject.BSB);
			Assert.Equal("22222222", acct.DataObject.Number);
			Assert.Equal(newBankAccountName, acct.DataObject.BankAccountName);
			Assert.Equal(true, acct.DataObject.BankFileCreationEnabled);
			Assert.Equal("B", acct.DataObject.BankCode);
			Assert.Equal("333", acct.DataObject.UserNumber);
			Assert.Equal(_accountHelper.BankAcctId, acct.DataObject.MerchantFeeAccountId);
			Assert.Equal(false, acct.DataObject.IncludePendingTransactions);
		}

		[Fact]
		public void UpdateBankAccountBankFileCreationEnabled()
		{
			var accountProxy = new AccountProxy();

			//Get account, change name then update.
			var acct = accountProxy.GetAccount(_accountHelper.BankAcctId);

			var newBankName = string.Format("UpdatedBankName_{0}", Guid.NewGuid());

			acct.DataObject.BankAccountName = newBankName;
			acct.DataObject.BankFileCreationEnabled = true;
			acct.DataObject.BankCode = "AAA";
			acct.DataObject.UserNumber = "222";

			var response = accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), acct.DataObject);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");

			//Get account again and verify change.
			acct = accountProxy.GetAccount(_accountHelper.BankAcctId);

			Assert.NotNull(acct);
			Assert.Equal(newBankName, acct.DataObject.BankAccountName);
			Assert.Equal(true, acct.DataObject.BankFileCreationEnabled);
			Assert.Equal("AAA", acct.DataObject.BankCode);
			Assert.Equal("222", acct.DataObject.UserNumber);

			//Reset Bank Code and Customer Number for other tests.
			acct.DataObject.BankFileCreationEnabled = true;
			acct.DataObject.BankCode = "TBA";
			acct.DataObject.UserNumber = "111";

			accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), acct.DataObject);
		}

		[Fact]
		public void UpdateBankAccountBankFileCreationNotEnabled()
		{
			var accountProxy = new AccountProxy();

			//Get account, change fields then update.
			var acct = accountProxy.GetAccount(_accountHelper.BankAcctId);

			var newBankName = string.Format("UpdatedBankName_{0}", Guid.NewGuid());

			acct.DataObject.BankAccountName = newBankName;
			acct.DataObject.BankFileCreationEnabled = false;
			acct.DataObject.BankCode = null;
			acct.DataObject.UserNumber = null;

			var response = accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), acct.DataObject);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");

			//Get account again and verify change.
			acct = accountProxy.GetAccount(_accountHelper.BankAcctId);

			Assert.NotNull(acct);
			Assert.Equal(newBankName, acct.DataObject.BankAccountName);
			Assert.Equal(false, acct.DataObject.BankFileCreationEnabled);

			//Bank code and user number should not have changed because BankFileCreationEnabled was false.
			Assert.Null(acct.DataObject.BankCode);
			Assert.Null(acct.DataObject.UserNumber);
		}

		[Fact]
		public void UpdateHeaderAccount()
		{
			//Create and Insert
			var account = _accountHelper.GetTestHeaderAccount();

			var accountProxy = new AccountProxy();
			var response = accountProxy.InsertAccount(account);

			Assert.NotNull(response);
			Assert.True(response.IsSuccessfull, "Reponse has not been successful");
			Assert.True(response.DataObject.InsertedEntityId > 0, "Zero accounts returned");

			var accountId = response.DataObject.InsertedEntityId;

			//Get account again and verify inserted fields.
			var insertedAcctFromDb = accountProxy.GetAccount(accountId);

			var newName = string.Format("TestAccount_{0}", Guid.NewGuid());
			account.Name = newName;
			account.LastUpdatedId = insertedAcctFromDb.DataObject.LastUpdatedId;

			var updateResponse = accountProxy.UpdateAccount(response.DataObject.InsertedEntityId, account);
			Assert.NotNull(updateResponse);
			Assert.True(updateResponse.IsSuccessfull, "Reponse has not been successful");

			//Get account again and verify inserted fields.
			var updatedAcctFromDb = accountProxy.GetAccount(accountId);
			Assert.NotNull(updatedAcctFromDb);
			Assert.True(updatedAcctFromDb.IsSuccessfull, "Reponse has not been successful");

			Assert.Equal(newName, updatedAcctFromDb.DataObject.Name);
		}
	}
}



















