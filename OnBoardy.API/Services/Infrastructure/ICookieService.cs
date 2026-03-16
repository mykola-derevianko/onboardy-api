using OnBoardy.API.DTOs;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface ICookieService
    {
        void SetAuthCookies(TokenDTO token, HttpContext context);
        void ClearAuthCookies(HttpContext context);
    }
}
