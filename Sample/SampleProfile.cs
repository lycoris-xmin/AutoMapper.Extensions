using Lycoris.AutoMapper.Extensions;

namespace Sample
{
    public class SampleProfile : AutoMapperProfile
    {
        public SampleProfile()
        {
            CreateMap<SampleModel, SampleDto>().ReverseMap();
        }
    }
}
