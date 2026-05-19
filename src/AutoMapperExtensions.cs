using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Threading.Tasks;

namespace Lycoris.AutoMapper.Extensions
{
    /// <summary>
    ///
    /// </summary>
    public static class AutoMapperExtensions
    {
        private static IServiceProvider? _serviceProvider;
        private static IMapper? _mapper;

        private static IMapper Mapper => _mapper ??= (_serviceProvider?.GetRequiredService<IMapper>()
            ?? throw new InvalidOperationException("AutoMapper extensions not initialized. Call app.UseAutoMapperExtensions() or serviceProvider.UseAutoMapperExtensions() during startup."));

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceProvider"></param>
        internal static void SetAutoMapperServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider ??= serviceProvider;
            _mapper = null;
        }

        /// <summary>
        /// 添加AutoMapper扩展
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void UseAutoMapperExtensions(this WebApplication applicationBuilder)
        {
            _serviceProvider = applicationBuilder.Services;
            _mapper = null;
        }

        /// <summary>
        /// 添加AutoMapper扩展
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void UseAutoMapperExtensions(this IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _mapper = null;
        }

        // ==================== ToMap ====================

        /// <summary>
        /// 实体映射
        /// </summary>
        public static TDestination ToMap<TSource, TDestination>(this TSource source)
            => Mapper.Map<TSource, TDestination>(source);

        /// <summary>
        /// 实体映射（带配置回调）
        /// </summary>
        public static TDestination ToMap<TSource, TDestination>(this TSource source, Action<TDestination> configure)
        {
            var destination = Mapper.Map<TSource, TDestination>(source);
            configure.Invoke(destination);
            return destination;
        }

        /// <summary>
        /// 实体映射
        /// </summary>
        public static TDestination ToMap<TDestination>(this object source)
            => Mapper.Map<TDestination>(source);

        /// <summary>
        /// 实体映射（带配置回调）
        /// </summary>
        public static TDestination ToMap<TDestination>(this object source, Action<TDestination> configure)
        {
            var destination = Mapper.Map<TDestination>(source);
            configure.Invoke(destination);
            return destination;
        }

        /// <summary>
        /// 实体映射（异步配置回调）
        /// </summary>
        public static async Task<TDestination> ToMapAsync<TDestination>(this object source, Func<TDestination, Task> configure)
        {
            var destination = Mapper.Map<TDestination>(source);
            await configure.Invoke(destination);
            return destination;
        }

        /// <summary>
        /// 实体映射（带异步配置回调）
        /// </summary>
        public static async Task<TDestination> ToMapAsync<TSource, TDestination>(this TSource source, Func<TDestination, Task> configure)
        {
            var destination = Mapper.Map<TSource, TDestination>(source);
            await configure.Invoke(destination);
            return destination;
        }

        // ==================== ToMapList ====================

        /// <summary>
        /// 集合映射
        /// </summary>
        public static List<TDestination> ToMapList<TDestination>(this IEnumerable source)
            => Mapper.Map<List<TDestination>>(source);

        /// <summary>
        /// 集合映射
        /// </summary>
        public static List<TDestination> ToMapList<TSource, TDestination>(this IEnumerable<TSource> source)
            => Mapper.Map<List<TDestination>>(source);

        /// <summary>
        /// 集合映射（带配置回调）
        /// </summary>
        public static List<TDestination> ToMapList<TSource, TDestination>(this IEnumerable<TSource> source, Action<TDestination> configure)
        {
            var destination = Mapper.Map<List<TDestination>>(source);
            destination.ForEach(x => configure.Invoke(x));
            return destination;
        }

        /// <summary>
        /// 集合映射（带索引配置回调）
        /// </summary>
        public static List<TDestination> ToMapList<TSource, TDestination>(this IEnumerable<TSource> source, Action<TDestination, int> configure)
        {
            var destination = Mapper.Map<List<TDestination>>(source);
            for (int i = 0; i < destination.Count; i++)
                configure.Invoke(destination[i], i);
            return destination;
        }

        /// <summary>
        /// 集合映射（异步配置回调）
        /// </summary>
        public static async Task<List<TDestination>> ToMapListAsync<TSource, TDestination>(this IEnumerable<TSource> source, Func<TDestination, Task> configure)
        {
            var destination = Mapper.Map<List<TDestination>>(source);
            foreach (var item in destination)
                await configure.Invoke(item);
            return destination;
        }

