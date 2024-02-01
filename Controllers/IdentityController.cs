using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

public class IdentityController : Controller {
    public IActionResult Login() {
        return View();
    }

    public async Task<IActionResult> Login([FromForm] UserDto userDto) {
        HttpContext.Response.Cookies.Append("UserId", userDto.Login);
        return RedirectToAction("Index", "Home");
    }
}