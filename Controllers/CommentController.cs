using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class CommentController : Controller
{
    private readonly ICommentService service;
    private readonly UserManager<IdentityUser> userManager;

    public CommentController(ICommentService service, UserManager<IdentityUser> userManager)
    {
        this.service = service;
        this.userManager = userManager;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CommentDto dto)
    {
        try
        {
            string authorUsername = (await userManager.GetUserAsync(HttpContext.User)).UserName;

            await service.CreateAsync(dto, authorUsername);

            return RedirectPermanent($"/Recipes/Details?id={dto.RecipeId}");
        }
        catch (ArgumentException ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewData["ErrorMessage"] = ex.Message;
            return View();
        }
        catch (Exception)
        {
            return StatusCode(500, "Something went wrong!");
        }
    }
}