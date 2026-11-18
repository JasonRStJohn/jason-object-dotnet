namespace MeDotNet.Models;

/// <summary>A lightweight pointer to another record entry, for previous/next links.</summary>
public record PostLink(string Title, string Slug);

/// <summary>The published entries either side of one entry, in the record's own (CreatedAt) order.</summary>
public record PostNeighbours(PostLink? Newer, PostLink? Older);
