using BenBristow.EntityFrameworkCore.Pagination.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BenBristow.EntityFrameworkCore.Pagination.Tests.Extensions;

public sealed class QueryableExtensionsTests
{
    private readonly TestDbContext _context;

    public QueryableExtensionsTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        _context = new TestDbContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task ToPaginationResultAsync_ForFirstPage_ReturnsPaginatedResults()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var results = await _context.TestEntities
            .OrderBy(t => t.Id)
            .ToPaginationResultAsync(page: 1, pageSize: 10);

        // Assert
        results.Results.Should().HaveCount(10);
        results.TotalCount.Should().Be(100);
        results.Page.Should().Be(1);
        results.PageCount.Should().Be(10);
        results.PageSize.Should().Be(10);
        results.Results.Select(t => t.Id).Should().BeEquivalentTo(Enumerable.Range(1, 10));
    }

    [Fact]
    public async Task ToPaginationResultAsync_ForSecondPage_ReturnsPaginatedResults()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var results = await _context.TestEntities
            .OrderBy(t => t.Id)
            .ToPaginationResultAsync(page: 2, pageSize: 10);

        // Assert
        results.Results.Should().HaveCount(10);
        results.TotalCount.Should().Be(100);
        results.Page.Should().Be(2);
        results.PageCount.Should().Be(10);
        results.PageSize.Should().Be(10);
        results.Results.Select(t => t.Id).Should().BeEquivalentTo(Enumerable.Range(11, 10));
    }

    [Fact]
    public async Task ToPaginationResultAsync_CalledWithInvalidPage_ReturnsEmptyResults()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var results = await _context.TestEntities
            .OrderBy(t => t.Id)
            .ToPaginationResultAsync(page: 11, pageSize: 10);

        // Assert
        results.Results.Should().BeEmpty();
        results.TotalCount.Should().Be(100);
        results.Page.Should().Be(11);
        results.PageCount.Should().Be(10);
        results.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task ToPaginationResultAsync_WithoutPageSize_ReturnsAllResults()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var results = await _context.TestEntities
            .OrderBy(t => t.Id)
            .ToPaginationResultAsync(pageSize: null);

        // Assert
        results.Results.Should().HaveCount(100);
        results.TotalCount.Should().Be(100);
        results.Page.Should().Be(1);
        results.PageCount.Should().Be(1);
        results.PageSize.Should().BeNull();
    }

    private async Task SeedDatabaseAsync()
    {
        var testEntities = Enumerable.Range(0, 100).Select(i => new TestEntity
        {
            Id = i + 1
        });
        await _context.TestEntities.AddRangeAsync(testEntities);
        await _context.SaveChangesAsync();
    }
}

public sealed class TestEntity
{
    public int Id { get; init; }
}

public sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<TestEntity> TestEntities { get; init; }
}