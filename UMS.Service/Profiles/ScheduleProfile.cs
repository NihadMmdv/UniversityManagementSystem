using AutoMapper;
using UMS.DAL.Entities;
using UMS.Service.DTOs.ScheduleDTOs;

namespace UMS.Service.Profiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<Schedule, ScheduleGetDTO>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm")))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm")))
                .ForMember(dest => dest.TeacherIds, opt => opt.MapFrom(src => src.Teachers.Select(t => t.Id).ToList()));

            CreateMap<Schedule, ScheduleCreateDTO>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm")))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm")))
                .ForMember(dest => dest.TeacherIds, opt => opt.MapFrom(src => src.Teachers.Select(t => t.Id).ToList()));

            CreateMap<ScheduleCreateDTO, Schedule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Teachers, opt => opt.Ignore())
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeOnly.Parse(src.StartTime)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => TimeOnly.Parse(src.EndTime)));
        }
    }
}