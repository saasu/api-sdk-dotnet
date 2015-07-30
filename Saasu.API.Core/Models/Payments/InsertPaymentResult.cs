using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Payments
{
    public class InsertPaymentResult : UpdatePaymentResult 
    {
        /// <summary>
        /// The Id/Key of the payment that was created/inserted.
        /// </summary>
        public int InsertedEntityId { get; set; }
       
        public override string ModelKeyValue()
        {
            return InsertedEntityId.ToString();
        }
    }
}
