using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.UserService.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("(?=^.*[a-z])(?=.[A-Z])(?=.*[!@#$%&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s)*$",
        ErrorMessage =" password must have 1 upper case, 1 lower case, 1 number, 1 non alphanumeric and at least 6 characters")]
        public string Password { get; set; }
    }
}
