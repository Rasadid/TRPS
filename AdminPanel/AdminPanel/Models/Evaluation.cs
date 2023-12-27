
public class Evaluation
{
    public int Id { get; set; }
    public User User { get; set; } = default!;
    public Book Book { get; set; } = default!;

    public int Rate { get; set; } = 0;
}
