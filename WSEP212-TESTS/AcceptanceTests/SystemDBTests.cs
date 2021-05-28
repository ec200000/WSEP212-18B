using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WSEP212;
using WSEP212.DomainLayer;

namespace WSEP212_TESTS.AcceptanceTests
{
    [TestClass]
    public class SystemDBTests
    {
        [ClassInitialize]
        public void init()
        {
            SystemDBAccess.mock = true;
        }
        
        //just use the SystemDBAccess -> because of the mock param the instance will be the Mock
        public void testInit()
        {
            SystemDBAccess.Instance.Users.Add(new User("check, 12, false"));
        }

        [TestMethod]
        public void registerTest()
        {
            testInit();
            Assert.Equals(1, SystemDBAccess.Instance.Users.Count());
        }
    }
}