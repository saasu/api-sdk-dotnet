using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Hypermedia
{
    /// <summary>
    /// Hypermedia link. Contains contextual links to possible next actions related to this resource.
    /// </summary>
    public class Link
    {
        public Link() { }
        public Link(string relValue, string hrefValue, string httpMethod = RelatedLinkHttpMethod.Get, string titleValue = null)
        {
            rel = relValue;
            href = hrefValue;
            method = httpMethod;
            title = titleValue;
        }
        public Link(string relValue, string hrefValue)
        {
            rel = relValue;
            href = hrefValue;

            method = RelatedLinkHttpMethod.Get;
            switch(rel)
            {
                case RelatedLinkType.Delete:
                    method = RelatedLinkHttpMethod.Delete;
                    break;
                case RelatedLinkType.Insert:
                    method = RelatedLinkHttpMethod.Post;
                    break;
                case RelatedLinkType.Update:
                    method = RelatedLinkHttpMethod.Put;
                    break;
            }
        }
        /// <summary>
        /// An identifier stating what relation this link is to the resource eg. detail, next, previous, insert etc
        /// </summary>
        public string rel { get; set; }
        /// <summary>
        /// The uri required to access or perform the action on this related resource
        /// </summary>
        public string href { get; set; }
        /// <summary>
        /// The HTTP method to use when accessing the uri to perform the related action on this resource. Eg. HTTP POST, PUT, GET or DELETE
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// An optional descriptive title for the related link. Where the 'rel' describes how the link is associated to the resource, 
        /// the title will present a human readable content better describing the specific context of this link. Mostly used to describe
        /// a related resource.
        /// </summary>
        public string title { get; set; }
    }
}
