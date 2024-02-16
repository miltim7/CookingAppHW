using Microsoft.AspNetCore.Mvc;

public class IdentityController : Controller
{
    private readonly IUserService service;

    public IdentityController(IUserService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult Login()
    {
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
    public async Task<IActionResult> Registration(UserDto userDto)
    {
        await service.CreateAsync(userDto);

        return RedirectToAction("Login", "Identity");
    }
}