using System;

namespace Saasu.API.Client.IntegrationTests.Helpers
{
    public static class StringExtensionMethods
    {
        public static string MakeUnique(this string text)
        {
            return text + " " + Guid.NewGuid();
        }
    }
}
