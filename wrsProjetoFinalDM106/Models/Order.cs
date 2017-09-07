using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wrsProjetoFinalDM106.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }

        public string userEmail { get; set; }

        public DateTime orderDate { get; set; }

        public DateTime deliveryDate { get; set; }

        public string status { get; set; }

        public decimal orderPrice { get; set; }

        public int orderWeight { get; set; }

        public decimal freightPrice { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}