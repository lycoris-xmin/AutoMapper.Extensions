#### **AutoMapper自定义扩展**

---

### 安装

```shell
dotnet add package Lycoris.AutoMapper.Extensions
```

---

### 引入扩展

**1. 注册扩展，手动引入全局扩展（推荐）**

```csharp
var builder = WebApplication.CreateBuilder(args);

// AutoMapper注册
builder.Services.AddAutoMapperService();

var app = builder.Build();

// 使用AutoMapper全局扩展
app.UseAutoMapperExtensions();

app.Run();
```

**2. 注册扩展并配置映射关系**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapperService(mapper =>
{
    // 单向映射：只能将 TestB 映射为 TestA
    mapper.AddMapper<TestB, TestA>(single: true);

    // 双向映射
    mapper.AddMapper<TestB, TestA>(single: false);

    // 添加自定义Profile
    mapper.AddMapperProfile<CustomProfile>();
});

var app = builder.Build();
app.UseAutoMapperExtensions();
app.Run();
```

---

### 创建映射关系

**1. Builder API 方式**

```csharp
builder.Services.AddAutoMapperService(mapper =>
{
    // 单向映射
    mapper.AddMapper<SourceA, DestA>(single: true);

    // 双向映射
    mapper.AddMapper<SourceB, DestB>(single: false);

    // 带 ForMember 自定义映射配置
    mapper.AddMapper<UserEntity, UserDto>(single: true, configure: opt =>
    {
        opt.ForMember(d => d.FullName, o => o.MapFrom(s => s.FirstName + " " + s.LastName));
        opt.ForMember(d => d.Password, o => o.Ignore());
    });

    // 添加自定义 Profile
    mapper.AddMapperProfile<CustomProfile>();
});
```

**2. IServiceCollection 扩展方式**

```csharp
// 单向映射
builder.Services.AddMapper<SourceA, DestA>(single: true);

// 双向映射（默认）
builder.Services.AddMapper<SourceB, DestB>();

// 带 ForMember 配置
builder.Services.AddMapper<UserEntity, UserDto>(single: true, configure: opt =>
{
    opt.ForMember(d => d.FullName, o => o.MapFrom(s => s.FirstName + " " + s.LastName));
});

// 添加自定义 Profile
builder.Services.AddMapperProfile<CustomProfile>();
```

---

### 使用方法

**1. 实体映射 ToMap**

```csharp
var entity = new UserEntity { Name = "张三", Email = "zhangsan@example.com" };
var dto = entity.ToMap<UserDto>();

// 带配置回调
var dto2 = entity.ToMap<UserDto>(d => d.Name = $"[VIP] {d.Name}");

// 指定源类型和目的类型
var dto3 = entity.ToMap<UserEntity, UserDto>();
var dto4 = entity.ToMap<UserEntity, UserDto>(d => d.Name = "Custom");
```

**2. 集合映射 ToMapList**

```csharp
var entities = new List<UserEntity> { ... };
var dtos = entities.ToMapList<UserDto>();

// 带配置回调
var dtos2 = entities.ToMapList<UserDto>(d => d.Name = d.Name.ToUpper());

// 带索引的配置回调
var dtos3 = entities.ToMapList<UserDto>((d, i) => d.Name = $"[{i}] {d.Name}");
```

**3. 分页映射 ToMapPageList**

```csharp
var entities = GetUsersFromDatabase();
var pageResult = entities.ToMapPageList<UserDto>(pageIndex: 1, pageSize: 10);

// 返回 PageList<UserDto>，包含：
// Items（当前页数据）、TotalCount（总记录数）、
// PageIndex（页码）、PageSize（页大小）、
// TotalPages（总页数）、HasPrevious / HasNext 等
```

**4. 合并映射 ToMerge / UpdateFrom（部分更新场景）**

```csharp
// 源对象视角：将 source 合并到 destination
var merged = updateRequest.ToMerge(existingUser);

// 目标对象视角：用 source 更新 destination（语义更自然）
var updated = existingUser.UpdateFrom(updateRequest);

// 带配置回调
var merged2 = updateRequest.ToMerge(existingUser, u => u.UpdatedAt = DateTime.Now);
var updated2 = existingUser.UpdateFrom(updateRequest, u => u.UpdatedAt = DateTime.Now);
```

**5. EF Core 投影映射 ProjectTo**

```csharp
// 使用 ProjectTo 翻译为 SQL 查询，避免全表加载到内存
var dtos = dbContext.Users
    .Where(u => u.IsActive)
    .ProjectTo<UserDto>()        // 扩展库封装
    .ToListAsync();

