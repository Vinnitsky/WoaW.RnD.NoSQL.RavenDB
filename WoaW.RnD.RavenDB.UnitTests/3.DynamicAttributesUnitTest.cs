using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Raven.Client.Document;

namespace WoaW.RnD.RavenDB.UnitTests 
{
    /// <summary>
    /// класс представляет сущность с динамическими свойствами
    /// </summary>
    class Entity1
    {
        public int Id { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public Entity1()
        {
            Attributes = new Dictionary<string, string>();
        }
    }

    [TestClass]
    public class DynamicAttributesUnitTest
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
        /// в методе демонстрируется как обавляется свойство в ассоциативный масив 
        /// м после этого документ отправляется в хранилище
        /// </summary>
        [TestMethod]
        [TestCategory("DynamicAttributes")]
        public void Add_DynamicProperties_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var e1 = new Entity1() { Id = 1 };
                    e1.Attributes["a1"] = "a1";
                    session.Store(e1);
                    session.SaveChanges();

                    var e2 = new Entity1() { Id = 2 };
                    e2.Attributes["a2"] = "a2";
                    session.Store(e2);
                    session.SaveChanges();

                    var e3 = new Entity1() { Id = 3 };
                    e3.Attributes["a3"] = "a3";
                    session.Store(e3);
                    session.SaveChanges();
                }
            }
        }

        /// <summary>
        /// в методе демонстрируется выборку документа по значениям в ассоциативном масиве
        /// </summary>
        [TestMethod]
        [TestCategory("DynamicAttributes")]
        public void Query_DynamicProperties_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var query = from e in session.Query<Entity1>() where e.Attributes["a2"] == "a2" select e;
                    foreach (var item in query.ToList())
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Entity: Id={0}, Name={1}", item.Id, item.Attributes["a2"]));
                    }
                }
            }
        }

    }
}
