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
        if (!await repository.Create(recipeDto))
            return BadRequest();

        HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

        return RedirectToAction("Recipes", "Cooking");
    }
}