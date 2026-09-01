using EsportShop.Api.DTOs;
using EsportShop.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace EsportShop.Api.Controllers
{
    [Route("api/v1/auth")] // Bonne pratique : versionner son API (v1)
    [ApiController]
    [Produces("application/json")] // Indique que l'API renvoie du JSON
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Inscrit un nouvel utilisateur dans le système.
        /// </summary>
        /// <param name="request">Les informations d'inscription (Email et Mot de passe)</param>
        /// <returns>Les informations de l'utilisateur créé</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserRegisterDto request, CancellationToken cancellationToken)
        {
            // La validation automatique des DataAnnotations (ou FluentValidation) s'exécute ici avant d'entrer
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           var response = await _authService.RegisterAsync(request, cancellationToken);

            _logger.LogInformation("Nouvel utilisateur inscrit avec succès : {Email}", request.Email);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Connecte un utilisateur et retourne un jeton JWT.
        /// </summary>
        /// <param name="request">Les identifiants de connexion (Email et Mot de passe)</param>
        /// <returns>Le jeton d'accès JWT</returns>
        [HttpPost("Login")]
        [EnableRateLimiting("StrictPolicy")] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _authService.LoginAsync(request, cancellationToken);

            _logger.LogInformation("Connexion réussie pour l'utilisateur : {Email}", request.Email);

            return Ok(new { token });
        }
    }
}
