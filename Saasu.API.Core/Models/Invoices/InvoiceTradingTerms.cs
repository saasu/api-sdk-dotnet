
namespace Saasu.API.Core.Models.Invoices
{
    /// <summary>
    /// The trading terms of the invoice.
    /// </summary>
	public class InvoiceTradingTerms
	{
        /// <summary>
        /// The trading terms type. Supported values are: Unspecified = 0, DueIn = 1, DueInEomPlusXDays = 2, CashOnDelivery = 3.
        /// </summary>
		public int? Type { get; set; }
        /// <summary>
        /// The trading terms interval.
        /// </summary>
		public int? Interval { get; set; }
        /// <summary>
        /// The type of the trading terms interval as an integer. Supported values are: Unspecified = 0, Day = 1, Week = 2, Month = 3, CashOnDelivery = 4, Year = 5.
        /// </summary>
		public int? IntervalType { get; set; }
        /// <summary>
        /// Trading terms type specifier/enumeration as a desctiptive string. Supported values are: Unspecified, DueIn, DueInEomPlusXDays, CashOnDelivery.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string TypeEnum { get; set; }
        /// <summary>
        /// The type of the trading terms interval as a decriptive string. Supported values are: Unspecified, Day, Week, Month, CashOnDelivery, Year.
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string IntervalTypeEnum { get; set; }
	}
}
