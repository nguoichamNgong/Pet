using System.ComponentModel.DataAnnotations.Schema;

namespace Pet.Models
{
	public class OrderDetails
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string OrderCode { get; set; }
		public long ProductId { get; set; }
		public decimal Price { get; set; }	

		public int Quantity { get; set; }
        public string FullName { get; set; } 
        public string ShippingAddress { get; set; }
        public string Notes { get; set; }
        public string PhoneNumber { get; set; }
        public OrderModel Order { get; set; }
        public ProductModel Product { get; set; }

	}
}	
