namespace Sample
{
    /// <summary>
    /// 用户实体
    /// </summary>
    public class UserEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
