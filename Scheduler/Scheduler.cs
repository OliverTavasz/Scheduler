using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public static class Scheduler
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
    public class Schedule
    {
        private const int HOURS_IN_A_DAY = 24;
        private readonly Session[] Sessions = new Session[HOURS_IN_A_DAY];
        public bool AddSession(int[] hours, Session session)
        {
            Array.Sort(hours);
            if (Sessions.Length < hours[^1] || hours[0] < 0)
                return false;

            for (int i = 0; i < hours.Length; i++)
            {
                Sessions[hours[i]] = session;
            }
            return true;
        }
        public Session? GetSession(int hour)
        {
            if (Sessions.Length < hour || hour < 0)
                return null;
            return Sessions[hour];
        }
    }
    public class Session(SessionType type, int hosts)
    {
        public readonly SessionType Type = type;
        public readonly int Hosts = hosts;
        public static Session Music => new(SessionType.Music, 0);
    }
    public enum SessionType
    {
        Music,
        Prerecorded,
        Live,
    }
}
