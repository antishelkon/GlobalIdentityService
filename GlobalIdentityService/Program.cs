
using GlobalIdentityService.Data;
using GlobalIdentityService.Dto;
using GlobalIdentityService.Models;
using GlobalIdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlobalIdentityService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("Identity");

            var services = builder.Services;

            services.AddControllers();
            services.AddProblemDetails();
            services.AddOpenApi();

            services.AddHttpContextAccessor();
            services.AddScoped<RequestContext>();
            services.AddScoped<UserSecurityAuditService>();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<ApplicationService>();
            services.AddScoped<UserService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            var app = builder.Build();

            app.UseExceptionHandler();
            app.UseStatusCodePages();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
