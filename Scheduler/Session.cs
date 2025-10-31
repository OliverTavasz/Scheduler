using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Scheduler
{
    public class Session
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Guests { get; set; } = "";
        public string Hosts { get; set; } = "";
        public string date { get; set; } = "";
        public int Hour { get; set; } = 0;

        public Session(int type, int hour, Schedule parent)
        {
            Type = type;
            Hour = hour;
            date = parent.Date.ToString();
        }
        public Session(int type, int hour, DateOnly datetime)
        {
            Type = type;
            Hour = hour;
            date = datetime.ToString();
        }
        public Session()
        {

        }

        public void SetValues(Session s)
        {
            Type = s.Type;
            Hour = s.Hour;
            Hosts = s.Hosts;
            Guests = s.Guests;
            date = s.date;
        }

        public void AddGuests(string[]? guests)
        {
            if (guests is null)
                return;

            List<string> g = [.. Guests.Split(',')];
            g.AddRange(guests);

            string r = "";
            for (int i = 0; i < g.Count; i++)
            {
                r += g[i] + ",";
            }
            r.Remove(r.Length - 1);

            Guests = r;
        }
        public void RemoveGuests(string[] guests)
        {
            List<string> g = [.. Guests.Split(',')];
            for (int i = 0; i < guests.Length; i++)
            {
                if (g.Contains(guests[i]))
                    g.Remove(guests[i]);
            }

            string r = "";
            for (int i = 0; i < g.Count; i++)
            {
                r += g[i] + ",";
            }
            r.Remove(r.Length - 1);

            Guests = r;
        }
        public void AddHosts(string[]? hosts)
        {
            if (hosts is null)
                return;

            List<string> h = [.. Hosts.Split(',')];
            h.AddRange(hosts);

            string r = "";
            for (int i = 0; i < h.Count; i++)
            {
                r += h[i] + ",";
            }
            r.Remove(r.Length - 1);

            Hosts = r;
        }
        public void RemoveHosts(string[] hosts)
        {
            List<string> h = [.. Hosts.Split(',')];
            for (int i = 0; i < hosts.Length; i++)
            {
                if (h.Contains(hosts[i]))
                    h.Remove(hosts[i]);
            }

            string r = "";
            for (int i = 0; i < h.Count; i++)
            {
                r += h[i] + ",";
            }
            r.Remove(r.Length - 1);

            Hosts = r;
        }
    }
}
