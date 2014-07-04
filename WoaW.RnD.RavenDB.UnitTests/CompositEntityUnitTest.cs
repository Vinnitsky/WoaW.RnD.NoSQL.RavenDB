using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Document;
using WoaW.RnD.RavenDB.UnitTests.Entities;

namespace WoaW.RnD.RavenDB.UnitTests 
{
    [TestClass]
    public class CompositEntityUnitTest
    {
        [TestMethod]
        public void SaveComposit_SuccessTest()
        {
            var id = Guid.NewGuid().ToString();

            //arrage
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var order = new Order() { Id = id, Title = "title" };
                    session.Store(order);
                    session.SaveChanges();
                }
            }

            //assert
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    //var order = (from o in session.Query<Order>() where o.Id == id select o).SingleOrDefault();//error
                    var order = session.Load<Order>(id);
                    Assert.AreEqual(0, order.Tasks.Count);
                }
            }

            //act
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var order = session.Load<Order>(id);
                    order.Tasks.Add(new Task() { Id = Guid.NewGuid().ToString(), Subject = "subject for task 1" });
                    //session.Store(order); //not requered
                    session.SaveChanges();
                }
            }

            //assert
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var order = session.Load<Order>(id);
                    Assert.AreEqual(1, order.Tasks.Count);
                }
            }

            //clear
            //using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            //{
            //    using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
            //    {
            //        session.Delete(id);
            //    }
            //}
        }

        [TestMethod]
        public void CreateOrder()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var order = new Order()
                    {
                        Id = "1",
                        Title = "Order 1",
                        Customer = new CustomerReference() { Id = "1", Title = "Mr. Who" }
                    };
                    session.Store(order);

                    var customer = new Customer() { Id = "1", Title = "Mr.", Name = "Who ", Email = "who@live.com" };
                    session.Store(order);
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
