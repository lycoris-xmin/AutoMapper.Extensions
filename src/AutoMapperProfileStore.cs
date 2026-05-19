using AutoMapper;
using System.Collections.Concurrent;

namespace Lycoris.AutoMapper.Extensions
{
    internal static class AutoMapperProfileStore
    {
        private static readonly ConcurrentDictionary<(Type, Type), TypeMapper> MapperConfigure = new();

        internal static void AddOrUpdateSingle<TSource, TDestination>(Action<IProfileExpression>? configureAction = null)
            => AddOrUpdateSingle(typeof(TSource), typeof(TDestination), configureAction);

        internal static void AddOrUpdateSingle(Type source, Type destination, Action<IProfileExpression>? configureAction = null)
        {
            if (source.FullName == destination.FullName)
                return;

            MapperConfigure.TryAdd((source, destination), new TypeMapper(source, destination, configureAction));
        }

        internal static void AddOrUpdate<TSource, TDestination>(Action<IProfileExpression>? configureAction = null)
            => AddOrUpdate(typeof(TSource), typeof(TDestination), configureAction);

        internal static void AddOrUpdate(Type source, Type destination, Action<IProfileExpression>? configureAction = null)
        {
            if (source.FullName == destination.FullName)
                return;

            MapperConfigure.TryAdd((source, destination), new TypeMapper(source, destination, configureAction));
            MapperConfigure.TryAdd((destination, source), new TypeMapper(destination, source, null));
        }

        internal static void AddOrUpdateSingleOrDual<TSource, TDestination>(
            bool single, Action<IMappingExpression<TSource, TDestination>>? configure = null)
            where TSource : class where TDestination : class
        {
            Action<IProfileExpression>? configureAction = null;
            if (configure != null)
            {
                configureAction = profile =>
                {
                    var expr = profile.CreateMap<TSource, TDestination>();
                    configure(expr);
                };
            }

            if (single)
                AddOrUpdateSingle<TSource, TDestination>(configureAction);
            else
                AddOrUpdate<TSource, TDestination>(configureAction);
        }

        internal static void AddOrUpdateSingleOrDual(
            Type source, Type destination,
            bool single, Action<IMappingExpression>? configure = null)
        {
            Action<IProfileExpression>? configureAction = null;
            if (configure != null)
            {
                configureAction = profile =>
                {
                    var expr = profile.CreateMap(source, destination);
                    configure(expr);
                };
            }

            if (single)
                AddOrUpdateSingle(source, destination, configureAction);
            else
                AddOrUpdate(source, destination, configureAction);
        }

        internal static List<TypeMapper> GetAllMapperConfigure()
            => MapperConfigure.Values.ToList();

        internal class TypeMapper
        {
            public TypeMapper(Type Source, Type Destination, Action<IProfileExpression>? ConfigureAction = null)
            {
                this.Source = Source;
                this.Destination = Destination;
                this.ConfigureAction = ConfigureAction;
            }

            public Type Source { get; set; }

            public Type Destination { get; set; }

            public Action<IProfileExpression>? ConfigureAction { get; set; }
        }
    }
}
