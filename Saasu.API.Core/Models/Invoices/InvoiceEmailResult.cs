namespace Saasu.API.Core.Models.Invoices
{
    public class InvoiceEmailResult : BaseResponseModel
    {
        /// <summary>
        /// The Id of the emailed invoice.
        /// </summary>
        public int InvoiceId { get; set; }

        public override string ModelKeyValue()
        {
            return InvoiceId.ToString();
        }
    }
}
