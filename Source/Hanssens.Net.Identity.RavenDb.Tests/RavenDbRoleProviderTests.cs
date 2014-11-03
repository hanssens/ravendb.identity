using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hanssens.Net.Identity.RavenDb.Tests
{
    [TestFixture]
    public class RavenDbMembershipProviderTests
    {
        [Test]
        public void CreateAccount_Should_CreateAccount()
        {
            var membership = new RavenDbMembershipProvider();
            var target = membership.CreateAccount("username", "password");
        }

    }
}
