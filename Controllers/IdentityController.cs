using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Net;

public class IdentityController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly SignInManager<IdentityUser> signInManager;

    public IdentityController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<IdentityUser> signInManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login(string? ReturnUrl)
    {
        ViewData["ReturnUrl"] = ReturnUrl;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginDto dto)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                ViewData["ErrorMessage"] = $"There is no '{dto.Email}' Email!";
                return View();
            }

            var result = await signInManager.PasswordSignInAsync(user, dto.Password, true, true);

            if (result.Succeeded == false)
            {
                ViewData["ErrorMessage"] = "Wrong Password!";
                return View();
            }

            return RedirectPermanent(dto.ReturnUrl ?? "/");
        }
        catch (ArgumentNullException ex)
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

    [HttpGet]
    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Registration([FromForm] UserDto dto)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(dto.Email);

            if (user != null)
                throw new ArgumentException($"User with email: {dto.Email} is already exist!");

            var newUser = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await userManager.CreateAsync(newUser, dto.Password);

            if (result.Succeeded == false)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return View("Registration");
            }

            if (dto.UserName == "Timur")
            {
                var role = new IdentityRole { Name = "Admin" };
                await roleManager.CreateAsync(role);

                await userManager.AddToRoleAsync(newUser, role.Name);
            }

            return RedirectToAction("Login", "Identity");
        }
        catch (ArgumentNullException ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewData["ErrorMessage"] = ex.Message;
            return View();
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return base.RedirectToAction("Main", "Home");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        IdentityUser user = await userManager.GetUserAsync(User);

        return View(user);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Profile([FromForm]UserDto dto)
    {
        IdentityUser user = await userManager.GetUserAsync(User);

        user.Email = dto.Email;
        user.UserName = dto.UserName;

        await userManager.UpdateAsync(user);

        return View(user);
    }
}