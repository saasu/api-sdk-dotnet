using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Saasu.API.Core.Serialisation
{
    public class AllowAllContractResolver : DataContractResolver
    {
        public override bool TryResolveType(Type dataContractType, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if (!knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(dataContractType.FullName);
                typeNamespace = dictionary.Add(dataContractType.Assembly.FullName);
            }
            return true;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null) ?? Type.GetType(typeName + ", " + typeNamespace);
        }
    }
}
