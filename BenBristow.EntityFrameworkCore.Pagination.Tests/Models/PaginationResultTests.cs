using System.Collections;
using BenBristow.EntityFrameworkCore.Pagination.Models;
using FluentAssertions;

namespace BenBristow.EntityFrameworkCore.Pagination.Tests.Models;

public sealed class PaginationResultTests
{
    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class DummyClass;

    [Fact]
    public void Empty_WithoutPageSize_ReturnsExpectedDefaults()
    {
        // Act
        var result = PaginationResult<DummyClass>.Empty();

        // Assert
        result.Results.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.Page.Should().Be(1);
        result.PageCount.Should().Be(1);
        result.PageSize.Should().BeNull();

        // Untyped collection should also be empty
        result.ResultsUntyped.Should().BeAssignableTo<ICollection>()
            .Which.Count.Should().Be(0);
    }

    [Fact]
    public void Empty_WithPageSize_SetsPageSizeAndDefaults()
    {
        // Arrange
        const int pageSize = 25;

        // Act
        var result = PaginationResult<DummyClass>.Empty(pageSize);

        // Assert
        result.Results.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.Page.Should().Be(1);
        result.PageCount.Should().Be(1);
        result.PageSize.Should().Be(pageSize);
    }
}
