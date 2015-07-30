using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saasu.API.Client.Framework
{
	public enum RequestContentType
	{
		ApplicationJson,
		ApplicationXml
	}

	public static class RequestContentTypeExtensions
	{
		public static string AsContentTypeString(this RequestContentType requestContentType)
		{
			var contentType = "application/json";
			if (requestContentType == RequestContentType.ApplicationXml)
			{
				contentType = "application/xml";
			}
			return contentType;
		}
	}
}
