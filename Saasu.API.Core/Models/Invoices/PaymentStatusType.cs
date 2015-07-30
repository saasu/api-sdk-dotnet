using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Invoices
{
    public enum PaymentStatusType
    {
        All,
        Unpaid,
        Paid,
    }

    public static class PaymentStatusTypeExtensions
    {
        const string StatusAll = "a";
        const string StatusPaid = "p";
        const string StatusUnpaid = "u";
        public static string ToQueryParameter(this PaymentStatusType paymentStatus)
        {
            switch (paymentStatus)
            {
                case PaymentStatusType.All:
                    return StatusAll.ToUpperInvariant();
                    break;
                case PaymentStatusType.Paid:
                    return StatusPaid.ToUpperInvariant();
                    break;
                case PaymentStatusType.Unpaid:
                    return StatusUnpaid.ToUpperInvariant();
            }
            return StatusUnpaid.ToUpperInvariant();
        }

        public static PaymentStatusType ToPaymentStatusType(this string paymentStatusParameter)
        {
            if (string.IsNullOrWhiteSpace(paymentStatusParameter))
            {
                return PaymentStatusType.Unpaid;
            }
            var lowerParamater = paymentStatusParameter.ToLowerInvariant();
            if (lowerParamater == StatusAll)
            {
                return PaymentStatusType.All;
            }
            if (lowerParamater == StatusPaid)
            {
                return PaymentStatusType.Paid;
            }
            return PaymentStatusType.Unpaid;
        }
    }
}
