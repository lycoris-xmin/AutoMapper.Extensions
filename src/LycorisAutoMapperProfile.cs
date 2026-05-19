using AutoMapper;

namespace Lycoris.AutoMapper.Extensions
{
    /// <summary>
    ///
    /// </summary>
    public class LycorisAutoMapperProfile : Profile
    {
        /// <summary>
        ///
        /// </summary>
        public LycorisAutoMapperProfile()
        {
            var mapperConfigure = AutoMapperProfileStore.GetAllMapperConfigure();

            if (mapperConfigure != null && mapperConfigure.Any())
            {
                foreach (var item in mapperConfigure)
                {
                    if (item.ConfigureAction != null)
                    {
                        item.ConfigureAction(this);
                    }
                    else
                    {
                        CreateMap(item.Source, item.Destination);
                    }
                }
            }
        }
    }
}
