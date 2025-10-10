namespace Scheduler
{
    public class Schedule
    {
        private const int HOURS_IN_A_DAY = 24;
        public DateOnly Date = DateOnly.MinValue;
        public readonly Session[] Sessions = new Session[HOURS_IN_A_DAY];

        public Schedule(DateOnly date)
        {
            Date = date;
        }

        public bool AddSessions(Session[] sessions)
        {
            //Array.Sort(hours);
            //if (!(hours[^1] < Sessions.Length && hours[0] >= 0))
            //    return false;

            //for (int i = 0; i < hours.Length; i++)
            //{
            //    Sessions[hours[i]] = session;
            //}
            //return true;

            for (int i = 0; i < sessions.Length; i++)
            {
                if (!(sessions[i].Hour < Sessions.Length && sessions[i].Hour >= 0))
                    return false;
            }
            for (int i = 0; i < sessions.Length; i++)
            {
                Sessions[sessions[i].Hour] = sessions[i];
            }
            return true;
        }
        public Session? GetSession(int hour)
        {
            if (hour < Sessions.Length && hour >= 0)
                return Sessions[hour];
            return null;
        }
        public void RemoveSession(int hour, Context con)
        {
            con.RemoveSession(Sessions[hour]);
            Sessions[hour] = null;
        }
    }
}
