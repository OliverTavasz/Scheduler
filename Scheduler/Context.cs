using Microsoft.EntityFrameworkCore;

namespace Scheduler
{
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        public DbSet<Session> Sessions { get; set; }
        public Session GetSession()
        {
            return GetSession(DateTime.Now) ?? Session.Default();
        }
        public Session? GetSession(DateTime date)
        {
            var matching = Sessions.FirstOrDefault(item => item.Date == date.ToShortDateString() && item.Hour == date.Hour);
            if (matching is not null)
                return matching;
            return null;
        }
        public Session[] GetSchedule()
        {
            return GetSchedule(DateOnly.FromDateTime(DateTime.Now));
        }
        public Session[] GetSchedule(DateOnly date)
        {
            var matching = Sessions.Where(item => item.Date == date.ToString());
            if (matching is not null)
                return [.. matching];

            return [.. Enumerable.Repeat(Session.Default(), 24)];
        }
        public bool AddSessions(Session[] session)
        {
            Sessions.AddRange(session);
            return true;
        }
        public bool RemoveSession(Session session)
        {
            if (Sessions.Contains(session))
            {
                Sessions.Remove(session);
                return true;
            }
            return false;
        }
    }
}
