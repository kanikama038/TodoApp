using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using RazorLight;
using TodoApp.Services;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    public class MailController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IRazorLightEngine _razorEngine;
        private readonly IWebHostEnvironment _env;

        public MailController(IEmailService emailService, IWebHostEnvironment env)
        {
            _emailService = emailService;
            _env = env;

            // RazorLight エンジンを初期化（テンプレートディレクトリを指定）.
            _razorEngine = new RazorLightEngineBuilder()
                .UseFileSystemProject(Path.Combine(env.ContentRootPath, "Templates"))
                .UseMemoryCachingProvider()
                .Build();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(string toEmail, string subject, string templateName)
        {
            // 任意のテンプレートモデル.
            var model = new _EmailToReviewerModel
            {
                UserName = "伊藤 誠",
                Link = "https://www.google.co.jp/search?q=伊藤誠+メール&tbm=isch"
            };

            var templateFileName = $"{templateName}.cshtml";
            var html = await _razorEngine.CompileRenderAsync(templateFileName, model);

            await _emailService.SendAsync(toEmail, subject, html);
            TempData["ResultSendEmail"] = "メールを送信しました。";
            return RedirectToAction(nameof(Index));
        }

    }
}

