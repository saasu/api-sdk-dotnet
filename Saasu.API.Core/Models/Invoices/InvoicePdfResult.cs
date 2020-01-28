namespace Saasu.API.Core.Models.Invoices
{
    public class InvoicePdfResult : BaseResponseModel
    {
        public int InvoiceId { get; set; }

        public byte[] Content { get; set; }


    }
}
