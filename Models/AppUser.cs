using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace App.Models
{
    public class AppUser : IdentityUser
    {
        [DataType(DataType.Date)]
        public DateTime? BirthDate {set; get;}
    }
}