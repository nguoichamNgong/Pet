using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pet.Repository.Validation;

namespace Pet.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

		[Required(ErrorMessage = "Yêu Cầu Nhập Tên Pet")]
		public string Name { get; set; }

        public string Slug { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu Cầu Nhập Mô Tả Pet")]
		public string Description { get; set; }

        [Range(0.01, 100000000, ErrorMessage = "Giá sản phẩm phải từ 0.01 đến 100,000,000")]
        [Required(ErrorMessage = "Yêu cầu nhập giá Sản phẩm")]
        [Column(TypeName = "decimal(20,2)")]
        public decimal Price { get; set; }

		[Required, Range(1,int.MaxValue, ErrorMessage = "Chọn pet")]

		public int BrandId { get; set; }

		[Required, Range(1, int.MaxValue, ErrorMessage = "Chọn Loài")]
		public int  CategoryId { get; set;}
        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
		public string Image { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
	}
}
