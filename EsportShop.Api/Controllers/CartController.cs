using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;
using EsportShop.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EsportShop.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        // Méthode utilitaire privée pour extraire proprement le UserId du JWT

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if(int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Utilisateur non identifié dans le token.");
        }

        /// <summary>
        /// Récupère le panier d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <returns>Le panier complet avec ses articles et le prix total</returns>
        [HttpGet]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            try
            {
                int userId = GetUserIdFromToken();
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du panier");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Récupère le panier d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <returns>Le panier complet avec ses articles et le prix total</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int userId = GetUserIdFromToken();
                await _cartService.AddToCartAsync(userId, request);
                return Ok(new { message = "Produit ajouté au panier avec succès." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Produit introuvable lors de l'ajout au panier");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Opération invalide lors de l'ajout au panier");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur critique lors de l'ajout au panier");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Modifie directement la quantité d'un article dans le panier.
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <param name="productId">ID du produit concerné</param>
        /// <param name="newQuantity">Nouvelle quantité (si <= 0, l'article sera supprimé)</param>
        [HttpPut("{userId:int}/items/{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateQuantity(int productId, [FromBody] int newQuantity)
        {
            if (newQuantity < 0)
            {
                return BadRequest(new { message = "La quantité ne peut pas être négative." });
            }

            try
            {
                int userId = GetUserIdFromToken();
                await _cartService.UpdateItemQuantityAsync(userId, productId, newQuantity);
                return Ok(new { message = "Quantité mise à jour avce succès" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Article ou panier introuvable lors de la mise à jour");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur critique lors de la mise à jour de la quantité");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Supprime un article spécifique du panier.
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <param name="productId">ID du produit à supprimer</param>
        [HttpDelete("{userId:int}/items/{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            try
            {
                int userId = GetUserIdFromToken();
                await _cartService.RemoveItemAsync(userId, productId);
                return Ok(new { message = "article supprimé du panier avec succès." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'article {ProductId}", productId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Une erreur interne est survenue." });
            }
        }

        /// <summary>
        /// Vide entièrement le panier d'un utilisateur.
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        [HttpDelete("{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                int userId = GetUserIdFromToken();
                await _cartService.ClearCartAsync(userId);
                return Ok(new { message = "Panier vidé avec succès" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du nettoyage du panier");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Une erreur interne est survenue." });
            }
        }
    }
}