// 带查询参数的投影
var dtos2 = dbContext.Users
    .ProjectTo<UserDto>(new { CurrentTime = DateTime.Now })
    .ToListAsync();

// 泛型源
IQueryable<UserEntity> query = dbContext.Users;
var dtos3 = query.ProjectTo<UserEntity, UserDto>().ToList();
```

---

### AutoMapperProfile 实用方法

继承 `AutoMapperProfile` 后可在映射配置中使用以下方法：

| 方法 | 说明 | 示例 |
|------|------|------|
| `ConvertBytesToMegabytes(long?)` | 字节转MB | `1MB = 1024*1024 bytes` |
| `ConvertBytesToReadableSize(long?)` | 字节转可读格式 | `"2.50 GB"` |
| `TryMapperEnum<T>(string?)` | 字符串转枚举 | 安全的Enum.TryParse |
| `HideEmailDetails(string?, int)` | 邮箱地址脱敏 | `"abc***@example.com"` |
| `HidePhoneNumber(string?)` | 手机号脱敏 | `"138****5678"` |
| `HideIdCard(string?)` | 身份证号脱敏 | `"320****1234"` |
| `HideBankCard(string?)` | 银行卡号脱敏 | `"**** **** **** 8888"` |
| `HideSensitiveInfo(string?, int, bool)` | 通用敏感信息脱敏 | 自定义保留位数 |
| `TrimAndNullIfEmpty(string?)` | 修剪空白并转null | `"  "` → `null` |
| `UnixTimestampToDateTime(long?)` | Unix毫秒时间戳转DateTime | - |
| `DateTimeToUnixTimestamp(DateTime?)` | DateTime转Unix毫秒时间戳 | - |
| `ChangeEmptyStringToNull(string?)` | 空字符串转null | `""` → `null` |

**使用示例：**

```csharp
public class UserProfile : AutoMapperProfile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDto>()
            .ForMember(d => d.Phone, o => o.MapFrom(s => HidePhoneNumber(s.Phone)))
            .ForMember(d => d.Email, o => o.MapFrom(s => HideEmailDetails(s.Email, 2)))
            .ForMember(d => d.FileSize, o => o.MapFrom(s => ConvertBytesToReadableSize(s.FileSize)))
            .ForMember(d => d.CreatedTime, o => o.MapFrom(s => DateTimeToUnixTimestamp(s.CreatedAt)));
    }
}
```

---

### 配置验证

```csharp
var app = builder.Build();
app.UseAutoMapperExtensions();

// 在开发环境验证所有映射配置是否有效
app.Services.AssertAutoMapperConfigurationIsValid();

app.Run();
```

---

### PageList&lt;T&gt; 分页结果类型

`ToMapPageList()` 返回 `PageList<T>`，包含以下属性：

| 属性 | 类型 | 说明 |
|------|------|------|
| `Items` | `List<T>` | 当前页数据 |
| `TotalCount` | `int` | 总记录数 |
| `PageIndex` | `int` | 当前页码 |
| `PageSize` | `int` | 每页大小 |
| `TotalPages` | `int` | 总页数 |
| `HasPrevious` | `bool` | 是否有上一页 |
| `HasNext` | `bool` | 是否有下一页 |

---

### 使用建议

- **小项目 / 简单映射**：使用 `AddMapper<TSource, TDestination>()` 和 `ToMap()` 扩展
- **中大型项目 / 复杂映射**：继承 `AutoMapperProfile`，使用 `ForMember` 配置，通过 `AddMapperProfile<T>()` 注册
- **EF Core 场景**：使用 `ProjectTo<T>()` 代替 `ToMapList()`，避免全表数据加载到内存
- **部分更新场景**：使用 `UpdateFrom()` 或 `ToMerge()` 将更新 DTO 的字段合并到现有实体
- **分页接口**：使用 `ToMapPageList()` 返回带分页信息的 `PageList<T>`

---

### 版本历史

- **8.2.0** - 新增 ForMember Builder 支持、ToMerge 合并映射、UpdateFrom 更新映射、ToMapPageList 分页映射、ProjectTo EF Core 支持、配置验证、实用脱敏方法、修复多个 bug
- **8.1.1** - AutoMapper 15.x 迁移
- **8.1.0** - 初始版本
