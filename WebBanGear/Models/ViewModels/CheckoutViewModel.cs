﻿using System.ComponentModel.DataAnnotations;
namespace Pet.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string PhoneNumber { get; set; }

        // Giữ nguyên các trường hiện có
        public List<CartItemModel> CartItems { get; set; }

        // Thêm trường Notes
        public string Notes { get; set; }

        // Thêm trường GrandTotal
        public decimal GrandTotal { get; set; }
    }
}
