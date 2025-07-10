#### **AutoMapper自定义扩展**


#### **引入扩展**

- **1. 注册扩展，手动引入全局扩展**
```csharp
var builder = WebApplication.CreateBuilder(args);

// AutoMapper注册
builder.Services.AddAutoMapperService();

// Add services to the container.

var app = builder.Build();

// 使用AutoMapper全局扩展
app.UseAutoMapperExtensions();

app.UseRouting();

//
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
```

- **2. 注册扩展，自动引入全局扩展(不推荐使用，因为就一句话，没必要，做这个的目的是因为作者朋友经常忘记这一句话)**

```csharp
var builder = WebApplication.CreateBuilder(args);

// AutoMapper注册
builder.Services.AddAutoMapperService(builder =>
{
    builder.AddMapper<TestB, TestA>();
    builder.EnableAutoReferenceExtensions();
});

// Add services to the container.

var app = builder.Build();

app.UseRouting();

//
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
```


#### **创建映射关系**

- **1. 注册扩展时同时创建映射关系**

```csharp
builder.Services.AddAutoMapperService(builder =>
{
    // 单向映射：只能将 TestB 映射为 TestA，不能将 TestA 映射为 TestB
    builder.AddSingleMapper<TestB, TestA>();
    // 双向映射：支持 TestB 映射为 TestA，也支持 TestA 映射为 TestB
    builder.AddMapper<TestB, TestA>();
    // 添加自定义AutoMapper映射关系实体，CusotmeProfile 需要继承 Profile
    builder.AddMapperProfile<CusotmeProfile>();
});
```

- **2. IServiceCollection扩展注册**

```csharp
//单向映射
builder.Service.AddSingleMapper<TestB, TestA>();
builder.Service.AddMapper<TestB, TestA>();
builder.Service.AddMapperProfile<CusotmeProfile>();
```


**以上提供的扩展方式，仅建议小项目映射关系少的并且没有复杂的映射逻辑使用，中大型项目还是建议使用`AutoMapper`提供的映射关系实体处理，然后使用`AddMapperProfile`添加映射关系实体处理**

#### **使用方法**

- **1. 实体映射**
```csharp
 var a = new TestA()
 {
     A = 1,
     B = "123"
 };

 var b = a.ToMap<TestB>();
```

- **2. 集合映射**

```csharp
 var alist = new List<TestA>()
 {
     new TestA(){ A = 1, B = "123" }, 
     new TestA(){ A = 1, B = "123" }
 };

 var blist = alist.ToMapList<TestB>();
```

#### **复杂的映射逻辑请使用`AddMapperProfile`添加映射关系实体处理，以下代码仅举例简单的配置**

```csharp
public class ServerServiceProfile : Profile
{
    public ServerServiceProfile()
    {
        // 基础映射(注意此处只是单向映射)
        CreateMap<TestDto, TestViewModel>();

        // 双向映射需要分别写出
        CreateMap<TestServiceDto, TestServiceViewModel>();
        CreateMap<TestServiceViewModel, TestServiceDto>();

        // 指定映射属性
        CreateMap<TestServiceDto, TestServiceViewModel>()
            .ForMember(x => x.StringList, opt => opt.MapFrom(src => src.JsobString));

        // 指定映射属性并处理成需要的内容
        CreateMap<TestServiceDto, TestServiceViewModel>()
            .ForMember(x => x.ObjectText, opt => opt.MapFrom(src => new TestObj()
            {
                Number = int.Parse(src.StringText),
                Title = src.Title
            }));

        // 忽略某些字段的映射
        CreateMap<TestServiceDto, TestServiceDto>()
            .ForMember(x => x.Info, opt => opt.Ignore()))
            .ForMember(x => x.Detail, opt => opt.Ignore()));
    }
}
```

#### 使用小技巧：
**`AutoMapper`会自动转换以下不同的类型的属性**
- **`int`与`enum`的相互转换，但需要注意的是，由`int`转换为`enum`的时候，需要注意`int`值是否存在于`enum`上**


**例：当`int`值为`1`的时候转为`enum`就会出现异常**
```
public enum Test
{
    EN = 0,
    EB = 2
}
```

- **`string`类型与数值类型(`int`、`long`,其他数值类型博主平时使用没怎么需要转换，所以没有测试过)的转换，同样的当`string`转换成数值类型的时候要注意`string`是否是正确的可以转换的数值**
  
