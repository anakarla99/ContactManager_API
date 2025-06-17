using ContactManagerApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ContactContext>(opt =>
    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "GSI Challenge Authenticator",

            ValidateAudience = true,
            ValidAudience = "www.gsichanllengeapi.com",

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("qwertyuiopasdfghjklzxcvbnm123456")),

            NameClaimType = "sub", // Esto indica que el 'username' está en 'sub'
            RoleClaimType = "Role"
        };
    });

builder.Services.AddScoped<ContactManagerApi.Services.IContactService, ContactManagerApi.Services.ContactService>();

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        return new BadRequestObjectResult(context.ModelState);
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CubanAdminsOnly", policy =>
    {
        policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/country", "CU");
        policy.RequireRole("Administrator");
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
