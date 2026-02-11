using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using UMS.Service.Profiles;
using UMS.Service.Services.Implementations;
using UMS.Service.Services.Interfaces;

namespace UMS.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<DAL.CustomDBContext>(options =>
                options.UseNpgsql(connectionString));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ExamProfile>();
                cfg.AddProfile<LessonProfile>();
                cfg.AddProfile<SectionProfile>();
                cfg.AddProfile<StudentProfile>();
                cfg.AddProfile<TeacherProfile>();
            });

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IExamService, ExamService>();
            builder.Services.AddScoped<ILessonService, LessonService>();
            builder.Services.AddScoped<ISectionService, SectionService>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<ITeacherService, TeacherService>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            app.UseWhen(
                context => context.Request.Path.StartsWithSegments("/swagger"),
                swaggerApp =>
                {
                    swaggerApp.Use(async (context, next) =>
                    {
                        var token = context.Request.Query["token"].FirstOrDefault();

                        if (string.IsNullOrEmpty(token))
                            token = context.Request.Cookies["swagger_token"];

                        if (string.IsNullOrEmpty(token))
                        {
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync("Unauthorized. Access Swagger from the Dashboard.");
                            return;
                        }

                        var handler = new JsonWebTokenHandler();
                        var result = await handler.ValidateTokenAsync(token, tokenValidationParameters);

                        if (!result.IsValid)
                        {
                            context.Response.Cookies.Delete("swagger_token");
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync(
                                $"Token validation failed: {result.Exception?.Message}");
                            return;
                        }

                        var role = result.ClaimsIdentity?.FindFirst(ClaimTypes.Role)?.Value;
                        if (role != "Admin")
                        {
                            context.Response.StatusCode = 403;
                            await context.Response.WriteAsync("Forbidden. Admin role required.");
                            return;
                        }

                        if (context.Request.Query.ContainsKey("token"))
                        {
                            context.Response.Cookies.Delete("swagger_token", new CookieOptions
                            {
                                Path = "/swagger",
                                Secure = context.Request.IsHttps,
                                SameSite = SameSiteMode.Strict
                            });

                            context.Response.Cookies.Append("swagger_token", token, new CookieOptions
                            {
                                HttpOnly = false,
                                Secure = context.Request.IsHttps,
                                SameSite = SameSiteMode.Strict,
                                MaxAge = TimeSpan.FromHours(1),
                                Path = "/"
                            });
                        }

                        await next();
                    });
                }
            );

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors();
            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
