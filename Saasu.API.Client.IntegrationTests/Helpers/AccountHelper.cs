using System;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Accounts;

namespace Saasu.API.Client.IntegrationTests.Helpers
{
    public class AccountHelper
    {

        public AccountHelper()
        {
            CreateTestData();
        }

        public int NonBankAcctId { get; private set; }

        public int BankAcctId { get; private set; }

        public int AccountToBeUpdated { get; private set; }

        public int BankAccountToBeUpdated { get; private set; }

        public int InactiveAccountId { get; private set; }

        public int HeaderAccountId { get; private set; }

        public int AccountToAssignToHeaderAccount { get; private set; }


        private void CreateTestData()
        {
            var accountProxy = new AccountProxy();

            if (NonBankAcctId == 0)
            {
                var account = GetTestAccount();
                var insertResult = accountProxy.InsertAccount(account);

                NonBankAcctId = insertResult.DataObject.InsertedEntityId;
            }

            if (BankAcctId == 0)
            {
                var account = GetTestBankAccount();
                var insertResult = accountProxy.InsertAccount(account);

                BankAcctId = insertResult.DataObject.InsertedEntityId;
            }

            if (InactiveAccountId == 0)
            {
                var account = GetTestAccount();
                account.IsActive = false;
                var insertResult = accountProxy.InsertAccount(account);

                InactiveAccountId = insertResult.DataObject.InsertedEntityId;
            }

            if (AccountToBeUpdated == 0)
            {
                var account = GetTestAccount();
                var insertResult = accountProxy.InsertAccount(account);

                AccountToBeUpdated = insertResult.DataObject.InsertedEntityId;
            }

            if (BankAccountToBeUpdated == 0)
            {
                var account = GetTestBankAccount();
                var insertResult = accountProxy.InsertAccount(account);

                BankAccountToBeUpdated = insertResult.DataObject.InsertedEntityId;
            }

            if (HeaderAccountId == 0)
            {
                var account = GetTestHeaderAccount();
                
                var insertResult = accountProxy.InsertAccount(account);

                HeaderAccountId = insertResult.DataObject.InsertedEntityId;
            }

            if (AccountToAssignToHeaderAccount == 0)
            {
                var account = GetTestAccount();
                account.HeaderAccountId = HeaderAccountId;
                var insertResult = accountProxy.InsertAccount(account);
                AccountToAssignToHeaderAccount = insertResult.DataObject.InsertedEntityId;
            }
        }
        
        public AccountDetail GetTestAccount()
        {
            return new AccountDetail
            {
                Name = string.Format("TestAccount_{0}", Guid.NewGuid()),
                AccountType = "Income",
                IsActive = true,
                DefaultTaxCode = "G1",
                LedgerCode = "AA",
                Currency = "AUD",
                IsBankAccount = false
            };
        }

        public AccountDetail GetTestBankAccount()
        {
            return new AccountDetail
            {
                Name = string.Format("TestAccount_{0}", Guid.NewGuid()),
                AccountType = "Asset",
                IsActive = true,
                DefaultTaxCode = null,
                LedgerCode = "BB",
                Currency = "AUD",
                IsBankAccount = true,
                IncludeInForecaster = true,
                BSB = "010101",
                Number = "11111111",
                BankAccountName = string.Format("Test Bank Account_{0}", Guid.NewGuid()),
                BankFileCreationEnabled = true,
                BankCode = "TBA",
                UserNumber = "111",
                MerchantFeeAccountId = null,
                IncludePendingTransactions = true
            };
        }

        public AccountDetail GetTestHeaderAccount()
        {
            return new AccountDetail
            {
                Name = string.Format("TestAccount_{0}", Guid.NewGuid()),
                AccountLevel = "Header",
                AccountType = "Income",
                LedgerCode = "AA"
            };
        }
    }
    
}