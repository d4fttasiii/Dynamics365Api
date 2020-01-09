using Dynamics365.Api.Client.Tests.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamics365.Api.Client.Tests
{
    [TestFixture]
    public class UpdateTests
    {
        [Test]
        public void Test()
        {
            var account = new AccountEntity
            {
                Id = Guid.NewGuid(),
                City = "Washington DC",
                Street = "Bleeker street 177"
            };
        }
    }
}
