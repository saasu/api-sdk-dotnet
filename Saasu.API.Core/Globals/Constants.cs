using System;
using System.Collections.Generic;
using System.Text;

namespace Saasu.API.Core.Globals
{
    public static class Constants
    {
        public static class TaxCode
        {
            public const string ExpInclGst = "G11";
            public const string ExpGstFree = "G11,G14";
            public const string CapExInclGst = "G10";
            public const string CapExGstFree = "G10,G14";
            public const string ExpAdjustments = "G18";
            public const string SaleInclGst = "G1";
            public const string SaleGstFree = "G1,G3";
            public const string SaleInputTaxed = "G1,G4";
            public const string SaleExports = "G1,G2";
            public const string SaleAdjustments = "G7";
            public const string SalaryWageOtherPaid = "W1";
            public const string WithheldTaxOnSalaryWage = "W1,W2";
            public const string WithheldInvestDistribNoTfn = "W3";
            public const string WithheldPaymentNoAbn = "W4";
            public const string WineEqualisationTaxPayable = "1C";
            public const string WineEqualisationTaxRefundable = "1D";
            public const string LuxuryCarTaxPayable = "1E";
            public const string LuxuryCarTaxRefundable = "1F";
        }

        public static class AccountType
        {
            public const string Income = nameof(Income);
            public const string Expense = nameof(Expense);
            public const string Asset = nameof(Asset);
            public const string Equity = nameof(Equity);
            public const string Liability = nameof(Liability);
            public const string OtherIncome = "Other Income";
            public const string OtherExpense = "Other Expense";
            public const string CostOfSales = "Cost of Sales";
        }

        public static class InvoiceLayout
        {
            public const string Item = "I";
            public const string Service = "S";
        }

        public static class InvoiceType
        {
            public const string TaxInvoice = "Tax Invoice";
            public const string SaleInvoice = "Sale Invoice";
            public const string PurchaseInvoice = "Purchase Invoice";
            public const string AdjustmentNote = "Adjustment Note";
            public const string CreditNote = "Credit Note";
            public const string DebitNote = "Debit Note";
            public const string PaymentInvoice = "Payment Invoice";
            public const string RctInvoice = "RCT Invoice";
            public const string MoneyIn = "Money In (Income)";
            public const string MoneyOut = "Money Out (Expense)";
            public const string PurchaseOrder = "Purchase Order";
            public const string SaleOrder = "Sale Order";
            public const string Quote = "Quote";
            public const string PreQuoteOpportunity = "Pre-Quote Opportunity";
            public const string SelfBilling = "Self-Billing";
            public const string Consignment = "Consignment";
        }
    }
}
