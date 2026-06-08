# Architecture Reference & Comparison

## SettingService vs Generated Services

### SettingService (Reference Implementation)
```csharp
public class SettingService : ISettingService
{
    private readonly ISettingRepository _repository;
    private readonly ICacheService _cache;  // Additional dependency for caching

    public SettingService(ISettingRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    // Unique methods specific to settings configuration
    public async Task<string?> GetValueAsync(string key)
    public async Task SetValueAsync(string key, string value)

    // Get by key specialization
    public async Task<GetSettingDto?> GetByKeyAsync(string key)
    public async Task DeleteByKeyAsync(string key)

    // Standard CRUD
    public async Task<GetSettingDto> CreateAsync(CreateSettingDto dto)
    public async Task<GetSettingDto> UpdateAsync(UpdateSettingDto dto)
    public async Task DeleteAsync(Guid id)
}
```

**Specializations:**
- Implements caching layer for performance
- Provides key-based operations (in addition to ID-based)
- Includes configuration value helpers

### Generated Services Pattern (Category, Skill, VacationType, Company, JobPost)
```csharp
public class [Entity]Service : I[Entity]Service
{
    private readonly I[Entity]Repository _repository;

    public [Entity]Service(I[Entity]Repository repository)
    {
        _repository = repository;
    }

    // Standard CRUD operations only
    public async Task<Get[Entity]Dto?> GetByIdAsync(Guid id)
    public async Task<List<Get[Entity]Dto>> GetAllAsync()
    public async Task<(List<Get[Entity]Dto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    public async Task<Get[Entity]Dto> CreateAsync(Create[Entity]Dto dto)
    public async Task<Get[Entity]Dto> UpdateAsync(Update[Entity]Dto dto)
    public async Task DeleteAsync(Guid id)
}
```

**Specializations:**
- None (follows core CRUD pattern)
- Can be extended with entity-specific methods as needed

---

## SettingsController vs Generated Controllers

### SettingsController (Reference Implementation)
```csharp
[Route("api/[controller]")]
[ApiController]
public class SettingsController : BaseController
{
    private readonly ISettingService _settingService;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)

    [HttpGet("by-key/{key}")]  // Specialized route
    public async Task<IActionResult> GetByKey(string key)

    [HttpGet]
    public async Task<IActionResult> GetAll()

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(int pageIndex = 1, int pageSize = 10)

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSettingDto dto)

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSettingDto dto)

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)

    [HttpDelete("by-key/{key}")]  // Specialized deletion
    public async Task<IActionResult> DeleteByKey(string key)

    [HttpGet("value/{key}")]  // Configuration helper
    public async Task<IActionResult> GetValue(string key)

    [HttpPost("value/{key}")]  // Configuration helper
    public async Task<IActionResult> SetValue(string key, [FromBody] SetValueRequest request)
}
```

**Specializations:**
- `/by-key/{key}` endpoints for string-key operations
- `/value/{key}` endpoints for configuration value helpers
- Additional domain-specific routes

### Generated Controllers Pattern (Categories, Skills, VacationTypes, Companies, JobPosts)
```csharp
[Route("api/[controller]")]
[ApiController]
public class [Entities]Controller : BaseController
{
    private readonly I[Entity]Service _service;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)

    [HttpGet]
    public async Task<IActionResult> GetAll()

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(int pageIndex = 1, int pageSize = 10)

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Create[Entity]Dto dto)

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Update[Entity]Dto dto)

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
}
```

**Specializations:**
- None (follows core REST pattern)
- Can be extended with entity-specific endpoints as needed

---

## DTO Patterns

### Setting DTOs (Reference)
```csharp
// Get DTO - inherits from BaseAuditableDto
public record GetSettingDto(
    Guid Id,
    string Key,
    string Value,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
) : BaseAuditableDto(Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy);

// Create DTO - simple record
public record CreateSettingDto(
    string Key,
    string Value
);

// Update DTO - inherits from BaseDto
public record UpdateSettingDto(
    Guid Id,
    string Key,
    string Value
) : BaseDto(Id);
```

### Generated DTOs Pattern

