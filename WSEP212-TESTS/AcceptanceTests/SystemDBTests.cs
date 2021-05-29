using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class SystemDBTests
    {
        SystemDBMock mock = SystemDBMock.Instance;
        public void testInit()
        {
            mock.Users.Add(new User("check, 12, false"));
        }

        [TestMethod]
        public void registerTest()
        {
            testInit();
            Assert.Equals(1, mock.Users.Count());
        }
    }
}