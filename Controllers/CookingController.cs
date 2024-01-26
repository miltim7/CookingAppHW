using System.Data.SqlClient;
using System.Net;
using Dapper;
using Microsoft.AspNetCore.Mvc;

public class CookingController : Controller
{
    private CookingRepository repository;
    public CookingController(IConfiguration configuration)
    {
        this.repository = new CookingRepository(configuration);
    }

    [Route("[controller]/Recipes/GetAll")]
    public async Task<IActionResult> Recipes()
    {
        var recipes = await repository.GetAll();
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

        if (!await repository.Create(recipeDto))
            return BadRequest($"Bad Request");

        HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

        return RedirectToAction("Recipes", "Cooking");
    }
}