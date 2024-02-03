using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

public class IdentityController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login([FromForm] UserDto userDto)
    {
        if (string.IsNullOrWhiteSpace(userDto.Login))
        {
            return BadRequest("Login must be filled!");
        }
        if (string.IsNullOrWhiteSpace(userDto.Password))
        {
            return BadRequest("Password must be filled!");
        }

        HttpContext.Response.Cookies.Append("UserId", userDto.Login);

        return RedirectToAction("Index", "Home");
    }
}