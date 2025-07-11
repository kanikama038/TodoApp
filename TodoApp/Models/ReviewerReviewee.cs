using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Models
{
    public class ReviewerReviewee
    {
        // 複合主キー及びUNIQUE制約は、DbContextファイルのOnModelCreatingメソッドで設定する.
        public int ReviewerId { get; set; } // PK, FK: User.Id, UNIQUE.

        public int RevieweeId { get; set; } // PK, FK: User.Id, UNIQUE.

        public User? Reviewer { get; set; } // ナビゲーションプロパティ: レビュワの情報(Viewに渡す際はInclude()する).
        public User? Reviewee { get; set; } // ナビゲーションプロパティ: レビューイの情報.
    }
}