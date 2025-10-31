using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Scheduler;
using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
//builder.Services.AddDbContext<Context>(options => options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Sessions;Trusted_Connection=True;TrustServerCertificate=True;"));
builder.Services.AddDbContext<Context>(options => options.UseMySql("Server=localhost;Database=scheduler;User=root;Password=root;", new MySqlServerVersion(new Version(9, 4, 0))));
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors("AllowReactApp");
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

    if (!DateTime.TryParse(date, out DateTime datetime))
        return false;
    if (!DateOnly.TryParse(date, out DateOnly result))
        return false;

    string[]? h = hosts?.Split(',');
    string[]? g = guests?.Split(',');

    string[] hoursArray = hours.Split(',');

    for (int i = 0; i < hoursArray.Length; i++)
    {
        if (!int.TryParse(hoursArray[i], out int result2))
            return false;

        Session s = new(type, result2, result);
        s.AddHosts(h);
        s.AddGuests(g);

        Session? os = con.Sessions.FirstOrDefault(s => s.date == result.ToString() && s.Hour == result2);
        if (os is not null) {
            os.SetValues(s);
        } else {
            con.Add(s);
        }
           
    }
    con.SaveChanges();
    return true;
});
app.MapPost("/Session/Remove", async (string date, Context con) =>
{
    if (!DateTime.TryParse(date, out DateTime fulldate))
        return false;

    var e = await con.Sessions.FirstOrDefaultAsync(s => s.date == DateOnly.FromDateTime(fulldate).ToString() && s.Hour == fulldate.Hour);
    if (e is not null) { 
        con.Sessions.Remove(e); 
    } else { 
        return false; 
    }
    await con.SaveChangesAsync();
    return true;
});
app.MapPost("/Session/Reschedule", (string sourceDate, string destinationDate, Context con) =>
{
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