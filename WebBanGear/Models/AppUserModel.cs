using Microsoft.AspNetCore.Identity;

namespace Pet.Models
{
	public class AppUserModel : IdentityUser
	{
		public string Occupation {  get; set; } // nghề nghiệp
	}
}
	