using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Items;
using Saasu.API.Core.Models.Payments;

namespace Saasu.API.Client.Proxies
{
    public class ItemProxy : BaseProxy
    {
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
    }
}
