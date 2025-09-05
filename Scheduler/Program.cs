using Scheduler;

Schedule s = new();
s.AddSession([0,1,2,3], new Session(SessionType.Prerecorded));
ScheduleManager.AddSchedule(new DateOnly(2025, 9, 6), s);

Session session = ScheduleManager.GetCurrentSession();

Console.ReadLine();