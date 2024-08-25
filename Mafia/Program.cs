using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Mafia.WebApi.Services;
using Mafia.Services;
using Mafia.Domain.Entities;
using Mafia.Domain.Data.Adapters;
using Quartz.Spi;
using Mafia.Application.Services.AccountAndUser;
using Quartz;
using Mafia.Application.Services;
using Quartz.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mafia.WebApi;
using Mafia.Application;
using System.Reflection;
using System.IO;
using Mafia.Domain.AutoAudit;
using DotNetEnv;
using static System.Environment;
using Serilog;
using Microsoft.Extensions.FileProviders;
using Mafia.Application.Services.Mafia;
using Microsoft.AspNetCore.SignalR;
using Mafia.Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Start DotNetEnv
string _path = Path.Combine(GetFolderPath(SpecialFolder.MyDocuments, SpecialFolderOption.DoNotVerify), "Mafia");

using (var stream = File.OpenRead($"{_path}/.env"))
{
    Console.WriteLine($"{_path}/.env");
    Env.Load(stream);
}
// End DotNetEnv
Serilog.Log.Logger = new LoggerConfiguration()
    .WriteTo.File("C:\\log.txt")
    .CreateLogger();

var optionsBuilder = new DbContextOptionsBuilder<MafiaDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:5000") // Укажите здесь URL фронтенда
                                 .AllowAnyHeader()
                                 .AllowAnyMethod().SetIsOriginAllowed(_ => true)
                                 .AllowCredentials();
                      });
});
if (builder.Environment.IsDevelopment())
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    builder.Services.AddDbContext<MafiaDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultServerConnection"));
    builder.Services.AddDbContext<MafiaDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultServerConnection")));
}
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 0;

    //Signin settings
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;

    // Default User settings.
    //options.User.AllowedUserNameCharacters =
    //        "0123456789";
    options.User.RequireUniqueEmail = false;
});
builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
        .RequireAuthenticatedUser().Build());
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(DotNetEnv.Env.GetString("JWT_KEY", "Variable not found"));

        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Change to true in production
            ValidateAudience = false, // Change to true in production
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is directed to the hub
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {
                    // Set the token from the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddSwaggerGen(c =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mafia", Version = "v1" });
    c.EnableAnnotations();
    c.OperationFilter<AuthorizationOperationFilter>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
              Enter 'Bearer' [space] and then your token in the text input below.
              \r\n\r\nExample: 'Bearer 12345abcdef'"
    });
});


#region DI (Dependency Injection)

builder.Services.AddApplicationServices();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
     .AddRoles<IdentityRole>()
     .AddEntityFrameworkStores<MafiaDbContext>()
     .AddDefaultTokenProviders()
     .AddUserManager<UserManager<ApplicationUser>>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<IJobFactory, JobFactory>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddHostedService<QuartzHostedService>();
builder.Services.AddScoped<IUserSession, UserSession>();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddScoped<IMafiaService, MafiaService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddTransient<ILogService, LogService>();
//builder.Services.AddSingleton(new JobSchedule(
//jobType: typeof(PlanSheduleService),
//cronExpression: "0 0 */4 ? * *"));
#endregion
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
builder.Services.AddControllersWithViews()
     .AddNewtonsoftJson(options =>
     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
 );

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSignalR();
//builder.Services.AddSession(options =>
//{
//    options.Cookie.Name = ".AdventureWorks.Session";
//    options.IdleTimeout = TimeSpan.FromDays(100);
//    options.Cookie.IsEssential = true;
//});

var app = builder.Build();

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(GetFolderPath(SpecialFolder.MyDocuments, SpecialFolderOption.DoNotVerify), "Ambfiles/uploads")),
//    RequestPath = "/uploads"
//});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

//if (builder.Environment.IsDevelopment())
//{
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mafia v1");
    c.DisplayRequestDuration();
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});
//}
//else
//{
app.UseHsts();
//app.UseSerilogRequestLogging();
//}
app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
app.UseSwaggerUI();


app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();
app.UseForwardedHeaders();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chatHub");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();
