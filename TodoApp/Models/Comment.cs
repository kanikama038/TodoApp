using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; } // PK.

        [Required]
        public int RevieweeId { get; set; } // FK: User.Id.

        [Required]
        public int ReviewerId { get; set; } // FK: User.Id.

        [Required(ErrorMessage = "本文は必須項目です。")]
        [Display(Name = "本文")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "本文は1～1000文字以内で入力してください。")]
        public string Content { get; set; } = "";


        [Required]
        [Display(Name = "作成日時")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "更新日時")]
        [DataType(DataType.DateTime)]
        public DateTime? UpDatedAt { get; set; }
        [Required]
        [Display(Name = "削除日時")]
        [DataType(DataType.DateTime)]
        public DateTime? DeletedAt { get; set; }

    }
}
