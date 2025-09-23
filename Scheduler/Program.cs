using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Scheduler;
using System;



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
        date = date.AddDays(1);
        schedules[i] = ScheduleManager.GetSchedule(date);
    }
    return JsonConvert.SerializeObject(schedules);
});

app.MapPost("/Session/Add", (string date, string hours, int type, string? hosts, string? guests) => {
    Session s = new((SessionType)type);
    if (s is null || !DateTime.TryParse(date, out DateTime result))
        return false;

    string[]? h = hosts?.Split(',');
    string[]? g = guests?.Split(',');

    if (h is not null)
        s.AddHosts(h);
    if (g is not null)
        s.AddGuests(g);

    string[] hoursArray = hours.Split(',');
    int[] hNum = new int[hoursArray.Length];

    for (int i = 0; i < hoursArray.Length; i++)
    {
        if (!int.TryParse(hoursArray[i], out int result2))
            return false;
        hNum[i] = result2;
    }

    return ScheduleManager.AddSession(result, hNum, s);
});
app.MapPost("/Session/Remove", (string date) => {
    if (DateTime.TryParse(date, out DateTime result))
        return ScheduleManager.RemoveSession(result);
    return false;
});
app.MapPost("/Session/Reschedule", (string sourceDate, string destinationDate) => {
    if (!(DateTime.TryParse(sourceDate, out DateTime sDate) && DateTime.TryParse(destinationDate, out DateTime dDate)))
        return false;
    Session s = ScheduleManager.GetSession(sDate);
    if (s is null)
        return false;

    ScheduleManager.RemoveSession(sDate);
    ScheduleManager.AddSession(dDate, [dDate.Hour], s);
    return true;
});

app.MapPost("/Session/AddPeople", (string date, string? hosts, string? guests) => {
    if (!DateTime.TryParse(date, out DateTime d))
        return false;
    Session s = ScheduleManager.GetSession(d);
    if(s is null) return false;

    string[]? h = hosts?.Split(',');
    string[]? g = guests?.Split(',');

    if (h is not null)
        s.AddHosts(h);
    if (g is not null)
        s.AddGuests(g);
    return true;
});
app.MapPost("/Session/RemovePeople", (string date, string? hosts, string? guests) => {
    if (!DateTime.TryParse(date, out DateTime d))
        return false;
    Session s = ScheduleManager.GetSession(d);
    if (s is null) return false;

    string[]? h = hosts?.Split(',');
    string[]? g = guests?.Split(',');

    if (h is not null)
        s.RemoveHosts(h);
    if (g is not null)
        s.RemoveGuests(g);
    return true;
});

app.Run();