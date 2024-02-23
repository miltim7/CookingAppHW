using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

public class IdentityController : Controller
{
    private readonly IUserService service;

    public IdentityController(IUserService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        ViewData["returnUrl"] = returnUrl;
        return View();
    }

    [HttpGet]
    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
    {
        try
        {
            await service.LoginAsync(loginDto);

            int userId = await service.GetIdByLogin(loginDto.Login);

            HttpContext.Response.Cookies.Append("UserId", userId.ToString());

            var claims = new List<Claim> {
                new("creation_date_utc", DateTime.UtcNow.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                principal: new ClaimsPrincipal(claimsIdentity)
            );

            if (string.IsNullOrWhiteSpace(loginDto.ReturnUrl))
                return RedirectToAction("Index", "Home");

            return RedirectPermanent(loginDto.ReturnUrl);
        }
        catch (ArgumentException ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewData["ErrorMessage"] = ex.Message;
            return View();
        }
        catch (Exception)
        {
            return StatusCode(500, "Something Went Wrong!");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Registration(UserDto userDto)
    {
        try
        {
            await service.CreateAsync(userDto);
        }
        catch (ArgumentException ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewData["ErrorMessage"] = ex.Message;
            return View();
        }
        catch (Exception)
        {
            return StatusCode(500, "Something Went Wrong!");
        }

        return RedirectToAction("Login", "Identity");
    }

    [HttpGet("[controller]/Profile")]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        return View(await service.GetUserById(int.Parse(HttpContext.Request.Cookies["UserId"])));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangeProfile([FromForm] User user)
    {
        try
        {
            user.Id = int.Parse(HttpContext.Request.Cookies["UserId"]);

            await service.ChangeProfileAsync(user);

            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Something Went Wrong!");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromForm] string newPassword)
    {
        try
        {
            await service.ChangePasswordAsync(newPassword, int.Parse(HttpContext.Request.Cookies["UserId"]));

            return RedirectToAction("Profile", "Identity");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Something Went Wrong!");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return base.RedirectToAction("Login", "Identity");
    }
}