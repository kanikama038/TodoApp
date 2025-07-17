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
            var totalUsers = await _context.Users.CountAsync();

            var users = await _context.Users
                .Include(u => u.Reviewee)
                .Include(u => u.Reviewer)
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new UserListViewModel
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize)
            };

            return View(viewModel);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // 新規登録画面
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Role == UserRole.Reviewer && string.IsNullOrWhiteSpace(model.RevieweeEmail))
            {
                ModelState.AddModelError("RevieweeEmail", "レビューイのメールアドレスは必須です。");
            }
            else if (model.Role == UserRole.Reviewer)
            {
                if (!Regex.IsMatch(model.RevieweeEmail!, @"^[\x21-\x7E]{1,100}$"))
                    ModelState.AddModelError("RevieweeEmail", "レビューイのメールアドレスは半角100文字以下で入力してください。");
                else if (!await _context.Users.AnyAsync(u => u.Email == model.RevieweeEmail))
                    ModelState.AddModelError("RevieweeEmail", "指定されたレビューイが存在しません。");
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
                    ModelState.AddModelError("ReviewerEmail", "指定されたレビュワが存在しません。");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

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

            // 関係設定
            if (model.Role == UserRole.Reviewer)
            {
                var reviewee = await _context.Users.FirstAsync(u => u.Email == model.RevieweeEmail);
                user.RevieweeId = reviewee.Id;
                _context.Update(user);
            }
            else if (model.Role == UserRole.Reviewee)
            {
                var reviewer = await _context.Users.FirstAsync(u => u.Email == model.ReviewerEmail);
                reviewer.RevieweeId = user.Id;
                _context.Update(reviewer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 編集画面
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.Reviewee)
                .Include(u => u.Reviewer)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                RevieweeEmail = user.Reviewee?.Email,
                ReviewerEmail = user.Reviewer?.Email
            };

            return View(model);
        }

        // 編集保存
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditViewModel model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            // 関係の妥当性チェック
            if (model.Role == UserRole.Reviewer && string.IsNullOrWhiteSpace(model.RevieweeEmail))
            {
                ModelState.AddModelError("RevieweeEmail", "レビューイのメールアドレスは必須です。");
            }
            else if (model.Role == UserRole.Reviewer)
            {
                if (!Regex.IsMatch(model.RevieweeEmail!, @"^[\x21-\x7E]{1,100}$"))
                    ModelState.AddModelError("RevieweeEmail", "レビューイのメールアドレスは半角100文字以下で入力してください。");
                else if (!await _context.Users.AnyAsync(u => u.Email == model.RevieweeEmail))
                    ModelState.AddModelError("RevieweeEmail", "指定されたレビューイが存在しません。");
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
                    ModelState.AddModelError("ReviewerEmail", "指定されたレビュワが存在しません。");
            }

            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users
                .Include(u => u.Reviewee)
                .Include(u => u.Reviewer)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            // 基本情報更新
            user.Name = model.Name;
            user.Email = model.Email;
            user.Role = model.Role;
            user.UpdatedAt = DateTime.Now;

            // 古い関係の解除
            if (user.Role == UserRole.Reviewer && user.RevieweeId != null)
            {
                user.RevieweeId = null;
            }
            else if (user.Role == UserRole.Reviewee && user.Reviewer != null)
            {
                user.Reviewer.RevieweeId = null;
                _context.Update(user.Reviewer);
            }

            // 新しい関係設定
            if (model.Role == UserRole.Reviewer)
            {
                var reviewee = await _context.Users.FirstAsync(u => u.Email == model.RevieweeEmail);
                user.RevieweeId = reviewee.Id;
            }
            else if (model.Role == UserRole.Reviewee)
            {
                var reviewer = await _context.Users.FirstAsync(u => u.Email == model.ReviewerEmail);
                reviewer.RevieweeId = user.Id;
                _context.Update(reviewer);
            }

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // 削除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users
                .Include(u => u.Reviewer)
                .Include(u => u.Reviewee)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Role == UserRole.Reviewer && user.Reviewee != null)
            {
                user.Reviewee.Role = UserRole.Other;
                user.Reviewee.UpdatedAt = DateTime.Now;
                _context.Update(user.Reviewee);
            }

            if (user.Role == UserRole.Reviewee && user.Reviewer != null)
            {
                user.Reviewer.Role = UserRole.Other;
                user.Reviewer.UpdatedAt = DateTime.Now;
                user.Reviewer.RevieweeId = null;
                _context.Update(user.Reviewer);
            }

            user.RevieweeId = null;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult BulkCreate()
        {
            return View();
        }
    }
}
