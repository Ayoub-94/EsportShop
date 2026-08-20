using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;
using EsportShop.Api.Services;

namespace EsportShop.Api.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Configurer le mock de l'UnitOfWork pour retourner le repository utilisateur
            _mockUnitOfWork.Setup(uow => uow.Users).Returns(_mockUserRepo.Object);

            // Simuler la configuration JWT (nécessaire pour la génération du token)
            var jwtSection = new Mock<IConfigurationSection>();
            jwtSection.Setup(s => s["Secret"]).Returns("CeciEstUneSuperCleSecreteDeTestPourLeJwt123456");
            jwtSection.Setup(s => s["Issuer"]).Returns("EsportShopIssuer");
            jwtSection.Setup(s => s["Audience"]).Returns("EsportShopAudience");

            _mockConfiguration.Setup(c => c.GetSection("JwtSettings")).Returns(jwtSection.Object);

            _authService = new AuthService(_mockUnitOfWork.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnUserResponse_WhenEmailIsUnique()
        {
            // 1. ARRANGE
            var registerDto = new UserRegisterDto("test@esport.com", "Password123!");

            // Simuler qu'aucun utilisateur n'existe avec cet e-mail
            _mockUserRepo.Setup(repo => repo.GetByEmailAsync(registerDto.Email))
                         .ReturnsAsync((User)null);

            // Simuler l'ajout et la sauvegarde
            _mockUserRepo.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                         .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.CompleteAsync())
                         .Returns(Task.FromResult(true));            
            var result = await _authService.RegisterAsync(registerDto);

            // 3. ASSERT
            result.Should().NotBeNull();
            result.Email.Should().Be(registerDto.Email);
            result.Role.Should().Be("User");
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // 1. ARRANGE
            var registerDto = new UserRegisterDto("existe@esport.com", "Password123!");

            var existingUser = new User { Id = 1, Email = registerDto.Email, PasswordHash = "hash" };

            // Simuler que l'utilisateur existe déjà
            _mockUserRepo.Setup(repo => repo.GetByEmailAsync(registerDto.Email))
                         .ReturnsAsync(existingUser);

            // 2. ACT
            Func<Task> act = async () => await _authService.RegisterAsync(registerDto);

            // 3. ASSERT
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Cet e-mail est déjà utilisé.");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // 1. ARRANGE

            var loginDto = new UserLoginDto("user@esport.com", "Password123!");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginDto.Password);
            var existingUser = new User
            {
                Id = 1,
                Email = loginDto.Email,
                PasswordHash = hashedPassword,
                Role = "User"
            };

            _mockUserRepo.Setup(repo => repo.GetByEmailAsync(loginDto.Email))
                         .ReturnsAsync(existingUser);

            // 2. ACT
            var token = await _authService.LoginAsync(loginDto);

            // 3. ASSERT
            token.Should().NotBeNullOrWhiteSpace();
        }
    }
}