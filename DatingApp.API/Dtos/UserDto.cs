using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserDto
    {
        [Required]
        public string username {get;set;}
        
        [Required]
        [StringLength(10, MinimumLength=4, ErrorMessage="Specify password from 4 to 10 chars")]
        public string password {get;set;}

        [Required]
        public string knownAs {get;set;}

        [Required]
        public DateTime dateOfBirth {get;set;}

        [Required]
        public string city {get;set;}

        [Required]
        public string country {get;set;}

        [Required]
        public DateTime createdTime {get;set;}

        [Required]
        public DateTime lastActive {get;set;}

        public UserDto (){
            this.createdTime = DateTime.Now;
            this.lastActive = DateTime.Now;
        }
    }
}