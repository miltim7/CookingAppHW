using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CookingAppHW.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<IdentityUser> userManager;

    public HomeController(UserManager<IdentityUser> userManager)
    {
        this.userManager = userManager;
    }

    public IActionResult Main()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Index() 
    {
        var user = await userManager.GetUserAsync(User);
        ViewData["Username"] = user?.UserName;
        return View();
    }
}