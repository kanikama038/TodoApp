using System.ComponentModel.DataAnnotations;
using TodoApp.Models;

public class UserEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ユーザ名は必須です。")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "ユーザ名は1〜100文字で入力してください。")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "メールアドレスは必須です。")]
    [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください。")]
    [StringLength(100, ErrorMessage = "メールアドレスは100文字以内で入力してください。")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "ロールは必須です。")]
    public UserRole Role { get; set; }

    public string? RevieweeEmail { get; set; }
    public string? ReviewerEmail { get; set; }
}
