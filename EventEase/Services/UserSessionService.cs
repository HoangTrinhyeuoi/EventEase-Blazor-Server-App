using EventEase.Data;
using System.Collections.Concurrent;

namespace EventEase.Services;

public class UserSessionService
{
    private readonly ConcurrentDictionary<string, UserSession> _sessions = new();
    private readonly Timer _cleanupTimer;

    public UserSessionService()
    {
        // Clean up expired sessions every 30 minutes
        _cleanupTimer = new Timer(CleanupExpiredSessions, null, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));
    }

    public UserSession GetOrCreateSession(string sessionId)
    {
        return _sessions.GetOrAdd(sessionId, id => new UserSession
        {
            SessionId = id,
            SessionStart = DateTime.Now,
            LastActivity = DateTime.Now
        });
    }

    public UserSession? GetSession(string sessionId)
    {
        _sessions.TryGetValue(sessionId, out var session);
        if (session != null)
        {
            session.LastActivity = DateTime.Now;
        }
        return session;
    }

    public void UpdateSession(string sessionId, Action<UserSession> updateAction)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            updateAction(session);
            session.LastActivity = DateTime.Now;
        }
    }

    public void AddViewedEvent(string sessionId, int eventId)
    {
        UpdateSession(sessionId, session =>
        {
            if (!session.ViewedEvents.Contains(eventId))
            {
                session.ViewedEvents.Add(eventId);
            }
        });
    }

    public void AddRegisteredEvent(string sessionId, int eventId)
    {
        UpdateSession(sessionId, session =>
        {
            if (!session.RegisteredEvents.Contains(eventId))
            {
                session.RegisteredEvents.Add(eventId);
            }
        });
    }

    public void SetUserInfo(string sessionId, string name, string email)
    {
        UpdateSession(sessionId, session =>
        {
            session.UserName = name;
            session.UserEmail = email;
        });
    }

    public List<UserSession> GetActiveSessions()
    {
        var activeThreshold = DateTime.Now.AddHours(-2);
        return _sessions.Values
            .Where(s => s.LastActivity > activeThreshold)
            .ToList();
    }

    private void CleanupExpiredSessions(object? state)
    {
        var expiredThreshold = DateTime.Now.AddHours(-24);
        var expiredSessions = _sessions
            .Where(kvp => kvp.Value.LastActivity < expiredThreshold)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var sessionId in expiredSessions)
        {
            _sessions.TryRemove(sessionId, out _);
        }

        Console.WriteLine($"Cleaned up {expiredSessions.Count} expired sessions");
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}