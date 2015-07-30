using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Hypermedia
{
    public class RelatedLinkType
    {
        public const string Self = "self";
        public const string Next = "next";
        public const string Previous = "previous";
        public const string Detail = "detail";
        public const string List = "list";
        public const string Delete = "delete";
        public const string Insert = "insert";
		public const string Update = "update";
        public const string NextPrevious = "paging";
        public const string RelatedResource = "related";
        public const string DeprecatedApiMethod = "deprecated";
    }

    public class RelatedLinkHttpMethod
    {
        public const string Get = "GET";
        public const string Post = "POST";
        public const string Head = "HEAD";
        public const string Put = "PUT";
        public const string Delete = "DELETE";
    }

    public class RelatedLinkTypeUri
    {
        public const string ApiHelpPage = "~/api.saasu.com/Help";
    }
}
