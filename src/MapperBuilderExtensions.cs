using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.AutoMapper.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class MapperBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapperService(this IServiceCollection services)
        {
            var builder = new MapperBuilder(services);
            builder.BuildMapper();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapperService(this IServiceCollection services, Action<MapperBuilder> configure)
        {
            var builder = new MapperBuilder(services);
            configure(builder);
            builder.BuildMapper();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="services"></param>
        /// <param name="single"></param>
        /// <returns></returns>
        public static IServiceCollection AddMapper<TSource, TDestination>(this IServiceCollection services, bool single = false) where TSource : class where TDestination : class
        {
            if (single)
                AutoMapperProfileStore.AddOrUpdateSingle<TSource, TDestination>();
            else
                AutoMapperProfileStore.AddOrUpdate<TSource, TDestination>();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="single"></param>
        /// <returns></returns>
        public static IServiceCollection AddMapper(this IServiceCollection services, Type source, Type destination, bool single = false)
        {
            if (single)
                AutoMapperProfileStore.AddOrUpdateSingle(source, destination);
            else
                AutoMapperProfileStore.AddOrUpdate(source, destination);
            return services;
        }

        /// <summary>
        /// 添加自定义AutpMapperProfile类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddMapperProfile<T>(this IServiceCollection services) where T : Profile
        {
            services.AddAutoMapper(typeof(T));
            return services;
        }
    }
}
