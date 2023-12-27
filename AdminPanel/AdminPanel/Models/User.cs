
public class User
{
    public int UserId { get; set; }

    public List<Evaluation> Preferences { get; set; } = new List<Evaluation>();
}
