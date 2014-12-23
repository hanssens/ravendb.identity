using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Hanssens.Net.Identity.RavenDb.Tests
{
    [TestFixture]
    public class RavenDbMembershipProviderTests
    {

        [Test]
        public void CreateAccount_Should_Insert_Account()
        {
            // arrange
            var expectedUsername = Guid.NewGuid().ToString("N");
            var expectedPassword = "12345";
            var membershipProvider = new RavenDbMembershipProvider();

            // act
            var target = membershipProvider.CreateAccount(expectedUsername, expectedPassword);

            // assert
            target.Should().NotBeNullOrEmpty();
            target.Should().Be(expectedUsername);
        }

        [Test]
        [ExpectedException(typeof(DuplicateNameException))]
        public void CreateAccount_Should_Verify_If_Username_Existst()
        {
            // arrange
            var firstUsername = Guid.NewGuid().ToString("N");
            var secondUsername = firstUsername;
            var password = "12345";
            var membershipProvider = new RavenDbMembershipProvider();

            // act
            var firstUser = membershipProvider.CreateAccount(firstUsername, password);
            var secondUser = membershipProvider.CreateAccount(secondUsername, password);

            // assert
            Assert.Fail("Expected was a DuplicateNameException");
        }

        [Test]
        public void CreateAccount_Should_Hash_The_Password_And_Be_Validatable()
        {
            // arrange
            var expectedUsername = Guid.NewGuid().ToString("N");
            var expectedPassword = "12345";
            var membershipProvider = new RavenDbMembershipProvider();

            // act
            var givenUsername = membershipProvider.CreateAccount(expectedUsername, expectedPassword);

            var target = membershipProvider.ValidateUser(expectedUsername, expectedPassword);
            
            // assert
            target.Should().BeTrue();

        }

    }
}
