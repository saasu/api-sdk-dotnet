using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Invoices
{
    public enum InvoiceStatusType
    {
        Unspecified,
        Invoice,
        Quote,
        Order
    }

    public static class InvoiceStatusTypeExtensions
    {
        const string StatusQuote = "q";
        const string StatusInvoice = "i";
        const string StatusOrder = "o";
        public static string ToQueryParameter(this InvoiceStatusType invoiceStatus)
        {
            switch (invoiceStatus)
            {
                case InvoiceStatusType.Invoice:
                    return StatusInvoice.ToUpperInvariant();
                    break;
                case InvoiceStatusType.Order:
                    return StatusOrder.ToUpperInvariant();
                    break;
                case InvoiceStatusType.Quote:
                    return StatusQuote.ToUpperInvariant();
            }
            return null;
        }

        public static InvoiceStatusType ToInvoiceStatusType(this string invoiceStatusParameter)
        {
            if (string.IsNullOrWhiteSpace(invoiceStatusParameter))
            {
                return InvoiceStatusType.Unspecified;
            }
            var lowerParamater = invoiceStatusParameter.ToLowerInvariant();
            if (lowerParamater == StatusInvoice)
            {
                return InvoiceStatusType.Invoice;
            }
            if (lowerParamater == StatusOrder)
            {
                return InvoiceStatusType.Order;
            }
            if (lowerParamater == StatusQuote)
            {
                return InvoiceStatusType.Quote;
            }
            return InvoiceStatusType.Unspecified;
        }
    }

}
