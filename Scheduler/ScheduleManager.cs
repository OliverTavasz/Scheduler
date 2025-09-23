using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public static class ScheduleManager
    {
        private static readonly Dictionary<DateOnly, Schedule> FullSchedule = [];
        public static bool AddSchedule(DateOnly date, Schedule schedule) => FullSchedule.TryAdd(date, schedule);
        public static bool RemoveSchedule(DateOnly date) => FullSchedule.Remove(date);
        public static bool AddSession(DateTime date, int[] hours, Session session)
        {
            if (!FullSchedule.TryGetValue(DateOnly.FromDateTime(date), out Schedule? s))
                 s = new Schedule();
            return s.AddSession(hours, session) && AddSchedule(DateOnly.FromDateTime(date), s);
        }
        public static bool RemoveSession(DateTime date)
        {
            if(FullSchedule.TryGetValue(DateOnly.FromDateTime(date), out Schedule? s))
            {
                s.RemoveSession(date.Hour);
                return true;
            }
            return false;
        }
        public static Session GetSession(DateTime date)
        {
            return FullSchedule.TryGetValue(DateOnly.FromDateTime(date), out Schedule? schedule) ? schedule.GetSession(date.Hour) ?? Session.Music : Session.Music;
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
        public static Schedule? GetSchedule(DateTime date)
        {
            FullSchedule.TryGetValue(DateOnly.FromDateTime(date), out Schedule? s);
            return s;
        }
    }
}
