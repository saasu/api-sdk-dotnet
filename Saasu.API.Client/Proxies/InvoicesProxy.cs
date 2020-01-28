using System;
using System.Collections.Generic;
using System.Text;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Invoices;
using System.Net.Http;
using Saasu.API.Core.Models.RequestFiltering;
using Saasu.API.Core.Framework;

namespace Saasu.API.Client.Proxies
{
	public class InvoicesProxy : BaseProxy
	{
		public InvoicesProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;

		}

        public InvoicesProxy(string bearerToken) : base(bearerToken) { }

		public InvoicesProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
			ContentType = RequestContentType.ApplicationXml;
		}

		public override string RequestPrefix
		{
			get { return ResourceNames.Invoices; }
		}

		public ProxyResponse<InvoiceTransactionSummaryResponse> GetInvoices(int? pageNumber = null, int? pageSize = null, DateTime? invoiceFromDate = null, DateTime? invoiceToDate = null, 
			DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null, string invoiceNumber = null, string purchaseOrderNumber = null, string transactionType = null, int? paymentStatus = null,
			int? billingContactId = null, string invoiceStatus = null, string tags = null, string tagFilterType = null)
        {
            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();
          
			if (invoiceFromDate.HasValue && invoiceToDate.HasValue)
			{
				AppendQueryArg(queryArgs,ApiConstants.FilterInvoiceFromDate, invoiceFromDate.Value.ToString("o"));
				AppendQueryArg(queryArgs, ApiConstants.FilterInvoiceToDate, invoiceToDate.Value.ToString("o"));
			}
            if (lastModifiedFromDate.HasValue && lastModifiedToDate.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterLastModifiedFromDate, lastModifiedFromDate.Value.ToString("o"));
				AppendQueryArg(queryArgs, ApiConstants.FilterLastModifiedToDate, lastModifiedToDate.Value.ToString("o"));
            }
            if (!string.IsNullOrWhiteSpace(invoiceNumber))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterInvoiceNumber, invoiceNumber);
            }
            if (!string.IsNullOrWhiteSpace(purchaseOrderNumber))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterPurchaseOrderNumber, purchaseOrderNumber);
            }
            if (!string.IsNullOrWhiteSpace(transactionType))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterTransactionTypeInvoices, transactionType);
            }
			if (!string.IsNullOrWhiteSpace(invoiceStatus))
			{
                AppendQueryArg(queryArgs, ApiConstants.FilterPaymentStatus, paymentStatus.ToString());
			}
			if (billingContactId != null && billingContactId > 0)
			{
                AppendQueryArg(queryArgs, ApiConstants.FilterBillingContactId, billingContactId.ToString());
			}
			if (!string.IsNullOrWhiteSpace(invoiceStatus))
			{
                AppendQueryArg(queryArgs, ApiConstants.FilterInvoiceStatus, invoiceStatus);
			}
			if (!string.IsNullOrWhiteSpace(tags))
			{
                AppendQueryArg(queryArgs, ApiConstants.FilterTags, tags);
			}
			if (!string.IsNullOrWhiteSpace(tagFilterType))
			{
                AppendQueryArg(queryArgs, ApiConstants.FilterTagFilterType, tagFilterType);
			}

			bool inclPageNumber;
			bool inclPageSize;

			base.GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);
			
			var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
            return base.GetResponse<InvoiceTransactionSummaryResponse>(uri);
        }
	}
}
