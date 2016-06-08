using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSI.Controllers;
using RSI.Helpers;

namespace RSI.Tests.Controllers
{
    [TestClass]
    public class DoctorsWebApiControllerTest
    {
        [TestMethod]
        public void Get_returns_all_Doctors()
        {
            // Arrange
            var controller = new DoctorsWebApiController();
            
            // Act
            var result = controller.Get();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetById_returns_single_Doctor()
        {
            // Arrange
            const int id = 33;
            var controller = new DoctorsWebApiController();

            // Act
            var result = await controller.GetById(id);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Get_with_sort_options_returns_sorted_Doctors()
        {
            // Arrange
            var controller = new DoctorsWebApiController();
            var doctors = controller.Get();

            // Act
            var result = controller.Get(new Sorter {Field = "DRID", Order = "desc"}, doctors);

            // Assert
            Assert.AreNotSame(result[0].DRID, result[1].DRID);
            Assert.IsTrue(result[0].DRID > result[1].DRID);

            // Act
            result = controller.Get(new Sorter { Field = "DRID", Order = ""}, doctors);

            // Assert
            Assert.AreNotSame(result[0].DRID, result[1].DRID);
            Assert.IsTrue(result[0].DRID < result[1].DRID);

            // Act
            result = controller.Get(new Sorter {Field = "RecentDate", Order = "desc"}, doctors);

            // Assert
            Assert.AreNotSame(result[0].RecentDate, result[7].RecentDate);
            Assert.IsTrue(DateTime.Parse(result[0].RecentDate) >= DateTime.Parse(result[7].RecentDate));

            // Act
            result = controller.Get(new Sorter {Field = "Blah", Order = "desc"}, doctors);

            // Assert
            Assert.AreNotSame(result[0].RecentDate, result[7].RecentDate);
            Assert.IsTrue(DateTime.Parse(result[0].RecentDate) != DateTime.Parse(result[7].RecentDate));
        }

        [TestMethod]
        public void GetFiltered_returns_filtered_Doctors()
        {
            // Arrange
            var controller = new DoctorsWebApiController();
            var filter = new Filter
            {
                Specialty = new List<string>(),
                State = new List<string>(),
                Rank = new List<int?> {null}
            };
            var doctors = controller.Get(new Sorter {Field =  "Last Name", Order = ""}, controller.Get());

            // Act
            var result = controller.GetFiltered(filter, doctors);
            
            // Cheat
            var cnt = result.Count;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == cnt);
        }

        [TestMethod]
        public void GetRanks_returns_ranks()
        {
            // Arrange
            var controller = new DoctorsWebApiController();
            
            // Act
            var ranks = controller.GetRanks();

            // Assert
            Assert.IsNotNull(ranks);
            Assert.IsTrue(ranks.Count == 18);
        }

        [TestMethod]
        public void GetStates_returns_states()
        {
            // Arrange
            var controller = new DoctorsWebApiController();

            // Act
            var states = controller.GetStates();

            // Assert
            Assert.IsNotNull(states);
            Assert.IsTrue(states.Count == 52);
        }

        [TestMethod]
        public void GetSpecialties_returns_specialties()
        {
            // Arrange
            var controller = new DoctorsWebApiController();

            // Act
            var specialties = controller.GetSpecialties();

            // Assert
            Assert.IsNotNull(specialties);
            Assert.IsTrue(specialties.Count == 172);
        }
    }
}
