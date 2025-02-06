using Microsoft.EntityFrameworkCore;
using WorldCities2.Server.Controllers;
using WorldCities2.Server.Data;
using WorldCities2.Server.Data.Models;

namespace WorldCities2.Server.Tests
{
    /// <summary>
    /// Unit test class for CitiesController.
    /// </summary>
    public class CitiesController_Test
    {
        #region Properties
        #endregion

        #region Public Methods

        /// <summary>
        /// Test the GetCity() method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetCity() 
        {
            // Define test database context
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WorldCities")
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Add(new City()
            {
                ID=1,
                CountryId=1,
                Latitude=1,
                Longitude=1,
                Name="TestCity1"
            });

            // Define test CitiesController object
            var controller = new CitiesController(context);

            // Get test values from database context.
            City? city_existing = (await controller.GetCity(1)).Value;
            City? city_notExisting = (await controller.GetCity(2)).Value;

            // Assert
            Assert.NotNull(city_existing);
            Assert.Null(city_notExisting);
        }

        #endregion
    }
}
