using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Items
{
    public class ItemSummary : BaseModel
    {
        /// <summary>
        /// The Unique Id/key for the resource.
        /// </summary>
        public int? Id { get; set; }        
        /// <summary>
        /// The code for this item.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Code { get; set; }
        /// <summary>
        /// The description for this item.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Description { get; set; }
        /// <summary>
        /// The type of this item. Supported types are 'I' = Inventory item, 'C' = Combo item.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Type { get; set; }
        /// <summary>
        /// Indicates if the item is active.
        /// </summary>
        public bool IsActive { get; set; } 
        /// <summary>
        /// Indicates if the item is inventoried.
        /// </summary>
        public bool IsInventoried { get; set; }
        /// <summary>
        /// The associated asset account id.
        /// </summary>
        public int? AssetAccountId { get; set; }
        /// <summary>
        /// Indicates if the item is sold.
        /// </summary>
        public bool IsSold { get; set; }
        /// <summary>
        /// The associated sale income account id.
        /// </summary>
        public int? SaleIncomeAccountId { get; set; }
        /// <summary>
        /// The associated sale tax code id.
        /// </summary>
        public int? SaleTaxCodeId { get; set; }
        /// <summary>
        /// The associated cost of sale account id.
        /// </summary>
        public int? SaleCoSAccountId { get; set; }
        /// <summary>
        /// Indicates if the item is bought.
        /// </summary>
        public bool IsBought { get; set; }
        /// <summary>
        /// The associated purchase expense account id.
        /// </summary>
        public int? PurchaseExpenseAccountId { get; set; }
        /// <summary>
        /// The associated purchase tax code id.
        /// </summary>
        public int? PurchaseTaxCodeId { get; set; }
        /// <summary>
        /// The minimumm stock level allowed.
        /// </summary>
        public decimal? MinimumStockLevel { get; set; }
        /// <summary>
        /// The current stock on hand. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public decimal? StockOnHand { get; set; }
        /// <summary>
        /// The current value of the item. This data is returned only and cannot be added or updated when issuing a POST or PUT.
        /// </summary>
        public decimal? CurrentValue { get; set; }
        /// <summary>
        /// The primary supplier's contact id (if any).
        /// </summary>
        public int? PrimarySupplierContactId { get; set; }
        /// <summary>
        /// The primary supplier's item code.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string PrimarySupplierItemCode { get; set; }
        /// <summary>
        /// The default re-order quantity when the minimum stock level is reached.
        /// </summary>
        public decimal? DefaultReOrderQuantity { get; set; }
        /// <summary>
        /// The unique id generated after an update that is required to be passed in when next updating this resource to ensure consistency.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string LastUpdatedId { get; set; }        
        /// <summary>
        /// Indicates if this item is visible.
        /// </summary>
        public bool IsVisible { get; set; }
        /// <summary>
        /// Indicate if this is a virtual item.
        /// </summary>
        public bool IsVirtual { get; set; }
        /// <summary>
        /// Indicates the 'virtual type' of the item.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string VType { get; set; }
        /// <summary>
        /// The selling price of the item.
        /// </summary>
        public decimal? SellingPrice { get; set; }
        /// <summary>
        /// Indicates if the selling price includes tax.
        /// </summary>
        public bool IsSellingPriceIncTax { get; set; }
        /// <summary>
        /// The date and time that the item was created in UTC.
        /// </summary>
        public System.DateTime? CreatedDateUtc { get; set; }
        /// <summary>
        /// The date and time that the item was modified in UTC.
        /// </summary>
        public System.DateTime? LastModifiedDateUtc { get; set; }
        /// <summary>
        /// The user id that last modified this item.
        /// </summary>
        public int? LastModifiedBy { get; set; }
        /// <summary>
        /// The buying price of this item.
        /// </summary>
        public decimal? BuyingPrice { get; set; }
        /// <summary>
        /// Indicates if the buying price includes tax.
        /// </summary>
        public bool IsBuyingPriceIncTax { get; set; }
        /// <summary>
        /// Indicates if the item represens a voucher.
        /// </summary>
        public bool IsVoucher { get; set; }
        /// <summary>
        /// If this item is a voucher (IsVoucher = true), this indicates the date and time that the voucher item is valid from.
        /// </summary>
        public DateTime? ValidFrom { get; set; }
        /// <summary>
        /// If this item is a voucher (IsVoucher = true), this indicates the date and time that the voucher item is valid to.
        /// </summary>
        public DateTime? ValidTo { get; set; }
        /// <summary>
        /// The number of items currently on order.
        /// </summary>
        public decimal? OnOrder { get; set; }
        /// <summary>
        /// The number of items currently committed.
        /// </summary>
        public decimal? Committed { get; set; }

        public override string ModelKeyValue()
        {
            return Id.GetValueOrDefault().ToString();
        }
    }

    public class BuildItem : BaseModel
    {
        /// <summary>
        /// The unique Id/key of the item.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The code of the item.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The description of the item.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The quantity on hand of the item.
        /// </summary>
        public decimal Quantity { get; set; }

        public override string ModelKeyValue()
        {
            return Id.ToString();
        }
    }

    public class ItemDetail : ItemSummary
    {
        /// <summary>
        /// Custom notes associated with this item.
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// The items that constitute or form the 'build' of this item.
        /// </summary>
        public List<BuildItem> BuildItems { get; set; }   
    }

    public class ItemSummaryResponse : BaseModel, IApiResponseCollection
    {
        public ItemSummaryResponse()
        {
            Items = new List<ItemSummary>();
        }

        /// <summary>
        /// The list of inventory items.
        /// </summary>
        public List<ItemSummary> Items { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }

        public IEnumerable<BaseModel> ListCollection()
        {
            return Items;
        }
    }
}
