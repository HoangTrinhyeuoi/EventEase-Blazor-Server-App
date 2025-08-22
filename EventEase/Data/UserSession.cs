namespace EventEase.Data;

public class UserSession
{
    public string SessionId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public List<int> ViewedEvents { get; set; } = new();
    public List<int> RegisteredEvents { get; set; } = new();
    public DateTime SessionStart { get; set; }
    public DateTime LastActivity { get; set; }
    public Dictionary<string, object> SessionData { get; set; } = new();
}