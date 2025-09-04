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
        public static Session GetSession(DateTime date)
        {
            return FullSchedule.TryGetValue(DateOnly.FromDateTime(date), out Schedule? schedule) ? schedule.GetSession(date.Hour) ?? Session.Music : Session.Music;
        }
        public static Session GetCurrentSession()
        {
            return GetSession(DateTime.Now);
        }
    }
}
