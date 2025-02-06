using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using WorldCities.Server.Controllers;
using WorldCities2.Server.Data;
using WorldCities2.Server.Data.Models;

namespace WorldCities2.Server.Tests
{
    /// <summary>
    /// Test class for the SeedController.
    /// </summary>
    public class SeedController_Test
    {
        #region Public Methods

        /// <summary>
        /// Test the CreateDefaultUsers() method
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateDefaultUsers() 
        {
            // Arrange
            // Create the option instances required by the database context.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WorldCities")
                .Options;

            // Create an IWebHost environment mock instance
            var mockEnvironment = Mock.Of<IWebHostEnvironment>();

            // Create an IConfiguration mock instance
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(config => config[It.Is<string>(stringValue => stringValue.Equals("DefaultPasswords:RegisteredUser", StringComparison.Ordinal))])
                             .Returns("M0ckP$$word");
            mockConfiguration.SetupGet(config => config[It.Is<string>(stringValue => stringValue.Equals("DefaultPasswords:Administrator", StringComparison.Ordinal))])
                             .Returns("M0ckP$$word");

            // Create a database context instance using the in-memory database
            using var context = new ApplicationDbContext(options);

            // Create UserManager and RoleManager instances
            var userManager = IdentityHelper.GetUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = IdentityHelper.GetRoleManager(new RoleStore<IdentityRole>(context));

            // Create a SeedController instance
            var controller = new SeedController(context, mockEnvironment, roleManager, userManager, mockConfiguration.Object);

            // Act
            // Execute the SeedController's CreateDefaultUsers() method to create the default users (and roles)
            await controller.CreateDefaultUsers();

            // Retrieve the users
            var user_Admin = await userManager.FindByEmailAsync("admin@email.com");
            var user_User = await userManager.FindByEmailAsync("user@email.com");
            var user_NotExisting = await userManager.FindByEmailAsync("notexisting@email.com");

            // Assert
            Assert.NotNull(user_Admin);
            Assert.NotNull(user_User);
            Assert.Null(user_NotExisting);
        }

        #endregion
    }
}
