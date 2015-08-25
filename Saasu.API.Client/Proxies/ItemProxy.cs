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
using Saasu.API.Core.Models.Items;
using Saasu.API.Core.Models.Payments;

namespace Saasu.API.Client.Proxies
{
    public class ItemProxy : BaseProxy
    {
        public ItemProxy()
			: base()
		{
			ContentType = RequestContentType.ApplicationJson;
		}

        public ItemProxy(string bearerToken) : base(bearerToken) { }

        public ItemProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
		}

        public ProxyResponse<ItemDetail> GetItem(int itemId)
        {
            OperationMethod = HttpMethod.Get;
            var uri = base.GetRequestUri(itemId.ToString());
            return base.GetResponse<ItemDetail>(uri);
        }

        public override string RequestPrefix
        {
            get { return ResourceNames.Item; }
        }

        public ProxyResponse<InsertItemResult> InsertItem(ItemDetail item)
        {
            OperationMethod = HttpMethod.Post;
            var uri = base.GetRequestUri(null);
            return base.GetResponse<ItemDetail, InsertItemResult>(uri, item);
        }

        public ProxyResponse<UpdateItemResult> UpdateItem(ItemDetail item, int itemId)
        {
            OperationMethod = HttpMethod.Put;
            var uri = base.GetRequestUri(itemId.ToString());
            return base.GetResponse<ItemDetail, UpdateItemResult>(uri, item);
        }

        public ProxyResponse<BaseResponseModel> DeleteItem(int itemId)
        {
            OperationMethod = HttpMethod.Delete;
            var uri = base.GetRequestUri(itemId.ToString());
            return base.GetResponse<BaseResponseModel>(uri);
        }
    }
}
