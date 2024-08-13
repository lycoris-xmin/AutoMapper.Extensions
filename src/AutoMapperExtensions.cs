using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace Lycoris.AutoMapper.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class AutoMapperExtensions
    {
        private static IServiceProvider? _serviceProvider = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        internal static void SetAutoMapperServiceProvider(IServiceProvider serviceProvider) => _serviceProvider ??= serviceProvider;

        /// <summary>
        /// 添加AutoMapper扩展
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void UseAutoMapperExtensions(this WebApplication applicationBuilder) => _serviceProvider = applicationBuilder.Services;

        /// <summary>
        /// 实体映射
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination ToMap<TSource, TDestination>(this TSource source)
        {
            if (_serviceProvider == null)
                throw new ArgumentNullException("please use app.UseAutoMapperExtensions reference extension", nameof(IServiceProvider));

            var mapper = _serviceProvider.GetRequiredService<IMapper>();
            return mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// 实体映射
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination ToMap<TDestination>(this object source)
        {
            if (_serviceProvider == null)
                throw new ArgumentNullException("please use app.UseAutoMapperExtensions reference extension", nameof(IServiceProvider));

            var mapper = _serviceProvider.GetRequiredService<IMapper>();
            return mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// 实体映射
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TDestination ToMap<TDestination>(this object source, Action<TDestination> action)
        {
            if (_serviceProvider == null)
                throw new ArgumentNullException("please use app.UseAutoMapperExtensions reference extension", nameof(IServiceProvider));

            var mapper = _serviceProvider.GetRequiredService<IMapper>();
            var destiantion = mapper.Map<TDestination>(source);
            action(destiantion);
            return destiantion;
        }

        /// <summary>
        /// 实体映射
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TDestination> ToMapList<TDestination>(this IEnumerable source)
        {
            if (_serviceProvider == null)
                throw new ArgumentNullException("please use app.UseAutoMapperExtensions reference extension", nameof(IServiceProvider));

            var mapper = _serviceProvider.GetRequiredService<IMapper>();
            return mapper.Map<List<TDestination>>(source);
        }

        /// <summary>
        /// 实体映射
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TDestination> ToMapList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            if (_serviceProvider == null)
                throw new ArgumentNullException("please use app.UseAutoMapperExtensions reference extension", nameof(IServiceProvider));

            var mapper = _serviceProvider.GetRequiredService<IMapper>();
            return mapper.Map<List<TDestination>>(source);
        }
    }
}
