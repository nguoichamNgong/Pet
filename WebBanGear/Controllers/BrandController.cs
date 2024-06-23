﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet.Models;
using Pet.Repository;

namespace Pet.Controllers
{
	public class BrandController : Controller
	{
		private readonly DataContext _dataContext;
		public BrandController(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IActionResult> Index(String Slug = "")
		{
			BrandModel brand = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();

			if (brand == null) return RedirectToAction("Index");

			var productByBrand = _dataContext.Products.Where(p => p.BrandId == brand.Id);

			return View(await productByBrand.OrderByDescending(p => p.Id).ToListAsync());
		}
	}
}
