using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saasu.API.Client.IntegrationTests
{
	public static class TestConfig
	{
        public static string TestUser
		{
			get { return GetValueWithDefault("TestUser","dev@saasu.com"); }
		}

		public static string TestUserPassword
		{
			get { return GetValueWithDefault("TestUserPassword", "csharp"); }
		}

	    public static int TestFileId
	    {
            get { return int.Parse(GetValueWithDefault("FileUid", "0")); }
	    }

		private static string GetValueWithDefault(string appSettingsKey, string defaultValue)
		{
			var value = System.Configuration.ConfigurationManager.AppSettings[appSettingsKey];
			if (string.IsNullOrWhiteSpace(value))
			{
				return defaultValue;
			}

			return value;
		}
	}
}
