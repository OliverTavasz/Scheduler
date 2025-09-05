namespace Scheduler
{
    public class Schedule
    {
        private const int HOURS_IN_A_DAY = 24;
        private readonly Session[] Sessions = new Session[HOURS_IN_A_DAY];
        public bool AddSession(int[] hours, Session session)
        {
            Array.Sort(hours);
            if (!(hours[^1] < Sessions.Length && hours[0] >= 0))
                return false;

            for (int i = 0; i < hours.Length; i++)
            {
                Sessions[hours[i]] = session;
            }
            return true;
        }
        public Session? GetSession(int hour)
        {
            if (hour < Sessions.Length && hour >= 0)
                return Sessions[hour];
            return null;
        }
    }
}
