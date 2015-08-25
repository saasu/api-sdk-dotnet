using System;
using System.Collections.Generic;
using System.Linq;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Accounts;
using Saasu.API.Core.Models.Contacts;
using Saasu.API.Core.Models.Items;

namespace Saasu.API.Client.IntegrationTests
{
    public class ItemHelper
    {
        private static int _inventoryAccountId;
        private static int _incomeAccountId;
        private static int _costOfSalesAccountId;
        private static int _purchaseTaxCodeId;
        private static int _salesTaxCodeId;
        private static int _primarySupplierId;

        public ItemHelper()
        {
            GetTaxCodes();
            GetPrimarySupplierContact();
            GetTestAccounts();
        }

        public ItemDetail GetTestInventoryItem()
        {
            var code = Guid.NewGuid().ToString().ToLower().Replace("-", "").Substring(0, 6);
            return new ItemDetail()
            {
                AssetAccountId = _inventoryAccountId,
                BuyingPrice = 100M,
                Code = code,
                DefaultReOrderQuantity = 7,
                Description = "Description " + code,
                IsActive = true,
                IsBought = true,
                IsBuyingPriceIncTax = true,
                IsInventoried = true,
                IsSellingPriceIncTax = true,
                IsSold = true,
                IsVirtual = false,
                IsVisible = true,
                IsVoucher = false,
                MinimumStockLevel = 5,
                Notes = "Some notes here",
                PrimarySupplierContactId = _primarySupplierId,
                PrimarySupplierItemCode = code.Reverse().ToString(),
                PurchaseTaxCodeId = _purchaseTaxCodeId,
                SaleCoSAccountId = _costOfSalesAccountId,
                SaleIncomeAccountId = _incomeAccountId,
                SaleTaxCodeId = _salesTaxCodeId,
                SellingPrice = 120M,
                Type = "I",
                ValidFrom = DateTime.Today.AddDays(-1),
                ValidTo = DateTime.Today.AddMonths(10)
            };
        }

        public ItemDetail GetTestComboItem()
        {
            var item1InsertResponse = new ItemProxy().InsertItem(GetTestInventoryItem());
            var item2InsertResponse = new ItemProxy().InsertItem(GetTestInventoryItem());

            var comboItem = GetTestInventoryItem();
            comboItem.Type = "C";
            comboItem.BuildItems = new List<BuildItem>(){
                new BuildItem()
                {
                    Id = item1InsertResponse.DataObject.InsertedItemId,
                    Quantity = 10
                },
                new BuildItem()
                {
                    Id = item2InsertResponse.DataObject.InsertedItemId,
                    Quantity = 15
                }
            };



            return comboItem;
        }

        private void GetTestAccounts()
        {
            _inventoryAccountId = GetAccount("Asset");
            _incomeAccountId = GetAccount("Income");
            _costOfSalesAccountId = GetAccount("Cost of Sales");
        }

        private void GetTaxCodes()
        {
            var taxCodeProxy = new TaxCodesProxy();
            var taxCodesForFileResponse = taxCodeProxy.GetTaxCodes(true, null, null);
            _purchaseTaxCodeId = taxCodesForFileResponse.DataObject.TaxCodes.First(x => x.IsPurchase).Id;
            _salesTaxCodeId = taxCodesForFileResponse.DataObject.TaxCodes.First(x => x.IsSale).Id;
        }

        private void GetPrimarySupplierContact()
        {
            var contact = new Contact()
            {
                GivenName = "Peter",
                FamilyName = "Baker",
                EmailAddress = "peter@bakertest.com",
                HomePhone = "0252113414"
            };

            var proxy = new ContactProxy();
            var response = proxy.InsertContact(contact);
            _primarySupplierId = response.DataObject.InsertedContactId;
        }

        private static int GetAccount(string accountType)
        {
            var accountsProxy = new AccountsProxy();
            var accountsResponse = accountsProxy.GetAccounts(isActive: true, accountType: accountType);

            if (accountsResponse.DataObject.Accounts.Count == 0)
            {
                var account = new AccountDetail
                {
                    Name = string.Format("TestAccount_{0}", Guid.NewGuid()),
                    AccountType = "Asset",
                    IsActive = true,
                    DefaultTaxCode = "G1",
                    LedgerCode = "AA",
                    Currency = "AUD",
                    IsBankAccount = false
                };

                var accountProxy = new AccountProxy();
                var accountResponse = accountProxy.InsertAccount(account);
                if (accountResponse.IsSuccessfull)
                    _inventoryAccountId = accountResponse.DataObject.InsertedEntityId;
                return _inventoryAccountId;
            }
            else
            {
                return accountsResponse.DataObject.Accounts.First().Id.Value;
            }
        }
    }
}