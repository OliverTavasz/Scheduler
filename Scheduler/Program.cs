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
using static System.Runtime.InteropServices.JavaScript.JSType;


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
    if (DateTime.TryParse(date, out DateTime dateTime))
    {
        return JsonConvert.SerializeObject(con.GetSession(dateTime));
    }
    return JsonConvert.SerializeObject(Session.Default());
});
app.MapGet("/Session/Get", (Context con) =>
{
    return JsonConvert.SerializeObject(con.GetSession());
});

app.MapGet("/Schedule/Get/{date}", (string date, Context con) =>
{
    if (DateOnly.TryParse(date, out DateOnly dateOnly))
    {
        return JsonConvert.SerializeObject(con.GetSchedule(dateOnly));
    }
    return JsonConvert.SerializeObject(Session.Default());
});
app.MapGet("/Schedule/Get", (Context con) =>
{
    return JsonConvert.SerializeObject(con.GetSchedule());
});

app.MapGet("/Schedule/GetSeven", (Context con) =>
{
    DateOnly date = DateOnly.FromDateTime(DateTime.Now);
    Session[][] schedules = new Session[7][];
    for(int i = 0; i < schedules.Length; i++)
    {
        schedules[i] = con.GetSchedule(date);
        date.AddDays(1);
    }
    return JsonConvert.SerializeObject(schedules);
});

app.MapPost("/Session/Add", (string name, int type, string dateOnly, int startHour, int endHour, string? hosts, string? guests, Context con) =>
{
    if (DateOnly.TryParse(dateOnly, out DateOnly date))
    {
        startHour = Math.Clamp(startHour, 0, 23);
        endHour = Math.Clamp(endHour, startHour, 23);

        for (int i = startHour; i < endHour + 1; i++)
        {
            Session s = new(name, type, date, i);
            if (hosts is not null)
                s.AddHosts(hosts.Split(","));
            if (guests is not null)
                s.AddGuests(guests.Split(","));

            DateTime time = new(date, new TimeOnly(i,0));
            Session? o = con.GetSession(time);
            if (o is not null)
                o.Set(s);
            else if (s.Hour != -1)
                con.Sessions.Add(s);
            else
                return false;
        }
        

        con.SaveChanges();
        return true;
    }
    return false;
});
app.MapPost("/Session/Remove", (string dateOnly, int startHour, int endHour, Context con) =>
{
    if (DateOnly.TryParse(dateOnly, out DateOnly date))
    {
        startHour = Math.Clamp(startHour, 0, 23);
        endHour = Math.Clamp(endHour, startHour, 23);

        for (int i = startHour; i < endHour + 1; i++)
        {
            DateTime dateTime = new(date, new TimeOnly(i, 1));
            Session? s = con.GetSession(dateTime);
            s?.Set(Session.Default(dateOnly, i));
        }
            
        con.SaveChanges();
        return true;
    }
    return false;
});
app.MapPost("/Session/Reschedule", (string sourceDate, string destinationDate, Context con) =>
{
    if (DateTime.TryParse(sourceDate, out DateTime sDate) && DateTime.TryParse(destinationDate, out DateTime dDate))
    {
        Session? source = con.GetSession(sDate);
        Session? destination = con.GetSession(dDate);

        if (source is null)
            return false;

        Session newSession = new();
        newSession.Set(source);
        newSession.Date = DateOnly.FromDateTime(dDate).ToString();
        newSession.Hour = dDate.Hour;

        if (destination is null)
            con.Sessions.Add(newSession);
        else
            destination?.Set(newSession);

        source.Set(Session.Default(DateOnly.FromDateTime(sDate).ToString(), sDate.Hour));

        con.SaveChanges();
        return true;
    }
    return false;
});

app.MapPost("/Session/AddPeople", (string date, string? hosts, string? guests, Context con) =>
{
    if (DateTime.TryParse(date, out DateTime dateTime))
    {
        Session? s = con.GetSession(dateTime);
        s?.AddHosts(hosts?.Split(","));
        s?.AddGuests(guests?.Split(","));
        con.SaveChanges();
        return true;
    }
    return false;
});
app.MapPost("/Session/RemovePeople", (string date, string? hosts, string? guests, Context con) =>
{
    if (DateTime.TryParse(date, out DateTime dateTime))
    {
        Session? s = con.GetSession(dateTime);
        s?.RemoveHosts(hosts?.Split(","));
        s?.RemoveGuests(guests?.Split(","));
        con.SaveChanges();
        return true;
    }
    return false;
});

app.Run();