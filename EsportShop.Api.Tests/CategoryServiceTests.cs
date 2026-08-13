using FluentAssertions;
using Xunit;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;
using EsportShop.Api.Services;
using Moq;

namespace EsportShop.Api.Tests
{
    public class CategoryServiceTests
    {
        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            // 1. ARRANGE
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCategoryRepo = new Mock<ICategoryRepository>();

            var categoryId = 1;
            var expectedCategory = new Category { Id = categoryId, Name = "Claviers Gaming" };

            mockCategoryRepo.Setup(repo => repo.GetByIdAsync(categoryId))
                            .ReturnsAsync(expectedCategory);

            mockUnitOfWork.Setup(uow => uow.Categories)
                          .Returns(mockCategoryRepo.Object);

            var service = new CategoryService(mockUnitOfWork.Object);

            // 2. ACT
            var result = await service.GetCategoryByIdAsync(categoryId);

            // 3. ASSERT
            result.Should().NotBeNull();
            result.Id.Should().Be(categoryId);
            result.Name.Should().Be("Claviers Gaming");
        }
    }
}