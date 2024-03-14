namespace Lycoris.AutoMapper.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class LycorisMapper
    {
        /// <summary>
        /// 
        /// </summary>
        internal LycorisAutoMapperProfile? Mapper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        protected void CreateMap<T1, T2>() => Mapper!.CreateMap<T1, T2>();
    }
}