#### Non-Auditable Entities (Category, Skill, VacationType)
```csharp
// Get DTO - simple record (no audit fields)
public record GetCategoryDto(
    Guid Id,
    string Name,
    Guid? ParentId
);

// Create DTO
public record CreateCategoryDto(
    string Name,
    Guid? ParentId = null
);

// Update DTO - inherits from BaseDto
public record UpdateCategoryDto(
    Guid Id,
    string Name,
    Guid? ParentId = null
) : BaseDto(Id);
```

#### Auditable Entities (Company, JobPost)
```csharp
// Get DTO - inherits from BaseAuditableDto (same as Setting)
public record GetCompanyDto(
    Guid Id,
    string Name,
    string Description,
    string? LogoUrl,
    string UserId,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
) : BaseAuditableDto(Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy);

// Create DTO
public record CreateCompanyDto(
    string Name,
    string Description,
    string? LogoUrl = null,
    string? UserId = null
);

// Update DTO - inherits from BaseDto (immutable audit fields)
public record UpdateCompanyDto(
    Guid Id,
    string Name,
    string Description,
    string? LogoUrl = null
) : BaseDto(Id);
```

---

## Exception Handling Pattern

All services follow consistent exception handling:

```csharp
public async Task<GetCategoryDto?> GetByIdAsync(Guid id)
{
    var category = await _repository.GetByIdAsync(id);
    if (category == null)
        throw new NotFoundException(nameof(Category), id);  // 404

    return Map(category);
}

public async Task<GetCategoryDto> CreateAsync(CreateCategoryDto dto)
{
    // Business rule validation
    if (await _repository.AnyAsync(c => c.Name == dto.Name))
        throw new ConflictException($"Category with name '{dto.Name}' already exists");  // 409

    Category entity = new Category { Name = dto.Name };
    await _repository.AddAsync(entity);
    await _repository.SaveChangesAsync();

    return Map(entity);
}
```

**Exception Types Used:**
- `NotFoundException` ? HTTP 404 (SettingService reference)
- `ConflictException` ? HTTP 409 (business rule violations)
- `ValidationException` ? HTTP 400 (handled by ValidationFilter middleware)

---

## Repository Interface Usage

### IRepository<T> - For Non-Auditable Entities
Used by: Category, Skill, VacationType

```csharp
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    // Query
    Task<TEntity?> GetByIdAsync(Guid id, ...);
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, ...);
    Task<List<TEntity>> GetAllAsync(...);
    Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(...);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, ...);

    // Command
    Task<TEntity> AddAsync(TEntity entity, ...);
    void Update(TEntity entity);
    void Delete(TEntity entity);  // Hard delete
    Task<int> SaveChangesAsync(...);
}
```

### IAuditableRepository<T> - For Auditable Entities
Used by: Company, JobPost

```csharp
public interface IAuditableRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseAuditableEntity
{
    // Additional auditable operations
    Task<TEntity?> GetByIdIncludingDeletedAsync(...);
    Task<List<TEntity>> GetAllIncludingDeletedAsync(...);
    Task<List<TEntity>> GetDeletedOnlyAsync(...);

    Task SoftDeleteAsync(Guid id, ...);  // Soft delete
    Task SoftDeleteRangeAsync(IEnumerable<Guid> ids, ...);
    Task RestoreAsync(Guid id, ...);
    Task HardDeleteAsync(Guid id, ...);

    Task<List<TEntity>> GetByCreatorAsync(string createdBy, ...);
    Task<List<TEntity>> GetByModifierAsync(string updatedBy, ...);
    Task<List<TEntity>> GetCreatedBetweenAsync(DateTime startDate, DateTime endDate, ...);
    Task<List<TEntity>> GetModifiedBetweenAsync(DateTime startDate, DateTime endDate, ...);
}
```

---

## Service Implementation Comparison

