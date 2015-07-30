namespace Saasu.API.Core.Models.Payments
{
    public class PaymentItem
    {
        /// <summary>
        /// The ID of the invoice paid.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]        
        public int InvoiceTransactionId { get; set; }
        /// <summary>
        /// The amount paid. Has to be equal to or less than the total amount owed.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]                       
        public decimal AmountPaid { get; set; }
    }
}