using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.AutoMapper.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MapperBuilder
    {
        private readonly List<Type> Mapper = new() { typeof(LycorisAutoMapperProfile) };

        private readonly IServiceCollection services;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public MapperBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="single"></param>
        /// <returns></returns>
        public MapperBuilder AddMapper<TSource, TDestination>(bool single = true) where TSource : class where TDestination : class
        {
            if (single)
                AutoMapperProfileStore.AddOrUpdateSingle<TSource, TDestination>();
            else
                AutoMapperProfileStore.AddOrUpdate<TSource, TDestination>();

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="single"></param>
        /// <returns></returns>
        public MapperBuilder AddMapper(Type source, Type destination, bool single = true)
        {
            if (single)
                AutoMapperProfileStore.AddOrUpdateSingle(source, destination);
            else
                AutoMapperProfileStore.AddOrUpdate(source, destination);

            return this;
        }

        /// <summary>
        /// 添加自定义AutpMapperProfile类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public MapperBuilder AddMapperProfile<T>() where T : Profile
        {
            var type = typeof(T);
            if (Mapper.Any(x => x != type))
                Mapper.Add(type);

            return this;
        }

        /// <summary>
        /// 程序启动后自动引用扩展
        /// 建议使用：<see cref="AutoMapperExtensions.UseAutoMapperExtensions"/> 手动引用全局扩展
        /// </summary>
        /// <returns></returns>
        [Obsolete("The automatic reference extension function is too simple and needs to consume a startup task. It is recommended to use 'app.UseAutoMapperExtensions' for extension reference")]
        public MapperBuilder EnableAutoReferenceExtensions()
        {
            this.services.AddHostedService<MapperHostedService>();
            return this;
        }

        /// <summary>
        /// 构建
        /// </summary>
        internal void BuildMapper()
        {
            this.services.AddAutoMapper(opt =>
            {
                var maps = new List<Profile>();

                foreach (var item in Mapper)
                    maps.Add((Activator.CreateInstance(item) as Profile)!);

                opt.AddProfiles(maps);
            });
        }
    }
}
