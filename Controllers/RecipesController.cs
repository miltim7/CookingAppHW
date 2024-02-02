using System.Data.SqlClient;
using System.Net;
using Dapper;
using Microsoft.AspNetCore.Mvc;

public class RecipesController : Controller
{
    private ICookingRepository repository;
    public RecipesController(ICookingRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet("[controller]/GetAll")]
    public async Task<IActionResult> Recipes()
    {
        var recipes = await repository.GetAllAsync();
        return View(recipes);   
    }

    [HttpGet("[controller]/Create")]
    public IActionResult Create() {
        return View();
    }

    [HttpPost("[controller]/Create")]
    public async Task<IActionResult> Create([FromForm] RecipeDto recipeDto)
    {
        if (string.IsNullOrWhiteSpace(recipeDto.Title)) {
            return BadRequest("'Title' Can not be empty");
        }

        if (string.IsNullOrWhiteSpace(recipeDto.Description)) {
            return BadRequest("'Description' Can not be empty");
        }

        if (string.IsNullOrWhiteSpace(recipeDto.Category)) {
            return BadRequest("'Category' Can not be empty");
        }

        if (recipeDto.Price < 0) {
            return BadRequest("Price must be positive number or 0");
        }

        if (await repository.CreateAsync(recipeDto) == 0) {
            return BadRequest();
        }

        HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

        return RedirectToAction("Recipes", "Recipes");
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