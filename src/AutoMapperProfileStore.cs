namespace Lycoris.AutoMapper.Extensions
{
    internal static class AutoMapperProfileStore
    {
        private static readonly List<TypeMapper> MapperConfigure = new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        internal static void AddOrUpdateSingle<TSource, TDestination>()
        {
            var source = typeof(TSource);
            var destination = typeof(TDestination);
            if (source.FullName != destination.FullName)
            {
                if (!MapperConfigure.Any(x => x.Source == source && x.Destination == destination))
                    MapperConfigure.Add(new TypeMapper(source, destination));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        internal static void AddOrUpdateSingle(Type source, Type destination)
        {
            if (source.FullName != destination.FullName && !MapperConfigure.Any(x => x.Source == source && x.Destination == destination))
                MapperConfigure.Add(new TypeMapper(source, destination));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        internal static void AddOrUpdate<TSource, TDestination>()
        {
            var source = typeof(TSource);
            var destination = typeof(TDestination);
            if (source.FullName != destination.FullName)
            {
                if (!MapperConfigure.Any(x => x.Source == source && x.Destination == destination))
                    MapperConfigure.Add(new TypeMapper(source, destination));

                if (!MapperConfigure.Any(x => x.Source == destination && x.Destination == source))
                    MapperConfigure.Add(new TypeMapper(destination, source));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        internal static void AddOrUpdate(Type source, Type destination)
        {
            if (!MapperConfigure.Any(x => x.Source == source && x.Destination == destination))
                MapperConfigure.Add(new TypeMapper(source, destination));

            if (!MapperConfigure.Any(x => x.Source == destination && x.Destination == source))
                MapperConfigure.Add(new TypeMapper(destination, source));
        }

        /// <summary>
        /// 
        /// </summary>
        internal static List<TypeMapper> GetAllMapperConfigure() => MapperConfigure;


        internal class TypeMapper
        {
            public TypeMapper(Type Source, Type Destination)
            {
                this.Source = Source;
                this.Destination = Destination;
            }

            public Type Source { get; set; }

            public Type Destination { get; set; }
        }
    }
}
