using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class SubTask
    {
        [Key]
        public int Id { get; set; } // PK.

        [Required]
        public int MainTaskId { get; set; } // FK: MainTask.Id.

        [Required(ErrorMessage = "小タスク名は必須項目です。")]
        [Display(Name = "小タスク名")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "小タスク名は1～100文字以内で入力してください。")]
        public string Title { get; set; }

        [Required(ErrorMessage = "終了日時は必須項目です。")]
        [Display(Name = "終了日時")]
        [DataType(DataType.DateTime)]
        public DateTime EndedAt { get; set; } // 期日.

        // 終了日時のカスタムバリデーション.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            // 終了日時が現在時刻以降であることをチェック.
            if (EndedAt.Date < DateTime.Today)
            {
                result.Add(new ValidationResult(
                    "開始日時は現在時刻以降の日時を指定してください。",
                    new[] { nameof(EndedAt) }
                ));
            }

            return result;
        }

        [Display(Name = "詳細")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, ErrorMessage = "詳細は1000文字以内で入力してください。")]
        public string? Description { get; set; }


        [Display(Name = "作成日時")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Display(Name = "更新日時")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }
        [Display(Name = "完了日時")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedAt { get; set; }
        [Display(Name = "削除日時")]
        [DataType(DataType.DateTime)]
        public DateTime? DeletedAt { get; set; }
    }
}
