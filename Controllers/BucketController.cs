using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

public class BucketController : Controller
{
    private readonly IBucketService service;
    private readonly UserManager<IdentityUser> userManager;

    public BucketController(IBucketService service, UserManager<IdentityUser> userManager)
    {
        this.service = service;
        this.userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        ViewBag.Id = userManager.GetUserId(HttpContext.User);
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromForm] BucketDto dto)
    {
        try
        {
            await service.CreateAsync(dto);

            return View();
        }
        catch (ArgumentException ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            base.ViewData["ErrorMessage"] = ex.Message;
            return View();
        }
        catch (Exception)
        {
            return StatusCode(500, "Something Went Wrong!");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Requests()
    {
        if (!this.User.IsInRole("Admin"))
            return StatusCode(403, "Have not access!");

        var recipes = await service.GetAllAsync();

        return View(recipes);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await service.DeleteAsync(id);

            return RedirectToAction("GetAll", "Recipes");
        }
        catch (ArgumentException ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewData["ErrorMessage"] = ex.Message;
            return RedirectToAction("GetAll", "Recipes");
        }
        catch (Exception)
        {
            return RedirectToAction("GetAll", "Recipes");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> OnVarification()
    {
        var recipes = await service.GetAllAsync();

        return View(recipes);
    }
}