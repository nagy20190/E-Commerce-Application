
using E_CommerceApplication.BLL.Filters;
using E_CommerceApplication.Middlewares;
using Microsoft.EntityFrameworkCore;
using E_CommerceApplication.DAL.Services;
using E_CommerceApplication.Helpers;
using E_CommerceApplication.BLL.Interfaces;
using E_CommerceApplication.DAL.Repositories;
using System.Security.Principal;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace E_CommerceApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // confige swagger to allow Jwt
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Please enter token",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                // https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters
                // add Security information to each operation for OAuth2
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
                options.UseSqlServer(connectionString);
            });


            //foreach (var entityType in entityTypes)
            //{
            //    var interfaceType = typeof(IBaseRepository<>).MakeGenericType(entityType);
            //    var implementationType = typeof(BaseRepository<>).MakeGenericType(entityType);
            //    builder.Services.AddScoped(interfaceType, implementationType);
            //}
            // Get all entity types from the DbContext model

            var entityTypes = Assembly.GetAssembly(typeof(IEntity))
               .GetTypes()
               .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(IEntity)))
               .ToList();

            // Register each entity repository
            foreach (var entityType in entityTypes)
            {
                var interfaceType = typeof(IBaseRepository<>).MakeGenericType(entityType);
                var implementationType = typeof(BaseRepository<>).MakeGenericType(entityType);
               builder.Services.AddScoped(interfaceType, implementationType);
            }

            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<EmailSender>();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
                        ValidAudience = builder.Configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:Key"]!))
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            // create inline  middleware
            //app.Use((context, next) =>
            //{
            //    DateTime requestTime = DateTime.Now;
            //    var result = next(context);
            //    DateTime responseTime = DateTime.Now;
            //    TimeSpan processDuration = responseTime - requestTime;
            //    Console.WriteLine($"[inline middlware] process duration {processDuration.TotalMilliseconds} ms"); // return the time in ms
            //    return result;
            //}); // simple middlware allows us to calculate the execution time of a requist

            // app.UseMiddleware<StatsMiddleware>(); // using class middleware

            app.UseStaticFiles(); // for static files --> in wwwroot

            app.UseHttpsRedirection();

            //app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
