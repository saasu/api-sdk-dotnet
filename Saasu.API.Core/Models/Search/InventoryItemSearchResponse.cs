using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Search
{
    /// <summary>
    /// Inventory Item that matches the search criteria. 
    /// This resource contains only fields that are indexed for an inventory item, not all the fields that would be found on the Item resource.
    /// </summary>
    public class InventoryItemSearchResponse : BaseModel
    {
        /// <summary>
        /// The type of this entity.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// The Unique Id/key for the Item.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The code for this Item.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Code { get; set; }

        /// <summary>
        /// The description for this Item.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// The primary supplier's item code.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string SupplierItemCode { get; set; }
        
        /// <summary>
        /// Indicates if the Item is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The selling price of the Item.
        /// </summary>
        public decimal SellingPrice { get; set; }
        
        /// <summary>
        /// The buying price of the Item.
        /// </summary>
        public decimal BuyingPrice { get; set; }

        /// <summary>
        /// List of tags associated with this Item.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Tags { get; set; }

        /// <summary>
        /// The current stock on hand.
        /// </summary>
        public decimal StockOnHand { get; set; }

        /// <summary>
        /// The type of this Item. Supported types are 'I' = Inventory item, 'C' = Combo item.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Type { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
