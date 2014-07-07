﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WoaW.RnD.RavenDB.UnitTests.Entities;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace WoaW.RnD.RavenDB.UnitTests
{
    public class OrderCount
    {
        public string Subj { get; set; }
        public int Count { get; set; }
    }

    public class Orders_OrdersCountByTaskSubj : AbstractIndexCreationTask<Order, Orders_OrdersCountByTaskSubj.ReduceResult>
    {
        public class ReduceResult
        {
            public string Subj { get; set; }
            public int Count { get; set; }
        }

        // The index name generated by this is going to be BlogPosts/PostsCountByTag
        public Orders_OrdersCountByTaskSubj()
        {
            Map = orders => from order in orders
                            from task in order.Tasks
                            select new
                            {
                                Subj = task.Subject,
                                Count = 1
                            };

            Reduce = results => from result in results
                                group result by result.Subj
                                    into g
                                    select new
                                    {
                                        Subj = g.Key,
                                        Count = g.Sum(x => x.Count)
                                    };
        }
    }


    [TestClass]
    public class IndexesUnitTests
    {
        [TestMethod]
        public void DefineNewIndex_M1_SeccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                store.DatabaseCommands.PutIndex("Customers/ByTitles",
                    new IndexDefinitionBuilder<Customer>
                    {
                        Map = customers => from customer in customers
                                            select new { customer.Title }
                    });
            }
        }

        [TestMethod]
        public void GetIndex_SeccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                var indexDefinition = store.DatabaseCommands.GetIndex("Customers/ByTitles");
                Assert.IsNotNull(indexDefinition);
            }
        }
        [TestMethod]
        public void DefineNewIndex_M2_SeccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                new Orders_OrdersCountByTaskSubj().Execute(store); //V1

                var indexDefinition = store.DatabaseCommands.GetIndex("Orders/OrdersCountByTaskSubj");
                Assert.IsNotNull(indexDefinition);
            }
        }
        [TestMethod]
        public void DefineNewIndex_M3_SeccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                IndexCreation.CreateIndexes(typeof(Orders_OrdersCountByTaskSubj).Assembly, store);

                var indexDefinition = store.DatabaseCommands.GetIndex("Orders/OrdersCountByTaskSubj");
                Assert.IsNotNull(indexDefinition);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Query_UsingIndex_V1_FailTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var orders = session.Query<OrderCount>("Orders/OrdersCountByTaskSubj").ToList();
                    TestHelpers.DumpOrders(orders);
                }
            }
        }

        [TestMethod]
        public void Query_UsingServerCreatedIndex_SeccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    CreateOrder(session);

                    var result = session.Query<OrderCount>("OrdersCountByTask_Temp").FirstOrDefault(x => x.Subj  == "aa");

                    Assert.IsNotNull(result);
                    System.Diagnostics.Debug.WriteLine(string.Format("ORDER: Id={0}, Title={1}", result.Count, result.Subj));
                }
            }
        }

        private static void CreateOrder(Raven.Client.IDocumentSession session)
        {
            var r = session.Load<Order>("order1");
            if (r != null)
                return;

            var order = new Order()
            {
                Id = "order1",
                Tasks = new System.Collections.Generic.List<Task>() 
                        { 
                            new Task() { Id = "task1", Subject = "aa" }, 
                            new Task() { Id = "task2", Subject = "aa-bb" }, 
                        }
            };
            session.Store(order);
            session.SaveChanges();
        }

        [TestMethod]
        public void Query_UsingIndex_V2_SeccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                IndexCreation.CreateIndexes(typeof(Orders_OrdersCountByTaskSubj).Assembly, store);

                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {

                    var indexDefinition = store.DatabaseCommands.GetIndex("Orders/OrdersCountByTaskSubj");
                    Assert.IsNotNull(indexDefinition);

                    var result = session.Query<Orders_OrdersCountByTaskSubj.ReduceResult, Orders_OrdersCountByTaskSubj>().FirstOrDefault(x => x.Subj == "aa");
                    System.Diagnostics.Debug.WriteLine(string.Format("ORDER: Id={0}, Title={1}", result.Count, result.Subj));

                }
            }
        }
    }
}
