﻿using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace Pet.Models
{
	public class OrderModel
	{
		public int Id { get; set; }
		public string OrderCode { get; set; }
		public string UserName { get; set; }
		public DateTime CreateDate { get; set; }
		public int Status { get; set; }
		public AppUserModel User { get; set; }
		public List<OrderDetails> OrderDetails { get; set; }
	}
}
