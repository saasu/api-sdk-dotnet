using Saasu.API.Client.Framework;
using Saasu.API.Core;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Activities;
using System.Net.Http;

namespace Saasu.API.Client.Proxies
{
    public class ActivityProxy : BaseProxy
    {
        public ActivityProxy()
			: base()
		{
            ContentType = RequestContentType.ApplicationJson;

        }

        public ActivityProxy(string bearerToken) : base(bearerToken) { }

        public ActivityProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
        }

        public override string RequestPrefix
        {
            get { return ResourceNames.Activity; }
        }

        public ProxyResponse<ActivityDetail> GetActivity(int activityId)
        {
            OperationMethod = HttpMethod.Get;
            var uri = base.GetRequestUri(activityId.ToString());
            return base.GetResponse<ActivityDetail>(uri);
        }

        public ProxyResponse<InsertActivityResult> InsertActivity(ActivityDetail activityDetail)
        {
            OperationMethod = HttpMethod.Post;
            var uri = base.GetRequestUri(null);
            return base.GetResponse<ActivityDetail, InsertActivityResult>(uri, activityDetail);
        }

        public ProxyResponse<UpdateActivityResult> UpdateActivity(int activityId, ActivityDetail activityDetail)
        {
            OperationMethod = HttpMethod.Put;
            var uri = base.GetRequestUri(activityId.ToString());
            return base.GetResponse<ActivityDetail, UpdateActivityResult>(uri, activityDetail);
        }

        public ProxyResponse<BaseResponseModel> DeleteActivity(int activityId)
        {
            OperationMethod = HttpMethod.Delete;
            var uri = base.GetRequestUri(activityId.ToString());
            return base.GetResponse<BaseResponseModel>(uri);
        }
    }
}
