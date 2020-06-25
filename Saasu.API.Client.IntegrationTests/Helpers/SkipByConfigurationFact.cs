using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Saasu.API.Client.IntegrationTests.Helpers
{
    public sealed class SkipByConfigurationFact : FactAttribute
    {
        public SkipByConfigurationFact()
        {
            if (SkipByConfiguration())
            {
                Skip = "Test ignored by configuration.";
            }
        }

        private static bool SkipByConfiguration()
        {
            if (bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["SkipTests"], out var skipTests))
            {
                return skipTests;
            }

            return false;
        }
    }
}
