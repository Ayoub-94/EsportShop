using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;
using EsportShop.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsportShop.Api.Controllers
{
    [Authorize]
    [Route("api/v1/products")]
    [ApiController]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // 1. CREATE (Créer un produit - Réservé aux Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductResponseDto>> Create([FromBody] ProductCreateDto dto)
        {
           if(!ModelState.IsValid)
           {
               return BadRequest(ModelState);
           }

            var createdProduct = await _productService.CreateProductAsync(dto);

            _logger.LogInformation("Produit créé avec succès ID : {ProductId}", createdProduct.Id);

            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id });
        }

        // 2. READ ALL (Récupérer tous les produits - Accessible à tous, ou [Authorize] selon votre besoin)
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // 3. READ BY ID (Récupérer un produit par son ID)
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponseDto>> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if(product == null)
            {
                throw new KeyNotFoundException($"Le produit avec l'ID {id} est introuvable.");
            }

            return Ok(product);
        }

        // 4. UPDATE (Mettre à jour un produit - Réservé aux Admin)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Votre service se chargera de mettre à jour ou de lancer une KeyNotFoundException si l'ID n'existe pas
            await _productService.UpdateProductAsync(id, dto);

            _logger.LogInformation("Produit mis à jour ID : {ProductID}", id);

            return NoContent();
        }

        // 5. DELETE (Supprimer un produit)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);

            _logger.LogInformation("Produit supprimé ID : {ProductId}", id);

            return NoContent();
        }

        [HttpDelete("all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteAll()
        {
            await _productService.DeleteAllProductAsync();

            _logger.LogWarning("Suppression massive de TOUS les produits de la boutique !");

            return NoContent();
        }

    }
}
