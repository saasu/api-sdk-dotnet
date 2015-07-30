using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saasu.API.Core.Globals;

namespace Saasu.API.Core.Models.RequestFiltering
{
	public class RequestPagingFilter
	{
		public RequestPagingFilter()
		{
		    Page = ApiConstants.DefaultPageNumber;
		    PageSize = ApiConstants.DefaultPageSize;
		}
		public int Page { get; set; }
		public int PageSize { get; set; }
	}
}
