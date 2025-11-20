using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tls;

namespace Scheduler
{
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        public DbSet<Session> Sessions { get; set; }
        public Session GetSession()
        {
            return GetSession(DateTime.Now) ?? Session.Default(DateOnly.FromDateTime(DateTime.Now).ToString(), DateTime.Now.Hour);
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

            Session[] s = new Session[24];

            for(int i = 0; i < s.Length; i++)
            {
                s[i] = Session.Default(date.ToString(), i);
            }

            Session[] matching = [.. Sessions.Where(item => item.Date == date.ToString())];
            if (matching is not null && matching.Length != 0)
            {
                for(int i = 0; i < matching.Length; i++)
                {
                    s[matching[i].Hour] = matching[i];
                }
            }    

            return s;
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
