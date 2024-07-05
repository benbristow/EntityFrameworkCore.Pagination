# BenBristow.EntityFrameworkCore.Pagination

This library provides an easy-to-use extension for paginating `IQueryable<T>` objects in Entity Framework Core, allowing
for efficient, dynamic pagination of database queries.

## Getting Started

### Installation

To install the library, use the following NuGet command:

```bash
dotnet add package BenBristow.EntityFrameworkCore.Pagination
```

### Basic Usage

1. **Reference the library** in your project by adding `using BenBristow.EntityFrameworkCore.Pagination.Extensions;`.

2. **Paginate your query** by calling `ToPaginationResultAsync` on any `IOrderedQueryable<T>` object, specifying the
   desired page and page size.

Example:

```csharp
var paginatedResults = await myDbContext.MyEntities
    .OrderBy(entity => entity.Id)
    .ToPaginationResultAsync(page: 1, pageSize: 10);
```

## API Reference

### `ToPaginationResultAsync<T>`

Paginates the source `IQueryable<T>` based on the provided parameters.

#### Parameters

- `source`: The ordered `IQueryable<T>` source to paginate.
- `page`: The page number to retrieve. Defaults to 1.
- `pageSize`: The number of items per page. Can be `null` to return all items.
- `cancellationToken`: A token to observe while waiting for the task to complete.

#### Returns

A `Task<PaginationResult<T>>` representing the asynchronous operation, containing the paginated results.

## Example

```csharp
public class MyEntityService
{
    private readonly MyDbContext _context;
    
    public MyEntityService(MyDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Asynchronously gets paginated entities.
    /// </summary>
    /// <param name="page">The page number to retrieve, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated results.</returns>
    public async Task<PaginationResult<MyEntity>> GetPaginatedEntities(int page = 1, int pageSize = 10)
    {
        var paginatedResults = await _context.Entities
            .OrderBy(e => e.Id)
            .ToPaginationResultAsync(page, pageSize);

        return paginatedResults;
    }
}
```

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any bugs or feature requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.