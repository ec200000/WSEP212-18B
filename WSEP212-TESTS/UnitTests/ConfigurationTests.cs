using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class ConfigurationTests
    {
        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            Startup.readConfigurationFile();
        }
        
        [TestMethod]
        public void configurationSuccess()
        {
            Assert.AreEqual(SystemDBAccess.database, "wsep212Dep");
            Assert.AreEqual(SystemDBAccess.server, "wsep212b18");
            Assert.AreEqual(SystemDBAccess.userID, "wsep212b@wsep212b18");
            Assert.AreEqual(SystemDBAccess.password, "Ab123456");
            Assert.AreEqual(DeliverySystemAPI.webAddressAPI, "https://cs-bgu-wsep.herokuapp.com/");
            Assert.AreEqual(PaymentSystemAPI.webAddressAPI, "https://cs-bgu-wsep.herokuapp.com/");
        }
    }
}