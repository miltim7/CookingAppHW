using System.Data.SqlClient;
using System.Net;
using Dapper;
using Microsoft.AspNetCore.Mvc;

public class CookingController : Controller
{
    private ICookingRepository repository;
    public CookingController(ICookingRepository repository)
    {
        this.repository = repository;
    }

    [Route("[controller]/Recipes/GetAll")]
    public async Task<IActionResult> Recipes()
    {
        var recipes = await repository.GetAllAsync();
        return View(recipes);
    }

    [Route("[controller]/Recipes/Create")]
    [HttpGet]
    public IActionResult Create() {
        return View();
    }

    [Route("[controller]/Recipes/Create")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] RecipeDto recipeDto)
    {
        if (string.IsNullOrWhiteSpace(recipeDto.Title) ||
            string.IsNullOrWhiteSpace(recipeDto.Description) ||
            string.IsNullOrWhiteSpace(recipeDto.Category)) {
            return BadRequest("Fields Can not be empty");
        }

        if (recipeDto.Price < 0) {
            return BadRequest("Price must be positive number or 0");
        }

        if (await repository.CreateAsync(recipeDto) == 0)
            return BadRequest($"Bad Request");

        HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

        return RedirectToAction("Recipes", "Cooking");
    }

    [Route("[controller]/Recipes/Details")]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var recipe = await repository.GetByIdAsync(id);

        if (recipe is null)
            return BadRequest($"There is no recipe to detail with id: {id}");

        return View(recipe);
    }
}