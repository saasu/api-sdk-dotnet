namespace Saasu.API.Core.Models.Search
{
    public enum SearchTransactionType
    {
        Sale,
        Purchase,
        Journal,
        Payroll

    }
    public static class SearchTransactionFilterExtensions
    {
        const string Sale = "transactions.sale";
        const string Purchase = "transactions.purchase";
        const string Journal = "transactions.journal";
        const string Payroll = "transactions.payroll";
        public static SearchTransactionType? ToSearchTransactionType(this string searchTransactionTypeParameter)
        {
            var lowerParamater = searchTransactionTypeParameter.ToLowerInvariant();
            if (lowerParamater == Sale)
            {
                return SearchTransactionType.Sale;
            }
            if (lowerParamater == Purchase)
            {
                return SearchTransactionType.Purchase;
            }
            if (lowerParamater == Journal)
            {
                return SearchTransactionType.Journal;
            }
            if (lowerParamater == Payroll)
            {
                return SearchTransactionType.Payroll;
            }

            return null;
        }
    }
}

