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
    [Route("api/v1/cart")]
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

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Utilisateur non identifié dans le token.");
        }

        /// <summary>
        /// Récupère le panier de l'utilisateur connecté.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CartDto>> GetCart(CancellationToken cancellationToken)
        {
            int userId = GetUserIdFromToken();
            var cart = await _cartService.GetCartByUserIdAsync(userId, cancellationToken);
            return Ok(cart);
        }

        /// <summary>
        /// Ajoute un produit au panier de l'utilisateur connecté.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = GetUserIdFromToken();
            await _cartService.AddToCartAsync(userId,request, cancellationToken);

            _logger.LogInformation("Produit ajouté au panier pour l'utilisateur ID : {UserId}", userId);
            return Ok(new { message = "Produit ajouté au panier avec succès." });
        }

        /// <summary>
        /// Modifie directement la quantité d'un article dans le panier.
        /// </summary>
        [HttpPut("items/{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateQuantity(int productId, [FromBody] int newQuantity, CancellationToken cancellationToken)
        {
            if (newQuantity < 0)
            {
                return BadRequest(new { message = "La quantité ne peut pas être négative." });
            }

            int userId = GetUserIdFromToken();
            await _cartService.UpdateItemQuantityAsync(userId, productId, newQuantity, cancellationToken);

            return Ok(new { message = "Quantité mise à jour avec succès" });
        }

        /// <summary>
        /// Supprime un article spécifique du panier.
        /// </summary>
        [HttpDelete("items/{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveItem(int productId, CancellationToken cancellationToken)
        {
            int userId = GetUserIdFromToken();
            await _cartService.RemoveItemAsync(userId, productId, cancellationToken);

            return Ok(new { message = "Article supprimé du panier avec succès." });
        }

        /// <summary>
        /// Vide entièrement le panier de l'utilisateur connecté.
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
        {
            int userId = GetUserIdFromToken();
            await _cartService.ClearCartAsync(userId, cancellationToken);

            return Ok(new { message = "Panier vidé avec succès" });
        }
    }
}