using Lycoris.AutoMapper.Extensions;

namespace Sample
{
    /// <summary>
    /// 用户映射配置 - 展示 ForMember 和 AutoMapperProfile 实用方法
    /// </summary>
    public class UserProfile : AutoMapperProfile
    {
        public UserProfile()
        {
            // 实体 -> DTO：使用实用方法进行脱敏和格式转换
            CreateMap<UserEntity, UserDto>()
                .ForMember(d => d.Phone, o => o.MapFrom(s => HidePhoneNumber(s.Phone)))
                .ForMember(d => d.Email, o => o.MapFrom(s => HideEmailDetails(s.Email, 2)))
                .ForMember(d => d.CreatedTime, o => o.MapFrom(s => DateTimeToUnixTimestamp(s.CreatedAt)))
                .ForMember(d => d.FileSize, o => o.MapFrom(s => ConvertBytesToReadableSize(s.FileSize)));

            // DTO -> 实体（反向映射）
            CreateMap<UserDto, UserEntity>()
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => UnixTimestampToDateTime(s.CreatedTime)));
        }
    }
}
