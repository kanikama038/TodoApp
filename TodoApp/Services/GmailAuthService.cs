using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Options;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class GmailAuthService
    {
        private readonly _GmailApiSettings _settings;
        private readonly IWebHostEnvironment _env;

        public GmailAuthService(IOptions<_GmailApiSettings> settings, IWebHostEnvironment env)
        {
            _settings = settings.Value;
            _env = env;
        }

        public async Task<GmailService> GetGmailServiceAsync()
        {
            var secrets = new ClientSecrets
            {
                ClientId = _settings.ClientId,
                ClientSecret = _settings.ClientSecret
            };

            var scopes = new[] { GmailService.Scope.GmailSend };

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(Path.Combine(_env.ContentRootPath, "tokens"), true)
            );

            return new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Gmail API App"
            });
        }
    }
    // new FileDataStore() はデフォルトだと、C:\Users\<ユーザー名>\AppData\Roaming\gmail-token-store に保存される?.
    // カスタムしたいなら、new FileDataStore(Path.Combine(env.ContentRootPath, "wwwroot", "tokens"), true) で wwwroot/tokens に作られる。やるならgitignoreにtoken管理ファイルを書くべき.
    // 毎回認証させたいならnew FileDataStore()をnullに置き換える(非推奨).
}
