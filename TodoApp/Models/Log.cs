using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; } // PK.

        [Required]
        public int UserId { get; set; } // FK: User.Id.

        [Required]
        [Display(Name = "作成日時")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "内容")]
        [DataType(DataType.MultilineText)]
        public string? Details { get; set; }
        
    }
}
