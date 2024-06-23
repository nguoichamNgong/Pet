using Pet.Models;
using System.Collections.Generic;

namespace Pet.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderModel Order { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
