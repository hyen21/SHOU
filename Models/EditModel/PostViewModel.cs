namespace SHOU.Models.EditModel
{
    public class PostViewModel
    {
        public string Id { get; set; } = null!;

        public string IdUser { get; set; } = null!;

        public string? Image { get; set; }

        public string? Content { get; set; }

        public string? Video { get; set; }

        public DateTime? CreateTime { get; set; }

        public int CountLike { get; set; }

        public int CountCmt { get; set; }

        public bool Liked { get; set; }
        public string? Avatar { get; set; }
        public string? Name { get; set; }
    }
}
