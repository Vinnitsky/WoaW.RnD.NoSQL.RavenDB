using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client;
using Raven.Client.Document;
using WoaW.RnD.RavenDB.UnitTests.Entities;
using System.Linq;

namespace WoaW.RnD.RavenDB.UnitTests 
{
    [TestClass]
    public class CRUDUnitTest
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Connection_InCode_SuccessTest()
        {
            using (var store = new DocumentStore { Url = "http://localhost:8080/" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    //var books = session.Query<Book>().ToList();
                }
            }
        }
        [TestMethod]
        public void Connection_UsingAppConfig_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    //var books = session.Query<Book>().ToList();
                }
            }
        }

        [TestMethod]
        public void EnumerateCustomers_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var customers = session.Query<Customer>().ToList();
                    TestHelpers.DumpCustomers(customers);
                }
            }
        }
        [TestMethod]
        public void GetElementById_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var customer = session.Load<Customer>("customers/1");
                    System.Diagnostics.Debug.WriteLine(string.Format("CUSTOMER: Id={0}, Name={1}, Email={2}",
                        customer.Id, customer.Name, customer.Email));
                }
            }
        }
        [TestMethod]
        public void CreateNewCustomer_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var customer = new Customer() { Id = 1.ToString(), Title = "Mr.", Name = "Who ", Email = "who@live.com" };
                    session.Store(customer);
                    session.SaveChanges();
                }
            }
        }
        [TestMethod]
        public void DeleteCustomer_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    session.Delete(1);
                    session.SaveChanges();
                }
            }
        }        
        [TestMethod]
        public void Update_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var customer = session.Load<Customer>("customers/1");
                    System.Diagnostics.Debug.WriteLine(string.Format("CUSTOMER before update: Id={0}, Title={1}, Name={2}, Email={3}",
                        customer.Id, customer.Title, customer.Name, customer.Email));

                    customer.Title = "PhD";
                    session.SaveChanges();

                    System.Diagnostics.Debug.WriteLine(string.Format("CUSTOMER after update: Id={0}, Title={1}, Name={2}, Email={3}",
                        customer.Id, customer.Title, customer.Name, customer.Email));
                }
            }
        }
    }
}
