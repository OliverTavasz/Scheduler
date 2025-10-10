using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Scheduler
{
    public static class ScheduleManager
    {
        public static Dictionary<DateOnly, Schedule> FullSchedule = [];
        public static bool AddSchedule(DateOnly date, Schedule schedule) => FullSchedule.TryAdd(date, schedule);
        public static bool RemoveSchedule(DateOnly date) => FullSchedule.Remove(date);
        public static bool AddSession(DateOnly date, int[] hours, int type, string[]? h, string[]? g, Context con)
        {
            if (!FullSchedule.TryGetValue(date, out Schedule? s))
                s = new Schedule(date);

            Session[] sessions = new Session[hours.Length];
            for(int i = 0; i < hours.Length; i++)
            {
                Session session = new Session(type, hours[i], s);
                if (h is not null)
                    session.AddHosts(h);
                if (g is not null)
                    session.AddGuests(g);

                sessions[i] = session;
            }
            

            return s.AddSessions(sessions) && AddSchedule(date, s) && con.AddSessions(sessions);
        }
        public static bool RemoveSession(DateTime date, Context con)
        {
            if(FullSchedule.TryGetValue(DateOnly.FromDateTime(date), out Schedule? s))
            {
                s.RemoveSession(date.Hour, con);
                return true;
            }
            return false;
        }
        public static Session GetSession(DateTime date)
        {
            return FullSchedule.TryGetValue(DateOnly.FromDateTime(date), out Schedule? schedule) ? schedule.GetSession(date.Hour) ?? new() : new();
        }
        public static Session GetCurrentSession()
        {
            return GetSession(DateTime.Now);
        }
        public static Schedule? GetCurrentSchedule()
        {
            FullSchedule.TryGetValue(DateOnly.FromDateTime(DateTime.Now), out Schedule? s);
            return s;
        }
        public static Schedule? GetSchedule(DateOnly date)
        {
            FullSchedule.TryGetValue(date, out Schedule? s);
            return s;
        }
    }
}
