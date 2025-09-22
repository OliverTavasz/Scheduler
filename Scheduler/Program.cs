using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Scheduler;



Schedule s = new();
s.AddSession([0], new Session(SessionType.Prerecorded));
ScheduleManager.AddSchedule(new DateOnly(2025, 1, 1), s);




var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
app.MapControllers();

app.MapGet("/Session/Get/{date}", (string date) =>
{
    if(DateTime.TryParse(date, out DateTime result))
        return JsonConvert.SerializeObject(ScheduleManager.GetSession(result));
    return null;
});
app.MapGet("/Session/Get", () =>
{
    return JsonConvert.SerializeObject(ScheduleManager.GetCurrentSession());
});

app.MapGet("/Schedule/Get/{date}", (string date) =>
{
    if (DateTime.TryParse(date, out DateTime result))
        return JsonConvert.SerializeObject(ScheduleManager.GetSchedule(result));
    return null;
});
app.MapGet("/Schedule/Get", () =>
{
    return JsonConvert.SerializeObject(ScheduleManager.GetCurrentSchedule());
});

app.MapGet("/Schedule/GetSeven", () =>
{
    DateTime date = DateTime.Now;
    Schedule?[] schedules = new Schedule[7];
    for(int i = 0; i < schedules.Length; i++)
    {
        date.AddDays(1);
        schedules[i] = ScheduleManager.GetSchedule(date);
    }
    return JsonConvert.SerializeObject(schedules);
});

app.MapPost("/Session/Add/", (string date, int[] hours, int type, string[]? hosts = null, string[]? guests = null) => {
    Session s = new((SessionType)type);
    if (hosts is not null)
        s.AddHosts(hosts);
    if (guests is not null)
        s.AddGuests(guests);

    if (DateTime.TryParse(date, out DateTime result) || s is null)
        return false;
    return ScheduleManager.AddSession(result, hours, s);
});
app.MapPost("/Session/Delete/", (string date) => {
    if (DateTime.TryParse(date, out DateTime result))
        return ScheduleManager.RemoveSession(result);
    return false;
});

app.Run();