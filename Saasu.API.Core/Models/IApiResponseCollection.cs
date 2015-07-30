using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Models
{
    /// <summary>
    /// This interface is used to mark an Api model as that which returns a collection of objects
    /// and provides a standard method by which 
    /// </summary>
    public interface IApiResponseCollection
    {
        /// <summary>
        /// Returns the collection as a generic list of BaseModel data represented by this
        /// object. This allows the collection property to be named specifically for serialisation
        /// purposes and also be easily accessible for consumers.
        /// </summary>
        /// <returns></returns>
        IEnumerable<BaseModel> ListCollection();
    }
}
