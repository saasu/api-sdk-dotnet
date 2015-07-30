using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saasu.API.Client.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Contacts;
using System.Net.Http;
using Saasu.API.Core.Models.Invoices;
using Saasu.API.Core.Framework;
using Saasu.API.Core;

namespace Saasu.API.Client.Proxies
{
    public class ContactProxy : BaseProxy
    {
        public ContactProxy()
            : base()
        {
            ContentType = RequestContentType.ApplicationJson;
        }

        public ContactProxy(string bearerToken) : base(bearerToken) { }

        public ContactProxy(string baseUri, string wsAccessKey, int fileUid)
            : base(baseUri, wsAccessKey, fileUid)
        {
        }

        public override string RequestPrefix
        {
            get { return ResourceNames.Contact; }
        }

        public ProxyResponse<Contact> GetContact(int id)
        {
            OperationMethod = HttpMethod.Get;
            var uri = base.GetRequestUri(id.ToString());
            return base.GetResponse<Contact>(uri);
        }

        public ProxyResponse<InsertContactResult> InsertContact(Contact contact)
        {
            OperationMethod = HttpMethod.Post;
            var uri = base.GetRequestUri(null);
            return base.GetResponse<Contact, InsertContactResult>(uri, contact);
        }

        public ProxyResponse<UpdateContactResult> UpdateContact(Contact contact, int contactId)
        {
            OperationMethod = HttpMethod.Put;
            var uri = base.GetRequestUri(contactId.ToString());
            return base.GetResponse<Contact, UpdateContactResult>(uri, contact);
        }

        public ProxyResponse<BaseResponseModel> DeleteContact(int contactId)
        {
            OperationMethod = HttpMethod.Delete;
            var uri = base.GetRequestUri(contactId.ToString());
            return base.GetResponse<BaseResponseModel>(uri);
        }
    }
}
