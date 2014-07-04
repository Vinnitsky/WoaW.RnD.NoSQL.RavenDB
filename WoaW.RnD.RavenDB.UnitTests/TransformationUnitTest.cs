using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Indexes;
using WoaW.RnD.RavenDB.UnitTests.Entities;
using Raven.Client.Document;
using Raven.Abstractions.Indexing;

namespace WoaW.RnD.RavenDB.UnitTests
{ 
    public class ShortCustomer  
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    /// <summary>
    /// server side transformation
    /// </summary>
    [TestClass]
    public class TransformationUnitTest
    {

        public class IdAndNamesOnly : AbstractTransformerCreationTask<Customer>
        {
            public IdAndNamesOnly()
            {
                //TransformResults = customers =>
                //    from customer in customers select new { customer.Id, customer.Name };
                TransformResults = customers =>
                    from customer in customers
                    let category = LoadDocument<Customer>(customer.Id)
                        select new { customer.Id, customer.Name };
            }
        }
        [TestMethod]
        public void Transformation_Insert_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                IndexCreation.CreateIndexes(typeof(IdAndNamesOnly).Assembly, store);
            }
        }
        /// <summary>
        /// метод возвращает все поля документа
        /// </summary>
        [TestMethod]
        public void Query_AllFields_Document_SuccessTest()
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
        /// <summary>
        /// НЕ РАБОТАЕТ
        /// метод должен возвращать несколько полей при помощи трансформации, но это не работае 
        /// </summary>
        [TestMethod]
        public void Query_WithTransform_PartialDocument_V1_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    IndexCreation.CreateIndexes(typeof(IdAndNamesOnly).Assembly, store);
                    //session.SaveChanges();

                    var customers = session.Query<Customer>()
                        .TransformWith<IdAndNamesOnly, Customer>()
                        .ToList();

                    TestHelpers.DumpCustomers(customers);
                }
            }
        }
        [TestMethod]
        public void Query_WithTransform_PartialDocument_V2_SuccessTest()
        {
            //https://groups.google.com/forum/#!topic/ravendb/ooJAZ0Io4ls
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                //store.ExecuteTransformer(new IdAndNamesOnly());
                //new IdAndNamesOnly().Execute(store);
                //IndexCreation.CreateIndexes(typeof(IdAndNamesOnly).Assembly, store);
                //store.DatabaseCommands.PutTransformer("", new TransformerDefinition());

                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                    var customers = session.Query<Customer>()
                        .TransformWith<IdAndNamesOnly, ShortCustomer>()
                        .ToList();

                    TestHelpers.DumpCustomers(customers);
                }
            }
        }
        /// <summary>
        /// метод возвращет два поля не используя кастомного индекса,  
        /// по сети передается только указанные поля 
        /// </summary>
        [TestMethod]
        public void Query_PartialDocument_V3_SuccessTest()
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
        public void Query_PartialDocument_V4_SuccessTest()
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

        /// <summary>
        /// add to the docuemnt referenced documents
        /// </summary>
        [TestMethod]
        public void Query_LinkedDocument_SuccessTest()
        {

        }

        [TestMethod]
        public void Query_LinkedDocument_FailTest()
        {

        }
    }
}
