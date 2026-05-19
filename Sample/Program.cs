using Lycoris.AutoMapper.Extensions;
using Sample;

var builder = WebApplication.CreateBuilder(args);

// 使用扩展库的 Builder API 注册 AutoMapper
builder.Services.AddAutoMapperService(mapper =>
{
    // 注册自定义 Profile
    mapper.AddMapperProfile<UserProfile>();

    // 使用 Builder 注册简单映射（单向）
    mapper.AddMapper<SampleModel, SampleDto>(single: true);

    // 使用 Builder 注册带 ForMember 的映射
    // mapper.AddMapper<UserEntity, UserDto>(single: true, configure: opt =>
    // {
    //     opt.ForMember(d => d.Phone, o => o.Ignore());
    // });
});

var app = builder.Build();

// 初始化 AutoMapper 扩展（必须在 app.Run() 之前调用）
app.UseAutoMapperExtensions();

// ==================== 演示端点 ====================

// 1. 基本实体映射 ToMap<T>()
app.MapGet("/api/user/{id}", (int id) =>
{
    var entity = GetSampleUser(id);
    return entity.ToMap<UserDto>();
});

// 2. 列表映射 ToMapList<T>()
app.MapGet("/api/users", () =>
{
    var entities = GetSampleUsers();
    return entities.ToMapList<UserDto>();
});

// 3. 分页映射 ToMapPageList<T>()
app.MapGet("/api/users/page", (int pageIndex, int pageSize) =>
{
    var entities = GetSampleUsers();
    return entities.ToMapPageList<UserDto>(pageIndex, pageSize);
});

// 4. UpdateFrom() - 从目标对象视角更新，语义更清晰
app.MapPut("/api/user/{id}/update", (int id, UserUpdateRequest request) =>
{
    var entity = GetSampleUser(id);
    var result = entity.UpdateFrom(request);
    return new { Message = "Updated from request", Result = result };
});

// 4b. ToMerge() - 从源对象视角合并
app.MapPut("/api/user/{id}", (int id, UserUpdateRequest request) =>
{
    var entity = GetSampleUser(id);
    var merged = request.ToMerge(entity);
    return new { Message = "Merged successfully", Original = entity, Request = request, Result = merged };
});

// 5. 带配置回调的单对象映射
app.MapGet("/api/user/{id}/summary", (int id) =>
{
    var entity = GetSampleUser(id);
    return entity.ToMap<UserDto>(dto =>
    {
        // 映射后追加自定义逻辑
        dto.Name = $"[VIP] {dto.Name}";
    });
});

// 6. 原始扩展映射（保留兼容性示例）
app.MapGet("/weatherforecast", () =>
{
    var dto = new SampleDto() { Test = "1" };
    var model = dto.ToMap<SampleModel>();

    var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
    return Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )).ToArray();
});

// 7. 配置验证端点（开发环境使用）
app.MapGet("/api/config/validate", (IServiceProvider sp) =>
{
    sp.AssertAutoMapperConfigurationIsValid();
    return new { Valid = true, Message = "All AutoMapper configurations are valid." };
});

app.Run();

// ==================== 辅助方法 ====================

static UserEntity GetSampleUser(int id) => new()
{
    Id = id,
    Name = "张三",
    Email = "zhangsan@example.com",
    Phone = "13812345678",
    FileSize = 2048576,
    CreatedAt = DateTime.Now.AddDays(-30)
};

static List<UserEntity> GetSampleUsers() => Enumerable.Range(1, 25).Select(i => new UserEntity
{
    Id = i,
    Name = $"用户{i:D2}",
    Email = $"user{i}@example.com",
    Phone = $"138{i:D8}",
    FileSize = Random.Shared.NextInt64(512, 10485760),
    CreatedAt = DateTime.Now.AddDays(-Random.Shared.Next(1, 365))
}).ToList();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
