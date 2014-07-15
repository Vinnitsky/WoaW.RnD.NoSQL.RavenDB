using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client;
using Raven.Client.Document;
using WoaW.RnD.RavenDB.UnitTests.Entities;
using System.Linq;

namespace WoaW.RnD.RavenDB.UnitTests
{
    /// <summary>
    /// класс создан для демонстрации реализации CRUD
    /// </summary>
    [TestClass]
    public class CRUDUnitTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        /// <summary>
        /// метод демонстрирует подключения. кстрока соединения увлючена в код.  
        /// в строке подключения используется IP адрес для того чтобы дать возможность 
        /// Fiddler прослушивать это соединение
        /// </summary>
        [TestMethod]
        [TestCategory("CRUD")]
        public void Connection_InCode_SuccessTest()
        {
            using (var store = new DocumentStore { Url = "http://127.0.0.1:8080/" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                }
            }
        }

        /// <summary>
        /// метод демонстрирует подключение с использоваине строки соединения в конфигурационном файла.
        /// </summary>
        [TestMethod]
        [TestCategory("CRUD")]
        public void Connection_UsingAppConfig_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {
                }
            }
        }

        /// <summary>
        /// метод демонстрирует ограничения Session на 30 запросов в рамках сессии 
        /// </summary>
        [TestMethod]
        [TestCategory("CRUD")]
        public void Connection_SessionLimit_SuccessTest()
        {
            using (var store = new DocumentStore { ConnectionStringName = "ravenDB" }.Initialize())
            {
                using (var session = store.OpenSession("WoaW.Raven.FirstApp"))
                {

                    TestHelpers.CreateCustomers(50);

                    var customers = from c in session.Query<Customer>() select c;
                    foreach (var item in customers)
                    {
                        var customer =  session.Query<Customer>().SingleOrDefault(c=>c.Id ==item.Id );
                        TestHelpers.DumpCustomer(customer);
                    }

                    TestHelpers.DeleteAllCustomers();
                }
            }
        }

        /// <summary>
        /// метод возвращает скисок кастомеров. метод расчитан на небольшое число возвращаемых эьектов
        /// так как RavenDB имеет предопределенное ограничение на запрос - 128 элементов, то для того 
        /// чтобы получить больше  нужно исопльзовать постраничный подход. в двльнейшем будет приведен 
        /// пример того как запросить и получить все элементы хранилища без пейджинга
        /// </summary>
        [TestMethod]
        [TestCategory("CRUD")]
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
        [TestCategory("CRUD")]
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

        /// <summary>
        /// метод демонстрирует создание документа 
        /// </summary>
        [TestMethod]
        [TestCategory("CRUD")]
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

        /// <summary>
        /// метод демонстрирует удаление документа с хранилища
        /// </summary>
        [TestMethod]
        [TestCategory("CRUD")]
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

        /// <summary>
        /// метод демоснтрирует обновление документа 
        /// </summary>
        [TestMethod]
        [TestCategory("CRUD")]
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
