using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class SubTask
    {
        [Key]
        public int Id { get; set; } // PK

        [Required]
        public int MainTaskId { get; set; } // FK to MainTask

        [Required(ErrorMessage = "小タスク名は必須項目です。")]
        [Display(Name = "小タスク名")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "小タスク名は1～255文字以内で入力してください。")]
        public string Title { get; set; }

        [Display(Name = "備考")]
        [DataType(DataType.MultilineText)]
        [StringLength(1023, ErrorMessage = "備考は1023文字以内で入力してください。")]
        public string? Description { get; set; }


        [Display(Name = "作成時刻")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Display(Name = "更新時刻")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }
        [Display(Name = "完了時刻")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedAt { get; set; }
        [Display(Name = "削除時刻")]
        [DataType(DataType.DateTime)]
        public DateTime? DeletedAt { get; set; }
        public bool IsCompleted { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }
}
