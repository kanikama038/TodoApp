using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    public class MasterController : Controller
    {
        private readonly TodoAppDbContext _context;

        public MasterController(TodoAppDbContext context)
        {
            _context = context;
        }

        // 一覧表示
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            // 総件数の取得
            var totalUsers = await _context.Users.CountAsync();

            // ページングしてユーザー取得（関連情報もInclude）
            var users = await _context.Users
                .Include(u => u.AsReviewer)
                    .ThenInclude(rr => rr.Reviewee)
                .Include(u => u.AsReviewee)
                    .ThenInclude(rr => rr.Reviewer)
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ViewModelに詰めてビューへ
            var viewModel = new UserListViewModel
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize)
            };

            return View(viewModel);
        }


        // ハッシュ化メソッド（SHA-256）
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // 新規登録
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            // まず基本的な属性バリデーションをチェック
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ここでRoleによるrevieweeEmail/reviewerEmailの必須チェック（手動）
            if (model.Role == UserRole.Reviewer && string.IsNullOrWhiteSpace(model.RevieweeEmail))
            {
                ModelState.AddModelError("RevieweeEmail", "レビューイのメールアドレスは必須です。");
            }
            else if (model.Role == UserRole.Reviewer)
            {
                // 形式チェックや存在チェックなど
                if (!Regex.IsMatch(model.RevieweeEmail!, @"^[\x21-\x7E]{1,100}$"))
                    ModelState.AddModelError("RevieweeEmail", "レビューイのメールアドレスは半角100文字以下で入力してください。");
                else
                {
                    var reviewee = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.RevieweeEmail);
                    if (reviewee == null)
                        ModelState.AddModelError("RevieweeEmail", "指定されたレビューイが存在しません。先に登録してください。");
                }
            }

            if (model.Role == UserRole.Reviewee && string.IsNullOrWhiteSpace(model.ReviewerEmail))
            {
                ModelState.AddModelError("ReviewerEmail", "レビュワのメールアドレスは必須です。");
            }
            else if (model.Role == UserRole.Reviewee)
            {
                if (!Regex.IsMatch(model.ReviewerEmail!, @"^[\x21-\x7E]{1,100}$"))
                    ModelState.AddModelError("ReviewerEmail", "レビュワのメールアドレスは半角100文字以下で入力してください。");
                else
                {
                    var reviewer = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.ReviewerEmail);
                    if (reviewer == null)
                        ModelState.AddModelError("ReviewerEmail", "指定されたレビュワが存在しません。先に登録してください。");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // パスワードハッシュ化などの処理
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                HashedPassword = HashPassword(model.Password),
                Role = model.Role,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (model.Role == UserRole.Reviewer)
            {
                var reviewee = await _context.Users.FirstAsync(u => u.Email == model.RevieweeEmail);
                _context.ReviewerReviewees.Add(new ReviewerReviewee
                {
                    ReviewerId = user.Id,
                    RevieweeId = reviewee.Id
                });
                await _context.SaveChangesAsync();
            }
            else if (model.Role == UserRole.Reviewee)
            {
                var reviewer = await _context.Users.FirstAsync(u => u.Email == model.ReviewerEmail);
                _context.ReviewerReviewees.Add(new ReviewerReviewee
                {
                    ReviewerId = reviewer.Id,
                    RevieweeId = user.Id
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // 削除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // 関連データの取得
            var relatedPairs = await _context.ReviewerReviewees
                .Where(r => r.ReviewerId == user.Id || r.RevieweeId == user.Id)
                .ToListAsync();

            foreach (var pair in relatedPairs)
            {
                // 自分がレビュワなら、相手（レビューイ）を Other にする
                if (pair.ReviewerId == user.Id)
                {
                    var reviewee = await _context.Users.FindAsync(pair.RevieweeId);
                    if (reviewee != null)
                    {
                        reviewee.Role = UserRole.Other;
                        reviewee.UpdatedAt = DateTime.Now;
                        _context.Update(reviewee);
                    }
                }

                // 自分がレビューイなら、相手（レビュワ）を Other にする
                if (pair.RevieweeId == user.Id)
                {
                    var reviewer = await _context.Users.FindAsync(pair.ReviewerId);
                    if (reviewer != null)
                    {
                        reviewer.Role = UserRole.Other;
                        reviewer.UpdatedAt = DateTime.Now;
                        _context.Update(reviewer);
                    }
                }
            }

            // 関連情報削除
            _context.ReviewerReviewees.RemoveRange(relatedPairs);

            // ユーザー削除
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // 編集
        // 編集画面表示
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.AsReviewer)
                .Include(u => u.AsReviewee)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                RevieweeEmail = user.AsReviewer?.Reviewee?.Email,
                ReviewerEmail = user.AsReviewee?.Reviewer?.Email
            };

            return View(model);
        }

        // 編集内容の保存
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            // Role に応じた revieweeEmail/reviewerEmail のバリデーション
            if (model.Role == UserRole.Reviewer && string.IsNullOrWhiteSpace(model.RevieweeEmail))
            {
                ModelState.AddModelError("RevieweeEmail", "レビューイのメールアドレスは必須です。");
            }
            else if (model.Role == UserRole.Reviewer)
            {
                if (!Regex.IsMatch(model.RevieweeEmail!, @"^[\x21-\x7E]{1,100}$"))
                    ModelState.AddModelError("RevieweeEmail", "レビューイのメールアドレスは半角100文字以下で入力してください。");
                else if (!await _context.Users.AnyAsync(u => u.Email == model.RevieweeEmail))
                    ModelState.AddModelError("RevieweeEmail", "指定されたレビューイが存在しません。先に登録してください。");
            }

            if (model.Role == UserRole.Reviewee && string.IsNullOrWhiteSpace(model.ReviewerEmail))
            {
                ModelState.AddModelError("ReviewerEmail", "レビュワのメールアドレスは必須です。");
            }
            else if (model.Role == UserRole.Reviewee)
            {
                if (!Regex.IsMatch(model.ReviewerEmail!, @"^[\x21-\x7E]{1,100}$"))
                    ModelState.AddModelError("ReviewerEmail", "レビュワのメールアドレスは半角100文字以下で入力してください。");
                else if (!await _context.Users.AnyAsync(u => u.Email == model.ReviewerEmail))
                    ModelState.AddModelError("ReviewerEmail", "指定されたレビュワが存在しません。先に登録してください。");
            }

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Email = model.Email;
            user.Role = model.Role;
            user.UpdatedAt = DateTime.Now;

            // リンク削除（既存のReviewerRevieweeを一旦削除）
            var existingLink = await _context.ReviewerReviewees
                .FirstOrDefaultAsync(r => r.ReviewerId == user.Id || r.RevieweeId == user.Id);
            if (existingLink != null)
            {
                _context.ReviewerReviewees.Remove(existingLink);
                await _context.SaveChangesAsync();
            }

            // 新しいリンク追加
            if (model.Role == UserRole.Reviewer)
            {
                var reviewee = await _context.Users.FirstAsync(u => u.Email == model.RevieweeEmail);
                _context.ReviewerReviewees.Add(new ReviewerReviewee
                {
                    ReviewerId = user.Id,
                    RevieweeId = reviewee.Id
                });
            }
            else if (model.Role == UserRole.Reviewee)
            {
                var reviewer = await _context.Users.FirstAsync(u => u.Email == model.ReviewerEmail);
                _context.ReviewerReviewees.Add(new ReviewerReviewee
                {
                    ReviewerId = reviewer.Id,
                    RevieweeId = user.Id
                });
            }

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // 一括登録
        public IActionResult BulkCreate()
        {
            return View();
        }










        //    public async Task<IActionResult> Details(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var user = await _context.Users
        //            .FirstOrDefaultAsync(u => u.Id == id);
        //        if (user == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(user);
        //    }

        //    public IActionResult Create()
        //    {
        //        return View();
        //    }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create(User user)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(user);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        return View(user);
        //    }

        //    public async Task<IActionResult> Edit(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var user = await _context.Users.FindAsync(id);
        //        if (user == null)
        //        {
        //            return NotFound();
        //        }
        //        return View(user);
        //    }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(int id, User user)
        //    {
        //        if (id != user.Id)
        //        {
        //            return NotFound();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                _context.Update(user);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!MainTaskExists(user.Id))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //        return View(user);

        //    }

        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var user = await _context.Users
        //            .FirstOrDefaultAsync(u => u.Id == id);
        //        if (user == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(user);
        //    }

        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int id)
        //    {
        //        var user = await _context.Users.FindAsync(id);
        //        if (user != null)
        //        {
        //            _context.Users.Remove(user);
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    private bool MainTaskExists(int id)
        //    {
        //        return _context.Users.Any(e => e.Id == id);
        //    }
    }
}