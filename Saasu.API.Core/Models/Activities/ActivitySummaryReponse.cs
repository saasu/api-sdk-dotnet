using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models.Activities
{
    /// <summary>
    /// Reponse model for call to get multiple activites.
    /// </summary>
    public class ActivitySummaryReponse: BaseModel, IApiResponseCollection
    {
        /// <summary>
        /// Response object for mulitple activity search
        /// </summary>
        public ActivitySummaryReponse()
        {
            Activities = new List<ActivitySummary>();
        }
        /// <summary>
        /// List of activities.
        /// </summary>
		public List<ActivitySummary> Activities { get; set; }

        /// <summary>
        /// key Identifier for the model
        /// </summary>
        public override string ModelKeyValue()
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns model's list of activites. Needed for hypermedia.
        /// </summary>
        public IEnumerable<BaseModel> ListCollection()
        {
            return Activities;
        }
    }
}