        // ==================== ProjectTo (EF Core) ====================

        /// <summary>
        /// 投影映射（用于EF Core，翻译为SQL执行，避免全表加载）
        /// </summary>
        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source)
            => Mapper.ProjectTo<TDestination>(source);

        /// <summary>
        /// 投影映射（用于EF Core，带参数）
        /// </summary>
        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source, object parameters)
            => Mapper.ProjectTo<TDestination>(source, parameters);

        /// <summary>
        /// 投影映射（用于EF Core，泛型源）
        /// </summary>
        public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> source)
            => Mapper.ProjectTo<TDestination>(source);

        /// <summary>
        /// 投影映射（用于EF Core，泛型源，带参数）
        /// </summary>
        public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> source, object parameters)
            => Mapper.ProjectTo<TDestination>(source, parameters);

        // ==================== ToMerge ====================

        /// <summary>
        /// 将源对象映射到已存在的目标对象（合并映射）
        /// </summary>
        public static TDestination ToMerge<TSource, TDestination>(this TSource source, TDestination destination)
        {
            Mapper.Map(source, destination);
            return destination;
        }

        /// <summary>
        /// 将源对象映射到已存在的目标对象（合并映射，带配置回调）
        /// </summary>
        public static TDestination ToMerge<TSource, TDestination>(this TSource source, TDestination destination, Action<TDestination> configure)
        {
            Mapper.Map(source, destination);
            configure.Invoke(destination);
            return destination;
        }

        /// <summary>
        /// 将源对象映射到已存在的目标对象（合并映射）
        /// </summary>
        public static TDestination ToMerge<TDestination>(this object source, TDestination destination)
        {
            Mapper.Map(source, destination);
            return destination;
        }

        // ==================== UpdateFrom ====================

        /// <summary>
        /// 将源对象的值更新到当前目标对象（同名属性赋值）
        /// <code>user.UpdateFrom(updateDto);</code>
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="destination">目标对象（this）</param>
        /// <param name="source">源对象</param>
        /// <returns></returns>
        public static TDestination UpdateFrom<TSource, TDestination>(this TDestination destination, TSource source)
        {
            Mapper.Map(source, destination);
            return destination;
        }

        /// <summary>
        /// 将源对象的值更新到当前目标对象（同名属性赋值，带配置回调）
        /// <code>user.UpdateFrom(updateDto, u => u.UpdatedAt = DateTime.Now);</code>
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="destination">目标对象（this）</param>
        /// <param name="source">源对象</param>
        /// <param name="configure">更新后的回调</param>
        /// <returns></returns>
        public static TDestination UpdateFrom<TSource, TDestination>(this TDestination destination, TSource source, Action<TDestination> configure)
        {
            Mapper.Map(source, destination);
            configure.Invoke(destination);
            return destination;
        }

        // ==================== ToMapPageList ====================

        /// <summary>
        /// 集合映射为分页列表
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页大小</param>
        public static PageList<TDestination> ToMapPageList<TDestination>(this IEnumerable source, int pageIndex, int pageSize)
        {
            var sourceList = source.Cast<object>().ToList();
            var totalCount = sourceList.Count;
            var pageItems = sourceList.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var mappedItems = Mapper.Map<List<TDestination>>(pageItems);
            return new PageList<TDestination>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 集合映射为分页列表
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页大小</param>
        public static PageList<TDestination> ToMapPageList<TSource, TDestination>(this IEnumerable<TSource> source, int pageIndex, int pageSize)
        {
            var sourceList = source.ToList();
            var totalCount = sourceList.Count;
            var pageItems = sourceList.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var mappedItems = Mapper.Map<List<TDestination>>(pageItems);
            return new PageList<TDestination>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 集合映射为分页列表（异步配置回调）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="configure">异步配置回调</param>
        public static async Task<PageList<TDestination>> ToMapPageListAsync<TSource, TDestination>(this IEnumerable<TSource> source, int pageIndex, int pageSize, Func<TDestination, Task> configure)
        {
            var sourceList = source.ToList();
            var totalCount = sourceList.Count;
            var pageItems = sourceList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var mappedItems = Mapper.Map<List<TDestination>>(pageItems);
            foreach (var item in mappedItems)
                await configure.Invoke(item);
            return new PageList<TDestination>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
