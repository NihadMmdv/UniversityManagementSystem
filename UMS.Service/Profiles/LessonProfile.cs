using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL.Entities;
using UMS.Service.DTOs.LessonDTOs;

namespace UMS.Service.Profiles
{
    public class LessonProfile:Profile
    {
        public LessonProfile()
        {
            CreateMap<Lesson, LessonGetDTO>()
                .ForMember(
                dest => dest.SectionIds,
                opt => opt.MapFrom(src => src.Sections.Select(s => s.Id)))
                .ForMember(
                dest => dest.TeacherIds,
                opt => opt.MapFrom(src => src.Teachers.Select(t => t.Id)));

            CreateMap<Lesson, LessonCreateDTO>()
                .ForMember(
                dest => dest.SectionIds,
                opt => opt.MapFrom(src => src.Sections.Select(s => s.Id)))
                .ForMember(
                dest => dest.TeacherIds,
                opt => opt.MapFrom(src => src.Teachers.Select(t => t.Id)));

            CreateMap<LessonCreateDTO, Lesson>()
                .ForMember(dest => dest.Sections, opt => opt.Ignore())
                .ForMember(dest => dest.Teachers, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<LessonPatchDTO, Lesson>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Lesson, LessonPatchDTO>();
        }
    }
}
