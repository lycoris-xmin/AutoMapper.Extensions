namespace Sample
{
    /// <summary>
    /// 用户DTO（返回给客户端）
    /// </summary>
    public class UserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public long CreatedTime { get; set; }
    }
}
