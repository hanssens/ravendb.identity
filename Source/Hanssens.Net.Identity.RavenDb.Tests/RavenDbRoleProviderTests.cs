using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using System.Data;

namespace Hanssens.Net.Identity.RavenDb.Tests
{
    [TestFixture]
    public class RavenDbRoleProviderTests
    {

        [Test]
        public void CreateRole_Should_Insert_Role()
        {
            var roleProvider = new RavenDbRoleProvider();
            roleProvider.CreateRole("Production");
        }

    }
}
