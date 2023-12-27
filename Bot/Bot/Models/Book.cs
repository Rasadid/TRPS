public class Book
{
    public int Id { get; set; }

    public string? Title { get; set; }
    public string? Author { get;set; }
    public string? Description { get; set; }
    public int? CountPages { get; set; }
    public string? Picture { get; set; }
    public List<Genre>? Genres { get; set; }
}
