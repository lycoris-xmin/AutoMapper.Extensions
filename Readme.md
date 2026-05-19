<h1 align="center">Lycoris.AutoMapper.Extensions</h1>

<p align="center">
  <strong>AutoMapper 的便捷扩展库，提供 Fluent API 注册、全局扩展方法和实用映射工具</strong>
</p>

<p align="center">
  <img alt=".NET 8" src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet">
  <img alt="NuGet" src="https://img.shields.io/badge/NuGet-v8.2.0-blue?logo=nuget">
  <img alt="License" src="https://img.shields.io/badge/license-MIT-green">
</p>

---

## 目录

- [快速开始](#快速开始)
- [注册映射](#注册映射)
- [API 参考](#api-参考)
  - [单一映射 ToMap](#单一映射-tomap)
  - [集合映射 ToMapList](#集合映射-tomaplist)
  - [分页映射 ToMapPageList](#分页映射-tomappagelist)
  - [合并映射 ToMerge / UpdateFrom](#合并映射-tomerge--updatefrom)
  - [EF Core 投影映射 ProjectTo](#ef-core-投影映射-projectto)
  - [配置验证](#配置验证)
- [AutoMapperProfile 实用方法](#automapperprofile-实用方法)
- [PageList&lt;T&gt; 类型](#pagelistt-分页结果类型)
- [最佳实践](#最佳实践)
- [版本历史](#版本历史)
- [开源协议](#开源协议)

---

## 快速开始

### 安装

```shell
dotnet add package Lycoris.AutoMapper.Extensions
```

### 最小配置

```csharp
using Lycoris.AutoMapper.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 注册 AutoMapper
builder.Services.AddAutoMapperService();

var app = builder.Build();

// 启用全局扩展（必须在 app.Run() 之前调用）
app.UseAutoMapperExtensions();

app.Run();
```

### 配置映射关系

```csharp
builder.Services.AddAutoMapperService(mapper =>
{
    // 单向映射
    mapper.AddMapper<SourceA, DestA>(single: true);

    // 双向映射（默认）
    mapper.AddMapper<SourceB, DestB>();

    // 带 ForMember 自定义映射
    mapper.AddMapper<UserEntity, UserDto>(single: true, configure: opt =>
    {
        opt.ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"));
        opt.ForMember(d => d.Password, o => o.Ignore());
    });

    // 注册自定义 Profile
    mapper.AddMapperProfile<CustomProfile>();
});
```

---

## 注册映射

### 方式一：Builder API（推荐）

```csharp
builder.Services.AddAutoMapperService(mapper =>
{
    // 单向映射：SourceA → DestA
    mapper.AddMapper<SourceA, DestA>(single: true);

    // 双向映射：SourceB ⇄ DestB
    mapper.AddMapper<SourceB, DestB>(single: false);

    // 带 ForMember 配置
    mapper.AddMapper<UserEntity, UserDto>(single: true, configure: opt =>
    {
        opt.ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"));
    });

    // 添加自定义 Profile
    mapper.AddMapperProfile<CustomProfile>();
});
```

### 方式二：IServiceCollection 扩展

```csharp
// 单向映射
builder.Services.AddMapper<SourceA, DestA>(single: true);

// 双向映射（默认）
builder.Services.AddMapper<SourceB, DestB>();

// 带 ForMember 配置
builder.Services.AddMapper<UserEntity, UserDto>(single: true, configure: opt =>
{
    opt.ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"));
});

// 添加自定义 Profile
builder.Services.AddMapperProfile<CustomProfile>();
```

---

## API 参考

### 单一映射 ToMap

将单个对象映射为目标类型。

| 方法 | 说明 |
|------|------|
| `obj.ToMap<TDestination>()` | 映射到目标类型，自动推断源类型 |
| `obj.ToMap<TSource, TDestination>()` | 显式指定源类型和目标类型 |
| `obj.ToMap<TDestination>(d => ...)` | 映射后执行回调 |
| `obj.ToMap<TSource, TDestination>(d => ...)` | 显式类型 + 回调 |
| `obj.ToMapAsync<TDestination>(async d => ...)` | 异步回调 |

```csharp
var dto = entity.ToMap<UserDto>();
var dto2 = entity.ToMap<UserDto>(d => d.Name = $"[VIP] {d.Name}");
var dto3 = entity.ToMap<UserEntity, UserDto>();
```

### 集合映射 ToMapList

将集合映射为目标类型列表。

| 方法 | 说明 |
|------|------|
| `list.ToMapList<TDestination>()` | 集合映射 |
| `list.ToMapList<TSource, TDestination>()` | 显式源类型 |
| `list.ToMapList<TDestination>((d, i) => ...)` | 带索引回调 |
| `list.ToMapListAsync<TSource, TDestination>(async d => ...)` | 异步回调 |

```csharp
var dtos = entities.ToMapList<UserDto>();
var dtos2 = entities.ToMapList<UserDto>((d, i) => d.Rank = i + 1);
```

### 分页映射 ToMapPageList

映射集合并返回分页结果 `PageList<T>`。

```csharp
var result = entities.ToMapPageList<UserDto>(pageIndex: 1, pageSize: 10);

// 返回 PageList<UserDto>
Console.WriteLine($"第 {result.PageIndex} 页 / 共 {result.TotalPages} 页");
Console.WriteLine($"总记录: {result.TotalCount}，当前页: {result.Items.Count} 条");
Console.WriteLine($"上一页: {result.HasPrevious}，下一页: {result.HasNext}");

// 异步回调版本
var result2 = await entities.ToMapPageListAsync<UserDto>(1, 10, async d => await DoSomething(d));
```

### 合并映射 ToMerge / UpdateFrom

将源对象属性覆盖到已存在的目标对象，适用于部分更新场景。

| 方法 | 调用视角 | 语义 |
|------|----------|------|
| `source.ToMerge(destination)` | 从源对象出发 | "将 source 合并到 destination" |
| `destination.UpdateFrom(source)` | 从目标对象出发 | "用 source 更新 destination" |

```csharp
var user = db.Users.Find(id);
var request = new UserUpdateRequest { Name = "新名字" };

// 方式一：ToMerge — 源视角
var merged = request.ToMerge(user);

// 方式二：UpdateFrom — 目标视角（更符合直觉）
var updated = user.UpdateFrom(request);

// 带回调
var result = user.UpdateFrom(request, u => u.UpdatedAt = DateTime.Now);
```

> **注意：** 两个方法底层逻辑相同，均调用 `IMapper.Map(source, destination)`。仅 `this` 扩展对象不同，按代码可读性选择。

### EF Core 投影映射 ProjectTo

将映射表达式翻译为 SQL，在数据库端执行，**避免全表数据加载到内存**。是 EF Core 场景下 `ToMapList()` 的最佳替代。

| 方法 | 说明 |
|------|------|
| `query.ProjectTo<TDestination>()` | 投影映射 |
| `query.ProjectTo<TDestination>(parameters)` | 带运行时参数 |
| `query.ProjectTo<TSource, TDestination>()` | 显式泛型源类型 |

```csharp
// 翻译为 SQL SELECT，只查询需要的列
var dtos = await dbContext.Users
    .Where(u => u.IsActive)
    .ProjectTo<UserDto>()
    .ToListAsync();

// 带运行时参数
var dtos2 = await dbContext.Users
    .ProjectTo<UserDto>(new { CurrentTime = DateTime.Now })
    .ToListAsync();
```

### 配置验证

在应用启动时验证所有映射配置是否正确。

```csharp
var app = builder.Build();
app.UseAutoMapperExtensions();

// 开发环境验证，配错会抛出异常
#if DEBUG
app.Services.AssertAutoMapperConfigurationIsValid();
#endif

app.Run();
```

---

## AutoMapperProfile 实用方法

继承 `AutoMapperProfile`（而非原生的 `Profile`）即可在映射配置中直接使用以下内置方法：

### 数据脱敏

| 方法 | 说明 | 输入 → 输出 |
|------|------|-------------|
| `HidePhoneNumber(string?)` | 手机号 | `13812345678` → `138****5678` |
| `HideEmailDetails(string?, int)` | 邮箱 | `zhangsan@example.com` → `zh****@example.com` |
| `HideIdCard(string?)` | 身份证 | `320106199001011234` → `320****1234` |
| `HideBankCard(string?)` | 银行卡 | `6222021234567890` → `**** **** **** 7890` |
| `HideSensitiveInfo(string?, int, bool)` | 通用脱敏 | 按比例保留 |
| `HideSensitiveInfo(string?, int, int, bool)` | 通用脱敏 | 指定保留字符数 |

### 格式转换

| 方法 | 说明 | 输入 → 输出 |
|------|------|-------------|
| `ConvertBytesToMegabytes(long?)` | 字节 → MB | `1048576` → `1.0` |
| `ConvertBytesToReadableSize(long?)` | 字节 → 可读格式 | `2048576` → `"1.95 MB"` |
| `UnixTimestampToDateTime(long?)` | Unix 毫秒 → DateTime | `1715152800000` → `2024-05-08 16:00:00` |
| `DateTimeToUnixTimestamp(DateTime?)` | DateTime → Unix 毫秒 | 反向转换 |
| `TryMapperEnum<T>(string?)` | 字符串 → 枚举 | `"EN"` → `TestEnum.EN` |

### 字符串处理

| 方法 | 说明 |
|------|------|
| `TrimAndNullIfEmpty(string?)` | 裁剪空白，空串转 `null` |
| `ChangeEmptyStringToNull(string?)` | 空字符串转 `null` |

### 使用示例

```csharp
public class UserProfile : AutoMapperProfile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDto>()
            .ForMember(d => d.Phone,    o => o.MapFrom(s => HidePhoneNumber(s.Phone)))
            .ForMember(d => d.Email,    o => o.MapFrom(s => HideEmailDetails(s.Email, 2)))
            .ForMember(d => d.FileSize, o => o.MapFrom(s => ConvertBytesToReadableSize(s.FileSize)))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => DateTimeToUnixTimestamp(s.CreatedAt)));
    }
}
```

---

## PageList&lt;T&gt; 分页结果类型

`ToMapPageList()` 返回的标准化分页模型：

| 属性 | 类型 | 说明 |
|------|------|------|
| `Items` | `List<T>` | 当前页数据 |
| `TotalCount` | `int` | 记录总数 |
| `PageIndex` | `int` | 当前页码（从 1 开始） |
| `PageSize` | `int` | 每页记录数 |
| `TotalPages` | `int` | 总页数 |
| `HasPrevious` | `bool` | 是否有上一页 |
| `HasNext` | `bool` | 是否有下一页 |

---

## 最佳实践

| 场景 | 推荐方案 |
|------|----------|
| 小项目、映射关系简单 | `AddMapper<T1, T2>()` + `ToMap()` |
| 中大型项目、复杂映射 | 继承 `AutoMapperProfile`，用 `AddMapperProfile<T>()` 注册 |
| 涉及 EF Core 数据库查询 | **必须**使用 `ProjectTo<T>()`，禁止用 `ToMapList()` |
| REST API 部分更新 (PATCH) | `UpdateFrom()` 或 `ToMerge()` |
| 列表接口需要分页信息 | `ToMapPageList()` |
| 开发/测试环境 | 调用 `AssertAutoMapperConfigurationIsValid()` |

---

## 版本历史

| 版本 | 日期 | 更新内容 |
|------|------|----------|
| **8.2.0** | 2026-05 | 新增 ForMember Builder 支持、ToMerge、UpdateFrom、ToMapPageList、ProjectTo、配置验证、实用脱敏方法；升级 AutoMapper 15.1.3；修复多个 bug |
| **8.1.1** | 2025 | AutoMapper 15.x 迁移 |
| **8.1.0** | — | 初始版本 |

---

## 开源协议

[MIT License](LICENSE) · Copyright &copy; 2026 Lycoris
