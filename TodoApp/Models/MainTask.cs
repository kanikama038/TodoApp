using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class MainTask : IValidatableObject
    {
        [Key]
        public int Id { get; set; } // PK

        [Required]
        public int UserId { get; set; } // FK to User

        [Required(ErrorMessage = "タスク名は必須項目です。")]
        [Display(Name = "タスク名")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "タスク名は1～255文字以内で入力してください。")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "進捗")]
        [Range(0.0, 100.0, ErrorMessage = "Progress rate takes 0-100.")]
        public double Progress { get; set; } // Progress percentage (0-100)

        [Required(ErrorMessage = "優先度は必須項目です。")]
        [Display(Name = "優先度")]
        [Range(0, 255, ErrorMessage = "Priority takes 0-255.")]
        public int Priority { get; set; } // Priority level (0:Low, 1:Medium, 2～:High)

        [Required(ErrorMessage = "開始時刻は必須項目です。")]
        [Display(Name = "開始時刻")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "終了時刻は必須項目です。")]
        [Display(Name = "終了時刻")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; } // Due date for the task

        // カスタムバリデーション
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            // 開始時刻が現時刻以降であることをチェック
            if (StartTime.Date < DateTime.Today)
            {
                result.Add(new ValidationResult(
                    "開始日は今日以降の日付を指定してください",
                    new[] { nameof(StartTime) }
                ));
            }
            // 終了時刻が開始時刻以降であることをチェック
            if (EndTime < StartTime)
            {
                result.Add(new ValidationResult(
                    "終了時刻は開始時刻以降の時刻を入力してください。",
                    new[] { nameof(EndTime) }
                ));
            }
            return result;
        }

        [Display(Name = "備考")]
        [StringLength(1024, ErrorMessage = "備考は1024文字以内で入力してください。")]
        public string? Description { get; set; }



        [Display(Name = "作成日")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Display(Name = "最終更新日")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }
        [Display(Name = "完了日")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedAt { get; set; }
        [Display(Name = "削除日")]
        [DataType(DataType.DateTime)]
        public DateTime? DeletedAt { get; set; }
        public bool IsCompleted { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }
}
