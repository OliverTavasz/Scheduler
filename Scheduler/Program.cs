using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Scheduler;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
        .AllowAnyOrigin()
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

var key = Encoding.ASCII.GetBytes("nbufdubhzphbhdzfpgijpdzrguirpghzuzhgoiurhgzohugbhdzghuizp");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; //only false for now
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors("AllowReactApp");
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/Session/Get/{date}", (string date, Context con) =>
{
    if (DateTime.TryParse(date, out DateTime dateTime))
    {
        return JsonConvert.SerializeObject(con.GetSession(dateTime));
    }
    return "Error parsing date";
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
    return "Error parsing date";
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


app.MapPost("/login", (UserLoginModel login) =>
{
    if (login.Username == "admin" && login.Password == "admin")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, login.Username)]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
    }

    return Results.Unauthorized();
});

app.Run();