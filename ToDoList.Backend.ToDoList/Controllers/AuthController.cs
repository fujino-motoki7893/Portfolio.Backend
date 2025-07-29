using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ToDoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors] 
    public class AuthController : ControllerBase
    {
        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = "/")
        {
            // Googleの認証ページにリダイレクト
            return new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties { 
                    RedirectUri = Url.Action(nameof(GoogleCallback), "Auth", new { returnUrl }) 
            });
        }

        [HttpGet("status")]
        public IActionResult GetAuthStatus()
        {
            var isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            return Ok(new 
            { 
                isAuthenticated = isAuthenticated,
                user = isAuthenticated ? HttpContext.User.Identity.Name : null,
                hasAccessToken = !string.IsNullOrEmpty(HttpContext.Session.GetString("GoogleAccessToken"))
            });
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = "/")
        {
            // 認証が成功すると、ASP.NET Coreが自動でユーザー情報をHttpContextに設定する
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return BadRequest("Authentication failed.");
            }

            // トークンを取得
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            // 本番環境ではDBなどに永続化するが、ここではデモのためセッションに保存
            if (accessToken != null)
            {
                HttpContext.Session.SetString("GoogleAccessToken", accessToken);
            }
            if (refreshToken != null)
            {
                HttpContext.Session.SetString("GoogleRefreshToken", refreshToken);
            }

            // 認証後のリダイレクト先（例: フロントエンドのページなど）
            return Redirect(returnUrl ?? "/");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // セッションもクリア
            return Ok("Logged out successfully.");
        }
    }
}
