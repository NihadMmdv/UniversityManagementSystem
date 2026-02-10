using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL.Entities;
using UMS.Service.DTOs.TeacherDTOs;

namespace UMS.Service.Profiles
{
    public class TeacherProfile:Profile
    {
        public TeacherProfile()
        {
            CreateMap<Teacher, TeacherGetDTO>()
                .ForMember(
                dest => dest.LessonIds,
                opt => opt.MapFrom(src => src.Lessons.Select(s => s.Id)));

            CreateMap<Teacher, TeacherCreateDTO>()
                .ForMember(
                dest => dest.LessonIds,
                opt => opt.MapFrom(src => src.Lessons.Select(s => s.Id)));
            
            CreateMap<TeacherCreateDTO, Teacher>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<TeacherPatchDTO, Teacher>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Teacher, TeacherPatchDTO>();
        }
    }
}
