using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Document;

namespace WoaW.RnD.RavenDB.UnitTests 
{
    [TestClass]
    public class NoSchemaUnitTest
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

        /// <summary>
        /// class A version 1
        /// </summary>
        class B
        {
            public int Id { get; set; }
            public string Field1 { get; set; }
        }

        /// <summary>
        /// class A version 2
        /// </summary>
        class A
        {
            public int Id { get; set; }
            public string Field1 { get; set; }
            public string Field2 { get; set; }
        }

        [TestMethod]
        [TestCategory("NoSchema")]
        public void AddOrderV1_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var a = new A() { Id = 2, Field1 = "f1" , Field2="f2"};
                    session.Store(a);
                    session.SaveChanges();
                }
            }
        }


        [TestMethod]
        [TestCategory("NoSchema")]
        public void Query_f2_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var a = new A() { Id = 2, Field1 = "f1", Field2 = "f2" };
                    var c = from t in session.Query<A>() where t.Field2 == "f2" select t;
                    Assert.IsNotNull(c);
                }
            }
        }
    }
}
