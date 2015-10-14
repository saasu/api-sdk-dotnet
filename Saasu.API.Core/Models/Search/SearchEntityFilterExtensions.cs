namespace Saasu.API.Core.Models.Search
{
    public enum SearchEntityType
    {
        Sale,
        Purchase,
        Journal,
        Payroll

    }
    public static class SearchEntityFilterExtensions
    {
        const string Sale = "transactions.sale";
        const string Purchase = "transactions.purchase";
        const string Journal = "transactions.journal";
        const string Payroll = "transactions.payroll";
        public static SearchEntityType? ToSearchEntityType(this string searchEntityTypeParameter)
        {
            var lowerParamater = searchEntityTypeParameter.ToLowerInvariant();
            if (lowerParamater == Sale)
            {
                return SearchEntityType.Sale;
            }
            if (lowerParamater == Purchase)
            {
                return SearchEntityType.Purchase;
            }
            if (lowerParamater == Journal)
            {
                return SearchEntityType.Journal;
            }
            if (lowerParamater == Payroll)
            {
                return SearchEntityType.Payroll;
            }

            return null;
        }
    }
}

