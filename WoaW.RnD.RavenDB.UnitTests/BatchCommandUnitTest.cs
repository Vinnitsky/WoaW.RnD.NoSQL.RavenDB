using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Abstractions.Commands;
using Raven.Client.Document;
using Raven.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using WoaW.RnD.RavenDB.UnitTests.Entities;

namespace WoaW.RnD.RavenDB.UnitTests
{
    [TestClass]
    public class BatchCommandUnitTest
    {
        [TestMethod]
        [TestCategory("Batching")]
        public void Batch_AddCustomer_SunccessTest()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int lang = 10;
            for (int i = 0; i < lang; i++)
            {

                using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
                {
                    using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                    {
                        var num = 1500;
                        var  commands = new List<PutCommandData>();
                        for (int j = 0; j < num; j++)
                        {
                            var customer = new Customer() { Id = string.Format("{0}", j), Title = string.Format("Mr. {0}", j), Name = string.Format("Name:{0}", j), Email = string.Format("{0}@live.com", j) };
                            //var etag = session.Advanced.GetEtagFor(customer); // используется для получения уже существующего элемента в базе
                            var data = new PutCommandData()
                            {
                                Document = RavenJObject.FromObject(customer),
                                Key = customer.Id,
                                //Metadata = new RavenJObject()
                                Metadata = new RavenJObject
                                {
                                    {"Raven-Entity-Name", "Customers"}
                                }
                            };
                            commands.Add(data);
                        }

                        var batchResults = store.DatabaseCommands.Batch(commands.ToArray());
                        session.Advanced.Defer(commands.ToArray());
                        session.SaveChanges();
                    }
                }
            }

            stopwatch.Stop();
            //System.Diagnostics.Debug.WriteLine("Time elapsed: {0}", stopwatch.Elapsed / lang);
            System.Diagnostics.Debug.WriteLine("Time elapsed: {0}", stopwatch.ElapsedMilliseconds / lang);
            //358
        }
        [TestMethod]
        [TestCategory("Batching")]
        public void Batch_Add10Customers_SunccessTest()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int lang = 10;
            for (int i = 0; i < lang; i++)
            {
                TestHelpers.CreateCustomers(1500);
            }

            stopwatch.Stop();
            //System.Diagnostics.Debug.WriteLine("Time elapsed: {0}", stopwatch.Elapsed / lang);
            System.Diagnostics.Debug.WriteLine("Time elapsed: {0}", stopwatch.ElapsedMilliseconds / lang);
            //257
        }
    }
}
