using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Document;
using WoaW.RnD.RavenDB.UnitTests.Entities;

namespace WoaW.RnD.RavenDB.UnitTests 
{
    [TestClass]
    public class IncludeUnitTests
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void AddCustomers_SucessTest()
        {
            int length = 10;
            for (int i = 0; i < length; i++)
            {
                CreateCustomerRecord(i);
            }
        }
        [TestMethod]
        public void GetCustomerForOrder_SucessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var order = session.Include("Customer.Id").Load<Order>(15);
                    var customer = session.Load<Customer>("customers/5");
                    Assert.AreEqual(5, customer.Id);
                }
            }
        }

        [TestMethod]
        public void AddOrder_SucessTest()
        {
            int length = 20;
            for (int i = 10; i < length; i++)
            {
                CreateOrderRecord(i);
            }

            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var customer = session.Include("Customer.Id").Load<Order>("Orders/5");
                    Assert.AreEqual(5, customer.Id);
                }
            }
        }

        private static void CreateOrderRecord(int i)
        {
            string id = (i - 10).ToString();
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var order = new Order()
                    {
                        Id = i.ToString(),
                        Title = "Description for Mr. Who",
                        Customer = new CustomerReference() { Id = id, Title = "Mr. Who"}
                    };
                    session.Store(order);
                    session.SaveChanges();
                }
            }
        }

        private static void CreateCustomerRecord(int i)
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var customer = new Customer() { Id = i.ToString(), Title = "Mr. Who " + i.ToString(), Name = "Who " + i.ToString(), Email = "who@live.com" };
                    session.Store(customer);
                    session.SaveChanges();
                }
            }
        }


        [TestMethod]
        public void Query_Composit_SuccessTest()
        {
            string id = "1";

            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    //var order = session.Advanced.LuceneQuery<Order>().Where("Title:Order*").SingleOrDefault();
                    var order = session.Include("Customer.Id").Load<Order>(id);
                    Assert.IsNotNull(order);

                    var customer = session.Load<Customer>(order.Customer.Id);
                    Assert.IsNotNull(customer);
                }
            }
        }
    }
}
