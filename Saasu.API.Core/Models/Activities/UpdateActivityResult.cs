using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Activities
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateActivityResult : BaseUpdateResultModel
    {
        /// <summary>
        /// The id of the updated Activity.
        /// </summary>
        public int UpdatedActivityId { get; set; }

        /// <summary>
        /// Key Identifier for the model.
        /// </summary>
        public override string ModelKeyValue()
        {
            return UpdatedActivityId.ToString(CultureInfo.InvariantCulture);
        }
    }
}
