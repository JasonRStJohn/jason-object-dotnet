namespace MeDotNet.Models;

public class Post
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Body { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required string AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = null!;
}
