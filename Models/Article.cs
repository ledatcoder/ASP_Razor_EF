using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace  ASP_Razor_EF.models
{
   public class Article
   {
        [Key]
        public int Id {set; get;}
        [StringLength(255)]
        [Required]
        [Column(TypeName="nvarchar")]
        public string Title {set; get;}
        [DataType(DataType.Date)]
        [Required]
        public DateTime Created {set; get;}
        [Column(TypeName ="ntext")]
        public string Content {set; get;}
   }
}