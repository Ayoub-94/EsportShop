using EsportShop.Api.Data;
using EsportShop.Api.DTOs;
using EsportShop.Api.Models; // Assurez-vous que votre modèle s'appelle bien comme ça
using EsportShop.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/v1/[controller]")]
[ApiController]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    // 1. READ ALL (Récupérer toutes les catégories - Public)
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    // 2. READ BY ID (Récupérer une catégorie par son ID - Public)
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Tentative d'accès à une catégorie inexistante avec l'ID : {Id}", id);
            return NotFound(new { message = $"La catégorie avec l'ID {id} est introuvable." });
        }
        return Ok(category);
    }

    // 3. CREATE (Créer une catégorie - Réservé Admin)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CategoryCreateDto dto)
    {

        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
       
        var createdCategory = await _categoryService.CreateCategoryAsync(dto);
        _logger.LogInformation("Catégorie créée avec succès : {Name}", dto.Name);

        return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
    }

    // 4. UPDATE (Modifier une catégorie - Réservé Admin) [NOUVELLE MÉTHODE]
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
    {
        if(ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _categoryService.UpdateCategoryAsync(id, dto);
        _logger.LogInformation("Catégorie mise à jour avec succès ID : {Id}", id);

        return NoContent();
    }

    // 5. DELETE (Supprimer une catégorie - Réservé Admin) [NOUVELLE MÉTHODE]
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        _logger.LogInformation("Catégorie supprimée avec succès ID : {Id}", id);

        return NoContent();
    }

}