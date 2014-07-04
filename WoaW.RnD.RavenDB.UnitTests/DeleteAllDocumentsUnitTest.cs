using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WoaW.RnD.RavenDB.UnitTests.Entities;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Abstractions.Data;
using System.Diagnostics;

namespace WoaW.RnD.RavenDB.UnitTests 
{
    [TestClass]
    public class DeleteAllDocumentsUnitTest
    {
        [TestMethod]
        [TestCategory("DeleteAllDocuments")]
        public void DeleteAllCustomersV1()
        {
            TestHelpers.CreateCustomers(1500);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var indexName = "DeleteAllCustomers";
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    //if (store.DatabaseCommands.GetIndex(indexName) != null)
                    //{
                    //    session.Advanced.DocumentStore.DatabaseCommands.PutIndex(indexName, new IndexDefinitionBuilder<Customer>
                    //    {
                    //        Map = documents => documents.Select(entity => new { })
                    //    });
                    //}
                    store.DatabaseCommands.DeleteByIndex(indexName, new IndexQuery(), true);
                    session.SaveChanges();

                    var query = session.Query<Customer>();
                    var count1 = query.Count();
                    Assert.AreEqual(0, count1);
                }
            }

            stopwatch.Stop();
            //System.Diagnostics.Debug.WriteLine("Time elapsed: {0}", stopwatch.Elapsed / lang);
            System.Diagnostics.Debug.WriteLine("Time elapsed: {0}", stopwatch.ElapsedMilliseconds);

        }

        [TestMethod]
        [TestCategory("DeleteAllDocuments")]
        public void DeleteAllCustomersV2()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    store.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName",
                        new IndexQuery { Query = "Tag:" + "Customer" }, allowStale: true);
                    //store.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName",
                    //    new IndexQuery { Query = "Raven-Entity-Name:" + "Customer" }, allowStale: true);
                    session.SaveChanges();

                    var query = session.Query<Customer>();
                    var count1 = query.Count();
                    Assert.AreEqual(0, count1);
                }
            }
        }

        //[TestMethod]
        [TestCategory("DeleteAllDocuments")]
        public void DeleteAllCustomersV3()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    //for less then 30 only
                    var items = session.Query<Customer>().ToList();
                    while (items.Any())
                    {
                        foreach (var obj in items)
                        {
                            session.Delete(obj);
                        }

                        session.SaveChanges();
                        items = session.Query<Customer>().ToList();
                    }

                    var query = session.Query<Customer>();
                    var count1 = query.Count();
                    Assert.AreEqual(0, count1);

                }
            }
        }
        [TestMethod]
        [TestCategory("DeleteAllDocuments")]
        public void DeleteAllCustomersV4()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    new RavenDocumentsByEntityName().Execute(store);
                    store.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName", new IndexQuery { Query = "Tag: Customers" });

                    var query = session.Query<Customer>();
                    var count1 = query.Count();
                    Assert.AreEqual(0, count1);

                }
            }
        }
    }
}
