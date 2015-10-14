namespace Saasu.API.Core.Models.Search
{
    /// <summary>
    /// The search scope determines which types will be included in the search.
    /// </summary>
    public enum SearchScope
    {
        /// <summary>
        /// Search Transactions, Contacts and Inventory Items.
        /// </summary>
        All = 0,
        /// <summary>
        /// Search Transactions only.
        /// </summary>
        Transactions = 1,
        /// <summary>
        /// Search Contacts only.
        /// </summary>
        Contacts = 2,
        /// <summary>
        /// Search Inventory Items only.
        /// </summary>
        InventoryItems = 3
    }

    public static class SearchScopeExtensions
    {
        const string All = "all";
        const string Transactions = "transactions";
        const string Contacts = "contacts";
        const string InventoryItems = "inventoryitems";
        public static SearchScope ToSearchScope(this string searchScopeParameter)
        {
            if (string.IsNullOrWhiteSpace(searchScopeParameter))
            {
                return SearchScope.All;
            }
            var lowerParamater = searchScopeParameter.ToLowerInvariant();
            if (lowerParamater == All)
            {
                return SearchScope.All;
            }
            if (lowerParamater == Transactions)
            {
                return SearchScope.Transactions;
            }
            if (lowerParamater == Contacts)
            {
                return SearchScope.Contacts;
            }
            if (lowerParamater == InventoryItems)
            {
                return SearchScope.InventoryItems;
            }
            return SearchScope.All;
        }
    }
}
