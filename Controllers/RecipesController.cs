using System.Data.SqlClient;
using System.Net;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class RecipesController : Controller
{
    private IRecipesRepository repository;
    private readonly IRecipeService service;

    public RecipesController(IRecipesRepository repository, IRecipeService service)
    {
        this.repository = repository;
        this.service = service;
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
        ViewData["UserId"] = HttpContext.Request.Cookies["UserId"];

        return View();
    }

    [HttpPost("[controller]/Create")]
    public async Task<IActionResult> Create([FromForm] RecipeDto recipeDto)
    {
        try
        {
            await service.CreateAsync(recipeDto);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

            return RedirectToAction("Recipes", "Recipes");
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

    [HttpGet("[controller]/Details")]
    public async Task<IActionResult> Details(int id)
    {
        var recipe = await repository.GetByIdAsync(id);

        if (recipe is null)
            return BadRequest($"There is no recipe to detail with id: {id}");

        return View(recipe);
    }
}