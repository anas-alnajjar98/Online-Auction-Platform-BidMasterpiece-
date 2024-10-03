using MasterPiece.Data;
using MasterPiece.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

namespace MasterPiece
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Register DbContext and connect to SQL Server using the connection string in appsettings.json
            builder.Services.AddDbContext<AuctionDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();

            // Register EmailHelper as a service for sending emails
            builder.Services.AddSingleton<EmailHelper>();

            // Register TokenGenerator as a service
            builder.Services.AddSingleton<TokenGenerator>();

            // Register the background service for checking pending payments
            builder.Services.AddHostedService<CheckPendingPaymentsService>();

            // Swagger configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // JWT Authentication Configuration
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings.GetValue<string>("Key"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                    ValidAudience = jwtSettings.GetValue<string>("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // Configure CORS to allow requests from your frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyOrigin() // Allow all origins for now
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // Configure Hangfire with SQL Server storage
            builder.Services.AddHangfire(config =>
                config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHangfireServer();
            builder.Services.AddSingleton<StripePaymentService>();

            var stripeSettings = builder.Configuration.GetSection("Stripe");
            StripeConfiguration.ApiKey = stripeSettings["SecretKey"];
            var app = builder.Build();

            // Apply CORS middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Use Authentication and Authorization Middleware
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowSpecificOrigins");

            // Use Hangfire Dashboard to monitor jobs
            app.UseHangfireDashboard("/hangfire");

            // Recurring Job for checking payments every hour
            RecurringJob.AddOrUpdate<CheckPendingPaymentsService>(
                "check-payments",
                x => x.CheckPendingPaymentsWithoutToken(), // Call the method without a CancellationToken
                Cron.Hourly);

            app.MapControllers();

            app.Run();
        }
    }
}
