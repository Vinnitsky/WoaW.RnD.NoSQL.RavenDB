using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoaW.RnD.RavenDB.UnitTests.Entities;

namespace WoaW.RnD.RavenDB.UnitTests 
{
    class TestHelpers
    {
        public static void CreateCustomers(int num)
        {

            var list = new List<Customer>();
            for (int i = 0; i < num; i++)
            {
                var customer = new Customer() { Id = string.Format("{0}", i), Title = string.Format("Mr. {0}", i), Name = string.Format("Name:{0}", i), Email = string.Format("{0}@live.com", i) };
                list.Add(customer);
            }

            var pgNo = 0;
            var pgSize = 30;
            var maxPgNo = num / pgSize;

            while (pgNo <= maxPgNo)
            {
                using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
                {
                    using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                    {
                        var records = GetPage(list, pgNo, pgSize);
                        foreach (var customer in records)
                        {
                            session.Store(customer);
                        }
                        session.SaveChanges();
                    }

                    //System.Diagnostics.Debug.WriteLine(string.Format("item:{0}", ii++));
                    //System.Diagnostics.Debug.WriteLine(string.Format("CUSTOMER: Id={0}, Name={1}, Email={2}",
                    //       customer.Id, customer.Name, customer.Email));
                }
                pgNo = pgNo + 1;
            }
        }
        public static IList<Customer> GetPage(IList<Customer> list, int page, int pageSize)
        {
            return list.Skip(page * pageSize).Take(pageSize).ToList();
        }
        internal static void DeleteAllCustomers()
        {
            throw new NotImplementedException();
        }
        public static void CreateOrderRecord(Raven.Client.IDocumentSession session)
        {
            if (session == null)
                throw new ArgumentNullException();

            var r = session.Load<Order>("order1");
            if (r != null)
                return;

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

        public static void DumpCustomers(IEnumerable<Customer> list)
        {
            foreach (var customer in list)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("CUSTOMER: Id={0}, Name={1}, Email={2}",
                    customer.Id, customer.Name, customer.Email));
            }
        }
        public static void DumpCustomers(IEnumerable<ShortCustomer> list)
        {
            foreach (var item in list)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("CUSTOMER: Id={0}, Name={1}", item.Id, item.Name));
            }
        }

        internal static void DumpOrders(List<Order> orders)
        {
            foreach (var item in orders)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ORDER: Id={0}, Title={1}", item.Id, item.Title));
                //foreach (var task in item)
                //{
                //    System.Diagnostics.Debug.WriteLine(string.Format("/t TASK: Id={0}, Subject={1}", task.Id, task.Subject));
                //}
            }
        }

        internal static void DumpOrders(List<Orders_OrdersCountByTaskSubj.ReduceResult> orders)
        {
            foreach (var item in orders)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ORDER: Id={0}, Title={1}", item.Count, item.Subj));
                //foreach (var task in item)
                //{
                //    System.Diagnostics.Debug.WriteLine(string.Format("/t TASK: Id={0}, Subject={1}", task.Id, task.Subject));
                //}
            }
        }

        internal static void DumpOrders(List<OrderCount> orders)
        {
            foreach (var item in orders)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ORDER: Id={0}, Title={1}", item.Count, item.Subj));
                //foreach (var task in item)
                //{
                //    System.Diagnostics.Debug.WriteLine(string.Format("/t TASK: Id={0}, Subject={1}", task.Id, task.Subject));
                //}
            }
        }
    }
}
