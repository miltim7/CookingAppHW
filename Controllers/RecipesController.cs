using System.Data.SqlClient;
using System.Net;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

public class RecipesController : Controller
{
    private IRecipesRepository repository;
    private readonly IRecipeService service;
    private readonly UserManager<IdentityUser> userManager;
    private readonly ICommentService commentsService;

    public RecipesController(IRecipesRepository repository, IRecipeService service,
    UserManager<IdentityUser> userManager, ICommentService commentsService)
    {
        this.repository = repository;
        this.service = service;
        this.userManager = userManager;
        this.commentsService = commentsService;
    }

    [HttpGet("[controller]/GetAll")]
    public async Task<IActionResult> Recipes()
    {
        var recipes = await repository.GetAllAsync();

        return View(recipes);
    }

    [HttpGet("[controller]/Create")]
    [Authorize]
    public IActionResult Create()
    {
        ViewBag.Id = userManager.GetUserId(HttpContext.User);

        return View();
    }

    [HttpGet("[controller]/MyRecipes")]
    [Authorize]
    public async Task<IActionResult> MyRecipes()
    {
        var recipes = await service.GetMyAsync(userManager.GetUserId(HttpContext.User));

        return View(recipes);
    }

    [HttpPost("[controller]/Create")]
    [Authorize]
    public async Task<IActionResult> Create([FromForm] RecipeDto recipeDto)
    {
        try
        {
            await service.CreateAsync(recipeDto);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

            var recipes = await repository.GetAllAsync();

            return RedirectToAction("MyRecipes", "Recipes");
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

    [HttpGet("[controller]/Details")]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            RecipeComments recipeComments = new RecipeComments();

            var recipe = await repository.GetByIdAsync(id);
            var comments = await commentsService.GetCommentsByRecipeIdAsync(id);

            recipeComments.recipe = recipe;
            recipeComments.comments = comments;

            ViewData["UserName"] = (await service.GetUserByRecipeIdAsync(id)).UserName;
            ViewData["MyUserName"] = (await userManager.GetUserAsync(User)).UserName;

            ViewBag.UserId = (await userManager.GetUserAsync(User)).Id;

            return View(recipeComments);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Something went wrong!");
        }
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
    public async Task<IActionResult> Edit(int id)
    {
        if (!this.User.IsInRole("Admin"))
        {
            return StatusCode(403, "Have not access!");
        }

        Recipe recipe = await service.GetById(id);

        return View(recipe);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit([FromForm] Recipe recipe)
    {
        try
        {
            await service.EditAsync(recipe);

            return RedirectPermanent($"/Recipes/Details?id={recipe.Id}");
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> MyEdit(int id)
    {
        try
        {
            Recipe recipe = await service.GetById(id);

            return View(recipe);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Something went wrong!");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> MyEdit([FromForm] Recipe recipe)
    {
        try
        {
            await service.EditAsync(recipe);

            System.Console.WriteLine("TRY");

            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewData["ErrorMessage"] = ex.Message;

            System.Console.WriteLine("CATCH");
            return View();
        }
        catch (Exception)
        {
            return StatusCode(500, "Something went wrong!");
        }
    }
}