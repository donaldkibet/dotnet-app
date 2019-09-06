using System.ComponentModel.DataAnnotations;

namespace DotnetAPI.Dots
{
    public class UserForRegisterDto
    {
        [Required]
        public string username  { get; set; }  

        [Required] 
        [StringLength(50,MinimumLength = 8, ErrorMessage =" Your password must be at least 8 characters")]      
        public string password { get; set; }
    }
}