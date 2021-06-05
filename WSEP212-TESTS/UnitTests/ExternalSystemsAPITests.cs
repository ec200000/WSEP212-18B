using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication;
using WSEP212;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;

namespace WSEP212_TESTS.UnitTests
{
    [TestClass]
    public class ExternalSystemsAPITests
    {
        private static PaymentInterface payment;
        private static DeliveryInterface delivery;

        [ClassInitialize]
        public static void SetupAuth(TestContext context)
        {
            Startup.readConfigurationFile();
            //SystemDBAccess.mock = true;
            
            SystemDBAccess.Instance.Bids.RemoveRange(SystemDBAccess.Instance.Bids);
            SystemDBAccess.Instance.Carts.RemoveRange(SystemDBAccess.Instance.Carts);
            SystemDBAccess.Instance.Invoices.RemoveRange(SystemDBAccess.Instance.Invoices);
            SystemDBAccess.Instance.Items.RemoveRange(SystemDBAccess.Instance.Items);
            SystemDBAccess.Instance.Permissions.RemoveRange(SystemDBAccess.Instance.Permissions);
            SystemDBAccess.Instance.Stores.RemoveRange(SystemDBAccess.Instance.Stores);
            SystemDBAccess.Instance.Users.RemoveRange(SystemDBAccess.Instance.Users);
            SystemDBAccess.Instance.DelayedNotifications.RemoveRange(SystemDBAccess.Instance.DelayedNotifications);
            SystemDBAccess.Instance.ItemReviewes.RemoveRange(SystemDBAccess.Instance.ItemReviewes);
            SystemDBAccess.Instance.UsersInfo.RemoveRange(SystemDBAccess.Instance.UsersInfo);

            payment = PaymentSystemAdapter.Instance;
            delivery = DeliverySystemAdapter.Instance;
        }

        private int paymentCharge()
        {
           return payment.paymentCharge("202581256", "1", "2021", "Sagiv", "051", "207551201", 0);
        }

        private bool cancelCharge(int transactionID)
        {
            return payment.cancelPaymentCharge(transactionID);
        }

        private int deliverItems()
        {
            return delivery.deliverItems("Sagiv", "Rabinovich", "Holon", "Israel", "501369");
        }

        private bool cancelDelivery(int transactionID)
        {
            return delivery.cancelDelivery(transactionID);
        }

        [TestMethod]
        public void paymentChargeTest()
        {
            int transactionID = paymentCharge();
            Assert.IsTrue(transactionID >= 10000 && transactionID <= 100000);

            cancelCharge(transactionID);
        }

        [TestMethod]
        public void cancelChargeTest()
        {
            int transactionID = paymentCharge();

            bool res = cancelCharge(transactionID);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void deliverItemsTest()
        {
            int transactionID = deliverItems();
            Assert.IsTrue(transactionID >= 10000 && transactionID <= 100000);

            cancelDelivery(transactionID);
        }

        [TestMethod]
        public void cancelDeliveryTest()
        {
            int transactionID = deliverItems();

            bool res = cancelDelivery(transactionID);
            Assert.IsTrue(res);
        }
    }
}
