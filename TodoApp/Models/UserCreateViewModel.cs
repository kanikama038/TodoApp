using System.ComponentModel.DataAnnotations;
using TodoApp.Models; // UserRole enumのため

public class UserCreateViewModel
{
    [Required(ErrorMessage = "ユーザ名は必須項目です。")]
    [Display(Name = "ユーザ名")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "ユーザ名は1～100文字以内で入力してください。")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "メールドレスは必須項目です。")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "無効なアドレスです。")]
    [StringLength(100, ErrorMessage = "メールアドレスは100文字以内で入力してください。")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "パスワードは必須です。")]
    [Display(Name = "パスワード")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "パスワードは8〜100文字で入力してください。")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[!-~]{8,100}$",
        ErrorMessage = "パスワードは大文字・小文字・数字・記号を含めてください。")]
    public string Password { get; set; } = "";


    [Required(ErrorMessage = "役割は必須項目です。")]
    [Display(Name = "役割")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "無効な役割です。")]
    [Range(0, 3, ErrorMessage = "役割の値が不正です。")]
    public UserRole Role { get; set; }

    // Reviewerのとき必須（ただし属性は使わずコントローラで手動チェック）
    [Display(Name = "レビューイ（Email）")]
    public string? RevieweeEmail { get; set; }

    // Revieweeのとき必須（属性は使わずコントローラで手動チェック）
    [Display(Name = "レビュワ（Email）")]
    public string? ReviewerEmail { get; set; }
}

