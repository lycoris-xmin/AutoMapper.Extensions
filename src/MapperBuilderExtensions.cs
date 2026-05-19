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
        /// <param name="single">是否单向映射，默认false（双向）</param>
        /// <param name="configure">成员自定义映射配置</param>
        /// <returns></returns>
        public static IServiceCollection AddMapper<TSource, TDestination>(
            this IServiceCollection services,
            bool single = false,
            Action<IMappingExpression<TSource, TDestination>>? configure = null)
            where TSource : class where TDestination : class
        {
            AutoMapperProfileStore.AddOrUpdateSingleOrDual<TSource, TDestination>(single, configure);
            return services;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="single">是否单向映射，默认false（双向）</param>
        /// <param name="configure">成员自定义映射配置</param>
        /// <returns></returns>
        public static IServiceCollection AddMapper(
            this IServiceCollection services,
            Type source, Type destination,
            bool single = false,
            Action<IMappingExpression>? configure = null)
        {
            AutoMapperProfileStore.AddOrUpdateSingleOrDual(source, destination, single, configure);
            return services;
        }

        /// <summary>
        /// 添加自定义AutpMapperProfile类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddMapperProfile<T>(this IServiceCollection services) where T : Profile, new()
        {
            services.AddAutoMapper(opt => opt.AddProfile<T>());
            return services;
        }

        /// <summary>
        /// 验证所有AutoMapper映射配置是否有效
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void AssertAutoMapperConfigurationIsValid(this IServiceProvider serviceProvider)
        {
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
