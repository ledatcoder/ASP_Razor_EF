using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace  ASP_Razor_EF.models
{
   public class Article
   {
        [Key]
        public int Id {set; get;}
        [StringLength(255,MinimumLength =5,ErrorMessage ="Faild!")]
        [Required]
        [Column(TypeName="nvarchar")]
        [DisplayName("Title(Tiêu đề)")]
        public string Title {set; get;}
        [DataType(DataType.Date)]
        [Required]
        [DisplayName("Day Create(Ngày Tạo)")]
        public DateTime Created {set; get;}
        [Column(TypeName ="ntext")]
        [DisplayName("Content(Nội dung)")]
        public string Content {set; get;}
   }
}