using Microsoft.EntityFrameworkCore;

namespace Scheduler
{
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        public DbSet<Session> Sessions { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Schedule>()
        //        .HasMany(s => s.Sessions);
        //}
        public void ConstructSchedules()
        {
            ScheduleManager.FullSchedule = [];

            //for(int i = 0; i < Sessions.Count(); i++)
            //{

            //}

            foreach(Session s in Sessions)
            {
                if (!DateOnly.TryParse(s.date, out DateOnly d))
                {
                    Sessions.Remove(s);
                    continue;
                }

                if (!ScheduleManager.FullSchedule.TryGetValue(d, out Schedule? sd))
                {
                    sd = new Schedule(d);
                    ScheduleManager.FullSchedule.Add(d, sd);
                }

                if (!sd.AddSessions([s]))
                {
                    Sessions.Remove(s);
                    continue;
                }
            }
        }
        public bool AddSessions(Session[] session)
        {
            for (int i = 0; i < session.Length; i++)
            {
                if (Sessions.Contains(session[i]))
                    continue;
                Sessions.Add(session[i]);
            }
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
