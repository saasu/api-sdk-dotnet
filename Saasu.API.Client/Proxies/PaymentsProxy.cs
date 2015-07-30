using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Invoices;
using System.Net.Http;
using Saasu.API.Core.Models.Payments;
using Saasu.API.Core.Models.RequestFiltering;
using Saasu.API.Core.Framework;

namespace Saasu.API.Client.Proxies
{
	public class PaymentsProxy : BaseProxy
	{
		public PaymentsProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;

		}

        public PaymentsProxy(string bearerToken) : base(bearerToken) { }

        public PaymentsProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
			ContentType = RequestContentType.ApplicationXml;
		}

		public override string RequestPrefix
		{
			get { return ResourceNames.Payments; }
		}

        public ProxyResponse<PaymentTransactionSummaryResponse> GetPayments(int? pageNumber, int? pageSize, int? forInvoiceId,
             int? paymentAccountId, string tranType, string sortString, DateTime? dateFrom, DateTime? dateTo, DateTime? dateClearedFrom, DateTime? dateClearedTo,
            DateTime? lastModifiedDateUtcFrom, DateTime? lastModifiedDateUtcTo)
        {
            OperationMethod = HttpMethod.Get;
            var queryArgs = new StringBuilder();

            if (dateFrom.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterPaymentDateFrom, dateFrom.Value.ToString("o"));
            }
            if (dateTo.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterPaymentDateTo, dateTo.Value.ToString("o"));
            }
            if (dateClearedFrom.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterDateClearedFromDate, dateClearedFrom.Value.ToString("o"));
            }
            if (dateClearedTo.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterDateClearedToDate, dateClearedTo.Value.ToString("o"));
            }
            if (lastModifiedDateUtcFrom.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterLastModifiedFromDate, lastModifiedDateUtcFrom.Value.ToString("o"));
            }
            if (lastModifiedDateUtcTo.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterLastModifiedToDate, lastModifiedDateUtcTo.Value.ToString("o"));
            }
            if (forInvoiceId != null && forInvoiceId > 0)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterPaymentsForInvoiceId, forInvoiceId.ToString());                
            }
            if (paymentAccountId != null && paymentAccountId > 0)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterPaymentAccountId, paymentAccountId.ToString());
            }                      
            if (!string.IsNullOrWhiteSpace(tranType))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterTransactionTypePayments, tranType);
            }
            if (!string.IsNullOrWhiteSpace(sortString))
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterResultsSortString, sortString);
            }

			bool inclPageNumber;
			bool inclPageSize;

			base.GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

            var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
            return base.GetResponse<PaymentTransactionSummaryResponse>(uri);
        }        
	}
}