| Feature | Setting | Category | Skill | VacationType | Company | JobPost |
|---------|---------|----------|-------|--------------|---------|---------|
| GetByIdAsync | ? | ? | ? | ? | ? | ? |
| GetAllAsync | ? | ? | ? | ? | ? | ? |
| GetPagedAsync | ? | ? | ? | ? | ? | ? |
| CreateAsync | ? | ? | ? | ? | ? | ? |
| UpdateAsync | ? | ? | ? | ? | ? | ? |
| DeleteAsync | ? | ? | ? | ? | ? (soft) | ? (soft) |
| GetByKeyAsync | ? | ? | ? | ? | ? | ? |
| DeleteByKeyAsync | ? | ? | ? | ? | ? | ? |
| GetValueAsync | ? | ? | ? | ? | ? | ? |
| SetValueAsync | ? | ? | ? | ? | ? | ? |
| Caching | ? | ? | ? | ? | ? | ? |
| Audit Trail | ? | ? | ? | ? | ? | ? |

---

## Controller Route Comparison

| Endpoint | Setting | Category | Skill | VacationType | Company | JobPost |
|----------|---------|----------|-------|--------------|---------|---------|
| GET /{id} | ? | ? | ? | ? | ? | ? |
| GET / | ? | ? | ? | ? | ? | ? |
| GET /paged | ? | ? | ? | ? | ? | ? |
| GET /by-key/{key} | ? | ? | ? | ? | ? | ? |
| GET /value/{key} | ? | ? | ? | ? | ? | ? |
| POST / | ? | ? | ? | ? | ? | ? |
| PUT /{id} | ? | ? | ? | ? | ? | ? |
| PUT /by-key/{key} | ? (commented) | ? | ? | ? | ? | ? |
| DELETE /{id} | ? | ? | ? | ? | ? | ? |
| DELETE /by-key/{key} | ? | ? | ? | ? | ? | ? |
| POST /value/{key} | ? | ? | ? | ? | ? | ? |

---

## Validation Rules Comparison

| Entity | Field Validation | Pattern Matching | Business Rules |
|--------|------------------|------------------|-----------------|
| Setting | Key (100 chars, regex) | ? | Unique key |
| Category | Name (200 chars) | ? | Optional parent |
| Skill | Name (100 chars) | ? | ? |
| VacationType | Name (100 chars) | ? | ? |
| Company | Name, Description, LogoUrl (URL regex) | ? | URL validation |
| JobPost | Title, Description, Requirements, Dates | ? | Future date only |

---

## Design Pattern Consistency

? **Repository Pattern**: All services use repository injection
? **DTO Pattern**: Clear separation of concerns with specialized DTOs
? **Async/Await**: Consistent throughout all layers
? **Exception Handling**: Standardized exception types
? **Validation**: FluentValidation with consistent rules
? **Mapping**: Static Map methods for entity-to-DTO conversion
? **Dependency Injection**: Constructor-based, scoped lifetime
? **API Conventions**: RESTful endpoints with proper HTTP methods
? **Documentation**: Comprehensive XML comments on all public members
? **Code Style**: Consistent naming, formatting, and organization

---

## Extension Points for Future Development

### 1. Entity-Specific Queries
```csharp
// Example: JobPostService extension
public async Task<List<GetJobPostDto>> GetByCategoryAsync(Guid categoryId)
public async Task<List<GetJobPostDto>> GetByCompanyAsync(Guid companyId)
public async Task<List<GetJobPostDto>> GetActiveAsync()
```

### 2. Advanced Filtering
```csharp
// Example: Controller extension
[HttpGet("search")]
public async Task<IActionResult> Search([FromQuery] JobPostSearchFilter filter)
```

### 3. Nested DTOs
```csharp
// Example: Include related data
public record GetJobPostDetailedDto(
    ...
    GetCompanyDto Company,
    GetCategoryDto Category,
    List<GetSkillDto> RequiredSkills
) : BaseAuditableDto(...);
```

### 4. Caching Strategy
```csharp
// Apply caching pattern to frequently accessed entities
public class CategoryService
{
    private readonly ICacheService _cache;
    private const string CacheKey = "categories:all";

    public async Task<List<GetCategoryDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<GetCategoryDto>>(CacheKey);
        if (cached is not null) return cached;

        var list = await _repository.GetAllAsync();
        var dtos = list.Select(Map).ToList();
        await _cache.SetAsync(CacheKey, dtos, TimeSpan.FromMinutes(30));
        return dtos;
    }
}
```

### 5. Authorization
```csharp
// Add to controllers
[Authorize(Roles = "Admin")]
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
```