using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WoaW.RnD.RavenDB.UnitTests.Entities
{
    public class Order
    {
        public Order()
        {
            Tasks = new List<Task>();
        }
        public string Id { get; set; } 
        public string Title { get; set; }
        public CustomerReference Customer { get; set; }
        public List<Task> Tasks { get; set; }

    }
}
