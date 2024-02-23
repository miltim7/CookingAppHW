using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CookingAppHW.Models;
using Microsoft.AspNetCore.Authorization;

namespace CookingAppHW.Controllers;

public class HomeController : Controller
{
    private readonly IUserService service;

    public HomeController(IUserService service)
    {
        this.service = service;
    }

    public IActionResult Main()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Index() 
    {
        int id = int.Parse(HttpContext.Request.Cookies["UserId"]);
        User user = await service.GetUserById(id);
        ViewData["Username"] = user.Name;
        return View();
    }
}