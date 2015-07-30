using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.TaxCode
{
    /// <summary>
    /// Tax code resource.
    /// </summary>
    public class TaxCodeDetail : TaxCodeSummary
    {       
        /// <summary>
        /// Custom notes associated with this tax code (if any).
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public string Notes { get; set; }                
    }
}
