using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserDto
    {
        [Required]
        public string username {get;set;}
        
        [Required]
        [StringLength(8, MinimumLength=4, ErrorMessage="Specify password from 4 to 8 chars")]
        public string password {get;set;}
    }
}