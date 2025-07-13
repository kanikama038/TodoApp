using System.ComponentModel.DataAnnotations;
using TodoApp.Models;

namespace TodoApp.Models
{
    // ユーザのロールを定義.
    public enum UserRole
    {
        Master,     // 管理者.
        Reviewer,   // レビュワ.
        Reviewee,   // レビューイ.
        Other       // その他.
    }
    /*
     * ログイン機能にIdentityを使うなら、IdentityRoleを使うことも視野。

    using Microsoft.AspNetCore.Identity;
    public class ApplicationRole : IdentityRole
    {
        // 任意のロール追加処理
    }
     */

    public class User
    {
        [Key]
        public int Id { get; set; } // PK

        [Required(ErrorMessage = "ユーザ名は必須項目です。")]
        [Display(Name = "ユーザ名")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "ユーザ名は1～100文字以内で入力してください。")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "メールドレスは必須項目です。")]
        [Display(Name = "メールアドレス")]
        [EmailAddress(ErrorMessage = "無効なアドレスです。")]
        [StringLength(100, ErrorMessage = "メールアドレスは100文字以内で入力してください。")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "パスワードは必須項目です。")]
        [Display(Name = "パスワード")]
        [DataType(DataType.Password)]
        [StringLength(70, ErrorMessage = "パスワードの値が不正です。")]
        // 以下、Modelバリデーションチェック用.
        //[StringLength(100, ErrorMessage = "パスワードは100文字以内で入力してください。")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", ErrorMessage = "パスワードは8文字以上で、少なくとも1つの英大文字、1つの英小文字、1つの数字を含めてください。")]
        public string HashedPassword { get; set; } = ""; // SHA-256.

        [Required(ErrorMessage = "ロールは必須項目です。")]
        [Display(Name = "ロール")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "無効なロールです。")]
        [Range(0, 3, ErrorMessage = "ロールの値が不正です。")]
        public UserRole Role { get; set; } // (0:Master, 1:Reviewer, 2:Reviewee, 3:other).

        [Display(Name = "備考")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, ErrorMessage = "備考は1000文字以内で入力してください。")]
        public string? Description { get; set; }



        [Display(Name = "作成日時")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Display(Name = "最終更新日時")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }
        [Display(Name = "最終ログイン日時")]
        [DataType(DataType.DateTime)]
        public DateTime? LoggedInedAt { get; set; }
        [Display(Name = "最終ログアウト日時")]
        [DataType(DataType.DateTime)]
        public DateTime? LoggedOutedAt { get; set; }


        public ReviewerReviewee? AsReviewer { get; set; }　// ナビゲーションプロパティ: レビュワとしてレビューイの情報を辿れる(Viewに渡す際はInclude()する).
        public ReviewerReviewee? AsReviewee { get; set; } // ナビゲーションプロパティ: レビューイとしてレビュワの情報を辿れる.

    }
}
