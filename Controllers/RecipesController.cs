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

    public RecipesController(IRecipesRepository repository, IRecipeService service,
    UserManager<IdentityUser> userManager)
    {
        this.repository = repository;
        this.service = service;
        this.userManager = userManager;
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
            var recipe = await repository.GetByIdAsync(id);

            ViewData["UserName"] = (await service.GetUserByRecipeIdAsync(id)).UserName;

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

    [HttpDelete]
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
        catch (Exception ex)
        {
            return RedirectToAction("GetAll", "Recipes");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!this.User.IsInRole("Admin"))
        {
            return StatusCode(403, "Have not access!");
        }

        Recipe recipe = await service.GetById(id);

        return View(recipe);
    }

    [HttpPut]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit([FromBody] Recipe recipe)
    {
        if (!this.User.IsInRole("Admin"))
            return StatusCode(403, "Have not access!");

        try
        {
            await service.Edit(recipe);

            return RedirectPermanent($"/Recipes/Details?id={recipe.Id}");
        }
        catch (ArgumentException ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewData["ErrorMessage"] = ex.Message;
            return RedirectPermanent($"Recipes/Edit?id={recipe.Id}");
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

    [HttpPut]
    [Authorize]
    [Consumes("application/json")]
    public async Task<IActionResult> MyEdit([FromBody] Recipe recipe)
    {
        try
        {
            await service.Edit(recipe);

            return RedirectPermanent($"/Recipes/Details?id={recipe.Id}");
        }
        catch (ArgumentException ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewData["ErrorMessage"] = ex.Message;
            return RedirectPermanent($"Recipes/Edit?id={recipe.Id}");
        }
        catch (Exception)
        {
            return StatusCode(500, "Something went wrong!");
        }
    }
}