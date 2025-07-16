using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Data
{
    public class TodoAppDbContext : DbContext
    {
        public TodoAppDbContext (DbContextOptions<TodoAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ReviewerReviewee テーブルの制約：
            // - ReviewerId と RevieweeId はどちらも一意.
            // - 自己レビューは禁止（ReviewerId ≠ RevieweeId）.
            // - 完全な1対1対応関係を実現.
            // - 削除時の制約：Reviewer または Reviewee が削除されたとき、関連する ReviewerReviewee レコードは削除されない(Restrict).
            // - そのため、消す処理を実装する必要がある. また、相手の Reviewer または Revieweeの Role情報も更新する必要がある.
            modelBuilder.Entity<ReviewerReviewee>(entity =>
            {
                entity.HasKey(rr => new { rr.ReviewerId, rr.RevieweeId });

                entity.HasOne(rr => rr.Reviewer)
                    .WithOne(u => u.AsReviewer)
                    .HasForeignKey<ReviewerReviewee>(rr => rr.ReviewerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rr => rr.Reviewee)
                    .WithOne(u => u.AsReviewee)
                    .HasForeignKey<ReviewerReviewee>(rr => rr.RevieweeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(rr => rr.ReviewerId).IsUnique();
                entity.HasIndex(rr => rr.RevieweeId).IsUnique();

                entity.ToTable(t => t.HasCheckConstraint("CK_ReviewerNotEqualReviewee", "[ReviewerId] <> [RevieweeId]"));
            });

        }


        public DbSet<Log> Logs { get; set; } = default!;
        public DbSet<MainTask> MainTasks { get; set; } = default!;
        public DbSet<SubTask> SubTasks { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<ReviewerReviewee> ReviewerReviewees { get; set; } = null!;


    }
}
