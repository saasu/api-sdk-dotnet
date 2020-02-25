using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Client.IntegrationTests.Helpers
{
    public class ContactFixture
    {
        public ContactHelper ContactHelper { get; }

        public ContactFixture()
        {
            ContactHelper = new ContactHelper(true);
        }
    }
}
