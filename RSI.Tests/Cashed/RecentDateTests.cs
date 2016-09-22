using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSI.Cashed;

namespace RSI.Tests.Cashed
{
    [TestClass()]
    public class DoctorControllerTests
    {
        [TestMethod()]
        public void Get_Years()
        {
            // Act
            var years = RecentDate.Instance.Get();

            // Assert
            Assert.IsNotNull(years);
            Assert.IsTrue(years.Count == 17);
        }
    }
}


