using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Framework;
using Saasu.API.Core;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.ItemAdjustments;

namespace Saasu.API.Client.Proxies
{
	public class ItemAdjustmentProxy : BaseProxy
	{
		public override string RequestPrefix
		{
			get { return ResourceNames.ItemAdjustment; }
		}

		public ProxyResponse<InsertItemAdjustmentResult> InsertItemAdjustment(AdjustmentDetail detail)
		{
			OperationMethod = HttpMethod.Post;
			var uri = base.GetRequestUri(null);
			return base.GetResponse<AdjustmentDetail, InsertItemAdjustmentResult>(uri, detail);
		}

		public ProxyResponse<UpdateItemAdjustmentResult> UpdateItemAdjustment(AdjustmentDetail detail, int id)
		{
			OperationMethod = HttpMethod.Put;
			var uri = base.GetRequestUri(id.ToString());
			return base.GetResponse<AdjustmentDetail, UpdateItemAdjustmentResult>(uri, detail);
		}

		public ProxyResponse<AdjustmentDetail> GetItemAdjustment(int id)
		{
			OperationMethod = HttpMethod.Get;
			var uri = base.GetRequestUri(id.ToString());
			return base.GetResponse<AdjustmentDetail>(uri);
		}

		public ProxyResponse<BaseResponseModel> DeleteItemAdjustment(int id)
		{
			OperationMethod = HttpMethod.Delete;
			var uri = base.GetRequestUri(id.ToString());
			return base.GetResponse<BaseResponseModel>(uri);
		}

	}
}
