using Saasu.API.Client.Framework;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.ContactAggregates;
using System.Net.Http;

namespace Saasu.API.Client.Proxies
{
    public class ContactAggregateProxy : BaseProxy
    {
        public ContactAggregateProxy() : base()
        {
            ContentType = RequestContentType.ApplicationJson;
        }

        public ContactAggregateProxy(string bearerToken) : base(bearerToken) { }

        public ContactAggregateProxy(string baseUri, string wsAccessKey, int fileUid)
            : base(baseUri, wsAccessKey, fileUid)
        {
        }

        public override string RequestPrefix
        {
            get
            {
                return ResourceNames.ContactAggregate;
            }
        }

        public ProxyResponse<ContactAggregate> GetContactAggregate(int id)
        {
            OperationMethod = HttpMethod.Get;
            var uri = base.GetRequestUri(id.ToString());
            return base.GetResponse<ContactAggregate>(uri);
        }

        public ProxyResponse<InsertContactAggregateResult> InsertContactAggregate(ContactAggregate contactAggregate)
        {
            OperationMethod = HttpMethod.Post;
            var uri = base.GetRequestUri(null);
            return base.GetResponse<ContactAggregate, InsertContactAggregateResult>(uri, contactAggregate);
        }

        public ProxyResponse<UpdateContactAggregateResult> UpdateContactAggregate(ContactAggregate contactAggregate, int contactId)
        {
            OperationMethod = HttpMethod.Put;
            var uri = base.GetRequestUri(contactId.ToString());
            return base.GetResponse<ContactAggregate, UpdateContactAggregateResult>(uri, contactAggregate);
        }
    }
}
