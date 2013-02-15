using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCBlog.Infrastructure;
using CCBlog.Model.Poco;
using CCBlog.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCBlog.Tests
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public void AddUserTest()
        {
            using (var repo = RepositoryFactory.Get())
            {
                var user = GetUser("TestClaimedId");

                repo.AddUser(user);

                Assert.IsTrue(user.UserId > 0);

                var userSaved = repo.GetUser(user.UserId);
                Assert.IsTrue(user.ClaimedIdentifier == userSaved.ClaimedIdentifier);
                Assert.IsTrue(user.Email == userSaved.Email);
                Assert.IsTrue(user.Nickname == userSaved.Nickname);
                Assert.IsTrue(user.FullName == userSaved.FullName);
                Assert.IsTrue(user.Role.RoleId == ((User)userSaved).RoleId);
            }
        }

        private static User GetUser(string claimedIdentifier)
        {
            return new User()
                {
                    ClaimedIdentifier = claimedIdentifier,
                    Email = @"test@email.com",
                    Nickname = "TestNickname",
                    FullName = "TestFullName",
                    Role = AppCache.Roles.GetByAlternateKey("Author")
                };
        }

        [TestMethod]
        public void NonUniqueUserAddTest()
        {
            using (var repo = RepositoryFactory.Get())
            {
                var u1 = GetUser("NonUniqueUserAddTest");
                var u2 = GetUser("NonUniqueUserAddTest");

                repo.AddUser(u1);
                repo.AddUser(u2);
            }
        }
    }
}
