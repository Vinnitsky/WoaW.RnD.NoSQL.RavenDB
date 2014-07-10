using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WoaW.RnD.RavenDB.UnitTests
{
    [TestClass]
    public class QueryPartialDocumentUnitTest
    {
        /// <summary>
        /// метод возвращет два поля не используя кастомного индекса,  
        /// по сети передается только указанные поля 
        /// </summary>
        [TestMethod]
        public void Query_PartialDocument_V1_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var customers = (from c in session.Query<Customer>() select new Customer { Name = c.Name, Id = c.Id }).ToList();
                    TestHelpers.DumpCustomers(customers);
                }
            }
        }

        /// <summary>
        /// метод использует предопределенный индекс в котором возвращаются два поля
        /// 1) в первом случае запрашивается только два поля и по сети передается два поля
        /// 2) во втором случае запрашиваются все поля, по сети передаются все поля не 
        /// смотря на то что используется индекс который возвращает два поля
        /// </summary>
        [TestMethod]
        public void Query_PartialDocument_V2_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var customers1 = session.Query<Customer>("Customers/MyIndex")
                    .Select(c => new Customer { Name = c.Name, Id = c.Id })
                    .ToList();
                    TestHelpers.DumpCustomers(customers1);

                    var customers2 = session.Query<Customer>("Customers/MyIndex").ToList();
                    TestHelpers.DumpCustomers(customers2);
                }
            }
        }

    }
}
