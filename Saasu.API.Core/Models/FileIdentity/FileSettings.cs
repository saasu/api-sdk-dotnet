using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.FileIdentity
{
    /// <summary>
    /// Settings that specify the default behaviour for a file.
    /// </summary>
    public class FileSettings : BaseModel
    {
        /// <summary>
        /// This setting indicates whether sale amounts should include tax by default. Default is true.
        /// </summary>
        public bool SaleAmountsIncludeTax { get; set; }
        /// <summary>
        /// This setting indicates whether purchase amounts should include tax by default. Default is true.
        /// </summary>
        public bool PurchaseAmountsIncludeTax { get; set; }

        public override string ModelKeyValue()
        {
            throw new NotImplementedException();
        }
    }
}
