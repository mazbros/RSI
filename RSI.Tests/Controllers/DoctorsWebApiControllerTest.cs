using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSI.API;
using RSI.Helpers;

namespace RSI.Tests
{
    [TestClass]
    public class DoctorsWebApiControllerTest
    {
        [TestMethod]
        public void Get_returns_all_Doctors()
        {
            // Arrange
            var controller = new DoctorsController();

            // Act
            var result = controller.GetAll();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetById_returns_single_Doctor()
        {
            // Arrange
            const int id = 33;
            var controller = new DoctorsController();

            // Act
            var result = await controller.GetById(id);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Get_with_sort_options_returns_sorted_Doctors()
        {
            // Arrange
            var controller = new DoctorsController();

            // Act
            var result = controller.GetSorted(new Sorter {Field = "DRID", Order = "desc"});

            // Assert
            Assert.AreNotSame(result[0].DRID, result[1].DRID);
            Assert.IsTrue(result[0].DRID > result[1].DRID);

            // Act
            result = controller.GetSorted(new Sorter {Field = "DRID", Order = ""});

            // Assert
            Assert.AreNotSame(result[0].DRID, result[1].DRID);
            Assert.IsTrue(result[0].DRID < result[1].DRID);

            // Act
            result = controller.GetSorted(new Sorter {Field = "RecentDate", Order = "desc"});

            // Assert
            Assert.AreNotSame(result[0].RecentDate, result[7].RecentDate);
            Assert.IsTrue(DateTime.Parse(result[0].RecentDate) >= DateTime.Parse(result[7].RecentDate));

            // Act
            result = controller.GetSorted(new Sorter {Field = "Blah", Order = "desc"});

            // Assert
            Assert.AreNotSame(result[0].RecentDate, result[7].RecentDate);
            Assert.IsTrue(DateTime.Parse(result[0].RecentDate) != DateTime.Parse(result[7].RecentDate));
        }

        [TestMethod]
        public void GetRanks_returns_ranks()
        {
            // Arrange
            var controller = new DoctorsController();

            // Act
            var ranks = controller.GetRanks();

            // Assert
            Assert.IsNotNull(ranks);
            Assert.IsTrue(ranks.Count == 12);
        }

        [TestMethod]
        public void GetStates_returns_states()
        {
            // Arrange
            var controller = new DoctorsController();

            // Act
            var states = controller.GetStates();

            // Assert
            Assert.IsNotNull(states);
            Assert.IsTrue(states.Count == 65);
        }

        [TestMethod]
        public void GetSpecialties_returns_specialties()
        {
            // Arrange
            var controller = new DoctorsController();

            // Act
            var specialties = controller.GetSpecialties();

            // Assert
            Assert.IsNotNull(specialties);
            Assert.IsTrue(specialties.Count == 310);
        }

        [TestMethod]
        public void GetCountries_Returns_List_of_Abbreviations()
        {
            // Arrange
            var controller = new DoctorsController();

            // Act
            var countries = controller.GetCountries();

            // Assert
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count == 23);
        }
    }
}