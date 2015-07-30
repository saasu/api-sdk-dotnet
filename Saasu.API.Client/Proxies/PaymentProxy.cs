using System.Net.Http;
using Saasu.API.Client.Framework;
using Saasu.API.Core;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Payments;

namespace Saasu.API.Client.Proxies
{
	public class PaymentProxy : BaseProxy
	{
		public PaymentProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;

		}

        public PaymentProxy(string bearerToken) : base(bearerToken) { }

        public PaymentProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
		}

		public override string RequestPrefix
		{
			get { return ResourceNames.Payment; }
		}

        public ProxyResponse<PaymentTransaction> GetPayment(int paymentId)
        {
            OperationMethod = HttpMethod.Get;
            var uri = base.GetRequestUri(paymentId.ToString());
            return base.GetResponse<PaymentTransaction>(uri);
        }

        public ProxyResponse<InsertPaymentResult> InsertInvoicePayment(PaymentTransaction invoiceDetail)
        {
            OperationMethod = HttpMethod.Post;
            var uri = base.GetRequestUri(null);
            return base.GetResponse<PaymentTransaction, InsertPaymentResult>(uri, invoiceDetail);
        }

        public ProxyResponse<UpdatePaymentResult> UpdateInvoicePayment(int invoiceId, PaymentTransaction invoiceDetail)
        {
            OperationMethod = HttpMethod.Put;            
            var uri = base.GetRequestUri(invoiceId.ToString());
            return base.GetResponse<PaymentTransaction, UpdatePaymentResult>(uri, invoiceDetail);
        }

        public ProxyResponse<BaseResponseModel> DeleteInvoicePayment(int invoiceId)
        {            
            OperationMethod = HttpMethod.Delete;
            var uri = base.GetRequestUri(invoiceId.ToString());
            return base.GetResponse<BaseResponseModel>(uri);
        }        
	}
}
