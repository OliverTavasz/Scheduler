namespace Scheduler
{
    public class Session(SessionType type, int studio = -1)
    {
        public readonly SessionType Type = type;
        public readonly int Studio = studio;
        public static Session Music => new(SessionType.Music);
    }
}
