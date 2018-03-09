using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saasu.API.Core.Framework;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Contacts;
using Saasu.API.Core.Models.RequestFiltering;
using System.Net.Http;
using Saasu.API.Client.Framework;

namespace Saasu.API.Client.Proxies
{
	public class ContactsProxy : BaseProxy
	{
		public ContactsProxy()
			: base()
		{
		}

		public ContactsProxy(string bearerToken) : base(bearerToken) { }

		public ContactsProxy(string baseUri, string wsAccessKey, int fileUid)
			: base(baseUri, wsAccessKey, fileUid)
		{
		}

		public override string RequestPrefix
		{
			get { return ResourceNames.Contacts; }
		}

		public ProxyResponse<ContactResponse> GetContacts(int? pageNumber = null, int? pageSize = null, DateTime? lastModifiedFromDate = null, DateTime? lastModifiedToDate = null,
			string givenName = null, string familyName = null, string organisationName = null,
            bool? isActive = null, bool? isCustomer = null, bool? isSupplier = null, bool? isContractor = null, bool? isPartner = null,
			string tags = null, string tagSelection = null, string email = null, string contactId = null, string companyName = null, int? companyId = null)
		{

			OperationMethod = HttpMethod.Get;
			var queryArgs = new StringBuilder();

			if (lastModifiedFromDate.HasValue && lastModifiedToDate.HasValue)
			{
				AppendQueryArg(queryArgs, "lastModifiedFromDate", lastModifiedFromDate.Value.ToString("o"));
				AppendQueryArg(queryArgs, "lastModifiedToDate", lastModifiedToDate.Value.ToString("o"));
			}
			if (!string.IsNullOrWhiteSpace(givenName))
			{
				AppendQueryArg(queryArgs, "givenName", givenName);
			}
			if (!string.IsNullOrWhiteSpace(familyName))
			{
				AppendQueryArg(queryArgs, "familyName", familyName);
			}
			if (!string.IsNullOrWhiteSpace(organisationName))
			{
				AppendQueryArg(queryArgs, "organisationName", organisationName);
			}
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                AppendQueryArg(queryArgs, "companyName", companyName);
            }
            if (companyId != null)
            {
                AppendQueryArg(queryArgs, "companyId", companyId.ToString());
            }
            if (isActive != null)
			{
				AppendQueryArg(queryArgs, "isActive", (bool)isActive ? "1" : "0");
			}
			if (isCustomer != null)
			{
				AppendQueryArg(queryArgs, "isCustomer", (bool)isCustomer ? "1" : "0");
			}
			if (isSupplier != null)
			{
				AppendQueryArg(queryArgs, "isSupplier", (bool)isSupplier ? "1" : "0");
			}
			if (isContractor != null)
			{
				AppendQueryArg(queryArgs, "isContractor", (bool)isContractor ? "1" : "0");
			}
			if (isPartner != null)
			{
				AppendQueryArg(queryArgs, "isPartner", (bool)isPartner ? "1" : "0");
			}
			if (!string.IsNullOrWhiteSpace(tags))
			{
				AppendQueryArg(queryArgs, "tags", tags);
			}
			if (!string.IsNullOrWhiteSpace(tagSelection))
			{
				AppendQueryArg(queryArgs, "tagSelection", tagSelection);
			}
			if (!string.IsNullOrWhiteSpace(email))
			{
				AppendQueryArg(queryArgs, "email", email);
			}
			if (!string.IsNullOrWhiteSpace(contactId))
			{
				AppendQueryArg(queryArgs, "contactId", contactId);
			}
			bool inclPageNumber;
			bool inclPageSize;

			base.GetPaging(queryArgs, pageNumber, pageSize, out inclPageNumber, out inclPageSize);

			var uri = base.GetRequestUri(queryArgs.ToString(), inclDefaultPageNumber: inclPageNumber, inclDefaultPageSize: inclPageSize);
			return base.GetResponse<ContactResponse>(uri);
		}
	}
}
