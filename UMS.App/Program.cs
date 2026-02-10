using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UMS.Service.Profiles;
using UMS.Service.Services.Implementations;
using UMS.Service.Services.Interfaces;

namespace UMS.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<DAL.CustomDBContext>(options =>
            options.UseNpgsql(connectionString));
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<ExamProfile>());
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<LessonProfile>());
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<SectionProfile>());
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<StudentProfile>());
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<TeacherProfile>());

            //// Replace the MapperConfiguration instantiation with the correct usage
            //var mapperConfig = new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile(new ExamProfile());
            //});
            //IMapper mapper = mapperConfig.CreateMapper();
            //builder.Services.AddSingleton(mapper);


            builder.Services.AddScoped<IExamService, ExamService>();
            builder.Services.AddScoped<ILessonService, LessonService>();
            builder.Services.AddScoped<ISectionService, SectionService>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<ITeacherService, TeacherService>();

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
