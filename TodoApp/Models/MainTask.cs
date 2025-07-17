using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class MainTask : IValidatableObject
    {
        [Key]
        public int Id { get; set; } // PK.

        [Required]
        public int UserId { get; set; } // FK: User.Id.

        [Required(ErrorMessage = "大タスク名は必須項目です。")]
        [Display(Name = "大タスク名")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "大タスク名は1～100文字以内で入力してください。")]
        public string Title { get; set; } = "";

        [Required]
        [Display(Name = "進捗度")]
        [Range(0, 100, ErrorMessage = "進捗度の値が不正です。")]
        public int Progress { get; set; } // Rank計算用.進捗度を視覚的に表現するのもアリ.

        [Required(ErrorMessage = "緊急度は必須項目です。")]
        [Display(Name = "緊急度")]
        [Range(1, 3, ErrorMessage = "緊急度の値が不正です。")]
        public int Urgency { get; set; } // (0:低, 1:中, 2:高).

        [Required(ErrorMessage = "重要度は必須項目です。")]
        [Display(Name = "重要度")]
        [Range(1, 3, ErrorMessage = "重要度の値が不正です。")]
        public int Importance { get; set; } // (0:低, 1:中, 2:高).

        [Required(ErrorMessage = "開始日時は必須項目です。")]
        [Display(Name = "開始日時")]
        [DataType(DataType.DateTime)]
        public DateTime StartedAt { get; set; }

        [Required(ErrorMessage = "終了日時は必須項目です。")]
        [Display(Name = "終了日時")]
        [DataType(DataType.DateTime)]
        public DateTime EndedAt { get; set; } // 期日.

        // 開始・終了日時のカスタムバリデーション.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            // 開始日時が現在時刻以降であることをチェック.
            if (StartedAt.Date < DateTime.Today)
            {
                result.Add(new ValidationResult(
                    "開始日時は現在時刻以降の日時を指定してください。",
                    new[] { nameof(StartedAt) }
                ));
            }
            // 終了日時が開始日時以降であることをチェック.
            if (EndedAt < StartedAt)
            {
                result.Add(new ValidationResult(
                    "終了日時は開始日時以降の日時を指定してください。",
                    new[] { nameof(EndedAt) }
                ));
            }
            return result;
        }

        [Display(Name = "備考")]
        [StringLength(1000, ErrorMessage = "備考は1000文字以内で入力してください。")]
        public string? Description { get; set; }

        [Display(Name = "完了")]
        public bool IsCompleted { get; set; } = false; // チェックボックス用の完了フラグ.



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
