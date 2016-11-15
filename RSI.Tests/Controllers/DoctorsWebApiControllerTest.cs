﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSI.API;
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
        public void GetFiltered_returns_filtered_Doctors()
        {
            // Arrange
            var controller = new DoctorsController();
            var filter = new Filter
            {
                //Country = new List<string>(),
                Specialty = new List<string>(),
                State = new List<string>(),
                MinRank = new List<int?>()
            };

            // Act
            var result = controller.GetFiltered(filter);

            // Cheat
            var cnt = result.Count;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == cnt);
        }

        [TestMethod]
        public void GetFiltered_Specialty_returns_filterd_Doctors()
        {
            // Arrange
            var controller = new DoctorsController();
            var filter = new Filter
            {
                Country = new List<string>(),
                Specialty = new List<string> {"Allergy"},
                State = new List<string>(),
                MinRank = new List<int?>(),
                MinPublications = new List<int?>(),
                MinPrescriptions = new List<int?>(),
                MinPatients = new List<int?>(),
                MinClaims = new List<int?>(),
                OldestRecentYear = new List<string>()
            };

            // Act
            var result = controller.GetFiltered(filter);

            // Cheat
            var cntExact = result.Count(x => x.Specialty.Equals(filter.Specialty[0]));
            var cntDifferent = result.Count(x => !x.Specialty.Equals(filter.Specialty[0]));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Count == cntExact);
            Assert.IsFalse(cntExact == cntDifferent);
        }

        [TestMethod]
        public void GetFiltered_NONUS_USA_Combined_returns_filtered_Doctors()
        {
            // Arrange
            var controller = new DoctorsController();
            var filter = new Filter
            {
                Country = new List<string> {"USA"},
                Specialty = new List<string>(),
                State = new List<string>(),
                MinRank = new List<int?>(),
                MinPublications = new List<int?>(),
                MinPrescriptions = new List<int?>(),
                MinPatients = new List<int?>(),
                MinClaims = new List<int?>(),
                OldestRecentYear = new List<string>()
            };

            // Act
            var result = controller.GetFiltered(filter);

            // Cheat
            var cntUSA = result.Count;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == cntUSA);

            filter = new Filter
            {
                Country = new List<string> { "CAN" },
                Specialty = new List<string>(),
                State = new List<string>(),
                MinRank = new List<int?>(),
                MinPublications = new List<int?>(),
                MinPrescriptions = new List<int?>(),
                MinPatients = new List<int?>(),
                MinClaims = new List<int?>(),
                OldestRecentYear = new List<string>()

            };

            // Act
            result = controller.GetFiltered(filter);

            // Cheat
            var cntCAN = result.Count;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == cntCAN);

            filter = new Filter
            {
                Country = new List<string> { "USA", "CAN" },
                Specialty = new List<string>(),
                State = new List<string>(),
                MinRank = new List<int?>(),
                MinPublications = new List<int?>(),
                MinPrescriptions = new List<int?>(),
                MinPatients = new List<int?>(),
                MinClaims = new List<int?>(),
                OldestRecentYear = new List<string>()

            };

            // Act
            result = controller.GetFiltered(filter);

            // Cheat
            var cnt = result.Count;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == cnt);
        }

        [TestMethod]
        public void GetFiltered_New_Filter_returns_filtered_Doctors()
        {
            // Arrange
            var controller = new DoctorsController();

            var filter = new Filter
            {
                Country = new List<string> {"USA"},
                Specialty = new List<string>(),
                State = new List<string>(),
                MinRank = new List<int?> {7},
                MinPublications = new List<int?> {3},
                MinPrescriptions = new List<int?> {0},
                MinPatients = new List<int?> {0},
                MinClaims = new List<int?> {0},
                OldestRecentYear = new List<string> {"2008"}
            };

            // Act
            var result = controller.GetFiltered(filter);

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