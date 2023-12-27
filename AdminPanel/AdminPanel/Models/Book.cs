public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = "def";
    public string Author { get;set; } = "def";
    public string Description { get; set; } = "def";
    public int CountPages { get; set; } = 0;
    public string Picture { get; set; } = "def";
    public List<Genre> Genres { get; set; } = new List<Genre>();
}
