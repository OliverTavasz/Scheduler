namespace Scheduler
{
    public class Session(SessionType type, int hosts)
    {
        public readonly SessionType Type = type;
        public readonly int Hosts = hosts;
        public static Session Music => new(SessionType.Music, 0);
    }
}
