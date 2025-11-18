using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Scheduler
{
    public class Session
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Type { get; set; }
        public string Date { get; set; } = string.Empty;
        public int Hour { get; set; }
        public string Guests { get; set; } = string.Empty;
        public string Hosts { get; set; } = string.Empty;

        public Session(string name, int type, DateOnly date, int hour)
        {
            Name = name;
            Type = type;
            Date = date.ToString();
            Hour = Math.Clamp(hour, 0, 23);
        }
        public Session(string name, int type, string date, int hour)
        {
            Name = name;
            Type = type;
            Hour = hour;
            Date = date;
        }
        public Session()
        {

        }
        public void Set(Session other)
        {
            Name = other.Name;
            Type = other.Type;
            Date = other.Date;
            Hour = other.Hour;
            Guests = other.Guests;
            Hosts = other.Hosts;
        }
        public static Session Default()
        {
            return new Session("Music", -1, DateOnly.MinValue, -1);
        }
        public static Session Default(string date, int hour)
        {
            return new Session("Music", -1, date, hour);
        }

        public void AddGuests(string[]? guests)
        {
            if (guests is not null)
                Guests = (Guests == "" ? "" : (Guests + ",")) + string.Join(",", guests);
        }
        public void RemoveGuests(string[]? guests)
        {
            if (guests is null)
                return;
            List<string> L = [.. Guests.Split(",")];
            for (int i = 0; i < guests.Length; i++)
            {
                if (L.Contains(guests[i]))
                    L.Remove(guests[i]);
            }
            Guests = string.Join(",", L);
        }
        public void AddHosts(string[]? hosts)
        {
            if (hosts is not null)
                Hosts = (Hosts == "" ? "" : (Hosts + ",")) + string.Join(",", hosts);
        }
        public void RemoveHosts(string[]? hosts)
        {
            if (hosts is null)
                return;
            List<string> L = [.. Hosts.Split(",")];
            for (int i = 0; i < hosts.Length; i++)
            {
                if (L.Contains(hosts[i]))
                    L.Remove(hosts[i]);
            }
            Hosts = string.Join(",", L);
        }
    }
}
