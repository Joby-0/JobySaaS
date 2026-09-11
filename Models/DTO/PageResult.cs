// public class PagedResult<T>
// {
//     public List<T> Items { get; set; } = [];
//     public string? NextPageToken { get; set; }
//     public bool HasNextPage => !string.IsNullOrEmpty(NextPageToken);
// }

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
}