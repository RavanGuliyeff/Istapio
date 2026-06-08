# Quick Start Guide for Adding New Entities

## Overview
This guide explains how to add a new entity following the established architecture pattern in the Istapio backend.

## Step-by-Step Process

### Step 1: Understand Your Entity
First, determine:
- **Is the entity auditable?** (has CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
  - If YES ? Use `BaseAuditableEntity` and `IAuditableRepository<T>`
  - If NO ? Use `BaseEntity` and `IRepository<T>`
- **What are its properties?**
- **Does it have relationships?**

---

### Step 2: Create Repository Interface
**File:** `src/Core/Istapio.Domain/Interfaces/Repositories/I[Entity]Repository.cs`

**For Non-Auditable Entity:**
```csharp
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories.Generics;

namespace Istapio.Domain.Interfaces.Repositories;

public interface I[Entity]Repository : IRepository<[Entity]>
{
    // Add any custom query methods here if needed
}
```

**For Auditable Entity:**
```csharp
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories.Generics;

namespace Istapio.Domain.Interfaces.Repositories;

public interface I[Entity]Repository : IAuditableRepository<[Entity]>
{
    // Add any custom query methods here if needed
}
```

---

### Step 3: Create DTOs

**File:** `src/Core/Istapio.Application/Models/DTOs/[Entity]/Get[Entity]Dto.cs`

**For Non-Auditable Entity:**
```csharp
namespace Istapio.Application.Models.DTOs.[Entity];

public record Get[Entity]Dto(
    Guid Id,
    string Name,
    string? Description = null
    // Add other properties
);
```

**For Auditable Entity:**
```csharp
using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.[Entity];

public record Get[Entity]Dto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
    // Add other properties
) : BaseAuditableDto(Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy);
```

**File:** `src/Core/Istapio.Application/Models/DTOs/[Entity]/Create[Entity]Dto.cs`
```csharp
namespace Istapio.Application.Models.DTOs.[Entity];

public record Create[Entity]Dto(
    string Name,
    string? Description = null
    // Add other properties (no Id, no audit fields)
);
```

**File:** `src/Core/Istapio.Application/Models/DTOs/[Entity]/Update[Entity]Dto.cs`
```csharp
using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.[Entity];

public record Update[Entity]Dto(
    Guid Id,
    string Name,
    string? Description = null
    // Add updatable properties (no audit fields)
) : BaseDto(Id);
```

---

### Step 4: Create AutoMapper Profile

**File:** `src/Core/Istapio.Application/Profiles/[Entity]Profile.cs`

```csharp
using AutoMapper;
using Istapio.Application.Models.DTOs.[Entity];
using Istapio.Domain.Entities;

namespace Istapio.Application.Profiles;

public class [Entity]Profile : Profile
{
    public [Entity]Profile()
    {
        CreateMap<[Entity], Get[Entity]Dto>();
        CreateMap<[Entity], Create[Entity]Dto>();
        CreateMap<[Entity], Update[Entity]Dto>();
    }
}
```

---

### Step 5: Create Validators

**File:** `src/Core/Istapio.Application/Validators/[Entity]/Create[Entity]DtoValidator.cs`

```csharp
using FluentValidation;
using Istapio.Application.Models.DTOs.[Entity];

namespace Istapio.Application.Validators.[Entity];

public class Create[Entity]DtoValidator : AbstractValidator<Create[Entity]Dto>
{
    public Create[Entity]DtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
```

**File:** `src/Core/Istapio.Application/Validators/[Entity]/Update[Entity]DtoValidator.cs`

```csharp
using FluentValidation;
using Istapio.Application.Models.DTOs.[Entity];

namespace Istapio.Application.Validators.[Entity];

public class Update[Entity]DtoValidator : AbstractValidator<Update[Entity]Dto>
{
    public Update[Entity]DtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
```

---

### Step 6: Create Service Interface

**File:** `src/Core/Istapio.Application/Services/Internal/Interfaces/I[Entity]Service.cs`

```csharp
using Istapio.Application.Models.DTOs.[Entity];

namespace Istapio.Application.Services.Internal.Interfaces;

public interface I[Entity]Service
{
    // Query
    Task<Get[Entity]Dto?> GetByIdAsync(Guid id);
    Task<List<Get[Entity]Dto>> GetAllAsync();
    Task<(List<Get[Entity]Dto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10);

    // Command
    Task<Get[Entity]Dto> CreateAsync(Create[Entity]Dto dto);
    Task<Get[Entity]Dto> UpdateAsync(Update[Entity]Dto dto);
    Task DeleteAsync(Guid id);
}
```

---

### Step 7: Create Service Implementation

**File:** `src/Core/Istapio.Application/Services/Internal/Implementations/[Entity]Service.cs`

**For Non-Auditable Entity:**
```csharp
using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.[Entity];
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;

namespace Istapio.Application.Services.Internal.Implementations;

public class [Entity]Service : I[Entity]Service
{
    private readonly I[Entity]Repository _repository;

    public [Entity]Service(I[Entity]Repository repository)
    {
        _repository = repository;
    }

    public async Task<Get[Entity]Dto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new NotFoundException(nameof([Entity]), id);

        return Map(entity);
    }

    public async Task<List<Get[Entity]Dto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(Map).ToList();
    }

    public async Task<(List<Get[Entity]Dto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        var dtos = items.Select(Map).ToList();
        return (dtos, total);
    }

    public async Task<Get[Entity]Dto> CreateAsync(Create[Entity]Dto dto)
    {
        [Entity] entity = new [Entity]
        {
            Name = dto.Name,
            Description = dto.Description
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<Get[Entity]Dto> UpdateAsync(Update[Entity]Dto dto)
    {
        [Entity]? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof([Entity]), dto.Id);

        entity.Name = dto.Name;
        entity.Description = dto.Description;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        [Entity]? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof([Entity]), id);

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }

    private static Get[Entity]Dto Map([Entity] entity)
        => new Get[Entity]Dto(
            entity.Id,
            entity.Name,
            entity.Description
            // Map other properties
        );
}
```

**For Auditable Entity:**
```csharp
// Same as above, but:
// 1. Use SoftDeleteAsync instead of Delete
// 2. Set UpdatedAt = DateTime.UtcNow in Update method
// 3. Map audit fields in the Map method
```

---

### Step 8: Create Controller

**File:** `Presentation/Istapio.API/Controllers/[Entities]Controller.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Istapio.API.Controllers.Common;
using Istapio.Application.Models.DTOs.[Entity];
using Istapio.Application.Services.Internal.Interfaces;

namespace Istapio.API.Controllers;

/// <summary>
/// Controller for managing [entity description]
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class [Entities]Controller : BaseController
{
    private readonly I[Entity]Service _[entity]Service;

    /// <summary>
    /// Initializes a new instance of the [Entities]Controller
    /// </summary>
    /// <param name="[entity]Service">The [entity] service instance</param>
    public [Entities]Controller(I[Entity]Service [entity]Service)
    {
        _[entity]Service = [entity]Service;
    }

    /// <summary>
    /// Retrieves a [entity] by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the [entity]</param>
    /// <returns>The requested [entity]</returns>
    /// <response code="200">Returns the [entity]</response>
    /// <response code="404">If the [entity] is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Get[Entity]Dto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _[entity]Service.GetByIdAsync(id);
        return Success(entity);
    }

    /// <summary>
    /// Retrieves all [entities]
    /// </summary>
    /// <returns>A list of all [entities]</returns>
    /// <response code="200">Returns the list of [entities]</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Get[Entity]Dto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var entities = await _[entity]Service.GetAllAsync();
        return Success(entities);
    }

    /// <summary>
    /// Retrieves a paginated list of [entities]
    /// </summary>
    /// <param name="pageIndex">The page number (default: 1)</param>
    /// <param name="pageSize">The number of items per page (default: 10)</param>
    /// <returns>A paginated list of [entities] with total count</returns>
    /// <response code="200">Returns the paginated [entities]</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _[entity]Service.GetPagedAsync(pageIndex, pageSize);
        return Success(new { items, totalCount, pageIndex, pageSize });
    }

    /// <summary>
    /// Creates a new [entity]
    /// </summary>
    /// <param name="dto">The [entity] data to create</param>
    /// <returns>The newly created [entity]</returns>
    /// <response code="201">Returns the newly created [entity]</response>
    /// <response code="400">If the request data is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(Get[Entity]Dto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] Create[Entity]Dto dto)
    {
        var entity = await _[entity]Service.CreateAsync(dto);
        return Created(entity, "[Entity] created successfully");
    }

    /// <summary>
    /// Updates an existing [entity] by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the [entity]</param>
    /// <param name="dto">The updated [entity] data</param>
    /// <returns>The updated [entity]</returns>
    /// <response code="200">Returns the updated [entity]</response>
    /// <response code="400">If the ID in route doesn't match the ID in body</response>
    /// <response code="404">If the [entity] is not found</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Get[Entity]Dto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] Update[Entity]Dto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var entity = await _[entity]Service.UpdateAsync(dto);
        return Success(entity, "[Entity] updated successfully");
    }

    /// <summary>
    /// Soft deletes a [entity] by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the [entity] to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the [entity] was successfully deleted</response>
    /// <response code="404">If the [entity] is not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _[entity]Service.DeleteAsync(id);
        return NoContent("[Entity] deleted successfully");
    }
}
```

---

## Implementation Checklist

- [ ] Create Repository Interface (if using custom queries)
- [ ] Create Get DTO
- [ ] Create Create DTO
- [ ] Create Update DTO
- [ ] Create AutoMapper Profile
- [ ] Create Create Validator
- [ ] Create Update Validator
- [ ] Create Service Interface
- [ ] Create Service Implementation
- [ ] Create Controller
- [ ] Run `dotnet build` to verify
- [ ] Test all endpoints in Swagger UI
- [ ] Add to git commit

---

## Common Mistakes to Avoid

? **Mistake 1:** Forgetting to implement IRepository or IAuditableRepository
- **Fix:** Check if entity should be auditable before choosing repository type

? **Mistake 2:** Not validating IDs in DTOs
- **Fix:** Always validate that Id is not empty in Update validators

? **Mistake 3:** Mixing soft delete with hard delete
- **Fix:** Use SoftDeleteAsync for IAuditableRepository, Delete for IRepository

? **Mistake 4:** Forgetting to save changes
- **Fix:** Always call `SaveChangesAsync()` after Add/Update/Delete operations

? **Mistake 5:** Not throwing NotFoundException when entity not found
- **Fix:** Always check for null and throw appropriate exceptions

? **Mistake 6:** Not mapping all DTO properties
- **Fix:** Ensure all DTO fields are assigned in the Map method

---

## Testing Your Implementation

### 1. Build Verification
```bash
cd backend
dotnet build
```

### 2. Test via Swagger UI
1. Start the application
2. Navigate to http://localhost:5000/swagger/index.html
3. Test all endpoints:
   - GET all
   - GET by ID
   - GET paged
   - POST create
   - PUT update
   - DELETE

### 3. Test Error Cases
- Try deleting non-existent ID (should return 404)
- Try invalid data (should return 400)
- Try ID mismatch in PUT (should return 400)

---

## Troubleshooting

**Problem:** `Build failed: 'I[Entity]Repository' does not exist`
- **Solution:** Ensure you've created the repository interface in Domain layer

**Problem:** `Build failed: 'Get[Entity]Dto' does not exist`
- **Solution:** Ensure you've created all three DTO files

**Problem:** `Build failed: Service not registered in DependencyInjection`
- **Solution:** Check that your service follows the naming convention: `I[ServiceName]` ? `[ServiceName]Service`

**Problem:** `Swagger shows 400 validation errors`
- **Solution:** Check your validator rules match your DTO properties

---

## Next Steps

After creating your entity service and controller:

1. **Add Authorization** (if needed)
   ```csharp
   [Authorize(Roles = "Admin")]
   [HttpPost]
   ```

2. **Add Caching** (if entity is frequently accessed)
   ```csharp
   private readonly ICacheService _cache;
   ```

3. **Add Custom Queries** (if needed)
   ```csharp
   public async Task<List<Get[Entity]Dto>> GetByCustomFilterAsync(...)
   ```

4. **Add Soft Delete Recovery** (for auditable entities)
   ```csharp
   [HttpPost("{id:guid}/restore")]
   public async Task<IActionResult> Restore(Guid id)
   ```