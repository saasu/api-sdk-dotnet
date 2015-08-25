using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Search
{
    /// <summary>
    /// Search results, includes Transactions, Contacts and Inventory Items.
    /// </summary>
    public class SearchResponse : BaseModel
    {
        public SearchResponse() 
        {
            Transactions = new List<TransactionSearchResponse>();
            InventoryItems = new List<InventoryItemSearchResponse>();
            Contacts = new List<ContactSearchResponse>();
        }

        /// <summary>
        /// List of indexed transactions returned for the search criteria.
        /// </summary>
        public List<TransactionSearchResponse> Transactions { get; set; }
        /// <summary>
        /// Total number of transactions found for the search criteria.
        /// </summary>
        public long TotalTransactionsFound { get; set; }

        /// <summary>
        /// List of indexed contacts returned for the search criteria.
        /// </summary>
        public List<ContactSearchResponse> Contacts { get; set; }

        /// <summary>
        /// Total number of contacts found for the search criteria.
        /// </summary>
        public long TotalContactsFound { get; set; }

        /// <summary>
        /// List of indexed inventory items returned for the search criteria.
        /// </summary>
        public List<InventoryItemSearchResponse> InventoryItems { get; set; }

        /// <summary>
        /// Total number of inventory items found for the search criteria.
        /// </summary>
        public long TotalInventoryItemsFound { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
