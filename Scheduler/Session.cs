namespace Scheduler
{
    public class Session(SessionType type)
    {
        public readonly SessionType Type = type;
        public readonly List<string> Guests = [];
        public readonly List<string> Hosts = [];
        public void AddGuests(string[] guests)
        {
            Guests.AddRange(guests);
        }
        public void RemoveGuests(string[] guests)
        {
            for (int i = 0; i < Guests.Count; i++)
            {
                if (guests.Contains(Guests[i]))
                {
                    Guests.RemoveAt(i);
                    i--;
                }
            }
        }
        public void AddHosts(string[] hosts)
        {
            Hosts.AddRange(hosts);
        }
        public void RemoveHosts(string[] hosts)
        {
            for (int i = 0; i < Hosts.Count; i++)
            {
                if (hosts.Contains(Hosts[i]))
                {
                    Hosts.RemoveAt(i);
                    i--;
                }
            }
        }
        public static Session Music => new(SessionType.Music);
    }
}
