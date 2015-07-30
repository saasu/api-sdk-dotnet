using System.Collections.Generic;

namespace Saasu.API.Core.Models.TaxCode
{
    /// <summary>
    /// The list of tax codes.
    /// </summary>
    public class TaxCodesResponse : BaseModel, IApiResponseCollection
    {
        public TaxCodesResponse()
        {
            TaxCodes = new List<TaxCodeSummary>();
        }

        /// <summary>
        /// The list of tax codes.
        /// </summary>
        public List<TaxCodeSummary> TaxCodes { get; set; } 

        public override string ModelKeyValue()
        {
            return string.Empty;
        }

        public IEnumerable<BaseModel> ListCollection()
        {
            return TaxCodes;
        }
    }
}