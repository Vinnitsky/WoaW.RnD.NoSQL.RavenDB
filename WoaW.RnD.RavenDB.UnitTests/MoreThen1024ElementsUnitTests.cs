using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WoaW.RnD.RavenDB.UnitTests.Entities;
using System.Collections.Generic;
using Raven.Client.Document;
using Raven.Abstractions.Data;

namespace WoaW.RnD.RavenDB.UnitTests 
{
    [TestClass]
    public class MoreThen1024ElementsUnitTests
    {
        /// <summary>
        /// you can get 1024 as maximum per query 
        /// 
        /// этот метод не генерирует никаких исключений он просто вернет 128 объектов и все.
        /// </summary>
        [TestMethod]
        [TestCategory("MoreThen1024")]
        public void Query_MoreThen1024Docuemnts_FailTest()
        {
            #region prepare test
            TestHelpers.CreateCustomers(1500);
            #endregion

            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var query = session.Query<Customer>();
                    var count1 = query.Count(); // покажет правильный результат 
                    var count2 = query.ToList().Count;
                    System.Diagnostics.Debug.WriteLine(string.Format("query.Count()={0},query.ToList().Count={1} ", count1, count2));

                    // что забавно что проблем здесь не будет - просто система отдасть 128 элементов и все
                    foreach (var customer in query.ToList())
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("CUSTOMER: Id={0}, Name={1}, Email={2}",
                            customer.Id, customer.Name, customer.Email));
                    }
                }
            }

            #region clean test
            TestHelpers.DeleteAllCustomers();
            #endregion

        }

        /// <summary>
        /// you can get 128 item per query
        /// </summary>
        [TestMethod]
        [TestCategory("MoreThen1024")]
        public void Query_MoreThen128Docuemnts_FailTest()
        {
        }

        /// <summary>
        /// you can avoid max query limit using Unboundary Stream
        /// </summary>
        [TestMethod]
        [TestCategory("MoreThen1024")]
        public void Query_MoreThen1024Docuemnts_SuccessTest()
        {
            #region prepare test
            TestHelpers.CreateCustomers(1500);
            #endregion

            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var query = session.Query<Customer>();
                    var enumerator = session.Advanced.Stream(query);
                    while (enumerator.MoveNext())
                    {
                        var customer = enumerator.Current.Document;

                        System.Diagnostics.Debug.WriteLine(string.Format("CUSTOMER: Id={0}, Name={1}, Email={2}",
                            customer.Id, customer.Name, customer.Email));
                    }
                }
            }

            #region clean test
            TestHelpers.DeleteAllCustomers();
            #endregion
        }

    }
}
