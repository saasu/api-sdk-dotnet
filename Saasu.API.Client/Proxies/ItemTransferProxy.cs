using Saasu.API.Client.Framework;
using Saasu.API.Core;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.ItemTransfers;
using System.Net.Http;

namespace Saasu.API.Client.Proxies
{
    public class ItemTransferProxy : BaseProxy
    {
        public override string RequestPrefix
        {
            get { return ResourceNames.ItemTransfer; }
        }

        public ProxyResponse<InsertItemTransferResult> InsertItemTransfer(TransferDetail detail)
        {
            OperationMethod = HttpMethod.Post;
            var uri = base.GetRequestUri(null);
            return base.GetResponse<TransferDetail, InsertItemTransferResult>(uri, detail);
        }

        public ProxyResponse<UpdateItemTransferResult> UpdateItemTransfer(TransferDetail detail, int id)
        {
            OperationMethod = HttpMethod.Put;
            var uri = base.GetRequestUri(id.ToString());
            return base.GetResponse<TransferDetail, UpdateItemTransferResult>(uri, detail);
        }

        public ProxyResponse<TransferDetail> GetItemTransfer(int id)
        {
            OperationMethod = HttpMethod.Get;
            var uri = base.GetRequestUri(id.ToString());
            return base.GetResponse<TransferDetail>(uri);
        }

        public ProxyResponse<BaseResponseModel> DeleteItemTransfer(int id)
        {
            OperationMethod = HttpMethod.Delete;
            var uri = base.GetRequestUri(id.ToString());
            return base.GetResponse<BaseResponseModel>(uri);
        }
    }
}
