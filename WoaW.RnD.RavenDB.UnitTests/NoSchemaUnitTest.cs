using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Document;

namespace WoaW.RnD.RavenDB.UnitTests 
{
    [TestClass]
    public class NoSchemaUnitTest
    {
        class B
        {
            public int Id { get; set; }
            public string Field1 { get; set; }
        }

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
