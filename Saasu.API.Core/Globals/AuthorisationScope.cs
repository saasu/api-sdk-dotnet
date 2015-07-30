using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saasu.API.Core.Globals
{
	public class AuthorisationScopeValue
	{
		public const string View = "view";
		public const string Modify = "modify";
		public const string Delete = "delete";
		public const string Full = "full";
		public const string FileId = "fileid";
	}

	public enum AuthorisationScopeType
	{
		None,
		View,
		Modify,
		Delete,
		Full,
		FileId
	}

	public class AuthorisationScope
	{
		public AuthorisationScopeType ScopeType { get; set; }
		public string ScopeContext { get; set; }
	}

	public static class AuthorisationScopeExtensions
	{
		public static AuthorisationScope ToAuthorisationScope(this string scopeValue)
		{
			if (!string.IsNullOrWhiteSpace(scopeValue))
			{
				var lowerValue = scopeValue.ToLowerInvariant();
				if (lowerValue == AuthorisationScopeValue.View)
				{
					return new AuthorisationScope { ScopeType = AuthorisationScopeType.View };
				}
				if (lowerValue == AuthorisationScopeValue.Delete)
				{
					return new AuthorisationScope { ScopeType = AuthorisationScopeType.Delete};
				}
				if (lowerValue == AuthorisationScopeValue.Modify)
				{
					return new AuthorisationScope { ScopeType = AuthorisationScopeType.Modify};
				}
				if (lowerValue == AuthorisationScopeValue.Full)
				{
					return new AuthorisationScope { ScopeType = AuthorisationScopeType.Full};
				}

				if (lowerValue.StartsWith(AuthorisationScopeValue.FileId))
				{
					var scope = new AuthorisationScope { ScopeType = AuthorisationScopeType.FileId };
					int index = lowerValue.IndexOf(':');
					if (index >= 0)
					{
						var scopeContext = lowerValue.Substring(index + 1, lowerValue.Length - (index + 1));
						scope.ScopeContext = scopeContext;
					}
					return scope;
				}
			}

			return new AuthorisationScope { ScopeType = AuthorisationScopeType.None};
		}

		public static string ToTextValue(this AuthorisationScope authScope)
		{
			if (authScope.ScopeType == AuthorisationScopeType.None)
			{
				return string.Empty;
			}
			var scopeText = authScope.ScopeType.ToString().ToLowerInvariant();
			if (!string.IsNullOrWhiteSpace(authScope.ScopeContext))
			{
				return string.Format("{0}:{1}", scopeText, authScope.ScopeContext);
			}
			return scopeText;
		}

		public static string ToTextValues(this IEnumerable<AuthorisationScope> authScopes)
		{
			if (authScopes == null || authScopes.Count() == 0)
			{
				return string.Empty;
			}

			var scopeTextValue = new StringBuilder();
			foreach (var scopeValue in authScopes)
			{
				if (scopeTextValue.Length != 0)
				{
					scopeTextValue.AppendFormat(" {0}", scopeValue.ToTextValue());
				}
				else
				{
					scopeTextValue.AppendFormat("{0}", scopeValue.ToTextValue());
				}
			}
			return scopeTextValue.ToString();
		}
		
		public static AuthorisationScope[] ToScopeArray(this string scopeTextValue)
		{
			var scopeList = new List<AuthorisationScope>();

			if (string.IsNullOrWhiteSpace(scopeTextValue))
			{
				return scopeList.ToArray();
			}

			var values = scopeTextValue.Split(' ');
			foreach (var value in values)
			{
				scopeList.Add(value.ToAuthorisationScope());
			}
			return scopeList.ToArray();
		}
	}
}
