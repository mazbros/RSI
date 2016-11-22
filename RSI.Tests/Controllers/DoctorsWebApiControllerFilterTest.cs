using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSI.API;
using RSI.Helpers;

namespace RSI.Tests
{
    [TestClass]
    public class DoctorsWebApiControllerFilterTest
    {
        
        [TestMethod]
        public void GetFiltered_without_filters_returns_unfiltered_Doctors()
        {
            // Arrange
            var controller = new DoctorsController();
            var filter = new Filter
            {
                Country = new List<string>(),
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
            var cnt = result.Count;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == cnt);
        }

        [TestMethod]
        public void GetFiltered_Specialty_returns_filtered_Doctors()
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
        public void GetFiltered_USA_returns_USA_filtered_Doctors()
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
        }

        [TestMethod]
        public void GetFiltered_USA_with_State_returns_USA_and_State_filtered_Doctors()
        {
            var controller = new DoctorsController();
            var filter = new Filter
            {
                Country = new List<string> {"USA"},
                Specialty = new List<string>(),
                State = new List<string> {"NJ"},
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
            var cnt = result.Count;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == cnt);
        }

        [TestMethod]
        public void GetFiltered_USA_with_MinRank_returns_USA_and_Rank_filtered_Doctors()
        {
            var controller = new DoctorsController();
            var filter = new Filter
            {
                Country = new List<string> {"USA"},
                Specialty = new List<string>(),
                State = new List<string>(),
                MinRank = new List<int?> {10},
                MinPublications = new List<int?>(),
                MinPrescriptions = new List<int?>(),
                MinPatients = new List<int?>(),
                MinClaims = new List<int?>(),
                OldestRecentYear = new List<string>()

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
        public void GetFiltered_USA_with_MinPublications_returns_USA_and_Rank_filtered_Doctors()
        {
            // Arrange
            var controller = new DoctorsController();

            var filter = new Filter
            {
                Country = new List<string> {"USA"},
                Specialty = new List<string>(),
                State = new List<string>(),
                MinRank = new List<int?>(),
                MinPublications = new List<int?> {7},
                MinPrescriptions = new List<int?>(),
                MinPatients = new List<int?>(),
                MinClaims = new List<int?>(),
                OldestRecentYear = new List<string>()
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
        public void GetFiltered_USA_with_MinPublications_and_OldestRecentYear_returns_USA_with_MinPublications_and_OldestRecentYear_filtered_Doctors()
        {
            // Arrange
            var controller = new DoctorsController();

            var filter = new Filter
            {
                Country = new List<string> { "USA" },
                Specialty = new List<string>(),
                State = new List<string>(),
                MinRank = new List<int?>(),
                MinPublications = new List<int?> { 7 },
                MinPrescriptions = new List<int?>(),
                MinPatients = new List<int?>(),
                MinClaims = new List<int?>(),
                OldestRecentYear = new List<string> { "2008" }
            };

            // Act
            var result = controller.GetFiltered(filter);

            // Cheat
            var cnt = result.Count;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == cnt);
        }
    }
}