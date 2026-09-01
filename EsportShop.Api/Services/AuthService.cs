using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;
using Mapster;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EsportShop.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<UserResponseDto> RegisterAsync(UserRegisterDto request, CancellationToken cancellationToken)
        {
            // 1. Vérifier si l'utilisateur existe déjà
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null) throw new InvalidOperationException("Cet e-mail est déjà utilisé.");

            // 2. Mapper le DTO vers l'entité User via Mapster
            var user = request.Adapt<User>();

            // 3. Hacher le mot de passe (on écrase le mot de passe en clair par le hash)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.Role = "user";

            // 4. Ajouter via le repository et sauvegarder via l'Unit of Work
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);


            return user.Adapt<UserResponseDto>(); 
        }

        public async Task<string> LoginAsync(UserLoginDto request, CancellationToken cancellationToken) 
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
            if(user == null)
            {
                throw new UnauthorizedAccessException("Identifiants invalides");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("identifiants invalides");
            }
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
