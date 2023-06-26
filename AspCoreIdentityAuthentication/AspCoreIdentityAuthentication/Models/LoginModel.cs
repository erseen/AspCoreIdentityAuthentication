using System.ComponentModel.DataAnnotations;

namespace AspCoreIdentityAuthentication.Models
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        //public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
       
    }
}
