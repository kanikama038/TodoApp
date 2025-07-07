using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public enum UserRole
    {
        Admin,
        Reviewer,
        Reviewee,
        Other
    }

    /*
     * IdentityRoleを使うことも可能。

    using Microsoft.AspNetCore.Identity;
    public class ApplicationRole : IdentityRole
    {
        // 任意のロール追加処理
    }

     */

    /*
     * レビュワとレビューイの関係をUserテーブルで管理するか、別でテーブルを作成するか。
     * 新たに作ったほうがよさそう。
     */

    public class User
    {
        [Key]
        public int Id { get; set; } // PK

        [Required(ErrorMessage = "ユーザ名は必須項目です。")]
        [Display(Name = "ユーザ名")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "ユーザ名は1～255文字以内で入力してください。")]
        public string Name { get; set; }

        [Required(ErrorMessage = "メールドレスは必須項目です。")]
        [Display(Name = "メールアドレス")]
        [EmailAddress(ErrorMessage = "無効なアドレスです。")]
        [StringLength(255, ErrorMessage = "メールアドレスは255文字以内で入力してください。")]
        public string Email { get; set; }

        [Required(ErrorMessage = "パスワードは必須項目です。")]
        [Display(Name = "パスワード")]
        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "パスワードは30文字以内で入力してください。")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", ErrorMessage = "パスワードは8文字以上で、少なくとも1つの英大文字、1つの英小文字、1つの数字を含めてください。")]
        public string Password { get; set; } // In a real application, consider hashing passwords

        [Required(ErrorMessage = "ロールは必須項目です。")]
        [Display(Name = "ロール")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "無効なロールです。")]
        [Range(0, 3, ErrorMessage = "ロールは有効な整数(0～3)を入力してください。")]
        public UserRole Role { get; set; } // User role (Admin, Reviewer, Reviewee, other)

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

    }
}
