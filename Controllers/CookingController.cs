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
    public IActionResult Create() => View();

    [Route("[controller]/Recipes/Create")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] RecipeDto recipeDto)
    {
        if (!await repository.Create(recipeDto))
            return BadRequest();

        HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

        return RedirectToAction("Recipes", "Cooking");
    }

    [Route("[controller]/Recipes/Details")]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var recipe = await repository.Details(id);

        if (recipe is null)
            return BadRequest($"There is no recipe to detail with id: {id}");

        return View(recipe);
    }

    [Route("[controller]/Recipes/Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var recipe = await repository.GetById(id);

        if (recipe is null)
            return BadRequest($"There is no recipe for edit with id: {id}");

        return View(recipe);
    }

    [Route("[controller]/Recipes/Edit")]
    [HttpPost]
    public async Task<IActionResult> Edit([FromForm] Recipe recipe)
    {
        await repository.Edit(recipe);
        return RedirectPermanent($"/Cooking/Recipes/Details?id={recipe.Id}");
    }
}