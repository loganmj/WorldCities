using Castle.Core.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCities2.Server.Tests
{
    /// <summary>
    /// Helps scaffold the UserManager and RoleManager objects for test classes.
    /// </summary>
    public static class IdentityHelper
    {
        #region Public Methods

        /// <summary>
        /// Gets a RoleManager
        /// </summary>
        /// <typeparam name="TIdentityRole"></typeparam>
        /// <param name="roleStore"></param>
        /// <returns></returns>
        public static RoleManager<TIdentityRole> GetRoleManager<TIdentityRole>(IRoleStore<TIdentityRole> roleStore) where TIdentityRole : IdentityRole 
        {
            return new RoleManager<TIdentityRole>(
                roleStore,
                [],
                new UpperInvariantLookupNormalizer(),
                new Mock<IdentityErrorDescriber>().Object, 
                new Mock<ILogger<RoleManager<TIdentityRole>>>().Object);
        }

        /// <summary>
        /// Gets a UserManager.
        /// </summary>
        /// <typeparam name="TIdentityUser"></typeparam>
        /// <param name="userStore"></param>
        /// <returns></returns>
        public static UserManager<TIdentityUser> GetUserManager<TIdentityUser>(IUserStore<TIdentityUser> userStore) where TIdentityUser : IdentityUser
        {
            return new UserManager<TIdentityUser>(
                userStore,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<TIdentityUser>>().Object,
                [],
                [],
                new UpperInvariantLookupNormalizer(),
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<TIdentityUser>>>().Object);
        }

        #endregion
    }
}
