using Saasu.API.Client.Framework;
using Saasu.API.Core;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Attachments;
using Saasu.API.Core.Models.Invoices;
using System.Net.Http;
using System.Text;

namespace Saasu.API.Client.Proxies
{
    public class InvoiceProxy : BaseProxy
    {
        public string _requestPrefix = ResourceNames.Invoice;

        public InvoiceProxy()
            : base()
        {
            ContentType = RequestContentType.ApplicationJson;

        }

        public InvoiceProxy(string bearerToken) : base(bearerToken) { }

        public InvoiceProxy(string baseUri, string wsAccessKey, int fileUid)
            : base(baseUri, wsAccessKey, fileUid)
        {
        }

        public override string RequestPrefix
        {
            get { return _requestPrefix; }
        }

        public ProxyResponse<InvoiceTransactionDetail> GetInvoice(int invoiceId)
        {
            OperationMethod = HttpMethod.Get;
            _requestPrefix = ResourceNames.Invoice;
            var uri = base.GetRequestUri(invoiceId.ToString());
            return base.GetResponse<InvoiceTransactionDetail>(uri);
        }

        public ProxyResponse<InsertAttachmentResult> AddAttachment(FileAttachment attachment)
        {
            OperationMethod = HttpMethod.Post;
            _requestPrefix = ResourceNames.InvoiceAttachment;
            var uri = base.GetRequestUri(string.Empty, HttpMethod.Post);
            return base.GetResponse<FileAttachment, InsertAttachmentResult>(uri, attachment);
        }

        public ProxyResponse<FileAttachmentListResponse> GetAllAttachmentsInfo(int invoiceId)
        {
            OperationMethod = HttpMethod.Get;
            PagingEnabled = false;
            ContentType = RequestContentType.ApplicationJson;
            _requestPrefix = ResourceNames.InvoiceAttachments;
            var uri = base.GetRequestUri(string.Format("{0}", invoiceId));
            return base.GetResponse<FileAttachmentListResponse>(uri);
        }

        public ProxyResponse<FileAttachment> GetAttachment(int attachmentId)
        {
            OperationMethod = HttpMethod.Get;
            PagingEnabled = false;
            ContentType = RequestContentType.ApplicationJson;
            _requestPrefix = ResourceNames.InvoiceAttachment;
            var uri = base.GetRequestUri(string.Format("{0}", attachmentId));
            return base.GetResponse<FileAttachment>(uri);
        }

        public ProxyResponse<BaseResponseModel> DeleteAttachment(int attachmentId)
        {
            OperationMethod = HttpMethod.Delete;
            PagingEnabled = false;
            _requestPrefix = ResourceNames.InvoiceAttachment;
            var uri = base.GetRequestUri(string.Format("{0}", attachmentId));
            return base.GetResponse<BaseResponseModel>(uri);
        }

        public ProxyResponse<UpdateInvoiceResult> UpdateInvoice(int invoiceId, InvoiceTransactionDetail invoiceDetail)
        {
            OperationMethod = HttpMethod.Put;
            _requestPrefix = ResourceNames.Invoice;
            var uri = base.GetRequestUri(invoiceId.ToString());
            return base.GetResponse<InvoiceTransactionDetail, UpdateInvoiceResult>(uri, invoiceDetail);
        }

        public ProxyResponse<BaseResponseModel> DeleteInvoice(int invoiceId)
        {
            _requestPrefix = ResourceNames.Invoice;
            OperationMethod = HttpMethod.Delete;
            var uri = base.GetRequestUri(invoiceId.ToString());
            return base.GetResponse<BaseResponseModel>(uri);
        }

        public ProxyResponse<InsertInvoiceResult> InsertInvoice(InvoiceTransactionDetail invoiceDetail)
        {
            _requestPrefix = ResourceNames.Invoice;
            OperationMethod = HttpMethod.Post;
            var uri = base.GetRequestUri(null);
            return base.GetResponse<InvoiceTransactionDetail, InsertInvoiceResult>(uri, invoiceDetail);
        }

        public ProxyResponse<InvoiceEmailResult> EmailInvoice(int invoiceId)
        {
            _requestPrefix = ResourceNames.Invoice;
            RequestPostfix = ApiConstants.EmailInvoiceToContactUrlPath;
            OperationMethod = HttpMethod.Post;
            PagingEnabled = false;
            var uri = base.GetRequestUri(invoiceId.ToString());
            return base.GetResponse<InvoiceEmailResult>(uri);
        }

        public ProxyResponse<byte[]> GenerateInvoicePdf(int invoiceId, int? templateId = null)
        {
            _requestPrefix = ResourceNames.Invoice;
            RequestPostfix = ApiConstants.GenerateInvoicePdfPath;
            OperationMethod = HttpMethod.Get;
            PagingEnabled = false;

            var queryArgs = new StringBuilder();

            if (templateId.HasValue)
            {
                AppendQueryArg(queryArgs, ApiConstants.FilterTemplateId, templateId.Value.ToString());
            }

            var uri = base.GetRequestUri(invoiceId.ToString());
            return base.GetBinaryResponse(uri);
        }

    }
}
