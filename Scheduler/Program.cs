using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Scheduler;
using System;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin();
    });
});
builder.Services.AddDbContext<Context>(options => options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Sessions;Trusted_Connection=True;TrustServerCertificate=True;"));
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

app.MapGet("/Session/Get/{date}", (string date, Context con) =>
{
    con.ConstructSchedules();
    if (DateTime.TryParse(date, out DateTime result))
        return JsonConvert.SerializeObject(ScheduleManager.GetSession(result));
    return null;
});
app.MapGet("/Session/Get", (Context con) =>
{
    con.ConstructSchedules();
    return JsonConvert.SerializeObject(ScheduleManager.GetCurrentSession());
});

app.MapGet("/Schedule/Get/{date}", (string date, Context con) =>
{
    con.ConstructSchedules();
    if (DateTime.TryParse(date, out DateTime result))
        return JsonConvert.SerializeObject(ScheduleManager.GetSchedule(DateOnly.FromDateTime(result)));
    return null;
});
app.MapGet("/Schedule/Get", (Context con) =>
{
    con.ConstructSchedules();
    return JsonConvert.SerializeObject(ScheduleManager.GetCurrentSchedule());
});

app.MapGet("/Schedule/GetSeven", (Context con) =>
{
    con.ConstructSchedules();
    DateTime date = DateTime.Now;
    Schedule?[] schedules = new Schedule[7];
    for (int i = 0; i < schedules.Length; i++)
    {
        date = date.AddDays(1);
        schedules[i] = ScheduleManager.GetSchedule(DateOnly.FromDateTime(date));
    }
    return JsonConvert.SerializeObject(schedules);
});

app.MapPost("/Session/Add", (string date, string hours, int type, string? hosts, string? guests, Context con) =>
{
    con.ConstructSchedules();
    if (!DateTime.TryParse(date, out DateTime result))
        return false;

    string[]? h = hosts?.Split(',');
    string[]? g = guests?.Split(',');

    string[] hoursArray = hours.Split(',');
    int[] hNum = new int[hoursArray.Length];

    for (int i = 0; i < hoursArray.Length; i++)
    {
        if (!int.TryParse(hoursArray[i], out int result2))
            return false;
        hNum[i] = result2;
    }

    bool success = ScheduleManager.AddSession(DateOnly.FromDateTime(result), hNum, type, h, g, con);
    if (success)
        con.SaveChanges();
    return success;
});
app.MapPost("/Session/Remove", (string date, Context con) =>
{
    con.ConstructSchedules();
    if (DateTime.TryParse(date, out DateTime result))
        return ScheduleManager.RemoveSession(result, con);
    con.SaveChanges();
    return false;
});
app.MapPost("/Session/Reschedule", (string sourceDate, string destinationDate, Context con) =>
{
    //con.ConstructSchedules();
    if (!(DateTime.TryParse(sourceDate, out DateTime sDate) && DateTime.TryParse(destinationDate, out DateTime dDate)))
        return false;

    Session? s = null;
    foreach (Session e in con.Sessions)
    {
        if (DateOnly.Parse(e.date) == DateOnly.FromDateTime(sDate) && e.Hour == sDate.Hour)
            s = e;
    }
    if (s is null)
        return false;

    s.date = DateOnly.FromDateTime(dDate).ToString();
    s.Hour = dDate.Hour;

    con.SaveChanges();
    return true;
});

app.MapPost("/Session/AddPeople", (string date, string? hosts, string? guests, Context con) =>
{
    if (!DateTime.TryParse(date, out DateTime d))
        return false;

    Session? s = null;
    foreach (Session e in con.Sessions)
    {
        if (DateOnly.Parse(e.date) == DateOnly.FromDateTime(d) && e.Hour == d.Hour)
            s = e;
    }
    if (s is null)
        return false;

    string[]? h = hosts?.Split(',');
    string[]? g = guests?.Split(',');

    if (h is not null)
        s.AddHosts(h);
    if (g is not null)
        s.AddGuests(g);
    con.SaveChanges();
    return true;
});
app.MapPost("/Session/RemovePeople", (string date, string? hosts, string? guests, Context con) =>
{
    if (!DateTime.TryParse(date, out DateTime d))
        return false;

    Session? s = null;
    foreach (Session e in con.Sessions)
    {
        if (DateOnly.Parse(e.date) == DateOnly.FromDateTime(d) && e.Hour == d.Hour)
            s = e;
    }
    if (s is null)
        return false;

    string[]? h = hosts?.Split(',');
    string[]? g = guests?.Split(',');

    if (h is not null)
        s.RemoveHosts(h);
    if (g is not null)
        s.RemoveGuests(g);
    con.SaveChanges();
    return true;
});

app.Run();