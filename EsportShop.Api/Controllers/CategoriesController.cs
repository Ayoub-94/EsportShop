using EsportShop.Api.Data;
using EsportShop.Api.DTOs;
using EsportShop.Api.Models; // Assurez-vous que votre modèle s'appelle bien comme ça
using EsportShop.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger _logger;

    public CategoriesController(ICategoryService categoryService, ILogger logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }
    /// <summary>
    /// Récupère une catégorie spécifique par son identifiant unique.
    /// </summary>
    
    [HttpGet("id")]
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
    /// <summary>
    /// Crée une nouvelle catégorie. (Réservé aux utilisateurs authentifiés)
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CategoryCreateDto dto)
    {
        try
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(dto);
            _logger.LogInformation("Catégorie créée avec succès : {Name}", dto.Name);

            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }
        catch (ArgumentException ex)
        {
            _logger.LogInformation("Échec de création de catégorie : {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

  
}