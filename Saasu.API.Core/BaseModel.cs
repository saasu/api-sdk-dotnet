using Saasu.API.Core.Hypermedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Saasu.API.Core
{
    public interface IModel
    {
        /// <summary>
        /// Hypermedia links. Contains contextual links to possible next actions related to this resource.
        /// Only present in responses. This data is not to be sent to the server.
        /// </summary>
        List<Link> _links { get; set; }

        /// <summary>
        /// This method provides a way to return the key or Id value of the underlying model without
        /// knowing its property name.
        /// </summary>
        /// <returns></returns>
        string ModelKeyValue();

    }

    public abstract class BaseModel : IModel
    {
        public BaseModel()
        {
            _links = new List<Link>();
            //links.Add(new SelfLink("http://host/resource/2"));
            //links.Add(new Link{ href="http://host/resource/2",rel="self"});
            //links.Add(new Link { href = "http://host/resource/2", rel="next" });
            //next=new Link { href = "http://host/resource/3", rel = "self" };
        }

        /// <summary>
        /// Hypermedia links. Contains contextual links to possible next actions related to this resource.
        /// Only present in responses. This data is not to be sent to the server.
        /// </summary>
        public List<Link> _links {get; set;}

        public abstract string ModelKeyValue();
    }

    public class BaseResponseModel : BaseModel
    {
        /// <summary>
        /// Status message from the response (if any).
        /// </summary>
        public string StatusMessage { get; set; }
        public override string ModelKeyValue()
        {
            return string.Empty;
        }
    }
}
