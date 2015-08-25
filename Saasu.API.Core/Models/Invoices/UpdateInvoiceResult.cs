using System;

namespace Saasu.API.Core.Models.Invoices
{
    public class UpdateInvoiceResult : BaseUpdateResultModel
	{
        /// <summary>
        /// Indicates whether this was sent to the contact.
        /// </summary>
		public bool SentToContact { get; set; }

        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
