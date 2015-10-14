using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Invoices
{
    public class InvoiceTransactionLineItem : BaseModel
    {
        /// <summary>
        /// The Id/Key of the line item.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Description of the line item.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Description { get; set; }
        /// <summary>
        /// The associated account id of the line item.
        /// </summary>
        public int? AccountId { get; set; }
        /// <summary>
        /// The tax code associated with this line item.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string TaxCode { get; set; }
        /// <summary>
        /// Total amount of this line item.
        /// </summary>
        public decimal? TotalAmount{ get; set; }
        /// <summary>
        /// Quantity of items for this line item.
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// Unit price of the item.
        /// </summary>
        public decimal? UnitPrice { get; set; }
        /// <summary>
        /// Percentage discount to apply to this line item.
        /// </summary>
        public decimal? PercentageDiscount { get; set; }
        /// <summary>
        /// The Id of the inventory item for this line item.
        /// </summary>
        public int? InventoryId { get; set; }

        /// <summary>
        /// The Inventory Item code for this line item. Not required when inserting a line item.
        /// </summary>
        public string ItemCode { get; set; }
 
        private List<string> _tagList;
        /// <summary>
        /// List of tags associated with this line item.
        /// </summary>
        public List<string> Tags
        {
            get { return _tagList ?? (_tagList = new List<string>()); }
            set { _tagList = value; }
        }

        private List<ItemAttribute> _attributes = new List<ItemAttribute>();
        /// <summary>
        /// List of attributes associated with this line item.
        /// </summary>
        public List<ItemAttribute> Attributes { get { return _attributes;} set { _attributes = value;} }


        public override string ModelKeyValue()
        {
            return Id.ToString();
        }
    }

    public class ItemAttribute
    {
        /// <summary>
        /// The Id/Key of the attribute.
        /// </summary>
        public int AttributeId {get; set;}
        /// <summary>
        /// Name of the attribute.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Name { get; set; }
        /// <summary>
        /// Value of the attribute.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Value { get; set; }
    }
}
