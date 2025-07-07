using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; } // PK

        [Required]
        public int UserId { get; set; } // FK to User

        [Required]
        [Display(Name = "更新時刻")]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; } = DateTime.Now; // When the action was performed

        [Display(Name = "操作")]
        [DataType(DataType.Text)]
        public string? Action { get; set; } // Action performed (e.g., "Create", "Update", "Delete")

        [Display(Name = "詳細")]
        [DataType(DataType.MultilineText)]
        public string? Details { get; set; } // Additional details about the action
        
    }
}
